using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DcsTcp {

    /// <summary>
    /// Two way connection to a DCS server
    /// </summary>
    public class DcsTcpReciver(string ip, int port) : IDisposable, IHostedService {

        public readonly string Ip = ip;
        public readonly int Port = port;
        public Thread? Thread { get; protected set; }
        public Func<string, string> OnMessageReceived { get; set; } = (message) => { return string.Empty; };

        const int BUFFER_SIZE = 1024 * 8;

        CancellationTokenSource? ServerToken;

        public void StartServer() {
            if(Thread is not null)
                throw new Exception("Server is already running, call StopServer");

            Thread = new Thread(async () => {
                try {
                    var endpoint = new IPEndPoint(IPAddress.Parse(Ip), Port);

                    using var listener = new Socket(
                        endpoint.AddressFamily,
                        SocketType.Stream,
                        ProtocolType.Tcp
                    );

                    ServerToken = new();

                    listener.Bind(endpoint);
                    listener.Listen(100);

                    await RunServerLoop(listener);
                } catch(OperationCanceledException) {
                    return;
                } catch(Exception ex) {
                    throw new Exception(
                        "An exception occurred when starting/running server.", ex
                    );
                }
            });

            Thread.Start();
        }

        public void StopServer() {
            if(Thread is not null) {
                if(ServerToken is null)
                    throw new Exception("Server thread did not create a cancellation token");
                ServerToken.Cancel();
                Thread.Join();
                ServerToken.Dispose();
                ServerToken = null;
                Thread = null;
            }
        }

        public void Dispose() {
            StopServer();
        }


        async Task RunServerLoop(Socket listener) {
            if(ServerToken is null)
                throw new Exception("Server Token cannot be null when running the server loop");

            while(!ServerToken.Token.IsCancellationRequested) {
                using var socket = await listener.AcceptAsync(ServerToken.Token);
                socket.SendBufferSize = BUFFER_SIZE;
                socket.ReceiveBufferSize = BUFFER_SIZE;

                var messageBuilder = new StringBuilder();
                var buffer = new byte[BUFFER_SIZE];

                try {
                    while(true) {
                        var received = await socket.ReceiveAsync(buffer);
                        if(received == 0) break;
                        messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, received));
                    }

                    var response = Encoding.UTF8.GetBytes(
                        OnMessageReceived(messageBuilder.ToString())
                    );

                    for(int i = 0; i < response.Length; i += BUFFER_SIZE)
                        await socket.SendAsync(
                            response.AsMemory(i, Math.Min(BUFFER_SIZE, response.Length - i))
                        );

                } catch(OperationCanceledException) {
                    break;
                } catch(Exception ex) {
                    throw new Exception($"An error occurred while running dcs tcp server loop.", ex);
                } finally {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken) {
            StartServer();
        }

        public async Task StopAsync(CancellationToken cancellationToken) {
            StopServer();
        }
    }
}
