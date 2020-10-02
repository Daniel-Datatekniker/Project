using Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Database
{
    public class DataConnection : DefBase, IConnect
    {
        //Creating a new sql connection
        SqlConnection Data = new SqlConnection();
        //setting connection string in constructor
        public DataConnection(string _connectionString) : base(_connectionString)
        {
            Data.ConnectionString = _connectionString;
        }

        //Close sql connection, made if you want to use your own connection instead of EntityFramework
        public string Close()
        {
            Data.Close();
            return "Connection Closed";
        }

        //Open connection to sql, if you dont want ot use EntityFramework
        public string Connect()
        {
            //Setting data connectionstring to the current connectionstring saved when class was created
            Data.ConnectionString = ConnectionString;

            Data.Open();

            return $"State: {Data.State}";
        }

        /// <summary>
        /// Insert collection of Type Readinngs
        /// </summary>
        /// <param name="reads"></param>
        /// <returns></returns>
        public string Insert(IEnumerable<Readings> reads)
        {
            try
            {

                //Using entity to add our recived readings
                using (SchoolEntities context = new SchoolEntities())
                {
                    foreach (Readings reading in reads)
                    {

                        context.Readings.Add(reading);
                    }

                    //Saving readings to our database
                    context.SaveChanges();
                }
                //Returning readings added
                return "Readings Added";
            }
            catch (Exception ex)
            {
                //if i failed give message for logging
                return ex.Message;
            }

        }


        /// <summary>
        /// Reading from database and return a collection of readings
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Readings> Read()
        {
            //For some reason i could not get Entity to retrieve data.
            //using (SchoolEntities context = new SchoolEntities())
            //{
            //    var tables = context.[Readings].Tolist();
            //    return tables;
            //}

            //So i made a orginal sql connection 
            //Create a list of readings
            List<Readings> readings = new List<Readings>();
            //Open connection to sql.
            Data.Open();
            //Select all from readings to return it, could change it later and make store procedure that you use instead by giving the method a string
            SqlCommand cmd = new SqlCommand("use school SELECT * FROM Readings", Data);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                readings.Add(new Readings()
                {
                    id = (int)dr["id"],
                    Date = (DateTime)dr["Date"],
                    RID = (string)dr["RID"],
                    Degrees = (double)dr["Degrees"],
                    Humidity = (double)dr["Humidity"],
                });
            }
            Data.Close();
            //SchoolEntities context = new SchoolEntities();
            //var reads = context.Readings;
            return readings;
        }

        //Clearing Database and resseting Primary key
        public void ClearDB()
        {
            using (SchoolEntities context = new SchoolEntities())
            {
                context.ClearReads();
                context.Database.ExecuteSqlCommand("use school DBCC CHECKIDENT('Readings', RESEED, 0)");
                context.SaveChanges();
            }


          
            //Data.Open();
            //SqlCommand cmd = new SqlCommand("DBCC CHECKIDENT ('[Readings]', RESEED, 0);", Data);
            //SqlDataReader dr = cmd.ExecuteReader();
            //Data.Close();
        }
    }
}
