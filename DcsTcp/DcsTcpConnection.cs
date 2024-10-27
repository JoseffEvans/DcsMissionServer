using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DcsTcp {

    /// <summary>
    /// Two way connection to a DCS server
    /// </summary>
    public class DcsTcpConnection(string ip, int port, string dcsIp, int dcsPort) : IDisposable {

        public readonly string Ip = ip;
        public readonly int Port = port;
        public readonly string DcsIp = dcsIp;
        public readonly int DcsPort = dcsPort;
        public Thread? Thread { get; protected set; }

        const int BUFFER_SIZE = 1024 * 8;

        CancellationTokenSource? ServerToken;

        /// <summary> Runs a TCP server in a new thread </summary>
        /// <param name="onMessageRecived">Parameter is the message from DCS, return value is response to DCS</param>
        public void StartServer(Func<string, string> onMessageRecived) {
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

                    await RunServerLoop(listener, onMessageRecived);
                } catch(OperationCanceledException) {
                    return;
                } catch(Exception ex) {
                    throw new Exception(
                        "An exception occurred when starting/runnning server.", ex
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

        /// <returns>Response from server</returns>
        public string SendMessage(string message) {
            throw new NotImplementedException();
        }

        async Task RunServerLoop(Socket listener, Func<string, string> onMessage) {
            if(ServerToken is null)
                throw new Exception("Server Token cannot be null when running the server loop");

            while(!ServerToken.Token.IsCancellationRequested) {
                using var handler = await listener.AcceptAsync(ServerToken.Token);
                handler.SendBufferSize = BUFFER_SIZE;
                handler.ReceiveBufferSize = BUFFER_SIZE;

                var messageBuilder = new StringBuilder();
                var buffer = new byte[BUFFER_SIZE];

                try {
                    while(true) {
                        var received = await handler.ReceiveAsync(buffer);
                        if(received == 0) break;
                        messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, received));
                    }

                    var response = Encoding.UTF8.GetBytes(
                        onMessage(messageBuilder.ToString())
                    );

                    for(int i = 0; i < response.Length; i += BUFFER_SIZE)
                        await handler.SendAsync(
                            response.AsMemory(i, Math.Min(BUFFER_SIZE, response.Length - i))
                        );

                } catch(OperationCanceledException) {
                    break;
                } catch(Exception ex) {
                    throw new Exception($"An error occurred while running dcs tcp server loop.", ex);
                } finally {
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
        }
    }
}
