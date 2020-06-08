using Zeroconf;
using System;
using System.Collections.ObjectModel;

namespace QSharp
{
    public class QBrowser
    {
        const string UDPServiceType = "_qlab._udp.local."; 
        const string TCPServiceType = "_qlab._tcp.local.";

        const int udpPort = 53000;

        IObservable<IZeroconfHost> netServices;
        ZeroconfResolver.ResolverListener netServiceTCPBrowser;

        ObservableCollection<QServer> servers = new ObservableCollection<QServer>();

        public QBrowser()
        {

            Console.WriteLine("QBrowser Init");
            netServiceTCPBrowser = ZeroconfResolver.CreateListener(TCPServiceType);
            netServiceTCPBrowser.ServiceFound += ServerFound;
            netServiceTCPBrowser.ServiceLost += ServerLost;
        }

        private void ServerLost(object sender, IZeroconfHost e)
        {
            Console.WriteLine($"Lost {e.DisplayName} : {e.IPAddress}");

        }

        private void ServerFound(object sender, IZeroconfHost e)
        {
            
            
            foreach(var service in e.Services)
            {
                if (service.Key.Equals(TCPServiceType))
                {
                    Console.WriteLine($"Found {e.DisplayName} : {e.IPAddress} : {service.Value.Port}");

                    QServer server = getServerFromAddress(e.IPAddress);

                    if(server == null)
                    {
                        Console.WriteLine("New Server Found so adding");
                        QServer serverToAdd = new QServer(e.IPAddress, service.Value.Port, e.DisplayName);
                        servers.Add(serverToAdd);
                        serverToAdd.refreshWorkspaces();
                    }
                    else
                    {
                        server.name = e.DisplayName;
                        Console.WriteLine("Server already exists in list so just updating name");
                    }
                }
            }

        }

        public QServer getServerFromAddress(string address)
        {
            QServer foundServer = null;
            foreach(var server in servers)
            {
                if (server.host.Equals(address))
                {
                    return server;
                }
            }

            return foundServer;
        }
    }
}