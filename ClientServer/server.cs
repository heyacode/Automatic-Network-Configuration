using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientServer
{
    /// <summary>
    /// The Server class sets up a TCP server that listens for incoming client connections.
        /// <summary>
        /// Waits for incoming connections on port 8888, and enables them to be handled
        /// simultaneously using threads.
        /// </summary>
    public class Server
    {
        /// <summary>
        /// Initiates the server to start listening for incoming connections.
        /// </summary>
        public async Task Connect()
        {
            // Create an instance of TcpListener to listen for incoming connections on port 3232
            TcpListener listener = new TcpListener(IPAddress.Any, 3232);
            // Instance of defender to desable the firewall (this one has been used for test purposes only)
            var defander = new Defander();
            await defander.DisableFirewall();

            // Start listening for incoming connection requests
            listener.Start();
            Console.WriteLine("Waiting for connections");

            while (true)
            {
                // Accept a pending client connection request
                TcpClient client = listener.AcceptTcpClient();

                // Handle each client connection in a separate thread
                Thread clientThread = new Thread(async () => await IpClient(client));
                clientThread.Start();
            }
        }

        /// <summary>
        /// Handles the client connection and initiates data exchange.
        /// </summary>
        static async Task IpClient(TcpClient client)
        {
            try
            {
                // Display the IP address of the connected client
                Console.WriteLine("Client connected with IP address: " + ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());

                // Send JSON data to the connected client
                EnvoyerJSON(client);
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during client handling
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                // Ensure the client connection is properly closed
                client.Close();
            }
        }

        /// <summary>
        /// Sends a JSON file to the connected client.
        /// </summary>
        /// <param name="client">The connected TcpClient instance.</param>
        static void EnvoyerJSON(TcpClient client)
        {
            // Get the network stream to send data to the client
            NetworkStream stream = client.GetStream();

            // Relative path of the JSON data file
            string relativePath = "add the file's path here";
            string jsonFilePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), relativePath));

            try
            {
                // Read the JSON file from the disk
                string jsonData = File.ReadAllText(jsonFilePath);

                // Convert the JSON string to a byte array for transmission
                byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);

                // Write the JSON byte array to the network stream
                stream.Write(jsonBytes, 0, jsonBytes.Length);
                Console.WriteLine("JSON file sent.");
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during file sending
                Console.WriteLine("Error sending the file: " + ex.Message);
            }
        }
    }
}
