using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServer
{
    /// <summary>
    /// The Program class contains the entry point of the application.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The main entry point of the application.
        /// </summary>
        /// <param name="args">An array of command-line arguments.</param>
        static void Main(string[] args)
        {
            // Create an instance of the Server class
            var server = new Server();

            // Start the server and listen for incoming connections
            server.Connect();

            // Wait for the user to press Enter before closing the application
            Console.ReadLine();
        }
    }
}
