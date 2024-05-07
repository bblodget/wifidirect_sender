using System;
using Windows.Networking.Sockets;
using Windows.Networking.Proximity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage.Streams;

namespace wifidirect_sender
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private StreamSocket socket;
        private int messageCount = 1; // To keep track of the number of messages sent

        public MainPage()
        {
            this.InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            PeerFinder.Start(); // Start the PeerFinder
            PeerFinder.DisplayName = "WiFiDirectSender"; // Set a friendly name for the device

            PeerFinder.ConnectionRequested += PeerFinder_ConnectionRequested;
        }

        private async void PeerFinder_ConnectionRequested(object sender, ConnectionRequestedEventArgs args)
        {
            try
            {
                socket = await PeerFinder.ConnectAsync(args.PeerInformation);
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    sendMessageButton.IsEnabled = true; // Enable the button once connection is established
                });
            }
            catch (Exception ex)
            {
                // Log or handle exceptions
                System.Diagnostics.Debug.WriteLine("Failed to connect: " + ex.Message);
            }
        }

        private async void sendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            if (socket != null)
            {
                var writer = new DataWriter(socket.OutputStream);
                string message = $"Hello, the count is {messageCount++}.";
                writer.WriteUInt32(writer.MeasureString(message));
                writer.WriteString(message);
                await writer.StoreAsync();
                writer.DetachStream();
                writer.Dispose();
            }
        }
    }
}
