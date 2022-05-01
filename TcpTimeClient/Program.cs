namespace Communicator
{
    using global::TcpTimeClient.Properties;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    internal class TcpTimeClient
    {
        private const int _port = 9999;

        private static void Main()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("tr");
            RunProgram();

            ExitProgram(0);
        }

        private static void RunProgram()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, _port);

            TcpClient client = new TcpClient();
            client.Connect(remoteEP);
            Console.WriteLine(Strings.LocalEndPointIs, client.Client.LocalEndPoint);

            NetworkStream ns = client.GetStream();
            byte[] bytes = new byte[1024];
            int bytesRead = ns.Read(bytes, 0, bytes.Length);
            Console.WriteLine(Encoding.ASCII.GetString(bytes, 0, bytesRead));
            client.Close();
        }

        private static void ExitProgram(int exitCode)
        {
            Console.WriteLine(Strings.PressEnterToExitProgram);
            _ = Console.ReadLine();
            Environment.Exit(exitCode);
        }
    }
}
