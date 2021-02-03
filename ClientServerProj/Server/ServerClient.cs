using System;
using Packets;
using System.Net.Sockets;
using System.IO;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace Client
{
    public class ServerClient
    {
        private readonly Socket m_socket;
        private readonly NetworkStream m_stream;
        private readonly BinaryWriter m_writer;
        private readonly BinaryReader m_reader;
        private readonly BinaryFormatter m_formatter;
        private readonly object m_readLock;
        private readonly object m_writeLock;
        public ClientData m_clientData;
        

        // Initialize class objects
        public ServerClient(Socket socket)
        {
            m_socket = socket;
            m_readLock = new object();
            m_writeLock = new object();
            m_stream = new NetworkStream(m_socket);
            m_reader = new BinaryReader(m_stream);
            m_writer = new BinaryWriter(m_stream);
            m_formatter = new BinaryFormatter();
        }


        // Close stream, reader, writer and socket objects
        public void Close()
        {
            m_stream.Close();
            m_reader.Close();
            m_writer.Close();
            m_socket.Close();
        }


        // Reads the serialized packets coming from the client
        public Packet Read()
        {
            int numberOfBytes;
            lock (m_readLock)
            {
                if((numberOfBytes = m_reader.ReadInt32()) != -1)
                {
                    byte[] buffer = m_reader.ReadBytes(numberOfBytes);
                    MemoryStream memStream = new MemoryStream(buffer);
                    return m_formatter.Deserialize(memStream) as Packet;
                }
                else
                {
                    EmptyPacket packet = new EmptyPacket("Error: Empty Packet detected");
                    return packet;
                }
            }
        }


        // Sends a packet to the client
        public void Send(Packet message)
        {
            lock (m_writeLock)
            {
                MemoryStream memStream = new MemoryStream();
                m_formatter.Serialize(memStream, message);
                byte[] buffer = memStream.GetBuffer();
                m_writer.Write(buffer.Length);
                m_writer.Write(buffer);
                m_writer.Flush();
            }
        }

        public void SetClientData(int ID, string userName)
        {
            m_clientData.ID = ID;
            m_clientData.Name = userName;
        }

        public ClientData GetClientData()
        {
            return m_clientData;
        }
    }
}
