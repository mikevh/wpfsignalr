using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;

namespace Message.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public HubConnection Connection { get; set; }
        public IHubProxy HubProxy { get; set; }
        const string ServerURI = "http://localhost:16845";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            ConnectAsync();
        }

        private async void ConnectAsync()
        {
            Connection = new HubConnection(ServerURI);
            Connection.Closed += Connection_Closed;
            HubProxy = Connection.CreateHubProxy("ChatHub");
            
            //Handle incoming event from server: use Invoke to write to console from SignalR's thread
            HubProxy.On<string, string>("broadcastMessage", (name, message) =>
                this.Dispatcher.Invoke(() =>richTextBox.AppendText($"{name}: {message}\r"))
            );

            try
            {
                await Connection.Start();
            }
            catch (HttpRequestException ex)
            {
                richTextBox.AppendText("Unable to connect to server: Start server before connecting clients:\n" + ex.Message + "\n\n");
                return;
            }

            //Show chat UI; hide login UI
            richTextBox.AppendText("Connected to server at " + ServerURI + "\r");
        }

        void Connection_Closed()
        {
            //Hide chat UI; show login UI
            var dispatcher = Application.Current.Dispatcher;
            dispatcher.Invoke(() => richTextBox.AppendText("Connection closed\n"));
        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            HubProxy.Invoke("Send", nameTextBox.Text, messageTextBox.Text);
        }
    }
}
