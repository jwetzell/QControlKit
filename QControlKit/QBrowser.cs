//TODO period updating of servers, proper server removing/workspace updating
using Zeroconf;
using System.Collections.ObjectModel;
using Serilog;

using QControlKit.Constants;
using QControlKit.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QControlKit
{
    public class QBrowser
    {
        private ILogger _log = Log.Logger.ForContext<QBrowser>();

        public ObservableCollection<QServer> servers = new ObservableCollection<QServer>();

        public event QServerFoundHandler ServerFound;
        public event QServerLostHandler ServerLost;
        public event QServerUpdatedHandler ServerUpdatedWorkspaces;

        public QBrowser()
        {
            ProbeForQLabInstances();
        }

        public async void ProbeForQLabInstances()
        {
            _log.Debug("probing for instances");

            IReadOnlyList<IZeroconfHost> results = await
                    ZeroconfResolver.ResolveAsync(QBonjour.TCPService,TimeSpan.FromSeconds(3));

            foreach (var zeroconfHost in results)
            {
                _log.Debug($"found host {zeroconfHost.IPAddress}:{zeroconfHost.DisplayName}");
                foreach (var service in zeroconfHost.Services)
                {
                    _log.Debug($"found {service.Key}:{service.Value}");
                    if (service.Key.Equals(QBonjour.TCPService))
                    {

                        QServer server = serverForAddress(zeroconfHost.IPAddress);

                        if (server == null)
                        {
                            _log.Information($"Found {zeroconfHost.DisplayName} : {zeroconfHost.IPAddress} : {service.Value.Port}");
                            QServer serverToAdd = new QServer(zeroconfHost.IPAddress, service.Value.Port);
                            serverToAdd.name = zeroconfHost.DisplayName;
                            serverToAdd.zeroconfHost = zeroconfHost;
                            servers.Add(serverToAdd);
                            serverToAdd.refreshWorkspaces();
                            serverToAdd.ServerUpdated += OnServerUpdatedWorkspace;
                            OnServerFound(serverToAdd);
                        }
                        else
                        {
                            server.name = zeroconfHost.DisplayName;
                            server.refreshWorkspaces();
                        }
                    }
                }
            }

            List<QServer> qServers = servers.ToList();

            foreach(var server in qServers)
            {
                IZeroconfHost found = results.ToList().Find(x => x.IPAddress.Equals(server.host));

                if (found == null)
                {
                    _log.Information($"Lost {server.name} : {server.host} : {server.port} disconnecting");
                    server.disconnect();
                    _log.Verbose($"after server disconnect()");
                    servers.Remove(server);
                    OnServerLost(server);
                }
            }
        }

        public QServer serverForAddress(string address)
        {
            foreach(var server in servers)
            {
                if (server.host.Equals(address))
                {
                    return server;
                }
            }

            return null;
        }

        public QServer serverForIZeroconfHost(IZeroconfHost zeroconfHost)
        {
            foreach (var server in servers)
            {
                if (server.zeroconfHost == zeroconfHost)
                {
                    return server;
                }
            }

            return null;
        }

        protected virtual void OnServerFound(QServer server)
        {
            ServerFound?.Invoke(this, new QServerFoundArgs { server = server });
        }

        protected virtual void OnServerLost(QServer server)
        {
            ServerLost?.Invoke(this, new QServerLostArgs { server = server });
        }

        protected virtual void OnServerUpdatedWorkspace(object source, QServerUpdatedArgs args)
        {
            ServerUpdatedWorkspaces?.Invoke(this, args);
        }

        public void Close()
        {
            _log.Information($"Close requested");
            foreach (var server in servers)
            {
                server.disconnect();
            }
        }
    }
}