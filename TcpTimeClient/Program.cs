namespace Communicator
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    class TcpTimeClient
    {
        const int _port = 9999;

        static void Main(string[] args)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, _port);
            
            var client = new TcpClient();
            client.Connect(remoteEP);
            Console.WriteLine("Client End Point: " + client.Client.LocalEndPoint.ToString());

            NetworkStream ns = client.GetStream();
            byte[] bytes = new byte[1024];
            int bytesRead = ns.Read(bytes, 0, bytes.Length);
            Console.WriteLine(Encoding.ASCII.GetString(bytes, 0, bytesRead));
            client.Close();

            Console.WriteLine("Press enter to exit program...");
            Console.ReadLine();
        }
    }
}
