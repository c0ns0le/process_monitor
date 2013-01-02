using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Configuration;
using System.Management.Automation.Runspaces;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.IO;
using System.Net;

namespace tasklistWinService
{
    /// <summary>
    /// This class override OnStart method of windows service 
    /// and write a logFile after a defined interval of time both locally n at centeralized server
    /// </summary>
    partial class LogFile : ServiceBase
    {
        public LogFile()
        {
            InitializeComponent();
        }

        /// <summary>
        /// As service starts this function will call ExecuteCommandSync Method and passes the command to execute
        /// On return it set up a timer which will reinvoke this function after a defined interval of time
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            Timer _timer = new Timer();
            //add this line to text file during start of service
            ExecuteCommandSync("Get-Process");
         


            //This statement is used to set interval to set value in milliseconds)
            //To Change time interval Goto App.config -> interval key and set its value
            string interval = ConfigurationManager.AppSettings["interval"];
            _timer.Interval = Convert.ToDouble(interval) * 60000;
            _timer.Start();
            //handle Elapsed, when the timer stops 
            _timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            //enabling the timer
            _timer.Enabled = true;

        }

        protected override void OnStop()
        {
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            //Elapsed method recalling ExecuteCommandSync method
            ExecuteCommandSync("Get-Process");
        }

        /// <summary>
        /// Each step has its explaination 
        /// After executing command it will check for Directory path locally and on server and will create one if required 
        /// At last will call
        /// </summary>
        /// <param name="scriptText"></param>
        public void ExecuteCommandSync(string scriptText)
        {
            //System.Diagnostics.Debugger.Launch();



            string hostName = System.Net.Dns.GetHostName();

            string IP4Address = getIP4Address(hostName);


            string filePath = hostName + "_" + DateTime.Now.Day+"_"+DateTime.Now.Month+"_"+DateTime.Now.Year + ".Csv";

            string serverPath = createServerDirectory(IP4Address, filePath);
            string localPath = createLocalDirectory(IP4Address, filePath);
            // create Powershell runspace

            Runspace runspace = RunspaceFactory.CreateRunspace();

            // open it

            runspace.Open();

            // create a pipeline and feed it the script text

            Pipeline pipeline = runspace.CreatePipeline();
            //***************************************************
            Command getProcess = new Command("Get-Process");
            Command sort = new Command("Export-Csv");
            string tempPath = ConfigurationManager.AppSettings["localDir"] + "\\Log.Csv";
            sort.Parameters.Add("Path", tempPath);
            pipeline.Commands.Add(getProcess);
            pipeline.Commands.Add(sort);
            Collection<PSObject> output = pipeline.Invoke();
            foreach (PSObject psObject in output)
            {
                Process process = (Process)psObject.BaseObject;
                Console.WriteLine("Process name: " + process.ProcessName);
            }


            try
            {
                if (!System.IO.File.Exists(serverPath))
                {
                    System.IO.FileStream serverInfo = System.IO.File.Create(serverPath);
                    serverInfo.Close();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex.ToString());
            }
            try
            {

                if (!System.IO.File.Exists(localPath))
                {
                    System.IO.FileStream localInfo = System.IO.File.Create(localPath);
                    localInfo.Close();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex.ToString());
            }
            WriteHeading(hostName, IP4Address, serverPath, localPath);

            string Csv1 = tempPath;
            string serverCsv = serverPath;
            string localCsv = localPath;

            FileStream FStream1 = null;
            FileStream FStream2 = null;
            FileStream FStream3 = null;
            FStream1 = File.Open(serverCsv, FileMode.Append);
            FStream3 = File.Open(localCsv, FileMode.Append);
            FStream2 = File.Open(Csv1, FileMode.Open);
            byte[] FStream2Content = new byte[FStream2.Length];
            FStream2.Read(FStream2Content, 0, (int)FStream2.Length);
            FStream1.Write(FStream2Content, 0, (int)FStream2.Length);
            FStream3.Write(FStream2Content, 0, (int)FStream2.Length);

            // MessageBox.Show("Merging Successfully ended!");

            FStream1.Close();
            FStream2.Close();
            FStream3.Close();


        }
        /// <summary>
        /// This method will write hostname datetime and ipaddress on top of each section of logfile
        /// </summary>
        /// <param name="hostName">Hostname</param>
        /// <param name="IP4Address">IP Address</param>
        /// <param name="serverPath">ServerPath of file</param>
        /// <param name="localPath">local path of file</param>

        private static void WriteHeading(string hostName, string IP4Address, string serverPath, string localPath)
        {
            TextWriter txtWriter = new StreamWriter(serverPath, true);
            txtWriter.WriteLine();
            txtWriter.WriteLine();
            txtWriter.WriteLine("Host Name:    " + hostName + "______________************************************************_______________IP Address:  " + IP4Address + "______________************************************************_______________Time:  " + DateTime.Now);
            txtWriter.Close();
            txtWriter = new StreamWriter(localPath, true);
            txtWriter.WriteLine();
            txtWriter.WriteLine();
            txtWriter.WriteLine("Host Name:    " + hostName + "______________************************************************_______________IP Address:  " + IP4Address + "______________************************************************_______________Time:  " + DateTime.Now);
            txtWriter.Close();
        }


        /// <summary>
        /// Check if directory exist
        /// if(true)
        /// delete the file of past 30th day [on 1/Feb del 1/Jan created file]
        /// </summary>
        /// <param name="IP4Address">IP address of PC to set the file name</param>
        /// <param name="filePath"> Path where directory is to be created</param>
        /// <returns></returns>
        private static string createLocalDirectory(string IP4Address, string filePath)
        {
            string[] files = { };
            DateTime date;
            string localPath = ConfigurationManager.AppSettings["localDir"];

            localPath = Path.Combine(localPath, IP4Address);
            if (!Directory.Exists(localPath))
            {
                System.IO.Directory.CreateDirectory(localPath);
            }
            else
            {
                // Delete a single file dated 30 day from today
                files = Directory.GetFiles(localPath);

                foreach (string tempFilename in files)
                {
                    date = File.GetLastWriteTime(tempFilename);
                    if (date.AddDays(30).Day == DateTime.Now.Day)
                    {
                        File.Delete(tempFilename);

                    }
                }
            }

            localPath = Path.Combine(localPath, filePath);
            return localPath;
        }


        /// <summary>
        /// Create Directory for this IP if not created
        /// </summary>
        /// <param name="IP4Address">IP to set name of Directory</param>
        /// <param name="filePath">Define path to Create directory</param>
        /// <returns></returns>
        private static string createServerDirectory(string IP4Address, string filePath)
        {

            string serverPath = ConfigurationManager.AppSettings["serverDir"];
            serverPath = Path.Combine(serverPath, IP4Address);
            try
            {
                if (!Directory.Exists(serverPath))
                {
                    System.IO.Directory.CreateDirectory(serverPath);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex.ToString());
            }
            serverPath = Path.Combine(serverPath, filePath);
            return serverPath;
        }


        /// <summary>
        /// Map IP address corrosponding to the host name
        /// </summary>
        /// <param name="hostName">PC name</param>
        /// <returns></returns>
        private static string getIP4Address(string hostName)
        {
            string IP4Address = String.Empty;

            foreach (IPAddress IPA in System.Net.Dns.GetHostAddresses(hostName))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }
            return IP4Address;
        }

    }
}
