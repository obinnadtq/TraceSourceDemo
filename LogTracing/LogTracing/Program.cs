using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogTracing
{
    class Program
    {
        private static TraceSource mySource = new TraceSource("LogTracing");
        static void Main(string[] args)
        {
            RunScript();
        }

        static void RunScript()
        {
            var psFile = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, @"log_tracing.ps1");
            if (File.Exists(psFile))
            {
                var startInfo = new ProcessStartInfo()
                {
                    FileName = "powershell.exe",
                    Arguments = $"-ExecutionPolicy unrestricted -file \"{psFile}\"",
                    UseShellExecute = false,
                    Verb = "runas",
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                };
                using(Process myprocess = Process.Start(startInfo))
                {
                    myprocess.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
                    myprocess.ErrorDataReceived += new DataReceivedEventHandler(ErrorHandler);
                    myprocess.BeginOutputReadLine();
                    myprocess.BeginErrorReadLine();
                    myprocess.WaitForExit();
                }

            }


        }
        private static void ErrorHandler(object sender, DataReceivedEventArgs e)
        {
            // write the error text to the file if there is something to write
            if (!String.IsNullOrEmpty(e.Data))
            {
                mySource.TraceEvent(TraceEventType.Error, 2, e.Data);
            }
        }

        private static void OutputHandler(object sender, DataReceivedEventArgs e)
        {
            // write the output text to the file if there is something to write
            if (!String.IsNullOrEmpty(e.Data))
            {
                mySource.TraceEvent(TraceEventType.Information, 1, e.Data);
            }
        }
    }
}
