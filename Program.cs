using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using CommandLine;
using CommandLine.Text;

namespace hello_tcp_service
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var parser = Parser.Default)
            {
                var result = parser.ParseArguments<Options>(args);
                switch (result.Tag)
                {
                    case ParserResultType.Parsed:
                        {
                            var parsed = (Parsed<Options>)result;
                            switch (parsed.Value.ServiceType)
                            {
                                case ServiceType.Server:
                                    RunServer(parsed.Value.Port);
                                    break;
                                case ServiceType.Client:
                                    RunClient(parsed.Value.Port, parsed.Value.IsServerShutdownRequested);
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    case ParserResultType.NotParsed:
                        var notParsed = (NotParsed<Options>)result;
                        Console.WriteLine(HelpText.AutoBuild(notParsed));
                        break;
                };
            }
        }

        private static void RunServer(int port)
        {
            var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            var endPoint = new IPEndPoint(IPAddress.Loopback, port);
            socket.Bind(endPoint);
            socket.Listen();

            var loopCount = 0;
            var shouldClose = false;
            while (!shouldClose)
            {
                if (socket.Poll(TimeSpan.FromSeconds(1).Milliseconds, SelectMode.SelectRead))
                {
                    var client = socket.Accept();
                    var buffer = new byte[256];
                    var count = client.Receive(buffer);

                    if (count == 1)
                    {
                        shouldClose = true;
                    }
                    else
                    {
                        var data = System.Text.Encoding.ASCII.GetString(buffer, 0, count);
                        Console.WriteLine(data);
                    }

                    var response = new byte[] { 0, 0 };
                    client.Send(response);
                }

                // Thread.Sleep(TimeSpan.FromSeconds(1).Milliseconds);
                Thread.Sleep(1000);
                Console.WriteLine($"Loop: {loopCount++}");
            }

            Console.WriteLine("Close Server");
        }

        private static void RunClient(int port, bool isServerShutdownRequested)
        {
            using (var client = new TcpClient(IPAddress.Loopback.ToString(), port))
            using (var stream = client.GetStream())
            {
                if (isServerShutdownRequested)
                {
                    stream.Write(new byte[] { 0 });
                }
                else
                {
                    var year = DateTime.Now.ToString("yyyy");
                    var month = DateTime.Now.ToString("MM");
                    var day = DateTime.Now.ToString("MM");
                    var hour = DateTime.Now.ToString("HH");
                    var minute = DateTime.Now.ToString("mm");
                    var second = DateTime.Now.ToString("ss");

                    var current = $"{year}-{month}-{day}-{hour}-{minute}-{second}";
                    var data = System.Text.Encoding.ASCII.GetBytes(current);
                    stream.Write(data);
                }

                _ = stream.ReadByte();
            }
        }
    }
}
