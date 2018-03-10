using NeoStudio.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoStudio
{
    internal static class SmartContract
    {
        internal static bool Build()
        {
            Process myProcess = new Process();
            DocumentView activeDocument = ((MainWindow)App.Current.MainWindow).ActiveDocument;
            
            try
            {
                
                if (activeDocument == null)
                    return false;

                string filename = System.IO.Path.Combine(FindExePath("neon.exe"));
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                startInfo.FileName = filename;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.Arguments = activeDocument.FilePath;

                try
                {
                    using (Process exeProcess = Process.Start(startInfo))
                    {
                        exeProcess.WaitForExit();
                    }
                }
                catch
                {
                    // Log error.
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            
            return true;
        }

        private static string FindExePath(string exe)
        {
            exe = Environment.ExpandEnvironmentVariables(exe);
            if (!File.Exists(exe))
            {
                if (System.IO.Path.GetDirectoryName(exe) == String.Empty)
                {
                    foreach (string test in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(';'))
                    {
                        string path = test.Trim();
                        if (!String.IsNullOrEmpty(path) && File.Exists(path = System.IO.Path.Combine(path, exe)))
                            return System.IO.Path.GetFullPath(path);
                    }
                }
                throw new FileNotFoundException(new FileNotFoundException().Message, exe);
            }
            return System.IO.Path.GetFullPath(exe);
        }
    }
}
