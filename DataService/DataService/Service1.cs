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

        System.Timers.Timer timer = new System.Timers.Timer();
        System.Timers.Timer ClearTimer = new System.Timers.Timer();

        WebCon WebCon = new WebCon("192.168.1.177", 80);
        DataConnection data = new DataConnection(@"Server=(localdb)\MSSQLLocalDB;");
        List<Readings> readings = new List<Readings>();
        private string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
        public Service1()
        {

            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
        
            WriteToFile($"Service started  {DateTime.Now}");
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 1500;
            timer.Enabled = true;
            WriteToFile($"timer  {timer.Interval}");



            ClearTimer.Elapsed += ClearTimer_Elapsed;
            ClearTimer.Interval = 3000;
            ClearTimer.Enabled = true;
        }

        private void ClearTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
         
            ClearFile();
            WriteToFile($"{data.Read().Count()} xxx");
            if (data.Read().Count() > 20)
            {
                data.ClearDB();
            }
          
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {


            try
            {
                Readings reads = new Readings()
                {
                    Date = DateTime.Now,
                    Degrees = Convert.ToDouble(WebCon.Read("T")) * 0.01,
                    Humidity = Convert.ToDouble(WebCon.Read("H")) * 0.01,
                    RID = WebCon.Read("R")
                };
                WriteToFile($"made new reading\nDate : {reads.Date}\nDegrees : {reads.Degrees}\nHumidity : {reads.Humidity}\nRID : {reads.RID}");
                readings.Add(reads);
            }
            catch (Exception ex)
            {

                WriteToFile($"Error connecting to sql Server\n\n{ex.Message}");

            }



            try
            {

                //WriteToFile(data.Connect());
                WriteToFile(data.Insert(readings));
                //WriteToFile(data.Close());

            }
            catch (Exception ex)
            {
                WriteToFile($"Error connecting to sql Server\n\n{ex.Message}");
            }
            finally
            {
                readings.Clear();
            }


            WriteToFile("\n\n\n\n\n");
        }

        protected override void OnStop()
        {
            WriteToFile($"Service stopped  {DateTime.UtcNow}");
        }

        private void ClearFile()
        {
            File.WriteAllText(filepath, string.Empty);
        }

        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
         
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
