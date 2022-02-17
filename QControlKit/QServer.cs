using Serilog;
using System.Collections.Generic;
using Zeroconf;

using QControlKit.Events;
using System.Collections.ObjectModel;
using System.Linq;

namespace QControlKit
{
    public class QServer
    {
        public event QServerUpdatedHandler ServerUpdated;
        public event QServerWorkspaceAddedHandler WorkspaceAdded;
        public event QServerWorkspaceRemovedHandler WorkspaceRemoved;

        private QClient client;

        public string host { get; set; }
        public int port { get; set; }
        public string name { get; set; }
        public string version { get; set; }
        public IZeroconfHost zeroconfHost;
        public ObservableCollection<QWorkspace> workspaces = new ObservableCollection<QWorkspace>();

        public QServer(string host, int port)
        {
            this.host = host;
            this.port = port;

            client = new QClient(host, port);
            client.WorkspacesUpdated += OnServerWorkspacesUpdated;

            if (!client.connect())
            {
                Log.Error($"[server] unable to connect to QLab Server: {host}:{port}");
            }
        }

        public string description { get { return $"{name} - {host} - {port}"; } }

        
        public void refreshWorkspaces()
        {
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
        private void OnServerWorkspacesUpdated(object source, QWorkspacesUpdatedArgs args)
        {
            foreach (var workspace in args.Workspaces)
            {
                QWorkspace existingWorkspace = workspaceWithID(workspace.uniqueID);
                if (existingWorkspace == null)
                {
                    QWorkspace workspaceToAdd = new QWorkspace(workspace, this);
                    workspaces.Add(workspaceToAdd);
                    OnServerAddedWorkspace(workspaceToAdd);
                    //set server version to the version of first workspace found...kind of hacky but eh
                    if (version == null)
                        version = workspaceToAdd.version;
                }
            }
            List<QWorkspace> tempWorkspaces = workspaces.ToList();
            for(int i = 0;i< tempWorkspaces.Count;i++)
            {
                QWorkspace workspace = workspaces[i];
                bool toBeRemoved = true;
                foreach (var foundWorkspace in args.Workspaces)
                {
                    if(workspace.uniqueID == foundWorkspace.uniqueID)
                    {
                        toBeRemoved = false;
                    }
                }
                if (toBeRemoved)
                {
                    workspace.disconnect();
                    workspaces.Remove(workspace);
                    OnServerRemovedWorkspace(workspace);
                }
            }
            OnServerUpdated(this);
        }

        protected virtual void OnServerAddedWorkspace(QWorkspace workspace)
        {
            WorkspaceAdded?.Invoke(this, new QServerWorkspaceChangedArgs { server = this, workspace = workspace });
        }

        protected virtual void OnServerRemovedWorkspace(QWorkspace workspace)
        {
            WorkspaceRemoved?.Invoke(this, new QServerWorkspaceChangedArgs { server = this, workspace = workspace });
        }

        protected virtual void OnServerUpdated(QServer server)
        {
            ServerUpdated?.Invoke(this, new QServerUpdatedArgs { server = server });
        }
        #endregion

        public void disconnect()
        {
            Log.Information($"[server] disconnect requested for server <{name}>");
            foreach (var workspace in workspaces)
            {
                if (workspace.connected)
                {
                    Log.Debug($"[server] Closing workspace <{workspace.name}> still connected to {name}");
                    workspace.disconnect();
                }
            }
            client.disconnect();
        }

    }
}
