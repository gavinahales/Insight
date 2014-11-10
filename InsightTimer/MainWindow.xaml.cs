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
using MySql.Data.MySqlClient;

namespace InsightTimer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string NTP_SERVER = "0.uk.pool.ntp.org";
        public const string MYSQL_SERVER = "178.62.25.124";

        public MainWindow()
        {
            InitializeComponent();

            //this.Closing += MainWindow_Closing;
            this.Loaded += MainWindow_Loaded;
            this.ContentRendered +=MainWindow_ContentRendered;
        }

        private void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            bool connected = TestConnectivity();

            if (connected)
            {
                lblStatus.Text = "Please enter your Participant ID, and start the experiment when ready.";
            }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
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
                ntpclient.Timeout = TimeSpan.FromSeconds(4);
                NtpPacket response = ntpclient.Query();
                DateTime time = (DateTime)response.ReceiveTimestamp;
                //MessageBox.Show(time.ToLongDateString() + " " + time.ToLongTimeString());
            }
            catch (Exception e)
            {
                lblStatus.Text = "Connection tests FAILED. NTP Error:\n" + e.Message;
                MessageBox.Show("Connection tests have failed. Please alert Gavin.", "Connection Failure", MessageBoxButton.OK, MessageBoxImage.Error);
                //return false;
            }

            try
            {
                MySqlConnection mysqlconn = new MySqlConnection("Server=" + MYSQL_SERVER + ";Database=test;Uid=resulter;PASSWORD=insightexternal4501;");
                mysqlconn.Open();

            }
            catch (Exception e)
            {
                lblStatus.Text = "Connection tests FAILED. MySQL Error:\n" + e.Message;
                MessageBox.Show("Connection tests have failed. Please alert Gavin.", "Connection Failure", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            //Test connection to MySQL database here

            return true;
        }
    }
}
