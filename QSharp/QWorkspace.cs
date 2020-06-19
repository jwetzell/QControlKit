//TODO: connect methods, cue property fetch methods, everything else
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace QSharp
{
    public class QWorkspace
    {

        private QServer server;
        private QClient client;

        public string name { get; set; }
        public string uniqueID;
        public bool connected;
        public bool hasPasscode;
        private string passcode;

        private QCue root;

        private Timer heartbeatTimer;
        private int heartbeatAttempts;

        public bool defaultSendUpdatesOSC;

        public string version;


        public event QWorkspaceUpdatedHandler WorkspaceUpdated;
        public event QCueListChangedPlaybackPositionHandler CueListChangedPlaybackPosition;
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
            Log.Debug($"[workspace] <{name}> initizalied for server: {server.name} ");
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
        } //TODO

        public string serverName { get { return server.name; } }

        public string fullName { get { return $"{name} ({server.name})"; } }

        public QCue firstCue { get { return firstCueList.firstCue; } }
        public QCue firstCueList { get { return root.firstCue; } }

        public List<QCue> cueLists { get { return root.cues; } }
        public string fullNameWithCueList(QCue cueList) { return ""; }
        //TODO other convenience methods
        public string[] versionParts { get { return version.Split('.'); } }
        public bool connectedToQLab3 { get { return versionParts[0] == "3"; } }

        #region Connection/reconnection

        public void connectWithPasscode(string passcode)
        {
            //TODO
            if (!client.connect())
            {
                Log.Error($"[workspace] *** Error: couldn't connect to server client is not connected");
                return;
            }

            //save password for reuse
            this.passcode = passcode;
            Log.Information("[workspace] connecting...");
            client.sendMessage($"{workspacePrefix}/connect",passcode);
        }

        private void finishConnection()
        {
            //TODO
            Log.Information($"[workspace] connected to <{name}> running on QLab version <{version}>");
            connected = true;
            startReceivingUpdates();
            //fetchQLabVersion();
            fetchCueLists();

        }

        public void reconnect()
        {
            if (connected)
                return;
            connectWithPasscode(passcode);

            //todo
        }

        public void disconnect()
        {
            Log.Information($"[workspace] disconnect: {name}");
            if (heartbeatTimer != null)
                stopHeartbeat();
            stopReceivingUpdates();
            disconnectFromWorkspace();

            connected = false;
            client.disconnect();
            
            //TODO
            //root.removeAllChildCues();

        }

        public void temporarilyDisconnect()
        {
            //TODO
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
        public void stopReceivingUpdates() { client.sendMessage($"{workspacePrefix}/updates", 0); }
        public void enableAlwaysReply() { client.sendMessage($"{workspacePrefix}/alwaysReply", 1); }
        public void disableAlwaysReply() { client.sendMessage($"{workspacePrefix}/alwaysReply", 0); }
        public void fetchQLabVersion() { client.sendMessage($"{workspacePrefix}/version"); } //TODO: EventHandler for this
        public void fetchCueLists() { client.sendMessage($"{workspacePrefix}/cueLists"); } //TODO: EventHandler for CueListUpdated
        public void fetchPlaybackPositionForCue(QCue cue) { client.sendMessage(addressForCue(cue, QOSCKey.PlaybackPositionId)); } //EventHandler for this? can I use the CueListPlaybackPosition one?
        public void go() { client.sendMessage($"{workspacePrefix}/go"); }
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
        //TODO
        public void startHeartbeat()
        {
            clearHeartbeatTimeout();
            sendHeartbeat();
        }

        public void stopHeartbeat()
        {
            clearHeartbeatTimeout();
            heartbeatAttempts = -1;
        }

        public void clearHeartbeatTimeout()
        {
            heartbeatTimer.Stop();
            heartbeatTimer = null;
            heartbeatAttempts = 0;
            //TODO
        }

        public void sendHeartbeat()
        {
            client.sendMessage("/thump");
            //TODO
        }

        public void heartbeatTimeout(object sender, ElapsedEventArgs e)
        {
            //TODO
        }



        #endregion

        #region Cue Actions
        public void startCue(QCue cue) { client.sendMessage(addressForCue(cue, "start")); }
        public void stopCue(QCue cue) { client.sendMessage(addressForCue(cue, "stop")); }
        public void pauseCue(QCue cue) { client.sendMessage(addressForCue(cue, "pause")); } //TODO: immediately update local for snappier whatever 
        public void loadCue(QCue cue) { client.sendMessage(addressForCue(cue, "load")); }
        public void resetCue(QCue cue) { client.sendMessage(addressForCue(cue, "reset")); }
        public void deleteCue(QCue cue) { client.sendMessage(addressForCue(cue, "")); }
        public void resumeCue(QCue cue) { client.sendMessage(addressForCue(cue, "resume")); }
        public void hardStopCue(QCue cue) { client.sendMessage(addressForCue(cue, "hardStop")); }
        public void hardPauseCue(QCue cue) { client.sendMessage(addressForCue(cue, "hardPause")); } //TODO: immediately update local for snappier whatever 
        public void togglePauseCue(QCue cue) { client.sendMessage(addressForCue(cue, "togglePause")); }
        public void previewCue(QCue cue) { client.sendMessage(addressForCue(cue, "preview")); }
        public void panicCue(QCue cue) { client.sendMessage(addressForCue(cue, "panic")); } //TODO: immediately update local for snappier whatever 
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
        //TODO


        public void fetchDefaultPropertiesForCue(QCue cue)
        {
            string[] keys = new string[] {  QOSCKey.UID, QOSCKey.Number, QOSCKey.Name, 
                                            QOSCKey.ListName, QOSCKey.Type, QOSCKey.ColorName, 
                                            QOSCKey.Flagged, QOSCKey.Armed, QOSCKey.Notes };
            fetchPropertiesForCue(cue, keys, false);
        }

        public void fetchBasicPropertiesForCue(QCue cue)
        {
            string[] keys = new string[] {  QOSCKey.Name, QOSCKey.Number, QOSCKey.FileTarget, QOSCKey.CueTargetNumber, 
                                            QOSCKey.HasFileTargets, QOSCKey.HasCueTargets, QOSCKey.Armed, QOSCKey.ColorName, 
                                            QOSCKey.ContinueMode, QOSCKey.Flagged, QOSCKey.PreWait, QOSCKey.PostWait, 
                                            QOSCKey.Duration, QOSCKey.AllowsEditingDuration };
            fetchPropertiesForCue(cue, keys, false);
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
                this.passcode = "";
                Log.Error($"[workspace] *** Password for workspace {name} was incorrect!");
            }                
            else
                Log.Error($"[workspace] *** Unable to connect to workspace: {name} on server: {server.name}");

        }

        private void OnWorkspaceConnected(object source, QWorkspaceConnectedArgs args)
        {
            Log.Debug($"[workspace] Connection being finished.");
            finishConnection();
        }

        private void OnWorkspaceDisconnected(object source, QWorkspaceDisconnectedArgs args)
        {
            //this might not be called with TCP?
            Log.Warning($"[workspace] *** Workspace has indicated it is disconnecting");
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
            QCue cueList = cueWithID(args.cueListID);

            if (cueList == null)
                return;
            if (cueList.ignoreUpdates)
                return;

            cueList.setProperty(args.cueID, QOSCKey.PlaybackPositionId, false);

            QCue selectedCue = cueWithID(args.cueID);
            if (selectedCue != null)
            {
                Log.Information($"[workspace] cue list <{cueList.displayName}> playback position changed to <{selectedCue.displayName}>");
                CueListChangedPlaybackPosition?.Invoke(this, new QCueListChangedPlaybackPositionArgs { cueListID = cueList.uid, cueID = selectedCue.uid });

            }


        }

        //OnWorkspaceUpdated
        //OnWorkspaceSettingsUpdated
        //LightDashboardUpdated
        //PreferencesUpdated

        private void OnCueNeedsUpdated(object source, QCueNeedsUpdatedArgs args)
        {
            QCue cueToUpdate = cueWithID(args.cueID);
            if (cueToUpdate == null)
                return;

            fetchBasicPropertiesForCue(cueToUpdate);
        }

        private void OnCueUpdated(object source, QCueUpdatedArgs args)
        {
            QCue cue = cueWithID(args.cueID);

            if (cue == null)
                return;
            if (cue.ignoreUpdates)
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
            Log.Information("[workspace] End of Workspace");
        }
        #endregion
    }
}
