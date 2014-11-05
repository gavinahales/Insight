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
using System.Windows.Shapes;
using TimelineLibrary;

namespace Insight
{
    /// <summary>
    /// Interaction logic for CustomEventWindow.xaml
    /// </summary>
    public partial class CustomEventWindow : Window
    {
        List<TimelineEvent> newEvents;
        List<TimelineEvent> existingEvents;

        public CustomEventWindow(List<TimelineEvent> customEvents)
        {
            InitializeComponent();

            newEvents = new List<TimelineEvent>();

            //Remove this code when code to load from file is implemented.
            existingEvents = new List<TimelineEvent>();

        }

        public event EventHandler<CustomEventsUpdatedEventArgs> CustomEventsUpdated;

        protected void OnCustomEventsUpdated(CustomEventsUpdatedEventArgs e)
        {
            EventHandler<CustomEventsUpdatedEventArgs> handler = CustomEventsUpdated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void btnAddNewEvent_Click(object sender, RoutedEventArgs e)
        {
            int nextAvailableID = (existingEvents.Count + newEvents.Count) + 1;
            lblEventID.Content = "custm" + nextAvailableID.ToString();

            chkHasEndDate.IsChecked = false;
            txtDescription.Text = "";
            txtURI.Text = "";
            dpStartDate.SelectedDate = null;
            dpEndDate.SelectedDate = null;

        }

        private void btnSaveAndExit_Click(object sender, RoutedEventArgs e)
        {
            CustomEventsUpdatedEventArgs args = new CustomEventsUpdatedEventArgs();

            //Change this to the actual list, rather than a blank one
            args.newEvents = new List<TimelineEvent>();
            OnCustomEventsUpdated(args);
        }

        private void chkHasEndDate_Checked(object sender, RoutedEventArgs e)
        {
            dpEndDate.Visibility = System.Windows.Visibility.Visible;
            lblEndDate.Visibility = System.Windows.Visibility.Visible;
        }

        private void chkHasEndDate_Unchecked(object sender, RoutedEventArgs e)
        {
            dpEndDate.Visibility = System.Windows.Visibility.Collapsed;
            lblEndDate.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void btnSaveEvent_Click(object sender, RoutedEventArgs e)
        {
            bool infoOK = false;

            //Check if all required information has been provided and set the infoOK boolean to reflect this.
            if (dpStartDate.SelectedDate == null || txtDescription.HorizontalContentAlignment.ToString().Trim() == "")
            {
                infoOK = false;
            }
            else if (chkHasEndDate.IsChecked == true && dpEndDate.SelectedDate == null)
            {
                infoOK = false;
            }
            else
            {
                infoOK = true;
            }

            //If all required information is provided, save event. Otherwise, alert user and do nothing.
            if (infoOK)
            {
                TimelineEvent newevent = new TimelineEvent();
                newevent.Id = lblEventID.Content.ToString();
                newevent.EventColor = "Blue";
                newevent.Link = txtURI.Text;
                newevent.StartDate = dpStartDate.SelectedDate.Value;

                if (chkHasEndDate.IsChecked == true)
                {
                    newevent.EndDate = dpEndDate.SelectedDate.Value;
                }

                bool eventExists = false;

                foreach (TimelineEvent item in existingEvents)
                {
                    if (item.Id.ToString() == newevent.Id.ToString())
                    {
                        //Test code, this must be changed
                        MessageBox.Show("This event already exists!");
                        eventExists = true;
                    }
                }

                foreach (TimelineEvent item in newEvents)
                {
                    if (item.Id.ToString() == newevent.Id.ToString())
                    {
                        //Test code, this must be changed
                        MessageBox.Show("This event already exists!");
                        eventExists = true;
                    }
                }


                newEvents.Add(newevent);

                lstCustomEvents.Items.Add(newevent.Id);

                //******************
                //Check if the event already exists in the custom events.
                //If it does, replace it. If it doesn't, add it to newEvents.
                //Check if the user wants to overwrite first!!!
                //******************

            }
            else
            {
                MessageBox.Show("You have not filled in all required information for this event.", "Information Missing", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    public class CustomEventsUpdatedEventArgs : EventArgs
    {
        public List<TimelineEvent> newEvents { get; set; }
    }
}
