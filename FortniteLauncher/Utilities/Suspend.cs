using System.Diagnostics;
using System.Threading.Tasks;
using Win32;

namespace ProcessSuspend
{
    public class SuspendProcess
    {
        // taken from https://stackoverflow.com/questions/71257/suspend-process-in-c-sharp
        public static Task Suspend(string text)
        {
            foreach (Process LeProcess in Process.GetProcesses())
            {
                if (LeProcess.ProcessName == text)
                {
                    LeProcess.Suspend();
                }
            }
            return Task.CompletedTask;
        }
    }
}