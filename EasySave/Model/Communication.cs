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
        private bool _connected = false;
        Socket client;
        Socket newsock;

        public bool Connected { get => _connected; set => _connected = value; }

        /// <summary>
        /// Create the socket, bind it and start listening on the networ
        /// </summary>
        public void LaunchConnection()
        {
            //Creation of a communication point between the local IP address and the port
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1050);

            // Creation of the socket to connect to the port
            newsock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the communication point
            newsock.Bind(ipep);

            //Create a new thread that listen the network and accept the client request
            Thread t = new Thread(new ThreadStart(ListenNetwork));
            t.Start();
            _connected = true;
        }

        /*
		// Get information from ProgressLog object
		void GetProgressLog(ProgressLog pl)
		{
			
		}

		bool stop()
		{
			
		}
		*/

        /// <summary>
        /// Listen the network called by the thread and accept the request of connection
        /// </summary>
        private void ListenNetwork()
        {
            newsock.Listen(1);
            client = newsock.Accept();
        }

        /// <summary>
        /// Send the state of the progress 
        /// </summary>
        public void SendUsedJob()
        {
            //Verify if the connection is established
            if (_connected)
            {
                //Send the list to the client
                try
                {
                    client.Send(Encoding.UTF8.GetBytes("test"));
                }
                catch (SocketException exp)
                {
                }

            }
        }
        /// <summary>
        /// Send the state of the progress 
        /// </summary>
        public void SendUsedJob(ProgressLog log)
        {
        JsonReadWriteModel json = new JsonReadWriteModel();
        //Verify if the connection is established
            if (_connected)
            {
                string jsonString = json.PrepareLogForRemote(log);
                MessageBox.Show(jsonString);
                //Send the list to the client
                try
                {
                    client.Send(Encoding.UTF8.GetBytes(jsonString));
                }
                catch (SocketException exp)
                {
                }
            }
        }

        /// <summary>
        /// Notify the remote app that the connection will be lost and close the working socket
        /// </summary>
        public void SendStopConnection()
        {
            //Verify if the connection is established
            if (_connected)
            {
                //Send the list to the client
                try
                {
                    client.Send(Encoding.UTF8.GetBytes(_connected.ToString()));
                    client.Shutdown(SocketShutdown.Both);
                    newsock.Shutdown(SocketShutdown.Both);
                    newsock.Close();
                    client.Close();
                }
                catch (SocketException exp)
                {
                }
            }
            _connected = false;
        }
    }
}
