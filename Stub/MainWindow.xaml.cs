using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Openfin.Desktop.Messaging;
using Fin = Openfin.Desktop;

namespace Stub
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string RuntimeVersion = "21.90.59.17";
        const string ChannelName = "ChannelExample";
        private Fin.Runtime _fin;
        private ChannelClient _channelClient;

        public MainWindow()
        {
            InitializeComponent();
            ConnectToRuntime();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ConnectToChannel();
        }

        private void ConnectToRuntime()
        {
            var runtimeOptions = new Fin.RuntimeOptions
            {
                UUID = "stub",
                Version = RuntimeVersion,
                LicenseKey = "dotnettest",
                EnableRemoteDevTools = true,
                RuntimeConnectTimeout = 20000,
//                Port = 9696,
//                RuntimeConnectOptions = Fin.RuntimeConnectOptions.DirectLaunch | Fin.RuntimeConnectOptions.UseExternal,
            };

            _fin = Fin.Runtime.GetRuntimeInstance(runtimeOptions);            
            _fin.Error += (sender, e) =>
            {
                Console.WriteLine(e);
            };

            _fin.Connect(() =>
            {
                Application.Current.Dispatcher.Invoke(new Action(() => { 
                    Connect.IsEnabled = true;
                    Console.WriteLine("Connected to Runtime");
                }));                        
            });
        }

        private void ConnectToChannel()
        {
            ChannelConnectOptions opts = new ChannelConnectOptions(ChannelName)
            {
                Wait = true,
                Payload = "Hello from donet"
            };
            _channelClient = _fin.InterApplicationBus.Channel.CreateClient(opts);
            _channelClient.Opened += ChannelClient_Opened;
            _channelClient.Closed += ChannelClient_Closed;
            _channelClient.ConnectAsync();
        }

        private void ChannelClient_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("channel opened");
//            _channelClient.DispatchAsync("getValue").ContinueWith((data) =>
//            {
//                Console.WriteLine(data);
//            });
        }
        private void ChannelClient_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("channel disconnected");
        }

        private void OnGetValue(object result)
        {
            Console.WriteLine(result);
        }
    }
}
