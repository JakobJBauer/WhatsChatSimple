using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using ChatLib;

namespace WhatsChat_Client
{
    class Program
    {
        static IPollingChat channel;
        static List<ChatMessage> ChatMessages = new List<ChatMessage>();
        static string refreshOutput = "***** WhatsChat by Jakob Bauer *****";

        static void Main(string[] args)
        {
            Console.WriteLine("***** WhatsChat by Jakob Bauer *****");
            Console.Write("Zielport: ");
            string port = Console.ReadLine();
            channel = new ChannelFactory<IPollingChat>(new WSHttpBinding(SecurityMode.None), "http://localhost:"+port+"/").CreateChannel();
            Console.WriteLine("Verbinde...");
            System.Threading.Thread.Sleep(1000);

            // add LogOn Text, use Userdata, Ask for Userdate

            var getMessages = new System.Threading.Thread(GetMessages);
            var update = new System.Threading.Thread(Update);
            var write = new System.Threading.Thread(Write);

            getMessages.Start();
            update.Start();
            write.Start();

        }

        static void Update()
        {

        }

        static void Write()
        {

        }

        static void GetMessages()
        {

        }
    }
}
