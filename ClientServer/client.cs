using CS;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientServer
{
    /// <summary>
    /// The Client class handles connecting to a server, sending requests, and receiving responses.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Initiates a connection to the server and handles the communication process.
        /// </summary>
        public async Task Connect()
        {
            // Instance of FindServer to scan and find the server's IP address
            var findServer = new FindServer();
            string ipServer = await findServer.ScanServer();

            while (true)
            {
                TcpClient client = new TcpClient();

                try
                {
                    // Attempt to connect to the server on the found active IP address and port 3232
                    client.Connect(ipServer, 3232);
                    Console.WriteLine("Client connected");

                    // Get the network stream for sending and receiving data
                    NetworkStream stream = client.GetStream();

                    // Read JSON data from the server
                    await LectureJSON(stream);
                    break; // Exit the loop after successful connection and data reception
                }
                catch (Exception ex)
                {
                    // Log any connection errors
                    Console.WriteLine("Connection error: " + ex.Message);

                    // Uncomment the following line if you want the client to retry connection after 2 seconds
                    // System.Threading.Thread.Sleep(2000);
                }
                finally
                {
                    // Ensure the client connection is closed properly
                    client.Close();
                }
            }
        }

        /// <summary>
        /// Reads JSON data from the network stream.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, with a string result containing the JSON data.</returns>
        public static async Task<string> LectureJSON(NetworkStream stream)
        {
            try
            {
                // MemoryStream to hold the incoming data
                MemoryStream flux = new MemoryStream();
                byte[] buffer = new byte[1024]; // Buffer for reading data
                int bytesRead;

                while (true)
                {
                    // Asynchronously read data from the stream into the buffer
                    bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead > 0)
                    {
                        // Write the read bytes into the memory stream
                        flux.Write(buffer, 0, bytesRead);
                    }
                    else
                    {
                        // Exit the loop if no more data is available
                        break;
                    }
                }

                // Convert the received byte array to a string
                string jsonData = Encoding.UTF8.GetString(flux.ToArray());
                Console.WriteLine("JSON file received:");
                Console.WriteLine(jsonData);

                return jsonData; // Return the received JSON data as a string
            }
            catch (Exception ex)
            {
                // Log any errors that occur during data reception
                Console.WriteLine("Error receiving the file: " + ex.Message);
                return null;
            }
        }
    }
}
