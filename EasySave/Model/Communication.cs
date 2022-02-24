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
            _newsock.Listen(1);
            _client = _newsock.Accept();
            IPEndPoint socketEndPoint = (IPEndPoint)_client.RemoteEndPoint;
            Trace.WriteLine(String.Format("Client connected: {0}:{1}", socketEndPoint.Address, socketEndPoint.Port));
            while (true)
            {
                byte[] data = new byte[128];

                //appel de la méthode receive qui reçoit les données envoyées par le serveur et les stocker 
                //dans le tableau data, elle renvoie le nombre d'octet reçus
                try
                {
                    int recv = _client.Receive(data);
                }
                catch (SocketException exception)
                {
                    Trace.WriteLine(exception.Message);
                }


                //transcodage de data en string
                String mg = (Encoding.UTF8.GetString(data));
                //affichage des données recues dans le label label1
                Trace.WriteLine(mg);
            }
        }

        public static void SendInformation(List<ProgressLog> progressList)
        {
            JsonReadWriteModel json = new JsonReadWriteModel();
            string jsonString = json.PrepareLogForRemote(progressList);
            if (_client.Connected)
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
        /*public static void SendInformation(ProgressLog progress)
        {
            JsonReadWriteModel json = new JsonReadWriteModel();
            string jsonString = json.PrepareLogForRemote(progress);
            Thread.Sleep(100);
            if (_client.Connected)
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
            
        }*/
        /// <summary>
        /// Send the state of the progress 
        /// </summary>
        public static void SendUsedJob(ProgressLog log)
        {
            JsonReadWriteModel json = new JsonReadWriteModel();

            _client.Send(Encoding.UTF8.GetBytes("test"));
            /*string jsonString = json.PrepareLogForRemote(log);
            //Send the list to the client
            try
            {
                _client.Send(Encoding.UTF8.GetBytes(jsonString));
            }
            catch (SocketException exp)
            {

            }*/
            
        }
        public static string receiveInformation()
        {
            //Déclaration d'un buffer de type byte pour enregistrer les données reçues
            byte[] data = new byte[128];

            //Call receive that get the data and stock it in server
            //return the number of data received
            try
            {
                int recv = _client.Receive(data);
            }
            catch (SocketException exception)
            {
            }

            //Change the data received from byte -> string -> ProgressLog
            return Encoding.UTF8.GetString(data);

        }
    }
}
