using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Data;
using Data.Database;

namespace DataService
{
    public partial class Service1 : ServiceBase
    {
        //cd C:\Windows\Microsoft.NET\Framework\v4.0.30319 
        // InstallUtil.exe C:\Users\danie\source\repos\DataService\DataService\bin\Debug\DataService.exe
        //Creating Timers
        System.Timers.Timer timer = new System.Timers.Timer();
        System.Timers.Timer ClearTimer = new System.Timers.Timer();
        //Creating object og Webcon setting ip and port
        WebCon WebCon = new WebCon("192.168.1.177", 80);
        //Creatinng object og DataConnection and giving the sql connection
        DataConnection data = new DataConnection(@"Server=(localdb)\MSSQLLocalDB;");
        //Creating a list og object readings
        List<Readings> readings = new List<Readings>();
        //Creating a filepath for our log file.
        private string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
        public Service1()
        {

            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //Writing to file with text
            WriteToFile($"Service started  {DateTime.Now}");
            //Setting out timer to loop each 1500 ms
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 1500;
            timer.Enabled = true;
            WriteToFile($"timer  {timer.Interval}");


            //setting timer for clearing our databse and logfile.
            ClearTimer.Elapsed += ClearTimer_Elapsed;
            ClearTimer.Interval = 3000;
            ClearTimer.Enabled = true;
        }

        //Clearing database if it contains more than 20 readings. So it doesnt spam our database.
        private void ClearTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Clearing our file
            ClearFile();
            
            //WriteToFile($"{data.Read().Count()} xxx");
            if (data.Read().Count() > 20)
            {
                data.ClearDB();
            }

        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {


            try
            {
                //creating a new object og readings
                Readings reads = new Readings()
                {
                    //Reading from WebServer
                    Date = DateTime.Now,
                    Degrees = Convert.ToDouble(WebCon.Read("T")) * 0.01,
                    Humidity = Convert.ToDouble(WebCon.Read("H")) * 0.01,
                    RID = WebCon.Read("R")
                };
                //Printing our the new reading to our logfile
                WriteToFile($"made new reading\nDate : {reads.Date}\nDegrees : {reads.Degrees}\nHumidity : {reads.Humidity}\nRID : {reads.RID}");
                //add the current reading to a list
                readings.Add(reads);
            }
            catch (Exception ex)
            {
                //catch error 
                WriteToFile($"Error connecting to sql Server\n\n{ex.Message}");

            }



            try
            {

                //WriteToFile(data.Connect());
                //Write to log, using Data.insert(our list of readings) returning if it failed or success
                WriteToFile(data.Insert(readings));
                //WriteToFile(data.Close());

            }
            catch (Exception ex)
            {
                //if it failed  write to file that  failed
                WriteToFile($"Error connecting to sql Server\n\n{ex.Message}");
            }
            finally
            {
                //Clearing list for a new reading
                readings.Clear();
            }

            //Making some space in log, to make it more readable
            WriteToFile("\n\n\n\n\n");
        }

        //When service stop let log file know time 
        protected override void OnStop()
        {
            WriteToFile($"Service stopped  {DateTime.UtcNow}");
        }

        //Clear the logfile.
        private void ClearFile()
        {
            File.WriteAllText(filepath, string.Empty);
        }


        public void WriteToFile(string Message)
        {
            //making log path
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            //if it doesnt exsist create the log file
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //if it exsist write our message to it
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {

                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}
