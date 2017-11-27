using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinServiceInstaller
{
    class Program
    {
        static void Main(string[] args)
        {
            repeat:
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("To add a service write \"add <service name or path>\"");
            Console.WriteLine("To remove a service write \"del <service name or path>\"");

            var cmd = Console.ReadLine();

            if (cmd.ToLower().StartsWith("add "))
            {
                Run(true, cmd.Substring(4));
            }
            else if (cmd.ToLower().StartsWith("del "))
            {
                Run(false, cmd.Substring(4));
            }

            goto repeat;
        }

        private static void Run(bool add, string servicePath)
        {
            var installUtilFilename = Path.Combine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), "InstallUtil.exe");

            var process = new Process()
            {
                StartInfo =
                {
                    FileName = installUtilFilename,
                    Arguments = (add ? "" : "/u ") + servicePath,
                    CreateNoWindow = true,
                    ErrorDialog = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };

            process.ErrorDataReceived += (sendingProcess, errorLine) =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(errorLine.Data);
            };

            process.OutputDataReceived += (sendingProcess, dataLine) =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(dataLine.Data);
            };

            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            process.WaitForExit();
        }
    }
}
