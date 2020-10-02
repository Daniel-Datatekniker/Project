using Data.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Data.Database
{
    public class WebCon : DefBase, IConnect
    {
        // TcpClient client = new TcpClient();
        TcpClient client = new TcpClient();

        public WebCon(string _connectionString, int _port) : base(_connectionString, _port)
        {

        }
        
        public string Close()
        {
            return "";
        }
        /// <summary>
        /// Connect to WebServer
        /// </summary>
        /// <returns></returns>
        public string Connect()
        {
            try
            {
                //creating a new tcp client
                client = new TcpClient();
                //parsing connectionstring and port into an ipaddres.
                client.Connect(IPAddress.Parse(ConnectionString), Port);
                //returning the new connection
                return client.Connected.ToString();
            }
            catch (SocketException ex)
            {

                return ex.Message;
            }
        }
        /// <summary>
        /// Write to webserver and get data back, H for humidity, R for room, T for temperature
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public string Insert(string message)
        {
            try
            {
                //creating a network stream, from the new connection
                using (NetworkStream stream = client.GetStream())
                {
                    //writing to our webserver to get response
                    stream.ReadTimeout = 2000;
                    byte[] bytes = Encoding.UTF8.GetBytes(message);
                    stream.Write(bytes, 0, bytes.Length);
                    return "Input Success";
                }
            }
            catch (Exception ex)
            {

                return ex.Message;
            }


        }
        /// <summary>
        /// Read the data from the webserver,  H for humidity, R for room, T for temperature.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public string Read(string query)
        {
            try
            {
                //Open connection
                Connect();
                //using a stream with client data
                using (NetworkStream stream = client.GetStream())
                {
                    //read from the stream
                    stream.ReadTimeout = 10000;
                    byte[] bytes = Encoding.UTF8.GetBytes(query);
                    stream.Write(bytes, 0, bytes.Length);
                    var reader = new StreamReader(stream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {

                return ex.Message;
            }

        }
    }
}
