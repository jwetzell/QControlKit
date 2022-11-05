using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpOSC;
using Serilog;

using QControlKit.Events;
using QControlKit.Constants;

namespace QControlKit
{
    public class QClient
    {
        private ILogger _log = Log.Logger.ForContext<QClient>();

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
            _log.Debug($"setup connection to: <{host}:{port}>");
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
            _log.Information($"disconnecting from {tcpClient.Address}");
            tcpClient.Close();
        }


        public void sendMessage(string address, params object[] args)
        {
            tcpClient.Send(new OscMessage(address, args));
            _log.Debug($"send message {address} : {args}");
        }

        private void ProcessMessage(object source, MessageEventArgs args)
        {

            QMessage message = new QMessage(args.Message);

            Log.Verbose(message.ToString());


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
                        //create object manually since single value replies don't have dictionaries
                        JObject properties = new JObject();
                        properties.Add(property, data);

                        OnCueUpdated(message.cueID, properties);
                        if (property == QOSCKey.PlaybackPositionId)
                        {
                            OnCueListChangedPlaybackPosition(message.cueID, data.ToString());
                        }
                    }
                    else
                    {
                        _log.Error($"unhandled reply from cue: Type: {data.Type} value: {message.response}");
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
                    if (message.response.ToString().Contains("ok"))
                        OnWorkspaceConnected();
                    else
                        OnWorkspaceConnectionError(message.response.ToString());
                }
                else
                {
                    _log.Error($"unhandled reply message: {message.address}");
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
                    OnWorkspaceDisconnected();
                }
                else
                {
                    _log.Error($"unhandled update message: {message.address}");
                }
            }
            else
            {
                _log.Error($"unhandled message: {message.address}");
            }
        }

        protected virtual void OnCueUpdated(string cueID, JToken properties)
        {
            _log.Debug($"cue updated: {cueID}");
            CueUpdated?.Invoke(this, new QCueUpdatedArgs { cueID = cueID, data = properties });
        }
        protected virtual void OnCueNeedsUpdated(string cueID)
        {
            _log.Debug($"cue needs updated: {cueID}");
            CueNeedsUpdated?.Invoke(this, new QCueNeedsUpdatedArgs { cueID = cueID });
        }

        protected virtual void OnCueListsUpdated(JToken response)
        {
            _log.Debug($"Cue Lists Updated");
            CueListsUpdated?.Invoke(this, new QCueListsUpdatedArgs { data = response });
        }

        protected virtual void OnCueListChangedPlaybackPosition(string cueListID, string cueID)
        {
            _log.Debug($"CueList <{cueListID}> Playback Position Changed to <{cueID}>");
            CueListChangedPlaybackPosition?.Invoke(this, new QCueListChangedPlaybackPositionArgs { cueListID = cueListID, cueID = cueID });
        }

        protected virtual void OnWorkspaceUpdated()
        {
            _log.Debug($"Workspace Updated");
            WorkspaceUpdated?.Invoke(this, new QWorkspaceUpdatedArgs());
        }

        protected virtual void OnWorkspaceSettingsUpdated(string settingsType)
        {
            _log.Debug($"Workspace Settings Updated");
            WorkspaceSettingsUpdated?.Invoke(this, new QWorkspaceSettingsUpdatedArgs { settingsType = settingsType });
        }

        protected virtual void OnWorkspaceLightDashboardUpdated()
        {
            _log.Debug($"Workspace Light Dashboard Updated");
            WorkspaceLightDashboardUpdated?.Invoke(this, new QWorkspaceLightDashboardUpdatedArgs());
        }

        protected virtual void OnQLabPreferencesUpdated(string key)
        {
            _log.Debug($"QLab Preferences Updated");
            QLabPreferencesUpdated?.Invoke(this, new QQLabPreferencesUpdatedArgs { key = key });
        }

        protected virtual void OnWorkspaceDisconnected()
        {
            _log.Debug($"Workspace Disconnected");
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
