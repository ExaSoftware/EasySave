using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace RemoteInterface
{
    class Communication
    {
        //Attributes
        private bool _connected = false;
        private static Socket server;
        public bool Connected { get => _connected; set => _connected = value; }

        /// <summary>
        /// Launch the connection to the main application
        /// </summary>
        public void LaunchConnection()
        {
            
            //check if the user clicked once or not
            if (!_connected)
            {
                //Creation of a communication point between the local IP address and the port
                IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1050);

                // Creation of the socket to connect to the port
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    //Request of connection
                    server.Connect(ipep);
                    _connected = true;

                }
                //Prevents an error to crash the app.
                catch (Exception)
                {
                    MessageBox.Show("This messagebox has to be changed..   Execute the app");
                }
            }
        }

        /// <summary>
        /// Send the state of the progress 
        /// </summary>
        public void SendStopConnection()
        {
            //Verify if the connection is established
            if (_connected)
            { 
                //Send the list to the client
                try
                {
                    server.Send(Encoding.UTF8.GetBytes(_connected.ToString()));
                    server.Shutdown(SocketShutdown.Both);
                    server.Close();
                }
                catch (SocketException exp)
                {
                }
            }
            _connected = false;
        }
        
        public void receiveJobBackup()
        {

            //Déclaration d'un buffer de type byte pour enregistrer les données reçues
            byte[] data = new byte[128];

            //appel de la méthode receive qui reçoit les données envoyées par le serveur et les stocker 
            //dans le tableau data, elle renvoie le nombre d'octet reçus
            try
            {
                int recv = server.Receive(data);
            }
            catch (SocketException exception)
            {

            }

            //transcodage de data en string
            String mg = (Encoding.UTF8.GetString(data));

        }
    }
}
