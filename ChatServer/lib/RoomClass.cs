﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using ChatLib;
using static ChatLib.Interactions;
namespace ChatServer.lib
{
    /// <summary>
    /// Класс, описывающий поведение комнаты
    /// </summary>
    class RoomClass
    {
        public string name { get; private set; }
        public List<ClientClass> connectedUsers = new List<ClientClass>();
        public MySqlConnection connection = DBmanager.Connect();
        public RoomClass(string name)
        {
            this.name = name;
        }
        protected internal void AddClient(ClientClass client)
        {
            client.room = this;
            connectedUsers.Add(client);
            Console.WriteLine("Successfully added client " + client.name + " to " + name +
                " room. There are " + connectedUsers.Count + " connected users.");
            connectedUsers.ForEach(user => SendToStream(new MessageClass(codes.SENDING_USERLIST,
                list: connectedUsers.Select(u => u.name).ToList()), ref user.client));
            SendToStream(new MessageClass(codes.SENDING_CHAT_HIST, list: DBmanager.GetHistory(name, client.connection)), ref client.client);
            Task.Run(() => SendBroadcastMessage(client.name + " joined the room.", connection));
        }
        protected internal void RemoveClient(int id)
        {
            ClientClass client = connectedUsers.FirstOrDefault(i => i.id == id);
            if (client != null)
                connectedUsers.Remove(client);
            SendBroadcastMessage(client.name + " left the room.", connection);
            connectedUsers.ForEach(user => SendToStream(new MessageClass(codes.SENDING_USERLIST,
                list: connectedUsers.Select(u => u.name).ToList()), ref user.client));
            Console.WriteLine(client.name + " left the " + name + " room. There are " + connectedUsers.Count + " connected users.");
        }
        /// <summary>
        /// Отправление всем пользователям комнаты сообщения
        /// </summary>
        /// <param name="message"></param>
        /// <param name="connection"></param>
        public void SendBroadcastMessage(string message, MySqlConnection connection)
        {
            DBmanager.SaveMessage(message, name, ref connection);
            Console.Write("Broadcasting for: ");
            for (int i = 0; i < connectedUsers.Count; i++)
            {
                Console.Write(connectedUsers[i].name + (i + 1 == connectedUsers.Count ? "" : ", "));
                SendToStream(new MessageClass(codes.SENDING_BROADCAST_MESSAGE, DateTime.Now.ToString("HH:mm:ss") +
                    "  ||  " + message), ref connectedUsers[i].client);
            }
            Console.WriteLine(".");
        }
    }
}
