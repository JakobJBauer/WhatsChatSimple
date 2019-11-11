using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        static List<string> Users = new List<string>();
        static Dictionary<string, ConsoleColor> UserColors = new Dictionary<string, ConsoleColor>();

        static void AssignUserColor(string userName)
        {
            ConsoleColor color;
            switch (Users.Count % 9)
            {
                case 0:
                    color = ConsoleColor.Blue;
                    break;
                case 1:
                    color = ConsoleColor.Green;
                    break;
                case 2:
                    color = ConsoleColor.Magenta;
                    break;
                case 3:
                    color = ConsoleColor.Red;
                    break;
                case 4:
                    color = ConsoleColor.DarkBlue;
                    break;
                case 5:
                    color = ConsoleColor.DarkCyan;
                    break;
                case 6:
                    color = ConsoleColor.DarkGreen;
                    break;
                case 7:
                    color = ConsoleColor.DarkMagenta;
                    break;
                default:
                    color = ConsoleColor.DarkRed;
                    break;
            }
            UserColors.Add(userName, color);
        }
        
        static List<Dictionary<string, string>> refreshOutput = new List<Dictionary<string, string>>(); // msg, userName, timeStamp

        static void AddOutput(string message, string usrName, string timeStamp)
        {
            refreshOutput.Add(new Dictionary<string, string>());
            refreshOutput[refreshOutput.Count - 1].Add("msg", message);
            refreshOutput[refreshOutput.Count - 1].Add("userName", usrName);
            refreshOutput[refreshOutput.Count - 1].Add("timeStamp", timeStamp);
        }

        static string userName = "";
        static string keyInput = "";
        private static int beitrittsID;
        private static int currentWritingLine;
        private static bool showTime = true;

        static void Main(string[] args)
        {
            bool failed = false;
            
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("***** WhatsChat by Jakob Bauer *****");

            do
            {
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
                        "http://" + IP + ":" + port + "/").CreateChannel();

                    channel.SendMessage(new ChatMessage
                    {
                        UserName = userName,
                        Message = null,
                        DateTime = DateTime.Now
                    });
                }
                catch (Exception e)
                {
                    Console.Clear();
                    failed = true;
                    WriteSystemMessage("Something went wrong, please repeat your inputs!\n\n");
                }
            } while (failed);

            WriteSystemMessage("Username: ");
            while (true)
            {
                while (true)
                {
                    userName = Console.ReadLine();

                    if (userName != "")
                        break;
                    Console.SetCursorPosition(10, 4);
                }

                if (channel.GetUsers().Contains(userName))
                    WriteSystemMessage("User already registered, pick another name: ");
                else if(userName.Length >= 20)
                    WriteSystemMessage("Name is to long, please pick a shorter one: ");
                else
                    break;
            }
            
            Console.Write("Deactivate timestamps? (y): ");
            if (Console.ReadLine() == "y")
                showTime = false;

            WriteSystemMessage("Verbinde...");
            System.Threading.Thread.Sleep(1000);

            beitrittsID = channel.GetLastMessageID();

            var getMessages = new System.Threading.Thread(GetMessages);
            var update = new System.Threading.Thread(Update);
            var writeKeys = new System.Threading.Thread(WriteKeys);

            getMessages.Start();
            update.Start();
            writeKeys.Start();

            currentWritingLine = 2;
        }

        static void Update()
        {
            while (true)
            {
                System.Threading.Thread.Sleep(100);
                Console.Clear();
                foreach (var outElement in refreshOutput)
                {
                    if (outElement["msg"] == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine(outElement["userName"]+" joined at "+outElement["timeStamp"]);
                        Console.ResetColor();
                    }
                    else
                    {
                        if (showTime)
                        {
                            Console.ForegroundColor = ConsoleColor.Gray; 
                            Console.Write("["+outElement["timeStamp"]+"] ");
                        }
                        
                        Console.ForegroundColor = UserColors[outElement["userName"]];
                        Console.Write(outElement["userName"]+": ");
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine(outElement["msg"]);
                    }


                }
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("Message: " + keyInput);
                Console.ResetColor();
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
                    AddOutput(msg.Message, msg.UserName, msg.DateTime.ToString());

                    if (!Users.Contains(msg.UserName))
                    {
                        Users.Add(msg.UserName);
                        AssignUserColor(msg.UserName);
                    }
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
