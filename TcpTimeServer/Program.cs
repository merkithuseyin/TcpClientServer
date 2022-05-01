namespace TcpTimeServer
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    public class TcpTimeServer
    {
        const int _port = 9999;

        public static void Main(String[] args)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEP = new IPEndPoint(ipAddress, _port);

            var listener = new TcpListener(localEP);

            listener.Start();
            Console.WriteLine($"Server {localEP} started listening...");

            bool done = false;
            while (!done)
            {
                string message = DateTime.Now.ToString();
                AcceptAndRespond(listener, message);
            }
            listener.Stop();
            Console.WriteLine($"Server {localEP} stopped listening.");

            Console.WriteLine("Press enter to exit program...");
            Console.ReadLine();
        }

        static void AcceptAndRespond(TcpListener listener, string message)
        {
            Console.WriteLine("Waiting for connection...");
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine($"Connection accepted from {client.Client.RemoteEndPoint}");
            NetworkStream stream = client.GetStream();
            byte[] byteTime = Encoding.ASCII.GetBytes(message);
            try
            {
                stream.Write(byteTime, 0, byteTime.Length);
                Console.WriteLine($"Message sent: \"{message}\" --> {client.Client.RemoteEndPoint} ");
                Console.WriteLine($"");
                stream.Close();
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
