﻿using Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    public class ServerImpl
    {
        private static ManualResetEvent allDone = new ManualResetEvent(false);

        public static List<Measurement> Datas = new List<Measurement>();

        public void Start()
        {
            string GetLocalIPAddress()
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
                throw new Exception("No network adapters with an IPv4 address in the system!");
            }

<<<<<<< HEAD
            IPAddress ipAddress = IPAddress.Parse("192.168.150.3");
=======
            IPAddress ipAddress = IPAddress.Parse(GetLocalIPAddress());
>>>>>>> 65dc5d7550be5f6ff5ad83d1db9f5dfd53029be4
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 12345);

            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(localEndPoint);
            listener.Listen(100);
            //Thread.Sleep(1000);
            Console.Clear();
            Console.WriteLine("\nOnline!\n");
            Console.WriteLine("Waiting for a connection...\n");

            while (true)
            {
                allDone.Reset();

             
                listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                

                allDone.WaitOne();
            }
        }

        public void Close()
        {
            allDone.Close();
        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            allDone.Set();
  
            Socket listener = (Socket)ar.AsyncState;
            Socket client = listener.EndAccept(ar);
            Console.WriteLine("\nClient has connected!\n");

            while (true)
            {
                byte[] buff = new byte[1024];
                int bytesReads = client.Receive(buff);

                if (bytesReads == 0)
                    break;
                else if (bytesReads < buff.Length)
                {
                    string message = Encoding.ASCII.GetString(buff, 0, bytesReads);
                   
                    Console.WriteLine(message);
                    Datas.Add(ConvertToObject(message));
                    //DataHandler.Serialize(Datas);
                    DataHandler.SaveToCSV(ConvertToObject(message));

                }
            }
        }

        private static  Measurement ConvertToObject(string stringdata)
        {
            string [] split = stringdata.Split("\n");
            string temp = "";
            bool check = false;

            foreach (string item in split)
            {
                foreach (char character in item)
                {
                    if (character.Equals(' '))
                    {
                        check = true;
                        continue;
                    }
                    if (check)
                    {
                        temp += character;
                    }
                }
                temp += ",";
                check = false;
            }

            return new Measurement(temp.Split(","));
        }
    }
}
