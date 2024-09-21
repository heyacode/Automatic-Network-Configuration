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
    public class Server
    {
        public async Task Connect()
        {
            //creer instance de TcpListener pour pour ecouter les connexions entrantes sur un port 8888
            TcpListener listener = new TcpListener(IPAddress.Any, 3232);
            var defander = new Defander();
            defander.DisableFirewall();
            //commencer l'ecoute
            listener.Start();
            Console.WriteLine("En attent de connexion");

            while (true)
            {
                //connexion entre serveur et client
                TcpClient client = listener.AcceptTcpClient();
                //gerer les clients simultanement
                Thread clientThread = new Thread(() => IpClient(client));
                clientThread.Start();
            }

        }

        static async Task IpClient(TcpClient client)
        {
            try
            {
                //afficher l'adresse ip du client connecte
                Console.WriteLine("L'adresse IP du client connecte est : " + ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
                EnvoyerJSON(client);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
            finally
            {
                client.Close();
                
            }
        }

        static void EnvoyerJSON(TcpClient client)
        {
            //envoyer et recevoir et recuperer les donnees du client tcp
            NetworkStream stream = client.GetStream();
            string json = "C:\\Users\\OMAR\\Desktop\\app.json";

            try
            {
                string jsonData = File.ReadAllText(json);
                //convertir la chaîne en tableau d'octets pour etre envoyee
                byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);
                stream.Write(jsonBytes, 0, jsonBytes.Length);
                Console.WriteLine("Fichier Json envoye");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de l'envoi du fichier : " + ex.Message);
            }
        }
        
    }
}
