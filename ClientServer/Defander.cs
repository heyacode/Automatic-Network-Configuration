using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServer
{
    /// <summary>
    /// The Defander class provides methods to enable and disable the Windows Firewall using PowerShell commands.
    /// This class has been created and used for test purposes only.
    /// </summary>
    public class Defander
    {
        /// <summary>
        /// Disables the Windows Firewall for all profiles (Domain, Public, Private).
        /// </summary>
        public async Task DisableFirewall()
        {
            await RunPowerShellCommand("Set-NetFirewallProfile -Profile Domain,Public,Private -Enabled False");
        }

        /// <summary>
        /// Enables the Windows Firewall for all profiles (Domain, Public, Private).
        /// </summary>
        public async Task EnableFirewall()
        {
            await RunPowerShellCommand("Set-NetFirewallProfile -Profile Domain,Public,Private -Enabled True");
        }

        /// <summary>
        /// Executes a PowerShell command with elevated privileges.
        /// </summary>
        /// <param name="command">The PowerShell command to execute.</param>
        private async Task RunPowerShellCommand(string command)
        {
            // Configure the process start info for PowerShell
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",                // Specify PowerShell as the executable
                Arguments = command,                        // PowerShell command to execute
                WindowStyle = ProcessWindowStyle.Hidden,    // Hide the PowerShell window
                UseShellExecute = true,                     // Use the operating system shell to start the process
                Verb = "runas"                              // Run the process with administrator privileges
            };

            // Start the PowerShell process and wait for it to exit
            Process.Start(psi).WaitForExit();
        }
    }
}
