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

        public string Connect()
        {
            try
            {
                client = new TcpClient();
                client.Connect(IPAddress.Parse(ConnectionString), Port);
                return client.Connected.ToString();
            }
            catch (SocketException ex)
            {

                return ex.Message;
            }
        }

        public string Insert(string message)
        {
            try
            {
                using (NetworkStream stream = client.GetStream())
                {
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

        public string Read(string query)
        {
            try
            {
                Connect();
                using (NetworkStream stream = client.GetStream())
                {
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
