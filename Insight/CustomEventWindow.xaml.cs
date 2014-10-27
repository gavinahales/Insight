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
    }

    public class CustomEventsUpdatedEventArgs : EventArgs
    {
        public List<TimelineEvent> newEvents { get; set; }
    }
}
