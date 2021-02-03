namespace Client
{
    class Server_Program
    {
        static void Main()
        {
            Server server = new Server("127.0.0.1", 4444);
            server.Start();
            server.Stop();
        }
    }
}
