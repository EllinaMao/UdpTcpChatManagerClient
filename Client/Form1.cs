using Logic;
using System.Net;
using System.Xml.Linq;
using Logic.MessagesFiles; 

namespace Client
{
    public partial class Form1 : Form
    {
        private readonly SynchronizationContext? _uiContext;
        private ClientController client;

        //private const int UDP_PORT = 8001;
        private const int TCP_PORT = 8002;
        private readonly string MULTICAST_GROUP = "239.0.0.1";
        private Dictionary<string, ListBox> _privateChatBoxes = new Dictionary<string, ListBox>();

        public Form1()
        {
            InitializeComponent();
            _uiContext = SynchronizationContext.Current;
            //textBoxIp.Text = MULTICAST_GROUP;
            //textBoxIp.Text = "127.0.0.1";


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            client = new ClientController(_uiContext);


            client.MessageGetEvent += OnPublicMessageReceived;
            client.PMGetEvent += OnPrivateMessageReceived;

            // 2. Добавляем недостающие подписки
            client.ClientLogEvent += Client_ClientLogEvent;
            client.UserListUpdatedEvent += OnUserListUpdated;
            client.HistoryReceivedEvent += OnHistoryReceived;
            client.HistoryPmReceivedEvent += OnPrivateHistoryReceived;
            send_btn.Enabled = false;
            button1.Enabled = false;
        }

        private void Client_ClientLogEvent(string msg)
        {
            systemLogListBox.Items.Add($"[{DateTime.Now.ToShortTimeString()}] {msg}");
            ScrollListBoxToEnd(systemLogListBox);
        }

        private void OnPrivateMessageReceived(string fromUser, string msg)
        {
            ListBox pmChatBox = FindOrCreatePmTab(fromUser);

            pmChatBox.Items.Add($"[{fromUser}]: {msg}");
            ScrollListBoxToEnd(pmChatBox);
        }

        private async void connect_btn_Click(object sender, EventArgs e)
        {

            try
            {
                string userName = textBoxUsername.Text;


                if (string.IsNullOrEmpty(userName))
                {
                    LogToSystemChat("Ошибка: Введите имя.");
                    return;
                }

                await client.ConnectAsync(textBoxIp.Text, TCP_PORT, userName);
                // Если успешно, меняем состояние кнопок
                connect_btn.Enabled = false;
                send_btn.Enabled = true;
                button1.Enabled = true;
            }
            catch (Exception ex)
            {
                LogToSystemChat($"Критическая ошибка подключения: {ex.Message}");
            }
        }

        private async void send_btn_Click(object sender, EventArgs e)
        {
            var message = textBoxMessage.Text;
            try
            {
                await client.SendUpdMessage(message);
                textBoxMessage.Clear();
            }
            catch (Exception ex)
            {
                mainChatListBox.Items.Add($"[ERROR] Не удалось отправить: {ex.Message}");
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string message = textBoxMessage.Text;

            // Проверяем, что юзер ВЫБРАН
            if (userListBox.SelectedItem == null)
            {
                LogToSystemChat("Ошибка: Выберите пользователя из списка для ПМ.");
                return;
            }

            string toUser = userListBox.SelectedItem.ToString();

            if (string.IsNullOrEmpty(message) || client == null) return;

            // Отправляем в TCP
            await client.SendPrivateMessageAsync(toUser, message);

            // Находим (или создаем) вкладку для этого ПМ
            ListBox pmChatBox = FindOrCreatePmTab(toUser);

            // Отображаем у себя
            pmChatBox.Items.Add($"[Я -> {toUser}]: {message}");
            ScrollListBoxToEnd(pmChatBox);

            textBoxMessage.Clear();
        }
        private void LogToSystemChat(string msg)
        {
            // (systemLogListBox - ListBox на вкладке "Логи")
            systemLogListBox.Items.Add($"[{DateTime.Now.ToShortTimeString()}] {msg}");
            ScrollListBoxToEnd(systemLogListBox);
        }

        private void OnHistoryReceived(List<Logic.MessagesFiles.Message> history)
        {
            mainChatListBox.Items.Clear();
            foreach (var msg in history)
            {
                mainChatListBox.Items.Add($"{msg.Name}: {msg.Msg}");
            }
            ScrollListBoxToEnd(mainChatListBox);
        }
        private void OnPrivateHistoryReceived(string withUser, List<Logic.MessagesFiles.Message> history)
        {
            if (_privateChatBoxes.TryGetValue(withUser, out ListBox chatBox))
            {
                chatBox.Items.Clear();
                foreach (var msg in history)
                {
                    string prefix = (msg.Name == withUser) ? $"[{withUser}]" : "[Я]";
                    chatBox.Items.Add($"{prefix}: {msg.Msg}");
                }
                ScrollListBoxToEnd(chatBox);
                LogToSystemChat($"История с {withUser} загружена.");
            }
        }
        private void OnPublicMessageReceived(string name, string msg)
        {
            mainChatListBox.Items.Add($"{name}: {msg}");
            ScrollListBoxToEnd(mainChatListBox);
        }

        private void OnUserListUpdated(List<string> users)
        {
            // (userListBox - ListBox в правой панели)
            userListBox.Items.Clear();
            foreach (var user in users)
            {
                userListBox.Items.Add(user);
            }
        }

        private ListBox FindOrCreatePmTab(string userName)
        {

            if (_privateChatBoxes.ContainsKey(userName))
            {
                return _privateChatBoxes[userName];
            }
            LogToSystemChat($"Создан приватный чат с '{userName}'");

            TabPage newTab = new TabPage(userName);

            ListBox pmChatBox = new ListBox
            {
                Dock = DockStyle.Fill
            };

            newTab.Controls.Add(pmChatBox);

            tabControl1.TabPages.Add(newTab);

            _privateChatBoxes.Add(userName, pmChatBox);

            return pmChatBox;
        }


        private void ScrollListBoxToEnd(ListBox lb)
        {
            if (lb.Items.Count > 0)
            {
                lb.TopIndex = lb.Items.Count - 1;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            client?.Dispose();
        }

        private void userListBox_DoubleClick(object sender, EventArgs e)
        {
            if (userListBox.SelectedItem == null || client == null) return;

            string selectedUser = userListBox.SelectedItem.ToString();
            string myName = textBoxUsername.Text;

            if (selectedUser == myName) return; // Не чатиться с собой

            ListBox chatBox = FindOrCreatePmTab(selectedUser);

            tabControl1.SelectedTab = (TabPage)chatBox.Parent;

            if (chatBox.Items.Count == 0)
            {
                LogToSystemChat($"Запрос истории сообщений с {selectedUser}...");
                client.RequestPrivateHistoryAsync(selectedUser);
            }
        }



    }
}