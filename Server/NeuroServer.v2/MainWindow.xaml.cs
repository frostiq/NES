using System.Windows;
using NeuroServer.Udp;
using NLog;

namespace NeuroServer.v2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly UdpServer _udpServer = (UdpServer) Application.Current.Resources["Server"];
        private readonly ILogger _log = LogManager.GetLogger("Window");

        public MainWindow()
        {
            InitializeComponent();
            TextBoxAppender.TextBox = Log;

            new EnvironmentMessageProcessor().AttachToServer(_udpServer);
            _udpServer.Init(int.Parse(PortTextBox.Text));
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            _udpServer.Start();
            _log.Info("\nServer Started");
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _udpServer.Stop();
            _log.Info("\nServer Stopped");
        }
    }
}
