using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace tasklistWinService
{
    /// <summary>
    /// Create a Directory
    /// Add a text file named on currnt date time (date_hr-min)
    /// write the error
    /// </summary>
    class ErrorHandler
    {
        public static void WriteError(string errorMessage)
        {

            try
            {
                string dir = "C:\\Program Files\\CodeScape";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                dir = "CodeScape\\Error";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                string path = "CodeScape\\Error\\" + DateTime.Now.Day + ".txt";
                if (!File.Exists(path))
                {
                    File.Create(path).Close();
                }
                using (StreamWriter w = File.AppendText(path))
                {
                    w.WriteLine("\r\nLog Entry : ");
                    w.WriteLine("{0}", DateTime.Now);
                    string err = "Error Message:" + errorMessage;
                    w.WriteLine(err);
                    w.WriteLine("__________________________");
                    w.Flush();
                    w.Close();
                }
            }
            catch (Exception ex)
            {
                WriteError(ex.Message);
            }

        }
    }
}
