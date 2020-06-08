//General information about a QLab Workspace
using System.Collections.Generic;

namespace QSharp
{
    public class QServer
    {
        private QClient qClient;

        public string host;
        public int port;
        public string name;
        List<QWorkspace> workspaces = new List<QWorkspace>();

        public QServer(string host, int port, string name)
        {
            this.host = host;
            this.port = port;
            this.name = name;

            qClient = new QClient(host, port);
            qClient.WorkspacesUpdated += OnWorkspacesUpdated;
        }

        private void OnWorkspacesUpdated(object source, QClient.WorkspacesUpdatedArgs args)
        {
            foreach(var workspace in args.Workspaces)
            {
                QWorkspace existingWorkspace = workspaceWithID(workspace.uniqueID);
                if(existingWorkspace == null)
                {
                    QWorkspace workspaceToAdd = new QWorkspace(workspace, this);
                    workspaces.Add(workspaceToAdd);
                }
            }
        }

        public void refreshWorkspaces()
        {
            qClient.sendMessage("/workspaces");
        }

        public QWorkspace workspaceWithID(string uniqueID)
        {
            foreach(var workspace in workspaces)
            {
                if (workspace.getUniqueID().Equals(uniqueID))
                    return workspace;
            }
            return null;
        }
    }
}
