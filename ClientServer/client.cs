using CS;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientServer
{
    public class Client
    {
        public async Task Connect()
        {
            var findServer = new FindServer();
            string ipServer = await findServer.ScanServer();
            while (true)
            {
                TcpClient client = new TcpClient();
                try
                {
                    client.Connect(ipServer, 3232);
                    Console.WriteLine("Client connecte");
                    //send and receive data
                    NetworkStream stream = client.GetStream();
                    //read data
                    await LectureJSON(stream);
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur de connexion : " + ex.Message);
                    //System.Threading.Thread.Sleep(2000);
                    //try a new connection with a new server after 2s
                }
                finally
                {
                    client.Close();
                }
            }

        }

        public static async Task<string> LectureJSON(NetworkStream stream)
        {
            try
            {
                MemoryStream flux = new MemoryStream();
                byte[] buffer = new byte[1024];
                int bytesRead;
                while (true)
                {
                    bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead >= 0)
                    {
                        flux.Write(buffer, 0, bytesRead);
                        break;
                    }
                }

                string jsonData = Encoding.UTF8.GetString(flux.ToArray());
                Console.WriteLine("Fichier JSON reçu :");
                Console.WriteLine(jsonData);
                return jsonData;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la réception du fichier : " + ex.Message);
                return null;
            }
        }

    }
}


