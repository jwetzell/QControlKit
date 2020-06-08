//TODO: connect methods, cue property fetch methods, everything else
namespace QSharp
{
    public class QWorkspace
    {

        private QServer server;
        private QClient client;

        private string name;
        private string uniqueID;
        private bool connected;
        private bool hasPasscode;
        private string oscPrefix;

        private void Init()
        {
            name = "";
            uniqueID = "";
            connected = false;

            hasPasscode = false;

            //TODO: add version 
        }

        public QWorkspace(QWorkspaceInfo workspaceInfo, QServer server)
        {
            if(workspaceInfo.uniqueID.Length > 0)
            {
                uniqueID = workspaceInfo.uniqueID;
                oscPrefix = workspacePrefix();
            }

            //TODO add version if it has one if no version then it is QLab 3?
            updateWithWorkspaceInfo(workspaceInfo);

            client = new QClient(server.host, server.port);
            this.server = server;

        }

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

        

        

        public string getUniqueID()
        {
            return uniqueID;
        }

        #region OSC address helpers
        private string addressForCue(QCue cue, string action)
        {
            return $"{oscPrefix}/cue_id/{cue.propertyForKey("uniqueId")}/{action}";
        }

        private string addressForWildcardNumber(string number, string action) {
            return $"{oscPrefix}/cue/{number}/{action}";
        }

        private string workspacePrefix()
        {
            return $"/workspace/{uniqueID}";
        }
        #endregion

        #region Workspace Methods
        public void disconnectFromWorkspace() { client.sendMessage($"{oscPrefix}/disconnect"); }
        public void startReceivingUpdates() { client.sendMessage($"{oscPrefix}/updates", 1); }
        public void stopReceivingUpdates() { client.sendMessage($"{oscPrefix}/updates", 0); }
        public void enableAlwaysReply() { client.sendMessage($"{oscPrefix}/alwaysReply", 1); }
        public void disableAlwaysReply() { client.sendMessage($"{oscPrefix}/alwaysReply", 0); }

        public void fetchQLabVersion() {
            //TODO
        }

        public void fetchCueLists()
        {
            //TODO
        }

        public void fetchPlaybackPositionForCue()
        {
            //TODO
        }

        public void go() { client.sendMessage($"{oscPrefix}/go"); }
        public void save() { client.sendMessage($"{oscPrefix}/save"); }
        public void undo() { client.sendMessage($"{oscPrefix}/undo"); }
        public void redo() { client.sendMessage($"{oscPrefix}/redo"); }
        public void resetAll() { client.sendMessage($"{oscPrefix}/reset"); }
        public void pauseAll() { client.sendMessage($"{oscPrefix}/pause"); }
        public void resumeAll() { client.sendMessage($"{oscPrefix}/resume"); }
        public void stopAll() { client.sendMessage($"{oscPrefix}/stop"); }
        public void panicAll() { client.sendMessage($"{oscPrefix}/panic"); }
        #endregion
    }
}
