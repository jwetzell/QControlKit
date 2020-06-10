using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpOSC;

namespace QSharp
{
    public class QClient
    {
        TCPClient tcpClient;

        public event QWorkspacesUpdatedHandler WorkspacesUpdated;
        
        public event QCueUpdatedHandler CueUpdated;
        public event QCueListsUpdatesHandler CueListsUpdated;
        public event QCueNeedsUpdatedHandler CueNeedsUpdated;

        public event QCueListChangedPlaybackPositionHandler CueListChangedPlaybackPosition;

        public event QWorkspaceUpdatedHandler WorkspaceUpdated;
        public event QWorkspaceSettingsUpdatedHandler WorkspaceSettingsUpdated;

        public event QWorkspaceLightDashboardUpdatedHandler WorkspaceLightDashboardUpdated;
        public event QQLabPreferencesUpdatedHandler QLabPreferencesUpdated;

        public event QWorkspaceDisconnectedHandler WorkspaceDisconnected;
        public event QWorkspaceConnectedHandler WorkspaceConnected;
        public event QWorkspaceConnectionErrorHandler WorkspaceConnectionError;

        public QClient(string host, int port)
        {
            tcpClient = new TCPClient(host, port);
            Console.WriteLine($"[QClient] setup connection to: <{host}:{port}>");
            tcpClient.MessageReceived += ProcessMessage;
        }

        
        public bool IsConnnected { get { return tcpClient.IsConnected;  } }

        public bool connect()
        {
            return tcpClient.Connect();
        }

        public void disconnect()
        {
            tcpClient.Close();
        }


        public void sendMessage(string address, params object[] args)
        {
            tcpClient.Send(new OscMessage(address, args));
           // Console.WriteLine($"QClient send message {address}");
        }

        private void ProcessMessage(object source, MessageEventArgs args)
        {

            QMessage message = new QMessage(args.Message);

            if (message.IsReply)
            {
                JToken data = message.response;
                //special case, want to update cue properties
                if (message.IsReplyFromCue)
                {
                    //todo check data type?
                }
                else if (message.IsReplyFromCueLists)
                {
                    OnCueListsUpdated(message.response);
                }
                else if (message.IsWorkspacesInfo)
                {
                    OnWorkspacesUpdated(message);
                }
                else if (message.IsConnect)
                {
                    if (message.response.ToString() == "ok")
                        OnWorkspaceConnected();
                    else
                        OnWorkspaceConnectionError(message.response.ToString());
                }
                else
                {
                    Console.WriteLine($"[client] unhandled reply message: {message.address}");
                }
            }
            else if(message.IsUpdate) {
                if (message.IsCueUpdate)
                {
                    OnCueNeedsUpdated(message.cueID);
                }
                else if (message.IsPlaybackPositionUpdate)
                {
                    OnCueListChangedPlaybackPosition(message.cueID);
                }
                else if (message.IsWorkspaceUpdate)
                {
                    OnWorkspaceUpdated();
                }
                else if (message.IsWorkspaceSettingsUpdate)
                {
                    string settingsType = message.AddressParts.Last();
                    if (settingsType == null)
                        return;
                    OnWorkspaceSettingsUpdated(settingsType);
                }
                else if (message.IsLightDashboardUpdate)
                {
                    //need to do checks for 4.2 or newer
                    OnWorkspaceLightDashboardUpdated();
                }
                else if ( message.IsPreferencesUpdate)
                {
                    //need to do checks for 4.2 or newer

                    string key = message.AddressParts.Last();
                    if (key == null)
                        return;
                    OnQLabPreferencesUpdated(key);
                }
                else if (message.IsDisconnect)
                {
                    Console.WriteLine($"[client] disconnect message received: {message.address}");
                    OnWorkspaceDisconnected();
                }
                else
                {
                    Console.WriteLine($"[client] unhandled update message: {message.address}");
                }
            }
            else
            {
                Console.WriteLine($"[client] unhandled message: {message.address}");
            }
        }

        protected virtual void OnCueNeedsUpdated(string cueID)
        {
            Console.WriteLine($"[client] cue needs updated: {cueID}");
            if (CueNeedsUpdated != null)
                CueNeedsUpdated(this, new QCueNeedsUpdatedArgs { cueID = cueID });
        }

        protected virtual void OnCueListsUpdated(JToken response)
        {
            Console.WriteLine($"[client] Cue Lists Updated");
            if (CueListsUpdated != null)
                CueListsUpdated(this, new QCueListsUpdatedArgs { data = response });
        }

        protected virtual void OnCueListChangedPlaybackPosition(string cueID)
        {
            Console.WriteLine($"[client] Playback Position Changed: {cueID}");
            if (CueListChangedPlaybackPosition != null)
                CueListChangedPlaybackPosition(this, new QCueListChangedPlaybackPositionArgs { cueID = cueID });
        }

        protected virtual void OnWorkspaceUpdated()
        {
            Console.WriteLine($"[client] Workspace Updated");
            if (WorkspaceUpdated != null)
                WorkspaceUpdated(this, new QWorkspaceUpdatedArgs());
        }

        protected virtual void OnWorkspaceSettingsUpdated(string settingsType)
        {
            Console.WriteLine($"[client] Workspace Settings Updated");
            if (WorkspaceSettingsUpdated != null)
                WorkspaceSettingsUpdated(this, new QWorkspaceSettingsUpdatedArgs { settingsType = settingsType});
        }

        protected virtual void OnWorkspaceLightDashboardUpdated()
        {
            Console.WriteLine($"[client] Workspace Light Dashboard Updated");
            if (WorkspaceLightDashboardUpdated != null)
                WorkspaceLightDashboardUpdated(this, new QWorkspaceLightDashboardUpdatedArgs());
        }

        protected virtual void OnQLabPreferencesUpdated(string key)
        {
            Console.WriteLine($"[client] QLab Preferences Updated");
            if (QLabPreferencesUpdated != null)
                QLabPreferencesUpdated(this, new QQLabPreferencesUpdatedArgs { key = key });
        }

        protected virtual void OnWorkspaceDisconnected()
        {
            Console.WriteLine($"[client] Workspace Disconnected");
            if (WorkspaceDisconnected != null)
                WorkspaceDisconnected(this, new QWorkspaceDisconnectedArgs());
        }

        protected virtual void OnWorkspaceConnected()
        {
            if (WorkspaceConnected != null)
                WorkspaceConnected(this, new QWorkspaceConnectedArgs());
        }

        protected virtual void OnWorkspaceConnectionError(string status)
        {
            if (WorkspaceConnectionError != null)
                WorkspaceConnectionError(this, new QWorkspaceConnectionErrorArgs { status = status});
        }

        protected virtual void OnWorkspacesUpdated(QMessage message)
        {
            if(WorkspacesUpdated != null)
            {
                List<QWorkspaceInfo> workspaces = new List<QWorkspaceInfo>();
                foreach (var item in message.response)
                {
                    QWorkspaceInfo workspacefound = JsonConvert.DeserializeObject<QWorkspaceInfo>(item.ToString());
                    workspaces.Add(workspacefound);
                }
                WorkspacesUpdated(this, new QWorkspacesUpdatedArgs { Workspaces = workspaces });
            }
        }

        
    }
}
