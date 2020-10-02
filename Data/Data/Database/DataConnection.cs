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
        SqlConnection Data = new SqlConnection();

        public DataConnection(string _connectionString) : base(_connectionString)
        {
            Data.ConnectionString = _connectionString;
        }

        public string Close()
        {
            Data.Close();
            return "Connection Closed";
        }

        public string Connect()
        {
            Data.ConnectionString = ConnectionString;

            Data.Open();

            return $"State: {Data.State}";
        }

        public string Insert(IEnumerable<Readings> reads)
        {
            try
            {


                using (SchoolEntities context = new SchoolEntities())
                {
                    foreach (Readings reading in reads)
                    {

                        context.Readings.Add(reading);
                    }

                    context.SaveChanges();
                }
                return "Readings Added";
            }
            catch (Exception ex)
            {

                return ex.Message;
            }

        }



        public IEnumerable<Readings> Read()
        {

            //using (SchoolEntities context = new SchoolEntities())
            //{
            //    var tables = context.[Readings].Tolist();
            //    return tables;
            //}
            List<Readings> readings = new List<Readings>();
            Data.Open();
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
