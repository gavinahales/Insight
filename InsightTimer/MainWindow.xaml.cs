using System;
using System.Collections.Generic;
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
using GuerrillaNtp;

namespace InsightTimer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string NTP_SERVER = "0.uk.pool.ntp.org";

        public MainWindow()
        {
            InitializeComponent();

            //this.Closing += MainWindow_Closing;
            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            bool connected = TestConnectivity();

            if (connected)
            {
                lblStatus.Text = "Please enter your Participant ID, and start the experiment when ready.";
            }
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBox.Show("This must not be closed while the experiment is taking place.", "Cannot Close", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Cancel = true;
        }

        bool TestConnectivity()
        {
            try
            {
                GuerrillaNtp.NtpClient ntpclient = new NtpClient(NTP_SERVER);
                ntpclient.Timeout = TimeSpan.FromSeconds(5);
                NtpPacket response = ntpclient.Query();
                DateTime time = (DateTime)response.ReceiveTimestamp;
                //MessageBox.Show(time.ToLongDateString() + " " + time.ToLongTimeString());
            }
            catch (Exception e)
            {
                lblStatus.Text = "Connection tests FAILED. NTP Error:\n" + e.Message;
                MessageBox.Show("Connection tests have failed. Please alert Gavin.", "Connection Failure", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            //Test connection to MySQL database here

            return true;
        }
    }
}
