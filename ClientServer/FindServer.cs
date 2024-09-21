using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace CS
{
    /// <summary>
    /// The FindServer class is responsible for scanning the local network to find active servers.
    /// </summary>
    public class FindServer
    {
        // Port number used for connecting to the server
        public int port = 3232;

        // Flag indicating if a connection to the server was successful
        public bool connected = false;

        /// <summary>
        /// Checks if a server is running on a given IP address and port.
        /// </summary>
        /// <param name="ip">The IP address to check.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the IP address if a connection is established, otherwise null.</returns>
        public async Task<string> CheckServers(string ip)
        {
            string ipAddress = null;
            try
            {
                TcpClient client = new TcpClient();
                await client.ConnectAsync(ip, port);
                if (client.Connected) // Check if the client is connected
                {
                    connected = true;
                    ipAddress = ip; // Connection successful
                    return ipAddress;
                }
                client.Close(); // Close the client after use
            }
            catch (Exception ex)
            {
                // Log the error message (commented out for now)
                Console.WriteLine($"Error checking server {ip}:{port}: {ex.Message}");
            }

            return ipAddress;
        }

        /// <summary>
        /// Scans the local network to find a server running on the specified port.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the IP address of the found server, otherwise null.</returns>
        public async Task<string> ScanServer()
        {
            string ipAddresses = null;
            var tasks = new List<Task<string>>();

            // Get the list of active IP addresses in the local network
            List<string> listIps = await ScanLANAsync();

            // Check each IP address for a running server
            foreach (var ip in listIps)
            {
                if (connected)
                {
                    break; // Stop the loop if already connected to a server
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

            // Wait for all tasks to complete
            await Task.WhenAll(tasks);

            // Retrieve the IP address of the first successfully connected server
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

        /// <summary>
        /// Scans the local area network (LAN) asynchronously to find active IP addresses.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of active IP addresses.</returns>
        public async Task<List<string>> ScanLANAsync()
        {
            List<string> activeIPs = new List<string>();
            string localIpAddress = GetLocalIPAddress();
            string networkPrefix = GetNetworkPrefix(localIpAddress);

            // Check if the network prefix and local IP address are valid
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

        /// <summary>
        /// Gets the local IP address of the machine.
        /// </summary>
        /// <returns>The local IP address as a string, or an empty string if not found.</returns>
        private string GetLocalIPAddress()
        {
            // Retrieve the host name of the local machine
            string hostName = Dns.GetHostName();
            // Get the DNS information for the host name
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
            // Find the first IPv4 address in the address list
            IPAddress ipAddress = Array.Find(hostEntry.AddressList, a => a.AddressFamily == AddressFamily.InterNetwork);
            // Return the IPv4 address as a string, or an empty string if no IPv4 address is found
            return ipAddress?.ToString() ?? string.Empty;

        }

        /// <summary>
        /// Gets the network prefix from the local IP address.
        /// </summary>
        /// <param name="ipAddress">The local IP address.</param>
        /// <returns>The network prefix as a string, or an empty string if the IP address is invalid.</returns>
        private string GetNetworkPrefix(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
                return string.Empty;

            string[] octets = ipAddress.Split('.');
            return $"{octets[0]}.{octets[1]}.{octets[2]}";
        }
    }
}
