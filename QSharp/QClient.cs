using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SharpOSC;

namespace QSharp
{
    public class QClient
    {
        TCPClient tcpClient;

        public class WorkspacesUpdatedArgs : EventArgs
        {
            public List<QWorkspaceInfo> Workspaces { get; set; }
        }


        public delegate void WorkspacesUpdatedHandler(object source, WorkspacesUpdatedArgs args);
        public event WorkspacesUpdatedHandler WorkspacesUpdated;

        public QClient(string host, int port)
        {
            tcpClient = new TCPClient(host, port);
            tcpClient.Connect();
            Console.WriteLine($"QClient setup {host}:{port}");
            tcpClient.MessageReceived += ResponseReceived;
        }

        private void ResponseReceived(object source, MessageEventArgs args)
        {
            OscMessage msg = args.Message;
            foreach (var obj in msg.Arguments)
            {
                QResponse response = JsonConvert.DeserializeObject<QResponse>(obj.ToString());

                switch (response.getReplyType())
                {
                    case "WORKSPACES":
                        Console.WriteLine("WORKSPACES RECEIVED");
                        OnWorkspacesUpdated(response);
                        break;
                    default:
                        break;
                }
                
            }
        }

        public void sendMessage(string address, params object[] args)
        {
            tcpClient.Send(new OscMessage(address, args));
            Console.WriteLine($"QClient send message {address}");
        }

        protected virtual void OnWorkspacesUpdated(QResponse response)
        {
            if(WorkspacesUpdated != null)
            {
                List<QWorkspaceInfo> workspaces = new List<QWorkspaceInfo>();
                foreach (var item in response.data)
                {
                    QWorkspaceInfo workspacefound = JsonConvert.DeserializeObject<QWorkspaceInfo>(item.ToString());
                    workspaces.Add(workspacefound);
                    Console.WriteLine(workspacefound.displayName);
                }
                WorkspacesUpdated(this, new WorkspacesUpdatedArgs { Workspaces = workspaces });
            }
        }
    }
}
