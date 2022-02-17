using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using QControlKit.Events;
using QControlKit.Constants;

namespace QControlKit
{
    public class QWorkspace: IEquatable<QWorkspace>
    {

        private QServer server;
        private QClient client;

        public string name { get; set; }
        public string uniqueID;
        public bool connected;
        public bool hasPasscode;
        private string passcode;

        private QCue root;

        public bool defaultSendUpdatesOSC;

        public string version;


        public event QWorkspaceUpdatedHandler WorkspaceUpdated;
        public event QCueListChangedPlaybackPositionHandler CueListChangedPlaybackPosition;
        public event QWorkspaceConnectionErrorHandler WorkspaceConnectionError;
        public event QWorkspaceDisconnectedHandler WorkspaceDisconnected;
        public event QWorkspaceConnectedHandler WorkspaceConnected;

        private void Init()
        {
            name = "";
            uniqueID = "";
            connected = false;
            passcode = "";
            hasPasscode = false;
            defaultSendUpdatesOSC = false;

            root = new QCue(this);
            root.setProperty(QIdentifiers.RootCue, QOSCKey.UID, false);
            root.setProperty("Cue Lists", QOSCKey.Name, false);
            root.setProperty(QCueType.CueList, QOSCKey.Type, false);
            root.nestLevel = -1;

            if (version == null || version.Length <= 0)
                version = "3.0.0";
        }

        public QWorkspace(QWorkspaceInfo workspaceInfo, QServer server)
        {
            if (workspaceInfo.version.Length > 0)
                version = workspaceInfo.version;

            Init();

            if (workspaceInfo.uniqueID.Length > 0)
                uniqueID = workspaceInfo.uniqueID;

            updateWithWorkspaceInfo(workspaceInfo);

            client = new QClient(server.host, server.port);
            
            client.WorkspaceConnected += OnWorkspaceConnected;
            client.WorkspaceConnectionError += OnWorkspaceConnectionError;
            client.WorkspaceDisconnected += OnWorkspaceDisconnected;
            client.CueListsUpdated += OnCueListsUpdated;
            client.CueListChangedPlaybackPosition += OnCueListChangedPlaybackPosition;
            client.CueNeedsUpdated += OnCueNeedsUpdated;
            client.CueUpdated += OnCueUpdated;

            this.server = server;
            Log.Debug($"[workspace] <{name}> on <{server.name}> initialized.");
        }

        //updateWithDictionary
        public bool updateWithWorkspaceInfo(QWorkspaceInfo workspaceInfo) {
            bool didUpdate = false;
            if(workspaceInfo.displayName.Length > 0 && !workspaceInfo.displayName.Equals(this.name))
            {
                this.name = workspaceInfo.displayName;
                didUpdate = true;
            }
            if (!workspaceInfo.hasPasscode.Equals(this.hasPasscode))
            {
                this.hasPasscode = workspaceInfo.hasPasscode;
                didUpdate = true;
            }
            return didUpdate;
        }

        public QWorkspaceInfo getWorkspaceInfo()
        {
            return new QWorkspaceInfo
            {
                displayName = name,
                hasPasscode = hasPasscode,
                version = version,
                uniqueID = uniqueID,
            };
        }

        public QServerInfo GetServerInfo()
        {
            return new QServerInfo
            {
                host = server.host,
                port = server.port
            };
        }

        public string description { get { return $"{name} : {uniqueID}"; } }

        public bool isOlderThanVersion(string version)
        {
            var thisVersion = new Version(this.version);
            var otherVersion = new Version(version);
            return thisVersion.CompareTo(otherVersion) < 0;
        }

        public bool isEqualToVersion(string version)
        {
            var thisVersion = new Version(this.version);
            var otherVersion = new Version(version);
            return thisVersion.CompareTo(otherVersion) == 0;
        }

        public bool isNewerThanVersion(string version)
        {
            var thisVersion = new Version(this.version);
            var otherVersion = new Version(version);
            return thisVersion.CompareTo(otherVersion) > 0;
        }

        public string nameWithoutPathExtension { 
            get {
                if (name.EndsWith(".cues"))
                    return name.Substring(0, name.Length - 5);
                else if (name.EndsWith(".qlab4"))
                    return name.Substring(0, name.Length - 6);
                return name;
            } 
        }

        public string serverName { get { return server.name; } }

        public string fullName { get { return $"{name} ({server.name})"; } }

        public QCue firstCue { get { return firstCueList.firstCue; } }
        public QCue firstCueList { get { return root.firstCue; } }

        public List<QCue> cueLists { get { return root.cues; } }
        public string fullNameWithCueList(QCue cueList) { return ""; }

        public string[] versionParts { get { return version.Split('.'); } }
        public bool connectedToQLab3 { get { return versionParts[0] == "3"; } }

        #region Connection/reconnection

        public void connect(string passcode = null)
        {
            Log.Information($"[workspace] connecting to <{name}> @ {this.server.host}:{this.server.port}");

            if(hasPasscode && passcode == null)
            {
                Log.Error($"[workspace] *** workspace <{name}> requires a passcode but none was supplied.");
                OnWorkspaceConnectionError(this, new QWorkspaceConnectionErrorArgs { status = QConnectionStatus.BadPass });
                return;
            }

            if (!client.connect())
            {
                Log.Error($"[workspace] *** couldn't connect to server client is not connected.");
                OnWorkspaceConnectionError(this, new QWorkspaceConnectionErrorArgs { status = QConnectionStatus.Error });
                return;
            }

            //save password for reuse
            this.passcode = passcode;
            if (passcode != null)
                client.sendMessage($"{workspacePrefix}/connect", passcode);
            else
                client.sendMessage($"{workspacePrefix}/connect");
        }

        private void finishConnection()
        {
            connected = true;
            startReceivingUpdates();
            //fetchQLabVersion(); //this is not needed since the version is loaded from the workspace info call
            fetchCueLists();

        }

        public void reconnect()
        {
            if (connected)
                return;
            if(hasPasscode)
                connect(passcode);
            else
                connect();

        }

        public void disconnect()
        {
            Log.Information($"[workspace] disconnecting from <{name}>");
            if (!connected)
                return;
            stopReceivingUpdates();
            disconnectFromWorkspace();
            client.disconnect();
            connected = false;
        }

        public void temporarilyDisconnect()
        {
            //TODO what does this mean?
        }

        #endregion  

        #region Cues
        public QCue cueWithID(string uid)
        {
            return root.cueWithID(uid);
        }

        public QCue cueWithNumber(string number)
        {
            return root.cueWithNumber(number);
        }
        #endregion

        #region Workspace Methods
        public void disconnectFromWorkspace() { client.sendMessage($"{workspacePrefix}/disconnect"); }
        public void startReceivingUpdates() { client.sendMessage($"{workspacePrefix}/updates", 1); }
        public void getPropertyForActive(string prop) { client.sendMessage($"{workspacePrefix}/cue/active/{prop}"); }
        public void stopReceivingUpdates() { client.sendMessage($"{workspacePrefix}/updates", 0); }
        public void enableAlwaysReply() { client.sendMessage($"{workspacePrefix}/alwaysReply", 1); }
        public void disableAlwaysReply() { client.sendMessage($"{workspacePrefix}/alwaysReply", 0); }
        public void fetchQLabVersion() { client.sendMessage($"{workspacePrefix}/version"); }
        public void fetchCueLists() { client.sendMessage($"{workspacePrefix}/cueLists"); }
        public void fetchPlaybackPositionForCue(QCue cue) { client.sendMessage(addressForCue(cue, QOSCKey.PlaybackPositionId)); } //EventHandler for this? can I use the CueListPlaybackPosition one?
        public void go() { client.sendMessage($"{workspacePrefix}/go"); }
        public void go(string cueNumber) { client.sendMessage($"{workspacePrefix}/go/{cueNumber}"); }
        public void save() { client.sendMessage($"{workspacePrefix}/save"); }
        public void undo() { client.sendMessage($"{workspacePrefix}/undo"); }
        public void redo() { client.sendMessage($"{workspacePrefix}/redo"); }
        public void resetAll() { client.sendMessage($"{workspacePrefix}/reset"); }
        public void pauseAll() { client.sendMessage($"{workspacePrefix}/pause"); }
        public void resumeAll() { client.sendMessage($"{workspacePrefix}/resume"); }
        public void stopAll() { client.sendMessage($"{workspacePrefix}/stop"); }
        public void panicAll() { client.sendMessage($"{workspacePrefix}/panic"); }
        #endregion

        #region Heartbeat
        //TODO? Do I even need this if I only allow TCP and just watch for a change in the state of the TCP connection

        #endregion

        #region Cue Actions
        public void startCue(QCue cue) { client.sendMessage(addressForCue(cue, "start")); }
        public void stopCue(QCue cue) { client.sendMessage(addressForCue(cue, "stop")); }
        public void pauseCue(QCue cue) { client.sendMessage(addressForCue(cue, "pause")); } //TODO: immediately update local for snappier whatever 
        public void loadCue(QCue cue) { client.sendMessage(addressForCue(cue, "load")); }
        public void resetCue(QCue cue) { client.sendMessage(addressForCue(cue, "reset")); }
        public void deleteCue(QCue cue) { client.sendMessage($"{workspacePrefix}/delete_id/{cue.propertyForKey(QOSCKey.UID)}"); }
        public void resumeCue(QCue cue) { client.sendMessage(addressForCue(cue, "resume")); }
        public void hardStopCue(QCue cue) { client.sendMessage(addressForCue(cue, "hardStop")); }
        public void hardPauseCue(QCue cue) { client.sendMessage(addressForCue(cue, "hardPause")); } //TODO: immediately update local for snappier whatever 
        public void togglePauseCue(QCue cue) { client.sendMessage(addressForCue(cue, "togglePause")); }
        public void previewCue(QCue cue) { client.sendMessage(addressForCue(cue, "preview")); }
        public void panicCue(QCue cue) { client.sendMessage(addressForCue(cue, "panic")); } //TODO: immediately update local for snappier whatever 

        public void moveCue(QCue cue, int newIndex, QCue newParentCue)
        {
            //TODO: Test this
            client.sendMessage($"{workspacePrefix}/move/{cue.propertyForKey(QOSCKey.UID)}", newIndex, newParentCue.propertyForKey(QOSCKey.UID));
        }

        #endregion

        #region Cue Getters/Setters
 
        public void valueForKey(QCue cue, string key) { client.sendMessage(addressForCue(cue, key)); }
        
        public void valuesForKeys(QCue cue, string[] keys) 
        {
            string keyString = JsonConvert.SerializeObject(keys);
            client.sendMessage(addressForCue(cue, "valuesForKeys"), keyString);
        }

        public void updatePropertySend(QCue cue, object value, string key) {
                client.sendMessage(addressForCue(cue, key), value);
        }

        public void updatePropertiesSend(QCue cue, object[] values, string key) { client.sendMessage(addressForCue(cue, key), values); }

        public void updateAllCueProperties()
        {
            root.sendAllPropertiesToQLab();
        }
        public void runningOrPausedCues()
        {
            client.sendMessage($"{workspacePrefix}/runningOrPausedCues");
        }
        #endregion

        #region Property Fetching

        public void fetchDefaultPropertiesForCue(QCue cue)
        {
            List<string> keys = new List<string> {
                QOSCKey.UID,
                QOSCKey.Number,
                QOSCKey.Name, 
                QOSCKey.ListName,
                QOSCKey.Type,
                QOSCKey.ColorName, 
                QOSCKey.Flagged,
                QOSCKey.Armed,
                QOSCKey.IsRunning,
                QOSCKey.IsBroken,
                QOSCKey.Parent,
                QOSCKey.Notes
            };

            fetchPropertiesForCue(cue, keys.ToArray(), false);
        }

        public void fetchBasicPropertiesForCue(QCue cue)
        {
            List<string> keys = new List<string> {
                QOSCKey.Name,
                QOSCKey.ListName,
                QOSCKey.Number,
                QOSCKey.FileTarget,
                QOSCKey.CueTargetNumber, 
                QOSCKey.HasFileTargets,
                QOSCKey.HasCueTargets,
                QOSCKey.Armed,
                QOSCKey.ColorName,
                QOSCKey.ContinueMode,
                QOSCKey.Flagged,
                QOSCKey.PreWait,
                QOSCKey.PostWait, 
                QOSCKey.Duration,
                QOSCKey.AllowsEditingDuration,
                QOSCKey.IsBroken,
                QOSCKey.IsRunning,
                QOSCKey.IsLoaded,
                QOSCKey.Notes
            };

            if (cue.IsGroup)
                keys.Add(QOSCKey.Children);
            fetchPropertiesForCue(cue, keys.ToArray(), false);
        }

        public void fetchPropertiesForCue(QCue cue, string[] keys, bool includeChildren)
        {
            valuesForKeys(cue, keys);

            if (!includeChildren)
                return;
            foreach (var childCue in cue.cues)
            {
                fetchPropertiesForCue(cue, keys, includeChildren);
            }
        }
        #endregion

        #region OSC address helpers
        public void sendMessage(string address, params object[] args)
        {
            //check for workspace prefix?
            if (!address.StartsWith(workspacePrefix))
                address = workspacePrefix + address;
            client.sendMessage(address, args);
        }

        public void sendMessageForCue(QCue cue, string action)
        {
            client.sendMessage(addressForCue(cue, action));
        }

        private string addressForCue(QCue cue, string action)
        {
            return $"{workspacePrefix}/cue_id/{cue.propertyForKey(QOSCKey.UID)}/{action}";
        }

        private string addressForWildcardNumber(string number, string action) {
            return $"{workspacePrefix}/cue/{number}/{action}";
        }

        private string workspacePrefix
        {
            get { return $"/workspace/{uniqueID}";  }
            
        }
        #endregion

        public string workspaceID
        {
            get
            {
                return uniqueID;
            }
        }

        #region Event Handling
        private void OnWorkspaceConnectionError(object source, QWorkspaceConnectionErrorArgs args)
        {
            if (args.status.Equals("badpass"))
            {
                //clear passcode if there was one set in the connect() method
                this.passcode = null;
                Log.Error($"[workspace] *** Password for workspace <{name}> was incorrect!");
            }
            else
            {
                Log.Error($"[workspace] *** Unable to connect to workspace: <{name}> on server: <{server.name}>");
            }

            WorkspaceConnectionError?.Invoke(this, new QWorkspaceConnectionErrorArgs { status = args.status });
        }

        private void OnWorkspaceConnected(object source, QWorkspaceConnectedArgs args)
        {
            Log.Information($"[workspace] Connection to <{name}> successful, finishing things up.");
            WorkspaceConnected?.Invoke(this, new QWorkspaceConnectedArgs());
            finishConnection();
        }

        private void OnWorkspaceDisconnected(object source, QWorkspaceDisconnectedArgs args)
        {
            //this might not be called with TCP?
            Log.Warning($"[workspace] *** Workspace has indicated it is disconnecting");
            WorkspaceDisconnected?.Invoke(this, new QWorkspaceDisconnectedArgs());
        }

        private void OnCueListsUpdated(object source, QCueListsUpdatedArgs args)
        {
            bool rootCueUpdated = false;
            List<QCue> currentCueLists = new List<QCue>(args.data.Count());
            int index = 0;

            foreach (var aCueList in args.data)
            {
                string uid = aCueList[QOSCKey.UID].ToString();
                if (uid == null)
                    continue;
                QCue cueList = root.cueWithID(uid, false);

                if(cueList != null)
                {
                    if(cueList.sortIndex != index)
                    {
                        cueList.sortIndex = index;
                        rootCueUpdated = true;
                    }
                    
                }
                else
                {
                    cueList = new QCue(aCueList, this);
                    cueList.sortIndex = index;
                    if (connectedToQLab3)
                        cueList.setProperty(QCueType.CueList, QOSCKey.Type);
                    rootCueUpdated = true;
                }
                currentCueLists.Add(cueList);
                index++;
            }
            QCue activeCuesList = root.cueWithID(QIdentifiers.ActiveCues, false);
            if(activeCuesList != null)
            {
                if (activeCuesList.sortIndex != index)
                {
                    activeCuesList.setProperty(QIdentifiers.ActiveCues, QOSCKey.UID, false);
                    activeCuesList.setProperty("Active Cues", QOSCKey.Name, false);
                    activeCuesList.setProperty(QCueType.CueList, QOSCKey.Type, false);

                    rootCueUpdated = true;
                }
            }
            else
            {
                activeCuesList = new QCue(this);
                activeCuesList.setProperty(QIdentifiers.ActiveCues, QOSCKey.UID, false);
                activeCuesList.setProperty("Active Cues", QOSCKey.Name, false);
                activeCuesList.setProperty(QCueType.CueList, QOSCKey.Type, false);
                rootCueUpdated = true;
            }
            currentCueLists.Add(activeCuesList);

            if (root.cues.Count() != currentCueLists.Count())
                rootCueUpdated = true;

            root.setProperty(currentCueLists, QOSCKey.Cues, false) ;

            if (rootCueUpdated)
            {
                //add Event handled? use CueUpdated one?
            }

            Log.Debug($"[workspace] cueLists finished processing. root updated? {rootCueUpdated}");


            OnWorkspaceUpdated();
        }

        private void OnCueListChangedPlaybackPosition(object source, QCueListChangedPlaybackPositionArgs args)
        {
            if (!args.cueID.Equals(QIdentifiers.NoSelection))
            {
                QCue cueList = cueWithID(args.cueListID);
                if (cueList == null)
                    return;
                if (cueList.ignoreUpdates)
                    return;
                cueList.setProperty(args.cueID, QOSCKey.PlaybackPositionId, false);
            }

            Log.Information($"[workspace] cue list <{args.cueListID}> playback position changed to <{args.cueID}>");
            CueListChangedPlaybackPosition?.Invoke(this, new QCueListChangedPlaybackPositionArgs { cueListID = args.cueListID, cueID = args.cueID });

        }

        //OnWorkspaceUpdated
        //OnWorkspaceSettingsUpdated
        //LightDashboardUpdated
        //PreferencesUpdated

        private void OnCueNeedsUpdated(object source, QCueNeedsUpdatedArgs args)
        {
            if (args.cueID.Equals(QIdentifiers.RootCueUpdate))
            {
                Log.Debug("[workspace] root cue update requested, updating all cue lists");
                foreach (var cuelist in this.cueLists)
                {
                    if (!cuelist.uid.Equals(QIdentifiers.ActiveCues))
                        OnCueNeedsUpdated(this, new QCueNeedsUpdatedArgs { cueID = cuelist.uid });
                }
                return;
            }
            else
            {
                QCue cueToUpdate = cueWithID(args.cueID);
            
                if (cueToUpdate == null)
                    return;

                fetchBasicPropertiesForCue(cueToUpdate);
            }

        }

        private void OnCueUpdated(object source, QCueUpdatedArgs args)
        {
            QCue cue = cueWithID(args.cueID);

            if (cue == null || cue.ignoreUpdates)
                return;

            cue.updatePropertiesWithDictionary(args.data);
        }

        private void OnWorkspaceUpdated()
        {
            WorkspaceUpdated?.Invoke(this, new QWorkspaceUpdatedArgs());
        }

        #endregion

        #region Printing
        public void Print()
        {
            Log.Information($"[workspace] {name}");
            foreach (var cueList in root.cues)
            {
                cueList.Print();
            }
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            QWorkspace objAsPart = obj as QWorkspace;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }

        public override int GetHashCode()
        {
            return this.uniqueID.GetHashCode();
        }

        public bool Equals(QWorkspace other)
        {
            return this.uniqueID.Equals(other.uniqueID);
        }
    }
}
