using System;
using System.Collections.Generic;
using ChatLib;

namespace ChatService
{
    public class ChatManager
    {
        public static List<ChatMessage> ChatMessages { get; private set; }
        

        public static void Init()
        {
            ChatMessages = new List<ChatMessage>();
        }

        public static void SendMessage(ChatMessage chatMessage)
        {
            ChatMessages.Add(chatMessage);
            chatMessage.ID = ChatMessages.IndexOf(chatMessage);
        }
    }
}