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
using System.Net.Http;
using RestSharp;

namespace InsightTimer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string INSIGHT_SERVER = "http://insight.mogwai-tech.xyz/";

        public MainWindow()
        {
            InitializeComponent();

            this.Closing += MainWindow_Closing;
            this.Loaded += MainWindow_Loaded;
            this.ContentRendered += MainWindow_ContentRendered;
        }

        private void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            TestConnectivity();
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
                RestClient client = new RestClient(INSIGHT_SERVER);
                client.Timeout=5000;
                RestRequest request = new RestRequest("testconnection.html", Method.GET);
                RestResponse response = (RestResponse)client.Execute(request);

                if (response.ResponseStatus == ResponseStatus.Completed && response.Content=="OK")
                {
                    lblStatus.Text = "Please enter your participant ID:";
                    return true;
                }
                else if (response.ResponseStatus == ResponseStatus.Completed && response.Content != "OK")
                {
                    lblStatus.Text = "Connectivity tests failed. Unexpected response: \n" + response.Content;
                    btnStart.IsEnabled = false;
                    return false;
                }
                else
                {
                    lblStatus.Text = "Connectivity tests failed. Error: " + response.ErrorMessage;
                    btnStart.IsEnabled = false;
                    return false;
                }

            }
            
            catch (Exception e)
            {
                lblStatus.Text = "Connectivity tests failed. Error: " + e.Message;
                return false;
            }

        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (txtParticipantID.Text.Trim() == "")
            {
                MessageBox.Show("You need to enter your participant ID!", "ID Needed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                RestClient client = new RestClient(INSIGHT_SERVER);
                client.Timeout = 5000;
                RestRequest request = new RestRequest("resultSubmit.php", Method.POST);
                request.AddParameter("APIKey", "INSIGHT_APIKEY_4501");
                request.AddParameter("partID", txtParticipantID.Text);
                request.AddParameter("timeType", "S");
                RestResponse response = (RestResponse)client.Execute(request);

                if (response.ResponseStatus == ResponseStatus.Completed && response.Content == "SUCCESS")
                {
                    btnStart.IsEnabled = false;
                    btnStop.IsEnabled = true;
                    txtParticipantID.IsEnabled = false;
                    lblStatus.Text = "Experiment Running...";
                }
                else
                {
                    lblStatus.Text = "Server failure. Experiment cannot be started!";
                }
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (txtParticipantID.Text.Trim() == "")
            {
                MessageBox.Show("You need to enter your participant ID!", "ID Needed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                RestClient client = new RestClient(INSIGHT_SERVER);
                client.Timeout = 5000;
                RestRequest request = new RestRequest("resultSubmit.php", Method.POST);
                request.AddParameter("APIKey", "INSIGHT_APIKEY_4501");
                request.AddParameter("partID", txtParticipantID.Text);
                request.AddParameter("timeType", "F");
                RestResponse response = (RestResponse)client.Execute(request);

                if (response.ResponseStatus == ResponseStatus.Completed && response.Content == "SUCCESS")
                {
                    btnStop.IsEnabled = false;
                    lblStatus.Text = "Experiment Complete! Thank you!";
                    this.Closing -= MainWindow_Closing;
                }
                else
                {
                    lblStatus.Text = "Server failure. Experiment cannot be stopped!";
                    MessageBox.Show("Contact with server lost! Experiment cannot be recorded as stopped, please alert Gavin!", "Server Failure!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
    }
}
