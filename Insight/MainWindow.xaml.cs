﻿using System;
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

            calculateDateRange();
            dpMinDate.SelectedDate = timeline.MinDateTime;
            dpMaxDate.SelectedDate = timeline.MaxDateTime;

            //Event Handler Bindings
            timeline.StylusSystemGesture += timeline_StylusSystemGesture;
            chkAttachedDevices.Click += refineTimeline;
            chkEXIF.Click += refineTimeline;
            chkInstalledProgs.Click += refineTimeline;
            chkWebHistory.Click += refineTimeline;
            dpMinDate.SelectedDateChanged += dpMinDate_SelectedDateChanged;
            dpMaxDate.SelectedDateChanged += dpMaxDate_SelectedDateChanged;

            

        }

        #region Event Handlers
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


        //!!! Probably not needed. Leads to duplicate event info boxes. Remove??
        //Change to manually reload the XML file. Saves relaunching the application.
        private void btnTimelineReload_Click(object sender, RoutedEventArgs e)
        {
            timeline.Reload();
        }


        //BUG: This event handler has a horrible habit of repeating itself. 10 message boxes telling you what the ID is.
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
                    DetailWindow d = new DetailWindow(timeevent);
                    d.Show();
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
            refineTimeline();
        }

        void dpMaxDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpMaxDate.SelectedDate < dpMinDate.SelectedDate)
            {
                MessageBox.Show("End date cannot be before start date.", "Invalid Date Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpMaxDate.SelectedDate = timeline.MaxDateTime;
            }
            else
            {
                timeline.MaxDateTime = (DateTime)dpMaxDate.SelectedDate;
                timeline.RefreshEvents();
            }
        }

        void dpMinDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpMinDate.SelectedDate > dpMaxDate.SelectedDate)
            {
                MessageBox.Show("Start date cannot be after end date.", "Invalid Date Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpMinDate.SelectedDate = timeline.MinDateTime;
            }
            else
            {
                timeline.MaxDateTime = (DateTime)dpMaxDate.SelectedDate;
                timeline.RefreshEvents();
            }
        }

        private void btnResetDateRange_Click(object sender, RoutedEventArgs e)
        {
            calculateDateRange();
            dpMinDate.SelectedDate = timeline.MinDateTime;
            dpMaxDate.SelectedDate = timeline.MaxDateTime;
            timeline.RefreshEvents();
        }

        private void btnCustomEvents_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        private bool matchesSearch(TimelineEvent timeevent)
        {
            if (timeevent.Title.ToLower().Contains(txtSearch.Text.ToLower()))
            {
                return true;
            }
            else if (timeevent.Description.ToLower().Contains(txtSearch.Text.ToLower()))
            {
                return true;
            }
            else if (timeevent.Link.ToLower().Contains(txtSearch.Text.ToLower()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void refineTimeline(object sender = null, RoutedEventArgs e = null)
        {

            List<TimelineEvent> refineEvents = new List<TimelineEvent>();

            String currentSearch = txtSearch.Text;

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
                    if (matchesSearch(timeevent) && !isEventExcluded(timeevent))
                    {
                        refineEvents.Add(timeevent);
                    }
                }

                timeline.ResetEvents(refineEvents);
            }

        }

        //Edit this method to implement new event type searches
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

        private void calculateDateRange()
        {
            timeline.MinDateTime = timelineEvents[0].StartDate;
            timeline.MaxDateTime = timelineEvents[0].StartDate;

            foreach (TimelineEvent item in timelineEvents)
            {
                if (item.StartDate < timeline.MinDateTime) { timeline.MinDateTime = item.StartDate; }
                if (item.StartDate > timeline.MaxDateTime) {timeline.MaxDateTime = item.StartDate;}
            }

        }


    }
}
