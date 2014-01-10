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
        }

        private void timeline_Initialized(object sender, EventArgs e)
        {

        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            if (hourTimeBand.TimelineWindowSize == 20)
            {
                hourTimeBand.TimelineWindowSize = 5;
                timeline.RefreshEvents();
            }
            else
            {
                hourTimeBand.TimelineWindowSize = 20;
                timeline.RefreshEvents();
            }

        }


    }
}
