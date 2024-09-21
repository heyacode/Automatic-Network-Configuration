using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace CS
{
    public class FindServer
    {
        public int port = 3232;
        public bool connected = false;

        public async Task<string> CheckServers(string ip)
        {
            string ipAddress = null;
            try
            {
                TcpClient client = new TcpClient();
                await client.ConnectAsync(ip, port);
                if (client.Connected) // Potential issue: Connected property may not reflect the actual connection status immediately
                {
                    connected = true;
                    ipAddress = ip; // connected successfully
                    return ipAddress;
                }
                client.Close(); // Close the client after use
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"Error checking server {ip}:{port}: {ex.Message}");
            }

            return ipAddress;
        }

        public async Task<string> ScanServer()
        {
            string ipAddresses = null;

            // Disable firewall

            var tasks = new List<Task<string>>();

            List<string> listIps = await ScanLANAsync(); // Call the async version of ScanLAN

            foreach (var ip in listIps)
            {
                if (connected)
                {
                    break; // Stop the loop if connected
                }
                else
                {
                    var checkip = CheckServers(ip);
                    if (checkip != null)
                    {
                        tasks.Add(checkip);
                    }
                }
            }

            await Task.WhenAll(tasks);

            foreach (var task in tasks)
            {
                if (task.Result != null)
                {
                    ipAddresses = task.Result;
                    break;
                }
            }

            return ipAddresses;
        }

        public async Task<List<string>> ScanLANAsync()
        {
            List<string> activeIPs = new List<string>();
            string localIpAddress = GetLocalIPAddress();
            string networkPrefix = GetNetworkPrefix(localIpAddress);

            if (!string.IsNullOrEmpty(networkPrefix) && !string.IsNullOrEmpty(localIpAddress))
            {
                for (int i = 1; i <= 255; i++)
                {
                    string ipAddress = $"{networkPrefix}.{i}";
                    try
                    {
                        Ping ping = new Ping();
                        PingReply reply = await ping.SendPingAsync(ipAddress, 5);
                        if (reply.Status == IPStatus.Success)
                        {
                            Console.WriteLine(ipAddress);   
                            activeIPs.Add(ipAddress);
                            break;
                        }
                    }
                    catch (PingException)
                    {
                        // Ignore ping exceptions (e.g., timeout)
                    }
                }
            }

            return activeIPs;
        }

        private string GetLocalIPAddress()
        {
            string hostName = System.Net.Dns.GetHostName();
            System.Net.IPHostEntry hostEntry = System.Net.Dns.GetHostEntry(hostName);
            System.Net.IPAddress ipAddress = Array.Find(hostEntry.AddressList, a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            return ipAddress?.ToString() ?? string.Empty;
        }

        private string GetNetworkPrefix(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
                return string.Empty;

            string[] octets = ipAddress.Split('.');
            return $"{octets[0]}.{octets[1]}.{octets[2]}";
        }
    }
}
