using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace EasySave
{
    class Communication
    {
        Socket _client;
        Socket _newsock;


        /// <summary>
        /// Create the socket, bind it and start listening on the networ
        /// </summary>
        public void LaunchConnection()
        {
            //Creation of a communication point between the local IP address and the port
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1050);

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
        private void ListenNetwork()
        {
            _newsock.Listen(1);
            _client = _newsock.Accept();
        }

        /// <summary>
        /// Send the state of the progress 
        /// </summary>
        public void SendUsedJob(ProgressLog log)
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
        public string receiveInformation()
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
