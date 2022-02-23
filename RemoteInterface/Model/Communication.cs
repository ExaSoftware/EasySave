using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace RemoteInterface
{
    class Communication
    {
        //Attributes
        private static Socket server;

        /// <summary>
        /// Launch the connection to the main application
        /// </summary>
        public void LaunchConnection()
        {
       
            //Creation of a communication point between the local IP address and the port
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1050);

            // Creation of the socket to connect to the port
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                //Request of connection
                server.Connect(ipep);

            }
            //Prevents an error to crash the app.
            catch (Exception)
            {
            }
            
        }
        
        public void receiveProgessLog()
        {
            JsonReadWriteModel jsonRWM = new JsonReadWriteModel();

            //Déclaration d'un buffer de type byte pour enregistrer les données reçues
            byte[] data = new byte[128];
           

            //Call receive that get the data and stock it in server
            //return the number of data received
            try
            {
                int recv = server.Receive(data);
                Trace.WriteLine(recv);
            }
            catch (SocketException exception)
            {
                Trace.WriteLine(exception.Message);
            }

            //Change the data received from byte -> string -> ProgressLog


            /*ProgressLog progress=  jsonRWM.ReadProgressLog((Encoding.UTF8.GetString(data)));*/
            //MessageBox.Show(progress.Name);
            string msg = Encoding.UTF8.GetString(data);
            MessageBox.Show(msg);
            //return progress;
        }

        /// <summary>
        /// Send the state of the progress 
        /// </summary>
        public void SendInformation(String txt)
        {
            JsonReadWriteModel json = new JsonReadWriteModel();
            //Verify if the connection is established
            
            //Send the list to the client
            try
            {
                server.Send(Encoding.UTF8.GetBytes(txt));
            }
            catch (SocketException exp)
            {
            }
            
        }
    }
}
