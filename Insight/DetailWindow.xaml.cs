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

namespace Insight
{
    /// <summary>
    /// Interaction logic for DetailWindow.xaml
    /// </summary>
    public partial class DetailWindow : Window
    {
        public DetailWindow()
        {
            InitializeComponent();
        }

        //Bit of a stupid way to parameterise this constructor. Why not just pass over entire event object and then strip out the rquired info in this method? Would ease any extensions.
        public DetailWindow(String id, String link, String accessed)
        {
            InitializeComponent();

            String eventType = null;

            switch (id.ToLower().Substring(0,5))
            {
                case "autwh":
                    eventType = "Web History (Autopsy)";
                    break;
                case "autad":
                    eventType = "Attached Device (Autopsy)";
                    break;
                case "autip":
                    eventType = "Installed Program (Autopsy)";
                    break;
                case "autex":
                    eventType = "EXIF Metadata (Autopsy)";
                    break;
                default:
                    eventType = "Unknown";
                    break;
                    

            }

            lblAccessed.Content = accessed;
            lblLink.Content = link;
            lblEventType.Content = eventType;
        }
    }
}
