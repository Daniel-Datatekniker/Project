using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Database
{
    public abstract class DefBase
    {
        //Attributes
        private string connectionString;
        private int port;

        //Properties
        public string ConnectionString
        {
            get { return connectionString; }
            private set { connectionString = value; }
        }
        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        //Constructor
        public DefBase(string _connectionString, int _port)
        {
            connectionString = _connectionString;
            port = _port;
        }
        public DefBase(string _connectionString)
        {
            connectionString = _connectionString;
        }
    }
}
