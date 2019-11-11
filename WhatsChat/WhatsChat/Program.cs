using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private static int beitrittsID;
        private static int currentWritingLine;

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("***** WhatsChat by Jakob Bauer *****");
            WriteSystemMessage("IP-Adress(empty for localhost): ");
            string IP = Console.ReadLine();
            if (IP.Length == 0)
            {
                IP = "localhost";
                Console.SetCursorPosition(32, 2);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("localhost");
                Console.ResetColor();
            }

            WriteSystemMessage("Zielport: ");
            string port = Console.ReadLine();
            try
            {
                channel = new ChannelFactory<IPollingChat>(new WSHttpBinding(SecurityMode.None),
                    "http://"+IP+":"+port+"/").CreateChannel();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            WriteSystemMessage("Username: ");
            while (true)
            {
                userName = Console.ReadLine();

                if (channel.GetUsers().Contains(userName))
                    WriteSystemMessage("User already registered, pick another name: ");
                else if(userName.Length >= 20)
                    WriteSystemMessage("Name is to long, please pick a shorter one: ");
                else
                    break;
            }

            WriteSystemMessage("Verbinde...");

            beitrittsID = channel.GetLastMessageID();

            var getMessages = new System.Threading.Thread(GetMessages);
            var update = new System.Threading.Thread(Update);
            var writeKeys = new System.Threading.Thread(WriteKeys);

            getMessages.Start();
            update.Start();
            writeKeys.Start();

            currentWritingLine = 2;

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
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(">" + keyInput);
                Console.ResetColor();
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

        static void WriteSystemMessage(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(msg);
            Console.ResetColor();
        }
    }
}
