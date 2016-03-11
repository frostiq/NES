using System.Windows;
using NeuroServer.Udp;

namespace NeuroServer.v2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Server _server = (Server) Application.Current.Resources["Server"];

        public MainWindow()
        {
            InitializeComponent();

            _server.OnProcess += obj => obj;
            _server.Init(int.Parse(PortTextBox.Text));
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            _server.Start();
            Log.AppendText("\nServer Started");
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _server.Stop();
            Log.AppendText("\nServer Stopped");
        }
    }
}
