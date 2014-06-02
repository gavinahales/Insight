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
using TimelineLibrary;
using System.Xml.Linq;

namespace Insight
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private List<TimelineEvent> timelineEvents;
        private String currentSearch;

        public MainWindow()
        {
            InitializeComponent();

            //Load in event data
            //Error handler in case data file does not exist

            XDocument input = null;

            try
            {
                input = XDocument.Load("insight.xml");
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show("The dataset file could not be found. Please contact the developer.", "Dataset Missing", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }

            timeline.ResetEvents(input);
            timelineEvents = timeline.TimelineEvents;

            //Event Handler Bindings
            timeline.StylusSystemGesture += timeline_StylusSystemGesture;
            chkAttachedDevices.Click += refineTimeline;
            chkEXIF.Click += refineTimeline;
            chkInstalledProgs.Click += refineTimeline;
            chkWebHistory.Click += refineTimeline;

        }

        private void timeline_Initialized(object sender, EventArgs e)
        {

        }

        private void btnTimelineZoomIn_Click(object sender, RoutedEventArgs e)
        {
            timeline.Zoom(true);

        }

        private void btnTimelineZoomOut_Click(object sender, RoutedEventArgs e)
        {
            timeline.Zoom(false);
        }

        private void btnTimelineZoomReset_Click(object sender, RoutedEventArgs e)
        {
            timeline.RefreshEvents();
        }

        private void btnTimelineReload_Click(object sender, RoutedEventArgs e)
        {
            timeline.Reload();
        }


        //BUG: This event handler has a horrible habit of repeating itself. 10 message boxes telling you what the ID is, nice...
        //This is DIRECTLY linked to how many times the Reload method has been called. May have to use ResetEvents.
        //Tried to use ClearEvents first, does not solve problem. Detaching, reloading and reattaching event handler does not work.
        private void timeline_SelectionChanged(object sender, EventArgs e)
        {
            TimelineEvent timeevent;

            if (timeline.SelectedTimelineEvents.Count != 0)
            {

                timeevent = timeline.SelectedTimelineEvents[0];

                if (timeevent.Id != "")
                {
                    MessageBox.Show("This event has an ID defined and at runtime could pull in additional information. ID is: " + timeevent.Id);
                }
            }
        }

        void timeline_StylusSystemGesture(object sender, StylusSystemGestureEventArgs e)
        {
            this.Title = e.SystemGesture.ToString();
            switch (e.SystemGesture)
            {
                case SystemGesture.TwoFingerTap:
                    timeline.Zoom(true);
                    break;

                case SystemGesture.Flick:
                    timeline.Zoom(false);
                    break;
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            currentSearch = txtSearch.Text;

            List<TimelineEvent> searchevents = new List<TimelineEvent>();

            if (txtSearch.Text == "")
            {
                timeline.ResetEvents(timelineEvents);

                refineTimeline();
            }

            else
            {

                foreach (TimelineEvent timeevent in timelineEvents)
                {
                    if (timeevent.Title.ToLower().Contains(txtSearch.Text.ToLower()))
                    {
                        searchevents.Add(timeevent);
                    }
                }

                timeline.ResetEvents(searchevents);

                refineTimeline();
            }
        }

        void refineTimeline(object sender = null, RoutedEventArgs e = null)
        {

            List<TimelineEvent> refineEvents = new List<TimelineEvent>();

            if (currentSearch == "")
            {
                foreach (TimelineEvent timeevent in timelineEvents)
                {
                    if (!isEventExcluded(timeevent))
                    {
                        refineEvents.Add(timeevent);
                    }
                }

                timeline.ResetEvents(refineEvents);
            }

            else
            {

                foreach (TimelineEvent timeevent in timelineEvents)
                {
                    if (timeevent.Title.ToLower().Contains(txtSearch.Text.ToLower()) && !isEventExcluded(timeevent))
                    {
                        refineEvents.Add(timeevent);
                    }
                }

                timeline.ResetEvents(refineEvents);
            }

        }

        private bool isEventExcluded(TimelineEvent timeEvent)
        {
            String eventPrefix = timeEvent.Id.Substring(0, 5);

            if (chkEXIF.IsChecked==false && eventPrefix == "autex")
            {
                return true;
            }
            else if (chkAttachedDevices.IsChecked == false && eventPrefix == "autad")
            {
                return true;
            }
            else if (chkInstalledProgs.IsChecked == false && eventPrefix == "autip")
            {
                return true;
            }
            else if (chkWebHistory.IsChecked == false && eventPrefix == "autwh")
            {
                return true;
            }
            else
            {
                return false;
            }

        }


    }
}
