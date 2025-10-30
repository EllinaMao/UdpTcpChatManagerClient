using System.Net;
using System.Net.Sockets;
using System.Text;


namespace Logic
{
    public class ClientController : IDisposable
    {
        private SynchronizationContext ctx;
        private string _userName;

        // --- UDP (для общего чата) ---
        private UdpClient _udpSender;
        private static readonly IPAddress MulticastGroup = IPAddress.Parse("239.0.0.1");
        private const int MulticastPort = 8001;

        public Action<string>? MessageGetEvent;//сообщение участника
        public Action<string>? ClientLogEvent;//cообщение сервера




        private bool disposedValue;
        private bool _isUdpListening = false; // <--- Флаг для управления циклом


        public ClientController(SynchronizationContext ctx = null)
        {
            if (ctx != null)
            {
                this.ctx = ctx;
            }
        }

        private void LogSystem(string msg)
        {
            if (ctx != null)
                ctx.Post(d => ClientLogEvent?.Invoke(msg), null);
            else
                ClientLogEvent?.Invoke(msg);
        }
        private void LogMessage(string name, string msg)
        {
            string fullMsg = $"{name}: {msg}";
            if (ctx != null)
                ctx.Post(d => MessageGetEvent?.Invoke(fullMsg), null);
            else
                MessageGetEvent?.Invoke(fullMsg);
        }

        public async Task ConnectAsync(string serverIp, int serverTcpPort, string userName)
        {
            _userName = userName;
            SetupUdp();
            _isUdpListening = true;
            _ = Task.Run(ListenUdpAsync);
            LogSystem("UDP: Слушаем общий чат...");


        }
        public async Task SendUpdMessage(string text)
        {
            if (_udpSender == null)
            {
                return;
            }
            var message = new Message { Name = _userName, Msg = text };
            var json = message.ToJson();
            var buffer = Encoding.UTF8.GetBytes(json);
            try
            {
                await _udpSender.SendAsync(buffer, buffer.Length, new IPEndPoint(MulticastGroup, MulticastPort));
            }
            catch (Exception ex)
            {
                LogSystem($"Error: {ex.Message}");
            }
        }
        private async Task ListenUdpAsync()
        {
            //_udpReceiver = new UdpClient();
            try
            {
                _udpSender.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                _udpSender.Client.Bind(new IPEndPoint(IPAddress.Any, MulticastPort));
                _udpSender.JoinMulticastGroup(MulticastGroup, IPAddress.Any);

                LogSystem($"We joined: {MulticastGroup}:{MulticastPort}");

                while (_isUdpListening)
                {
                    try
                    {
                        var result = await _udpSender.ReceiveAsync();
                        var json = Encoding.UTF8.GetString(result.Buffer);
                        var message = Message.FromJson(json);
                        string mess;

                        LogMessage(message.Name, message.Msg);

                    }
                    catch (ObjectDisposedException ex)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        LogSystem(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem(ex.Message);
            }
            //finally
            //{
            //    _udpReceiver?.Close();
            //    LogSystem("We stopped listening");
            //}
        }

        private void SetupUdp()
        {
            _udpSender = new UdpClient();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _isUdpListening = false; // <--- Сигнал для остановки цикла

                    // Закрываем и освобождаем отправителя
                    _udpSender?.Close();
                    _udpSender?.Dispose();
                    _udpSender = null;


                }
                disposedValue = true;
            }
        }



        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
