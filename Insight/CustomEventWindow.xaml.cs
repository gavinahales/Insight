using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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

            lstCustomEvents.SelectionChanged += lstCustomEvents_SelectionChanged;

        }

        void lstCustomEvents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Get the ID of the item currently selected in the event list
            String selectedID = lstCustomEvents.SelectedItem.ToString();

            //Check if the item is an existing item, and then fill in it's details.
            foreach (TimelineEvent item in existingEvents)
            {
                if (item.Id.ToString() == selectedID)
                {
                    lblEventID.Content = item.Id;
                    txtDescription.Text = item.Description;
                    txtURI.Text = item.Link;
                    dpStartDate.SelectedDate = item.StartDate;

                    if (item.EndDate != null)
                    {
                        chkHasEndDate.IsChecked = true;
                        dpEndDate.SelectedDate = item.EndDate;
                    }
                    else
                    {
                        chkHasEndDate.IsChecked = false;
                        dpEndDate.SelectedDate = null;
                    }

                    return;
                }
            }

            //Check if the item is a newly added item, and then fill the details.
            foreach (TimelineEvent item in newEvents)
            {
                if (item.Id.ToString() == selectedID)
                {
                    lblEventID.Content = item.Id;
                    txtDescription.Text = item.Description;
                    txtURI.Text = item.Link;
                    dpStartDate.SelectedDate = item.StartDate;

                    //Check if the event has a Duration, and if so, load the end date.
                    if (item.IsDuration)
                    {
                        chkHasEndDate.IsChecked = true;
                        dpEndDate.SelectedDate = item.EndDate;
                    }
                    else
                    {
                        chkHasEndDate.IsChecked = false;
                        dpEndDate.SelectedDate = null;
                    }

                    return;
                }
            }
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
            txtDescription.IsEnabled = true;
            txtURI.IsEnabled = true;
            dpStartDate.IsEnabled = true;
            chkHasEndDate.IsEnabled = true;

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
            List<TimelineEvent> combinedEvents = new List<TimelineEvent>();

            combinedEvents.AddRange(existingEvents);
            combinedEvents.AddRange(newEvents);

            List<SerializableTimelineEvent> outputList = combinedEvents.ConvertAll<SerializableTimelineEvent>(new Converter<TimelineEvent,SerializableTimelineEvent>(convertToSerializable));

            try
            {
                using (Stream stream = File.Open("customEvents.bin", FileMode.Create))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, outputList);
                }
            }
            catch (IOException IOerr)
            {
                //Don't handle this error for debug reasons, just throw it again.
                throw IOerr;
            }

            CustomEventsUpdatedEventArgs args = new CustomEventsUpdatedEventArgs();
            //Change this to the actual list, rather than a blank one
            args.newEvents = combinedEvents;
            OnCustomEventsUpdated(args);

            this.Close();
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
            if (dpStartDate.SelectedDate == null || txtDescription.Text.ToString().Trim() == "")
            {
                infoOK = false;
            }
            else if (chkHasEndDate.IsChecked == true && dpEndDate.SelectedDate == null)
            {
                infoOK = false;
            }
            else if(dpEndDate.SelectedDate!=null && (dpEndDate.SelectedDate < dpStartDate.SelectedDate))
            {
                MessageBox.Show("You selected an end date that is before the start date!", "Date Problem", MessageBoxButton.OK,MessageBoxImage.Warning);
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
                newevent.Description = txtDescription.Text;
                newevent.EventColor = "Blue";
                newevent.Link = txtURI.Text;
                newevent.StartDate = dpStartDate.SelectedDate.Value;

                if (chkHasEndDate.IsChecked == true)
                {
                    newevent.IsDuration = true;
                    newevent.EndDate = dpEndDate.SelectedDate.Value;
                }
                else
                {
                    newevent.IsDuration = false;
                }

                bool eventExists = false;

                foreach (TimelineEvent item in existingEvents)
                {
                    if (item.Id.ToString() == newevent.Id.ToString())
                    {
                        eventExists = true;
                        MessageBoxResult overwrite = MessageBox.Show("This event already exists, do you want to overwrite it?", "Overwrite Event?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (overwrite == MessageBoxResult.Yes)
                        {
                            existingEvents.Remove(item);
                            newEvents.Add(newevent);
                            break;
                        }
                    }
                }

                //IF statement to stop this next foreach loop running if the event has already been found
                if (!eventExists)
                {
                    foreach (TimelineEvent item in newEvents)
                    {
                        if (item.Id.ToString() == newevent.Id.ToString())
                        {
                            eventExists = true;
                            MessageBoxResult overwrite = MessageBox.Show("This event already exists, do you want to overwrite it?", "Overwrite Event?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (overwrite == MessageBoxResult.Yes)
                            {
                                newEvents.Remove(item);
                                newEvents.Add(newevent);
                                break;
                            }
                            else
                            {
                                MessageBox.Show("Event was not saved.", "Overwrite Cancelled", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                    }
                }

                //If the event doesnt already exist, add it.
                if (!eventExists)
                {
                    newEvents.Add(newevent);
                    lstCustomEvents.Items.Add(newevent.Id);
                }


            }
            else
            {
                MessageBox.Show("You have not filled in all required information for this event.", "Information Missing", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public SerializableTimelineEvent convertToSerializable(TimelineEvent timeEvent)
        {
            SerializableTimelineEvent newEvent = new SerializableTimelineEvent();

            newEvent.Id = timeEvent.Id;
            newEvent.EventColor = timeEvent.EventColor;
            newEvent.Description = timeEvent.Description;
            newEvent.StartDate = timeEvent.StartDate;
            newEvent.EndDate = timeEvent.EndDate;
            newEvent.IsDuration = timeEvent.IsDuration;
            newEvent.Link = timeEvent.Link;

            return newEvent;
        }
    }


    public class CustomEventsUpdatedEventArgs : EventArgs
    {
        public List<TimelineEvent> newEvents { get; set; }
    }
}
