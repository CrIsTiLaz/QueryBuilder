using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using static System.Net.Mime.MediaTypeNames;

namespace QueryBuilder.Models.Log
{
        public abstract class LogBase
        {
            public abstract void Log(string Messsage);
        }

        public class Logger : LogBase
        {

            private String CurrentDirectory
            {
                get;
                set;
            }

            private String FileName
            {
                get;
                set;
            }

            private String FilePath
            {
                get;
                set;
            }

            public Logger()
            {
                this.CurrentDirectory = Thread.GetDomain().BaseDirectory;
                this.FileName = "Log.txt";
                this.FilePath = this.CurrentDirectory + this.FileName;

            }

            public override void Log(string Messsage)
            {

                System.Console.WriteLine("Logged : {0}", Messsage);

                using (System.IO.StreamWriter w = File.AppendText(this.FilePath))
                {
                    w.Write("\r\nLog Entry : ");
                    w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                    w.WriteLine("  :{0}", Messsage);
                    w.WriteLine("-----------------------------------------------");
                }
            }
        }
    }