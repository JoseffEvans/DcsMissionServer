using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DcsTcp {
    public class DcsTcpSender(string dcsIp, int dcsPort) {
        public readonly string DcsIp = dcsIp;
        public readonly int DcsPort = dcsPort;

        public async Task SendMessage(string message) {
            var endpoint = new IPEndPoint(IPAddress.Parse(DcsIp), DcsPort);
            using var socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.IP);
            await socket.ConnectAsync(endpoint);

            socket.Send(Encoding.UTF8.GetBytes(message));
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }
}
