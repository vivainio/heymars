using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heymars
{
    // more minimal than CliWrap
    public class ProcWrap
    {
        public ProcessStartInfo StartInfo;
        public Process Proc;
        public ProcWrap(string bin)
        {
            Proc = new Process();
            StartInfo = Proc.StartInfo;
            StartInfo.FileName= bin;
            StartInfo.UseShellExecute = false;
        }

        public ProcWrap WithArguments(string args)
        {
            StartInfo.Arguments= args;
            return this;
        }

        public ProcWrap WithWorkingDirectory(string workingDirectory)
        {
            StartInfo.WorkingDirectory=workingDirectory;
            return this;
        }

        public void RunWithEvents(Action<string, DataReceivedEventArgs> ev) {
            StartInfo.RedirectStandardError= true;
            StartInfo.RedirectStandardOutput = true;
            Proc.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                ev("out", e);
            });
            Proc.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                ev("error", e);
            });
            Proc.Exited += new EventHandler((sender, e) =>
            {
                ev("exit", null);
            });

        }



    }
}
