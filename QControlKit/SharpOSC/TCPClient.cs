using System;
using System.Collections.Generic;
using System.Threading;
using Serilog;
using SuperSimpleTcp;

namespace SharpOSC
{
    public class MessageEventArgs : EventArgs
    {
        public OscMessage Message
        {
            get;
            set;
        }
    }

    public class TCPClient
    {
        public int Port
        {
            get { return _port; }
        }
        int _port;

        public string Address
        {
            get { return _address; }
        }
        public delegate void MessageReceivedHandler(object source, MessageEventArgs args);
        public event MessageReceivedHandler MessageReceived;

        private Queue<OscPacket> SendQueue = new Queue<OscPacket>();

        private Thread receivingThread;
        private Thread sendThread;

        string _address;

        byte END = 0xc0;
        byte ESC = 0xdb;
        byte ESC_END = 0xDC;
        byte ESC_ESC = 0xDD;

        SimpleTcpClient simpleClient;


        public TCPClient(string address, int port)
        {
            _port = port;
            _address = address;

            simpleClient = new SimpleTcpClient(address, port);
            simpleClient.Events.Connected += Connected;
            simpleClient.Events.Disconnected += Disconnected;
            simpleClient.Events.DataReceived += DataReceived;
            simpleClient.Logger = Log.Debug;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

        private void DataReceived(object sender, DataReceivedEventArgs e)
        {
            List<byte[]> messages = SlipDecode(e.Data);
            for(int i = 0; i < messages.Count; i++)
            {
                OscPacket packet = OscPacket.GetPacket(messages[i]);
                OscMessage responseMessage = (OscMessage)packet;
                OnMessageReceived(responseMessage);
            }
        }

        private void Disconnected(object sender, ConnectionEventArgs e)
        {
            Log.Debug("[tcpClient] connection to " + e.IpPort + " was disconnected");
        }

        private void Connected(object sender, ConnectionEventArgs e)
        {
            Log.Debug($"[tcpclient] connected to <{e.IpPort}>");
        }

        public bool Connect()
        {
            try
            {
                simpleClient.Connect();

                sendThread = new Thread(SendLoop);
                sendThread.IsBackground = true;
                sendThread.Start();

                
                return true;
            } 
            catch (Exception e)
            {
                Log.Error(e.Message);
                return false;
            }
        }

        public void QueueForSending(OscPacket packet)
        {
            SendQueue.Enqueue(packet);
        }

        private void SendLoop()
        {
            while (simpleClient != null && simpleClient.IsConnected)
            {
                if(SendQueue.Count > 0)
                {
                    OscPacket packet = SendQueue.Dequeue();
                    Send(packet);
                }
            }
        }

        public void Send(byte[] message)
        {
            byte[] slipData = SlipEncode(message);
            simpleClient.Send(slipData);
        }

        public void Send(OscPacket packet)
        {
            byte[] data = packet.GetBytes();
            Send(data);
        }

        public bool IsConnected
        {
            get
            {
                if (simpleClient == null)
                    return false;
                else
                    return simpleClient.IsConnected;
            }
        }

        public byte[] SlipEncode(byte[] data)
        {
            List<byte> slipData = new List<byte>();

            byte[] esc_end = { ESC, ESC_END };
            byte[] esc_esc = { ESC, ESC_ESC };
            byte[] end = { END };

            int length = data.Length;
            for (int i = 0; i < length; i++)
            {
                if (data[i] == END)
                {
                    slipData.AddRange(esc_end);
                }
                else if (data[i] == ESC)
                {
                    slipData.AddRange(esc_esc);
                }
                else
                {
                    slipData.Add(data[i]);
                }
            }
            slipData.AddRange(end);
            return slipData.ToArray();
        }

        public List<byte[]> SlipDecode(byte[] data)
        {
            List<byte[]> messages = new List<byte[]>();

            List<byte> buffer = new List<byte>();
            for(int i = 0; i < data.Length; i++)
            {
                if(data[i] == END && buffer.Count > 0)
                {
                    messages.Add(buffer.ToArray());
                    buffer.Clear();
                }
                else if (data[i] != END)
                {
                    buffer.Add(data[i]);
                }
            }
            return messages;
        }

        public void Close()
        {
            Log.Information("[tcpclient] Close() was requested");
            if (simpleClient != null)
            {
                if (simpleClient.IsConnected)
                {
                    Log.Information($"[tcpClient] closing connection to {Address}");
                    simpleClient.Disconnect();
                }
            }
        }

        protected virtual void OnMessageReceived(OscMessage msg)
        {
            MessageReceived?.Invoke(this, new MessageEventArgs() { Message = msg });
        }
    }
}
