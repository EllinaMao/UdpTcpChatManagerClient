using Logic;
using System.Net;

namespace Client
{
    public partial class Form1 : Form
    {
        private readonly SynchronizationContext? _uiContext;
        private ClientController client;

        private const int UDP_PORT = 8001;
        private readonly string MULTICAST_GROUP = "239.0.0.1";
        public Form1()
        {
            InitializeComponent();
            _uiContext = SynchronizationContext.Current;
            textBoxIp.Text = "239.0.0.1";
            
        }
        private void Log(string msg)
        {

        }
        private async void connect_btn_Click(object sender, EventArgs e)
        {
            var username = textBoxUsername.Text;
            await client.ConnectAsync(MULTICAST_GROUP, UDP_PORT, username);

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
                listBox1.Items.Add($"[ERROR] Не удалось отправить: {ex.Message}");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            client?.Dispose();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            client = new ClientController(_uiContext);
            
            client.MessageGetEvent += Client_MessageGetEvent; // Сообщения чата
            client.ClientLogEvent += Client_ClientLogEvent;   // Системные логи
            send_btn.Enabled = false;
        }

        private void Client_ClientLogEvent(string msg)
        {
            listBox1.Items.Add($"[SYSTEM] {msg}");
            // Автопрокрутка вниз
            listBox1.TopIndex = listBox1.Items.Count - 1;
        }
        private void Client_MessageGetEvent(string msg)
        {
            listBox1.Items.Add(msg);
            // Автопрокрутка вниз
            listBox1.TopIndex = listBox1.Items.Count - 1;
        }
    }
}
