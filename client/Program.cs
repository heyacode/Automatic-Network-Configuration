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
        static async Task Main(string[] args)
        {
            // Create an instance of the Client class
            var client = new Client();

            // Connect to the server asynchronously
            await client.Connect();

            // Wait for the user to press Enter before closing the application
            Console.ReadLine();
        }
    }
}
