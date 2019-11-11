using System;
using System.Collections.Generic;
using ChatLib;

namespace ChatService
{
    public class ChatManager
    {
        public static List<ChatMessage> ChatMessages { get; private set; }
        
        public static List<string> UserNames { get; private set; }
        

        public static void Init()
        {
            ChatMessages = new List<ChatMessage>();
            UserNames = new List<string>();
        }
    }
}