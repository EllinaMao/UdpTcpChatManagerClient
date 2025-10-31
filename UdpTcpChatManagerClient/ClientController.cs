using System; // Добавлено
using System.Collections.Generic; // Добавлено
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json; // Добавлено
using Logic.MessagesFiles; // Добавлено (убедись, что пространство имен верное)
using System.IO; // Добавлено

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
        private bool _isUdpListening = false;

        // --- TCP (для управления) ---
        private TcpClient _tcpClient;
        private NetworkStream _tcpStream;
        private bool _isTcpListening = false;

        // --- События для UI ---
        public Action<string>? MessageGetEvent; // (Уже было) сообщение участника
        public Action<string>? ClientLogEvent;  // (Уже было) cообщение сервера

        // --- НОВЫЕ СИСТЕМНЫЕ СОБЫТИЯ ---
        public Action<List<string>>? UserListUpdatedEvent; // Для ListBox'а
        public Action<List<Message>>? HistoryReceivedEvent; // Для чата

        private bool disposedValue;

        public ClientController(SynchronizationContext ctx = null)
        {
            if (ctx != null)
            {
                this.ctx = ctx;
            }
        }

        // --- Логгеры (без изменений) ---
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

        // --- ОБНОВЛЕННЫЙ МЕТОД ---
        public async Task ConnectAsync(string serverIp, int serverTcpPort, string userName)
        {
            _userName = userName;

            // 1. Настраиваем UDP (как и было)
            SetupUdp();
            _isUdpListening = true;
            _ = Task.Run(ListenUdpAsync);
            LogSystem("UDP: Слушаем общий чат...");

            // 2. --- НОВОЕ: ПОДКЛЮЧАЕМСЯ ПО TCP ---
            try
            {
                _tcpClient = new TcpClient();
                LogSystem($"TCP: Подключаемся к {serverIp}:{serverTcpPort}...");
                await _tcpClient.ConnectAsync(serverIp, serverTcpPort);

                if (_tcpClient.Connected)
                {
                    _tcpStream = _tcpClient.GetStream();
                    LogSystem("TCP: Подключено. Отправляем регистрацию...");

                    // 3. --- НОВОЕ: РЕГИСТРАЦИЯ ---
                    // Отправляем ConnectMessage, как договорились
                    var connectMsg = new ConnectMessage { Name = _userName };
                    string json = JsonSerializer.Serialize(connectMsg);
                    byte[] buffer = Encoding.UTF8.GetBytes(json);
                    await _tcpStream.WriteAsync(buffer, 0, buffer.Length);

                    // 4. --- НОВОЕ: ЗАПУСК TCP СЛУШАТЕЛЯ ---
                    _isTcpListening = true;
                    _ = Task.Run(ListenTcpServerAsync);
                }
            }
            catch (Exception ex)
            {
                LogSystem($"TCP Ошибка подключения: {ex.Message}");
                LogSystem("Убедитесь, что сервер запущен и IP/порт верны.");
            }
        }

        // --- НОВЫЙ МЕТОД (Отправка PM) ---
        // (Вызывать из UI, когда юзер пишет приватное сообщение)
        public async Task SendPrivateMessageAsync(string toUser, string message)
        {
            if (_tcpStream == null || !_isTcpListening) return;

            try
            {
                var pmRequest = new PrivateMessageRequest
                {
                    ToUser = toUser,
                    Message = message
                };

                string json = JsonSerializer.Serialize(pmRequest);
                byte[] buffer = Encoding.UTF8.GetBytes(json);

                await _tcpStream.WriteAsync(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                LogSystem($"TCP Ошибка отправки PM: {ex.Message}");
            }
        }

        // --- (UDP Отправка - без изменений) ---
        public async Task SendUpdMessage(string text)
        {
            if (_udpSender == null) return;

            var message = new Message { Name = _userName, Msg = text };

            // (Заменил message.ToJson() на прямой JsonSerializer)
            var json = JsonSerializer.Serialize(message);
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

        // --- НОВЫЙ МЕТОД (TCP Слушатель) ---
        private async Task ListenTcpServerAsync()
        {
            byte[] buffer = new byte[4096];
            try
            {
                while (_isTcpListening && _tcpClient.Connected)
                {
                    int bytesRead = await _tcpStream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        LogSystem("TCP: Сервер разорвал соединение.");
                        break;
                    }

                    string serverJson = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    // --- НОВОЕ: Логика "подсматривания" ---
                    TcpMessage baseMsg = null;
                    try
                    {
                        baseMsg = JsonSerializer.Deserialize<TcpMessage>(serverJson);
                    }
                    catch { /* Игнорируем битый JSON */ }

                    if (baseMsg == null) continue;

                    // --- НОВОЕ: Обработка команд от сервера ---
                    switch (baseMsg.Type)
                    {
                        case "Error":
                            var errorMsg = JsonSerializer.Deserialize<ErrorMessage>(serverJson);
                            LogSystem($"!!! ОШИБКА СЕРВЕРА: {errorMsg.Reason} !!!");
                            // Сервер сам разорвет соединение, так что выходим
                            break;

                        case "UserList":
                            var userListMsg = JsonSerializer.Deserialize<UserListMessage>(serverJson);
                            // Вызываем событие, передавая список в UI-поток
                            if (ctx != null)
                                ctx.Post(d => UserListUpdatedEvent?.Invoke(userListMsg.Users), null);
                            else
                                UserListUpdatedEvent?.Invoke(userListMsg.Users);
                            break;

                        case "HistoryMessage": // (Имя типа из твоего класса)
                            var historyMsg = JsonSerializer.Deserialize<HistoryMessage>(serverJson);
                            if (historyMsg?.Messages != null)
                            {
                                // Вызываем событие, передавая историю в UI-поток
                                if (ctx != null)
                                    ctx.Post(d => HistoryReceivedEvent?.Invoke(historyMsg.Messages), null);
                                else
                                    HistoryReceivedEvent?.Invoke(historyMsg.Messages);
                            }
                            break;

                        case "PrivateMessage":
                            var relayMsg = JsonSerializer.Deserialize<PrivateMessageRelay>(serverJson);
                            LogMessage($"[PM от {relayMsg.FromUser}]", relayMsg.Message);
                            break;
                    }
                }
            }
            catch (IOException)
            {
                LogSystem("TCP: Соединение с сервером потеряно.");
            }
            catch (ObjectDisposedException)
            {
                LogSystem("TCP: Соединение закрыто (Dispose).");
            }
            finally
            {
                _isTcpListening = false;
                // (Dispose позаботится о закрытии)
            }
        }

        // --- (UDP Слушатель - обновлен) ---
        private async Task ListenUdpAsync()
        {
            try
            {
                _udpSender.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                _udpSender.Client.Bind(new IPEndPoint(IPAddress.Any, MulticastPort));
                _udpSender.JoinMulticastGroup(MulticastGroup, IPAddress.Any);

                LogSystem($"UDP: Присоединились к {MulticastGroup}:{MulticastPort}");

                while (_isUdpListening)
                {
                    try
                    {
                        var result = await _udpSender.ReceiveAsync();
                        var json = Encoding.UTF8.GetString(result.Buffer);

                        // (Заменил Message.FromJson на прямой JsonSerializer)
                        var message = JsonSerializer.Deserialize<Message>(json);

                        if (message != null && message.Name != _userName) // Не показывать свои же UDP-сообщения
                        {
                            LogMessage(message.Name, message.Msg);
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                        break; // Ожидаемый выход
                    }
                    catch (Exception ex)
                    {
                        LogSystem(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem($"UDP Критическая ошибка: {ex.Message}");
            }
        }

        private void SetupUdp()
        {
            _udpSender = new UdpClient();
        }

        // --- ОБНОВЛЕННЫЙ МЕТОД ---
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // --- Остановка UDP ---
                    _isUdpListening = false;
                    _udpSender?.Close();
                    _udpSender?.Dispose();
                    _udpSender = null;

                    // --- НОВОЕ: Остановка TCP ---
                    _isTcpListening = false;
                    _tcpStream?.Close();
                    _tcpStream?.Dispose();
                    _tcpClient?.Close();
                    _tcpClient?.Dispose();
                    _tcpStream = null;
                    _tcpClient = null;
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}