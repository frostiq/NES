using System.Windows;
using NeuroServer.Udp;

namespace NeuroServer.v2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly UdpServer _udpServer = (UdpServer) Application.Current.Resources["Server"];

        public MainWindow()
        {
            InitializeComponent();

            new EnvironmentMessageProcessor().AttachToServer(_udpServer);
            _udpServer.OnException += exception => Log.AppendText(exception.ToString());
            _udpServer.Init(int.Parse(PortTextBox.Text));
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            _udpServer.Start();
            Log.AppendText("\nServer Started");
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _udpServer.Stop();
            Log.AppendText("\nServer Stopped");
        }
    }
}
