using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpOSC;
using Serilog;

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
            Log.Debug($"[client] setup connection to: <{host}:{port}>");
            tcpClient.MessageReceived += ProcessMessage;
        }

        
        public bool IsConnnected { get { return tcpClient.IsConnected;  } }

        public bool connect()
        {
            if (tcpClient == null)
                return false;
            else
                return tcpClient.Connect();
        }

        public void disconnect()
        {
            tcpClient.Close();
        }


        public void sendMessage(string address, params object[] args)
        {
            tcpClient.Send(new OscMessage(address, args));
            Log.Debug($"[client] send message {address} : {args}");
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
                    if(data.Type == JTokenType.Object)
                    {
                        OnCueUpdated(message.cueID, data);
                    }
                    else if (data.Type == JTokenType.String || data.Type == JTokenType.Integer || data.Type == JTokenType.Float)
                    {
                        string property = message.AddressParts.Last();
                        if (property == null)
                            return;
                        if (property == QOSCKey.PlaybackPositionId)
                        {
                            OnCueListChangedPlaybackPosition(message.cueID, data.ToString());
                            return;
                        }
                        //create object manually since single value replies don't have dictionaries
                        JObject properties = new JObject();
                        properties.Add(property, data);

                        OnCueUpdated(message.cueID, properties);
                    }
                    else
                    {
                        Log.Debug($"[client] unhandled reply from cue: Type: {data.Type} value: {message.response}");
                    }

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
                    Log.Debug($"[client] unhandled reply message: {message.address}");
                }
            }
            else if(message.IsUpdate) {
                if (message.IsCueUpdate)
                {
                    OnCueNeedsUpdated(message.cueID);
                }
                else if (message.IsPlaybackPositionUpdate)
                {
                    string cueListID = message.AddressParts[4];
                    OnCueListChangedPlaybackPosition(cueListID, message.cueID);
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
                    Log.Debug($"[client] disconnect message received: {message.address}");
                    OnWorkspaceDisconnected();
                }
                else
                {
                    Log.Debug($"[client] unhandled update message: {message.address}");
                }
            }
            else
            {
                Log.Debug($"[client] unhandled message: {message.address}");
            }
        }

        protected virtual void OnCueUpdated(string cueID, JToken properties)
        {
            Log.Debug($"[client] cue updated: {cueID}");
            CueUpdated?.Invoke(this, new QCueUpdatedArgs { cueID = cueID, data = properties });
        }
        protected virtual void OnCueNeedsUpdated(string cueID)
        {
            Log.Debug($"[client] cue needs updated: {cueID}");
            CueNeedsUpdated?.Invoke(this, new QCueNeedsUpdatedArgs { cueID = cueID });
        }

        protected virtual void OnCueListsUpdated(JToken response)
        {
            Log.Debug($"[client] Cue Lists Updated");
            CueListsUpdated?.Invoke(this, new QCueListsUpdatedArgs { data = response });
        }

        protected virtual void OnCueListChangedPlaybackPosition(string cueListID, string cueID)
        {
            Log.Debug($"[client] CueList <{cueListID}> Playback Position Changed to <{cueID}>");
            CueListChangedPlaybackPosition?.Invoke(this, new QCueListChangedPlaybackPositionArgs { cueListID = cueListID, cueID = cueID });
        }

        protected virtual void OnWorkspaceUpdated()
        {
            Log.Debug($"[client] Workspace Updated");
            WorkspaceUpdated?.Invoke(this, new QWorkspaceUpdatedArgs());
        }

        protected virtual void OnWorkspaceSettingsUpdated(string settingsType)
        {
            Log.Debug($"[client] Workspace Settings Updated");
            WorkspaceSettingsUpdated?.Invoke(this, new QWorkspaceSettingsUpdatedArgs { settingsType = settingsType });
        }

        protected virtual void OnWorkspaceLightDashboardUpdated()
        {
            Log.Debug($"[client] Workspace Light Dashboard Updated");
            WorkspaceLightDashboardUpdated?.Invoke(this, new QWorkspaceLightDashboardUpdatedArgs());
        }

        protected virtual void OnQLabPreferencesUpdated(string key)
        {
            Log.Debug($"[client] QLab Preferences Updated");
            QLabPreferencesUpdated?.Invoke(this, new QQLabPreferencesUpdatedArgs { key = key });
        }

        protected virtual void OnWorkspaceDisconnected()
        {
            Log.Debug($"[client] Workspace Disconnected");
            WorkspaceDisconnected?.Invoke(this, new QWorkspaceDisconnectedArgs());
        }

        protected virtual void OnWorkspaceConnected()
        {
            WorkspaceConnected?.Invoke(this, new QWorkspaceConnectedArgs());
        }

        protected virtual void OnWorkspaceConnectionError(string status)
        {
            WorkspaceConnectionError?.Invoke(this, new QWorkspaceConnectionErrorArgs { status = status });
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
