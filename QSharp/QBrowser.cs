using Zeroconf;
using System;
using System.Collections.ObjectModel;

namespace QSharp
{
    public class QBrowser
    {

        ZeroconfResolver.ResolverListener zeroconfTCPBrowser;

        public ObservableCollection<QServer> servers = new ObservableCollection<QServer>();

        public event QServerFoundHandler ServerFound;


        public QBrowser()
        {

            Console.WriteLine("QBrowser Init");
            zeroconfTCPBrowser = ZeroconfResolver.CreateListener(QBonjour.TCPService);
            zeroconfTCPBrowser.ServiceFound += ZeroconfHostFound;
            zeroconfTCPBrowser.ServiceLost += ZeroconfHostLost;
        }

        private void ZeroconfHostLost(object sender, IZeroconfHost e)
        {
            Console.WriteLine($"Lost {e.DisplayName} : {e.IPAddress}");

        }

        private void ZeroconfHostFound(object sender, IZeroconfHost e)
        {
            
            
            foreach(var service in e.Services)
            {
                if (service.Key.Equals(QBonjour.TCPService))
                {
                    Console.WriteLine($"Found {e.DisplayName} : {e.IPAddress} : {service.Value.Port}");

                    QServer server = serverForAddress(e.IPAddress);

                    if(server == null)
                    {
                        Console.WriteLine("New Server Found so adding");
                        QServer serverToAdd = new QServer(e.IPAddress, service.Value.Port);
                        serverToAdd.name = e.DisplayName;
                        serverToAdd.zeroconfHost = e;
                        servers.Add(serverToAdd);
                        serverToAdd.refreshWorkspaces();
                        OnServerFound(serverToAdd);
                    }
                    else
                    {
                        server.name = e.DisplayName;
                        Console.WriteLine("Server already exists in list so just updating name");
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
            if (ServerFound != null)
                ServerFound(this, new QServerFoundArgs { server = server });

        }
    }
}