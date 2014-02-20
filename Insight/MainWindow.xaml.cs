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
        public MainWindow()
        {
            InitializeComponent();

            //Load in event data
            XDocument input = XDocument.Load("insight.xml");
            timeline.ResetEvents(input);
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

        

        


    }
}
