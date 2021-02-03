using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Packets;


namespace Client
{
  
    class Server
    {
        readonly TcpListener tcpListener;
        ConcurrentDictionary<int, ServerClient> m_Clients;
        string m_chatLog;
        public Server(string ipAddress, int port)
        {
            IPAddress IPAddress = IPAddress.Parse(ipAddress);
            tcpListener = new TcpListener(IPAddress, port);
        }


        // Starts the server logic and a thread to listen to the clients concurrently
        public void Start()
        {
            bool running = true;
            m_Clients = new ConcurrentDictionary<int, ServerClient>();
            int clientIndex = 0;
            tcpListener.Start();

            while (running)
            {
                Socket socket = tcpListener.AcceptSocket();
                ServerClient client = new ServerClient(socket);
                m_Clients.TryAdd(clientIndex, client);
                int index = clientIndex;

                clientIndex++;

                Thread thread = new Thread(() => { ClientMethod(index, client); });
                thread.Start();

                Console.WriteLine("Connected...");
            }
        }


        // Stops the tcp listener
        public void Stop()
        {
            tcpListener.Stop();
        }


        // Logic to determine what to do with incoming packets 
        private void ClientMethod(int index, ServerClient client)
        {
            try
            {
                //m_Clients[index].SetClientData(index, "User" + index.ToString());
                Packet packet;
                ServerClient m_client = client;
                while ((packet = m_client.Read()) != null)
                {
                    Console.WriteLine("ServerClient >>> Message Recieved");
                    switch (packet.m_packetType)
                    {
                        case (PacketType.CHATMESSAGE):
                            {
                                ChatMessagePacket chatPacket = (ChatMessagePacket)packet;
                                for (int i = 0; i < m_Clients.Count; i++)
                                {
                                    m_Clients[i].Send(chatPacket);
                                }

                                Console.WriteLine("Server broadcasted >>> ChatMessagePacket from Client " + index);
                                break;
                            }
                        case (PacketType.PRIVATEMESSAGE):
                            {
                                PrivateChatMessagePacket privateMessage = (PrivateChatMessagePacket)packet;
                                m_Clients[privateMessage.m_clientID].Send(privateMessage);
                                m_Clients[privateMessage.m_receiverID].Send(privateMessage);
                                Console.WriteLine("Server Recieved >>> Private Message from Client " + privateMessage.m_clientID + " To Client " + privateMessage.m_receiverID);
                                break;
                            }
                        case (PacketType.CLIENTNAME):
                            {
                                m_Clients[index].SetClientData(index, m_Clients[index].m_clientData.Name);
                                Console.WriteLine("Client name: " + m_Clients[index].m_clientData.Name + "Saved to server");

                                break;
                            }

                        case (PacketType.CHATLOG):
                            {
                                ChatLog chatLog = (ChatLog)packet;
                                m_chatLog = chatLog.m_ChatLog;
                                break;
                            }

                        case (PacketType.CLIENTLISTUPDATE):
                            {
                                ClientListUpdate clientList = (ClientListUpdate)packet;
                                for (int i = 0; i < m_Clients.Count; i++)
                                {
                                    m_Clients[i].Send(clientList);
                                }
                                break;
                            }
                        case (PacketType.EMPTYPACKET):
                            {
                                break;
                            }
                        case (PacketType.EXIT):
                            {
                                m_client.Send(packet);
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
                m_client.Close();
                ServerClient c;
                m_Clients.TryRemove(index, out c);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured" + e.Message);
            }


        }


    //    private string GetReturnMessage(string code)
    //    {
    //        string returnValue;
    //        string altCode = code;
    //        altCode = altCode.ToLower();
    //        altCode = altCode.Trim();
    //        altCode = Regex.Replace(altCode, @"\s", "");


    //        switch (altCode)
    //        {
    //            case "hi":
    //                {
    //                    returnValue = "Hello User!";
    //                    break;
    //                }

    //            case "hello":
    //                {
    //                    returnValue = "Hello User!";
    //                    break;
    //                }

    //            case "youarepoorlymade":
    //                {
    //                    returnValue = "No you!";
    //                    break;
    //                }

    //            case "howareyou":
    //                {
    //                    returnValue = "Great";
    //                    break;
    //                }

    //            case "ilikemusic":
    //                {
    //                    returnValue = "Me too!";
    //                    break;
    //                }

    //            default:
    //                {
    //                    returnValue = "Input not understood, try again.";
    //                    break;
    //                }
    //        }
    //        return returnValue;
    //    }
    }
}
