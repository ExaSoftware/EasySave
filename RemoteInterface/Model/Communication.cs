using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace RemoteInterface
{
    static class Communication
    {
        //Attributes
        private static Socket server;

        /// <summary>
        /// Launch the connection to the main application
        /// </summary>
        public static void LaunchConnection()
        {

            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2222);

            // Creation of the socket to connect to the port
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //Request of connection
            server.Connect(ipep);

            Thread t = new Thread(new ThreadStart(Listen));
            t.Start();

        }
        
        public static void Listen()
        {
            while (true)
            {
                byte[] data = new byte[4096];

                //appel de la méthode receive qui reçoit les données envoyées par le serveur et les stocker 
                //dans le tableau data, elle renvoie le nombre d'octet reçus
                //nt recv = server.Receive(data);
                try
                {
                    int recv = server.Receive(data);
                }
                catch (SocketException exception)
                {

                }


                //transcodage de data en string
                //String mg = (Encoding.UTF8.GetString(data));
                receiveProgessLog(data);
                //affichage des données recues dans le label label1
                //MessageBox.Show(mg);
                //Trace.WriteLine(mg);
            }

        }
        public static void receiveProgessLog(byte[] data)
        {
            JsonReadWriteModel jsonRWM = new JsonReadWriteModel();
            ProgressLog progress = jsonRWM.ReadProgressLog((Encoding.UTF8.GetString(data)));
            Trace.WriteLine(String.Format("{0} {1} {2} {3}", progress.Name, progress.State, progress.TotalFilesRemaining, progress.Progression, Environment.NewLine));
            //Déclaration d'un buffer de type byte pour enregistrer les données reçues
            /*byte[] data = new byte[128];*/


            //Call receive that get the data and stock it in server
            //return the number of data received
            /*try
            {
                int recv = server.Receive(data);
                Trace.WriteLine(recv);
            }
            catch (SocketException exception)
            {
                Trace.WriteLine(exception.Message);
            }*/

            //Change the data received from byte -> string -> ProgressLog


            //ProgressLog progress=  jsonRWM.ReadProgressLog((Encoding.UTF8.GetString(data)));
            //MessageBox.Show(progress.Name);
            //string msg = Encoding.UTF8.GetString(data);
            //MessageBox.Show(msg);
            //return progress;
        }

        /// <summary>
        /// Send the state of the progress 
        /// </summary>
        public static void SendInformation(String txt)
        {
            //JsonReadWriteModel json = new JsonReadWriteModel();
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
