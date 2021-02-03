using System;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
using System.IO;
using Packets;


namespace Client
{
    class Client
    {
        private readonly TcpClient tcpClient;
        private ClientForm clientForm;
        NetworkStream m_stream;
        BinaryWriter m_writer;
        BinaryReader m_reader;
        BinaryFormatter m_formatter;
        private readonly object m_readLock;
        private readonly object m_writeLock;

        
        public int ID;
        public string Name;

        public Client()
        {
            tcpClient = new TcpClient();
            m_readLock = new object();
            m_writeLock = new object();
        }


        public bool Connect(string ipAddress, int port)
        {
            try
            {
                tcpClient.Connect(ipAddress, port);
                m_stream = tcpClient.GetStream();
                m_writer = new BinaryWriter(m_stream);
                m_reader = new BinaryReader(m_stream);
                m_formatter = new BinaryFormatter();

                return true;
            }
            catch(InvalidCastException e)
            {
                Console.WriteLine("Exception: " + e.Message);
                return false;
            }
        }

        public void Close()
        {
            m_stream.Close();
            m_reader.Close();
            m_writer.Close();
        }

        public void Run()
        {
            clientForm = new ClientForm(this);
            Thread thread = new Thread(() => { ProcessServerResponce(); });
            thread.Start();
            clientForm.ShowDialog();
            tcpClient.Close();
        }


        private void ProcessServerResponce()
        {
            Console.Write(">>>");
            Packet packet;
            while ((packet = Read()) != null)
            {
                    switch (packet.m_packetType)
                    {
                        case (PacketType.CHATMESSAGE):
                        {
                            ChatMessagePacket chatMessage = (ChatMessagePacket)packet;
                            clientForm.UpdateChatWindow(chatMessage.m_message);
                            break;
                        }
                        case (PacketType.CHATLOG):
                        {
                            ChatLog chatLog = (ChatLog)packet;
                            clientForm.UpdateChatWindow(chatLog.m_ChatLog);
                            break;
                        }
                        case (PacketType.CLIENTLISTUPDATE):
                        {
                            ClientListUpdate clientList = (ClientListUpdate)packet;
                            //clientForm.UpdateClientList(clientList.m_clientName);
                            break;
                        }
                        case (PacketType.EXIT):
                        {
                            ExitPacket exitPacket = (ExitPacket)packet;
                            Console.WriteLine(exitPacket.m_message);
                            break;
                        }
                        default:
                        {
                             break;
                        }
                    }
            }
        }


        public Packet Read()
        {
            int numberOfBytes;
            lock (m_readLock)
            {

                if ((numberOfBytes = m_reader.ReadInt32()) != -1)
                {
                    byte[] buffer = m_reader.ReadBytes(numberOfBytes);
                    MemoryStream memStream = new MemoryStream(buffer);
                    return m_formatter.Deserialize(memStream) as Packet;
                }
                else
                {
                    ExitPacket packet = new ExitPacket("User: " + Name + "Disconnected");
                    return packet;
                }
                

            }
        }
        public void SendMessage(Packet message)
        {
            lock (m_writeLock) 
            { 
                MemoryStream memStream = new MemoryStream();
                m_formatter.Serialize(memStream, message);
                byte[] buffer = memStream.GetBuffer();
                m_writer.Write(buffer.Length);
                m_writer.Write(buffer);
                m_writer.Flush();
                Console.WriteLine("Message Packet send to sever...");
            }
        }
    }
}
