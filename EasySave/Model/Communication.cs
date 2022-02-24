using EasySave.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace EasySave
{
    public static class Communication
    {
        private static Socket _client;
        private static Socket _newsock;


        /// <summary>
        /// Create the socket, bind it and start listening on the networ
        /// </summary>
        public static void LaunchConnection()
        {
            //Creation of a communication point between the local IP address and the port
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2222);

            // Creation of the socket to connect to the port
            _newsock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the communication point
            _newsock.Bind(ipep);

            //Create a new thread that listen the network and accept the client request
            Thread t = new Thread(new ThreadStart(ListenNetwork));
            t.Start();
        }

        /// <summary>
        /// Listen the network called by the thread and accept the request of connection
        /// </summary>
        private static void ListenNetwork()
        {
            MainViewModel vm = new MainViewModel(0);
            _newsock.Listen(1);
            _client = _newsock.Accept();
            IPEndPoint socketEndPoint = (IPEndPoint)_client.RemoteEndPoint;
            Trace.WriteLine(String.Format("Client connected: {0}:{1}", socketEndPoint.Address, socketEndPoint.Port));

            while (true)
            {
                byte[] data = new byte[128];
                try
                {
                    int recv = _client.Receive(data);
                }
                catch (SocketException exception)
                {
                    Trace.WriteLine(exception.Message);
                }

                string msg = Encoding.UTF8.GetString(data);
                if (msg == "stop")
                {
                    vm.Stop();
                }
                if (msg == "pause")
                {
                    vm.Pause();
                }
                //Trace.WriteLine(mg);
            }
        }
        public static void SendInformation(ProgressLog[] progressList)
        {
            JsonReadWriteModel json = new JsonReadWriteModel();
            string jsonString = json.PrepareLogForRemote(progressList);
            if (_client != null && _client.Connected)
            {
                try
                {
                    _client.Send(Encoding.UTF8.GetBytes(jsonString));
                }
                catch (SocketException exp)
                {
                    Trace.WriteLine(exp.Message);

                }
            }
        }
    }
}
