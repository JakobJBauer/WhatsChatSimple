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
        static string userName = "";
        static string keyInput = "";
        static int beitrittsID;

        static void Main(string[] args)
        {
            Console.WriteLine("***** WhatsChat by Jakob Bauer *****");
            Console.Write("Zielport: ");
            string port = Console.ReadLine();
            channel = new ChannelFactory<IPollingChat>(new WSHttpBinding(SecurityMode.None), "http://localhost:"+port+"/").CreateChannel();

            Console.Write("Username: ");
            userName = Console.ReadLine();

            Console.WriteLine("Verbinde...");

            beitrittsID = channel.GetLastMessageID();

            channel.SendMessage(new ChatMessage
            {

            });


            var getMessages = new System.Threading.Thread(GetMessages);
            var update = new System.Threading.Thread(Update);
            var writeKeys = new System.Threading.Thread(WriteKeys);

            getMessages.Start();
            update.Start();
            writeKeys.Start();

            System.Threading.Thread.Sleep(1000);
            Console.Clear();
            channel.SendMessage(new ChatMessage
            {
                UserName = userName,
                Message = null,
                DateTime = DateTime.Now
            });
        }

        static void Update()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine(refreshOutput);
                Console.Write(">" + keyInput);
                System.Threading.Thread.Sleep(100);
            }
        }

        static void WriteKeys()
        {
            ConsoleKeyInfo inpKey;
            while (true)
            {
                inpKey = Console.ReadKey();

                if(inpKey.Key == ConsoleKey.Enter)
                {
                    channel.SendMessage(new ChatMessage
                    {
                        UserName = userName,
                        DateTime = DateTime.Now,
                        Message = keyInput
                    });
                    keyInput = "";
                }
                else if(inpKey.Key == ConsoleKey.Backspace){
                    if (keyInput.Length >= 1)
                        keyInput = keyInput.Substring(0, (keyInput.Length - 1));
                }
                else
                {
                    try
                    {
                        keyInput += inpKey.KeyChar.ToString();
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                System.Threading.Thread.Sleep(50);
            }
        }

        static void GetMessages()
        {
            System.Threading.Thread.Sleep(20);
            while (true)
            {
                for(int i = ChatMessages.Count + beitrittsID + 1; i <= channel.GetLastMessageID(); i++)
                {
                    ChatMessage msg;
                    ChatMessages.Add(msg = channel.GetMessage(i));
                    if(msg.Message == null)
                        refreshOutput += "\n\n" + msg.UserName + " hat den Chat um " + msg.DateTime + " betreten.\n";
                    else
                        refreshOutput += "\n[" + msg.DateTime + "] - " + msg.UserName + ": " + msg.Message;
                }
            }
        }
    }
}
