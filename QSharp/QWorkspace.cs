//TODO: connect methods, cue property fetch methods, everything else
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Timers;

namespace QSharp
{
    public class QWorkspace
    {

        private QServer server;
        private QClient client;

        public string name;
        public string uniqueID;
        public bool connected;
        private bool hasPasscode;
        private string passcode;

        private QCue root;

        private Timer heartbeatTimer;
        private int heartbeatAttempts;

        public bool defaultSendUpdatesOSC;

        public string version;

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
            
            client.WorkspaceConnected += OnWorkspaceConnection;
            client.WorkspaceConnectionError += OnWorkspaceConnectionError;
            client.WorkspaceDisconnected += OnWorkspaceDisconnected;
            client.CueListsUpdated += OnCueListsUpdated;

            this.server = server;
            Console.WriteLine($"[workspace] <{name}> initizalied for server: {server.name} ");
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

        public string nameWithoutPathExtension { get { return name;  } } //TODO

        public string serverName { get { return server.name; } }

        public string fullName { get { return $"{name} ({server.name})"; } }

        public QCue firstCue { get { return firstCueList.firstCue; } }
        public QCue firstCueList { get { return root.firstCue; } }
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
                System.Console.WriteLine($"[workspace] *** Error: couldn't connect to server client is not connected");
                return;
            }

            //save password for reuse
            this.passcode = passcode;
            System.Console.WriteLine("[workspace] connecting...");
            client.sendMessage($"{workspacePrefix}/connect",passcode);
        }

        private void finishConnection()
        {
            //TODO
            System.Console.WriteLine($"[workspace] connected to <{name}> running on QLab version <{version}>");
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
            //TODO
            Console.WriteLine($"[workspace] disconnect: {name}");
            stopHeartbeat();
            stopReceivingUpdates();
            disconnectFromWorkspace();

            connected = false;

            client.disconnect();
            //root.removeAllChildCues();

        }

        public void temporarilyDisconnect()
        {
            //TODO
        }

        #endregion  

        #region Cues
        public QCue cueWithId(string uid)
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

        //TODO
        public void valuesForKeys(QCue cue, string[] keys) { }
        //TODO
        public void updatePropertySend(QCue cue, object value, string key) { }
        //TODO
        public void updatePropertiesSend(QCue cue, object[] values, string key) { }
        //TODO
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

        public void fetchPropertiesForCue(QCue cue, string[] keys)
        {

        }
        #endregion

        #region OSC address helpers
        public void sendMessage(string address, params object[] args)
        {
            //check for workspace prefix?
            client.sendMessage(address, args);
        }

        private string addressForCue(QCue cue, string action)
        {
            return $"{workspacePrefix}/cue_id/{cue.propertyForKey("uniqueId")}/{action}";
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
                Console.WriteLine($"[workspace] *** Error: Password for workspace {name} was incorrect!");
            }                
            else
                Console.WriteLine($"[workspace] *** Error: Unable to connect to workspace: {name} on server: {server.name}");

        }

        private void OnWorkspaceConnection(object source, QWorkspaceConnectedArgs args)
        {
            finishConnection();
            Console.WriteLine($"[workspace] Connection finished this is the first cue in this workspace {root.firstCue.displayName}");
        }

        private void OnWorkspaceDisconnected(object source, QWorkspaceDisconnectedArgs args)
        {
            //this might not be called with TCP?
            Console.WriteLine($"[workspace] *** Workspace has indicated it is disconnecting");
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

            root.setProperty(currentCueLists, QOSCKey.Cues);

            if (rootCueUpdated)
            {
                //add Event handled? use CueUpdated one?
            }

            Console.WriteLine($"[workspace] cueLists finished processing. root updated? {rootCueUpdated}");

            Print();
        }


        //OnWorkspaceUpdated
        //OnWorkspaceSettingsUpdated
        //LightDashboardUpdated
        //PreferencesUpdated
        //CueNeedsUpdated
        //CueUpdated
        //ClientShouldDisconnect
        #endregion


        #region Printing
        public void Print()
        {
            foreach(var cueList in root.cues)
            {
                cueList.Print();
            }
        }
        #endregion
    }
}
