namespace TcpTimeServer
{
    using global::TcpTimeServer.Properties;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public class TcpTimeServer
    {
        private const int _port = 9999;
        private static TcpListener listener;

        public static void Main()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("tr");

            try
            {
                SetUpEventHandlers();

                RunProgram();
            }
            catch (SocketException)
            {
                // stoping TcpListener from another thread thorws WSACancelBlockingCall exception
                ExitProgram(0);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                ExitProgram(310);
            }
        }

        #region Methods
        private static void RunProgram()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEP = new IPEndPoint(ipAddress, _port);

            listener = new TcpListener(localEP);

            listener.Start();
            Console.WriteLine(Strings.ServerStartedListening, localEP);

            while (true)
            {
                string message = DateTime.Now.ToString();
                AcceptAndRespond(listener, message);
            }
        }

        private static void AcceptAndRespond(TcpListener listener, string message)
        {
            Console.WriteLine(Strings.WaitingForConnection);
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine(Strings.ConnectionAcceptedFrom, client.Client.RemoteEndPoint);
            NetworkStream stream = client.GetStream();
            byte[] byteTime = Encoding.ASCII.GetBytes(message);
            try
            {
                stream.Write(byteTime, 0, byteTime.Length);
                Console.WriteLine(Strings.MessageSent, message, client.Client.RemoteEndPoint);
                stream.Close();
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                stream.Dispose();
                client.Dispose();
            }
        }

        /// <param name="exitCode">
        ///     The exit code to return to the operating system. 
        ///     Use 0 (zero) to indicate that the process completed successfully.</param>
        private static void ExitProgram(int exitCode)
        {
            // https://docs.microsoft.com/en-gb/windows/win32/debug/system-error-codes--0-499-
            Console.WriteLine(Strings.PressEnterToExitProgram);
            _ = Console.ReadLine();
            Environment.Exit(exitCode);
        }
        #endregion

        #region Event Handlers
        private static event EventHandler OnEscPressed;

        private static void SetUpEventHandlers()
        {
            _ = Task.Run(() => ListenToEscapeKey());
        }

        private static void ListenToEscapeKey()
        {
            OnEscPressed += Host_OnEscPressed;
            while (true)
            {
                ConsoleKey key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Escape)
                {
                    OnEscPressed?.Invoke(null, EventArgs.Empty);
                }
            }
        }
        #endregion

        #region Events
        private static void Host_OnEscPressed(object sender, EventArgs e)
        {
            listener.Stop();
            Console.WriteLine(Strings.ServerStoppedListening);
        }
        #endregion
    }
}
