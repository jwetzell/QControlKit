using Zeroconf;
using System.Collections.ObjectModel;
using Serilog;

namespace QSharp
{
    public class QBrowser
    {

        ZeroconfResolver.ResolverListener zeroconfTCPBrowser;

        public ObservableCollection<QServer> servers = new ObservableCollection<QServer>();

        public event QServerFoundHandler ServerFound;
        public event QServerUpdatedHandler ServerUpdatedWorkspaces;

        public QBrowser()
        {
            zeroconfTCPBrowser = ZeroconfResolver.CreateListener(QBonjour.TCPService);
            zeroconfTCPBrowser.ServiceFound += ZeroconfHostFound;
            zeroconfTCPBrowser.ServiceLost += ZeroconfHostLost;
        }

        private void ZeroconfHostLost(object sender, IZeroconfHost e)
        {
            Log.Information($"[browser] Lost {e.DisplayName} : {e.IPAddress}");

        }

        private void ZeroconfHostFound(object sender, IZeroconfHost e)
        {
            
            
            foreach(var service in e.Services)
            {
                if (service.Key.Equals(QBonjour.TCPService))
                {
                    Log.Information($"Found {e.DisplayName} : {e.IPAddress} : {service.Value.Port}");

                    QServer server = serverForAddress(e.IPAddress);

                    if(server == null)
                    {
                        QServer serverToAdd = new QServer(e.IPAddress, service.Value.Port);
                        serverToAdd.name = e.DisplayName;
                        serverToAdd.zeroconfHost = e;
                        servers.Add(serverToAdd);
                        serverToAdd.refreshWorkspaces();
                        serverToAdd.ServerUpdated += OnServerUpdatedWorkspace;
                        OnServerFound(serverToAdd);
                    }
                    else
                    {
                        server.name = e.DisplayName;
                    }
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

        protected virtual void OnServerUpdatedWorkspace(object source, QServerUpdatedArgs args)
        {
            ServerUpdatedWorkspaces?.Invoke(this, args);
        }

        public void Close()
        {
            foreach(var server in servers)
            {
                server.disconnect();
            }
            zeroconfTCPBrowser.Dispose();
        }
    }
}