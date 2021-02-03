using System;
using System.IO;
using System.Runtime.Serialization;


namespace Packets
{
    public enum PacketType
    {
        CHATMESSAGE,
        PRIVATEMESSAGE,
        CLIENTNAME,
        CLIENTLISTUPDATE,
        CHATLOG,
        EXIT,
        EMPTYPACKET
    };

    public struct ClientData
    {
        public int ID;
        public string Name;
    }

    [Serializable()]
    public abstract class Packet
    {
        public PacketType m_packetType { get; protected set; }
    }

    [Serializable()]
    public class ChatMessagePacket : Packet
    {
        public string m_message;
        public int m_clientID;
        public ChatMessagePacket(string message, int clientID)
        {
            m_message = message;
            m_clientID = clientID;
            m_packetType = PacketType.CHATMESSAGE;
        }
    }

    [Serializable()]
    public class PrivateChatMessagePacket : Packet
    {
        public string m_message;
        public int m_clientID;
        public int m_receiverID;
        public PrivateChatMessagePacket(string message, int clientID, int receiverID)
        {
            m_message = message;
            m_clientID = clientID;
            m_receiverID = receiverID;
            m_packetType = PacketType.PRIVATEMESSAGE;
        }
    }


    [Serializable()]
    public class ClientName : Packet
    {
        public string m_clientName;
        public int m_clientID;

        public ClientName(string clientName, int clientID)
        {
            m_clientName = clientName;
            m_clientID = clientID;
            m_packetType = PacketType.CLIENTNAME;
        }

    }


    [Serializable()]
    public class ClientListUpdate : Packet
    {
        public string m_clientName;
        public int m_clientID;

        public ClientListUpdate(string clientName, int clientID)
        {
            m_clientName = clientName;
            m_clientID = clientID;
            m_packetType = PacketType.CLIENTLISTUPDATE;
        }

    }


    [Serializable()]
    public class ChatLog : Packet
    {
        public string m_ChatLog;

        public ChatLog(string log)
        {
            m_ChatLog = log;
            m_packetType = PacketType.CHATLOG;
        }
    }

    [Serializable()]
    public class ExitPacket : Packet
    {
        public string m_message;

        public ExitPacket(string message)
        {
            m_message = message;
            m_packetType = PacketType.EXIT;
        }
    }

    [Serializable()]
    public class EmptyPacket : Packet
    {
        public string m_message;

        public EmptyPacket(string message)
        {
            m_message = message;
            m_packetType = PacketType.EMPTYPACKET;
        }
    }
}