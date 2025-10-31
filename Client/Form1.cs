using Logic;
using System.Net;

namespace Client
{
    public partial class Form1 : Form
    {
        private readonly SynchronizationContext? _uiContext;
        private ClientController client;

        private const int UDP_PORT = 8001;
        private const int TCP_PORT = 8002;
        private readonly string MULTICAST_GROUP = "239.0.0.1";
        private Dictionary<string, ListBox> _privateChatBoxes = new Dictionary<string, ListBox>();

        public int OnPrivateMessageReceived { get; private set; }

        public Form1()
        {
            InitializeComponent();
            _uiContext = SynchronizationContext.Current;
            textBoxIp.Text = MULTICAST_GROUP;


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            client = new ClientController(_uiContext);


            client.MessageGetEvent += OnPublicMessageReceived;
            client.PMGetEvent += PrivateMessageReceived;

            // 2. Добавляем недостающие подписки
            client.ClientLogEvent += Client_ClientLogEvent;
            client.UserListUpdatedEvent += OnUserListUpdated;
            client.HistoryReceivedEvent += OnHistoryReceived;
            send_btn.Enabled = false;
        }

        private void PrivateMessageReceived(string name, string msg)
        {
            // Находим (или создаем) вкладку и ListBox для этого ПМ
            ListBox pmChatBox = FindOrCreatePmTab(fromUser);

            // Добавляем сообщение
            pmChatBox.Items.Add($"[{fromUser}]: {msg}");
            ScrollListBoxToEnd(pmChatBox);
        }



        private async void connect_btn_Click(object sender, EventArgs e)
        {
            var username = textBoxUsername.Text;

            await client.ConnectAsync(textBoxIp.Text, TCP_PORT, username);
            connect_btn.Enabled = false;
            textBoxUsername.ReadOnly = true;
            send_btn.Enabled = true;

            textBoxMessage.Focus();
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

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void OnPublicMessageReceived(string name, string msg) 
        {
            mainChatListBox.Items.Add($"{name}: {msg}");
            mainChatListBox.TopIndex = mainChatListBox.Items.Count - 1;
        }

        private void Client_ClientLogEvent(string msg) //
        {
            mainChatListBox.Items.Add($"[SYSTEM] {msg}");
            mainChatListBox.TopIndex = mainChatListBox.Items.Count - 1;
        }

        private void OnHistoryReceived(List<Logic.MessagesFiles.Message> history)
        {
            mainChatListBox.Items.Clear(); 
            foreach (var msg in history)
            {
                mainChatListBox.Items.Add($"{msg.Name}: {msg.Msg}");
            }
            mainChatListBox.TopIndex = mainChatListBox.Items.Count - 1;
        }

        private void OnUserListUpdated(List<string> users)
        {
             userListBox.Items.Clear();
            foreach (var user in users)
            {
                userListBox.Items.Add(user);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            client?.Dispose();
        }
    }
}
