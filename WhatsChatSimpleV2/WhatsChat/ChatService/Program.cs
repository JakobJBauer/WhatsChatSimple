using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ChatLib;

namespace ChatService
{
    class Program
    {
        static void Main(string[] args)
        {
            ChatManager.Init();
            
            Console.WriteLine("***** WhatsChat - Jakob Bauer, 5AHEL, 2019/20 ******");
            Console.WriteLine("Starting Polling-Service...");
            var pollingService = new ServiceHost(typeof(PollingChat));
            pollingService.AddServiceEndpoint(
                typeof(IPollingChat),
                new WSHttpBinding(SecurityMode.None),
                "http://localhost:2310/"
            );
            pollingService.Open();
            Console.WriteLine("Chat Service started! Press any key to stop service...\n\n");

            Console.ReadLine();

            pollingService.Close();
        }
    }
}
