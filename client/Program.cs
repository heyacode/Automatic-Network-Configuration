using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var client = new Client();
            await client.Connect();
            Console.ReadLine();

        }
    }
}
