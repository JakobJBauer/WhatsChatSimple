﻿using System;
using System.Collections.Generic;
using System.Linq;
using ChatLib;

namespace ChatService
{
    class PollingChat : IPollingChat
    {
        public int GetLastMessageID()
        {
            try
            {
                return ChatManager.ChatMessages.Count == 0 ? 0 : ChatManager.ChatMessages.Last().ID;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
        }

        public ChatMessage GetMessage(int id)
        {
            try
            {
                return ChatManager.ChatMessages.FindLast(message => message.ID == id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
            
        }

        public int SendMessage(ChatMessage chatMessage)
        {
            try
            {
                ChatManager.ChatMessages.Add(chatMessage);
                chatMessage.ID = ChatManager.ChatMessages.IndexOf(chatMessage);

                if (!ChatManager.UserNames.Contains(chatMessage.UserName))
                {
                    ChatManager.UserNames.Add(chatMessage.UserName);
                }
                
                return chatMessage.ID;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
        }

        public List<string> GetUsers()
        {
            return ChatManager.UserNames;
        }
    }
}
