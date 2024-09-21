using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServer
{
    public class Defander
    {
        public async Task DisableFirewall()
        {
            await RunPowerShellCommand("Set-NetFirewallProfile -Profile Domain,Public,Private -Enabled False");
        }

        public async Task EnableFirewall()
        {
            await RunPowerShellCommand("Set-NetFirewallProfile -Profile Domain,Public,Private -Enabled True");
        }

        private async Task RunPowerShellCommand(string command)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "powershell.exe";
            psi.Arguments = command;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = true;
            psi.Verb = "runas"; // Run as administrator

            Process.Start(psi).WaitForExit();
        }
    }
}
