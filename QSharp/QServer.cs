//General information about a QLab Workspace
using System.Collections.Concurrent;
using System.Collections.Generic;
using Zeroconf;

namespace QSharp
{
    public class QServer
    {
        private QClient client;

        public string host;
        public int port;
        public string name;
        public IZeroconfHost zeroconfHost;
        List<QWorkspace> workspaces = new List<QWorkspace>();

        public QServer(string host, int port)
        {
            this.host = host;
            this.port = port;

            client = new QClient(host, port);
            client.WorkspacesUpdated += OnWorkspacesUpdated;
        }

        public string description { get { return $"{name} - {host} - {port}"; } }

        
        public void refreshWorkspaces()
        {
            if (!client.connect())
            {
                System.Console.WriteLine($"Error: QServer unable to connect to QLab Server: {host}:{port}");
                return;
            }
            client.sendMessage("/workspaces");
        }

        public QWorkspace workspaceWithID(string uniqueID)
        {
            foreach(var workspace in workspaces)
            {
                if (workspace.uniqueID.Equals(uniqueID))
                    return workspace;
            }
            return null;
        }

        #region EventHandlers
        private void OnWorkspacesUpdated(object source, QWorkspacesUpdatedArgs args)
        {
            foreach (var workspace in args.Workspaces)
            {
                QWorkspace existingWorkspace = workspaceWithID(workspace.uniqueID);
                if (existingWorkspace == null)
                {
                    QWorkspace workspaceToAdd = new QWorkspace(workspace, this);
                    workspaces.Add(workspaceToAdd);
                    workspaceToAdd.connectWithPasscode("1234");
                }
            }
        }
        #endregion

    }
}
