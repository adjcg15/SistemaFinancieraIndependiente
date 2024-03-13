using SFIServices;
using System;
using System.ServiceModel;

namespace SFIHost
{
    internal class Server
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(SFIService)))
            {
                host.Open();
                Console.WriteLine("Server is running on http://localhost:8080");
                Console.ReadLine();
            }
        }
    }
}
