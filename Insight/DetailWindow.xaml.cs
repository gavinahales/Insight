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
using System.Drawing;
using TimelineLibrary;
using System.IO;

namespace Insight
{
    /// <summary>
    /// Interaction logic for DetailWindow.xaml
    /// </summary>
    public partial class DetailWindow : Window
    {
        public DetailWindow(TimelineEvent timeEvent)
        {
            InitializeComponent();

            String eventType = timeEvent.Id;

            switch (eventType.ToLower().Substring(0, 5))
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
                case "auttm":
                    eventType = "File Type Mismatch";
                    break;
                default:
                    eventType = "Unknown";
                    break;
            }

            lblAccessed.Content = (String)(timeEvent.StartDate.ToShortDateString() + " " + timeEvent.StartDate.ToShortTimeString());
            lblLink.Content = timeEvent.Link;
            lblEventType.Content = eventType;

            //Load preview image
            if (timeEvent.Id.ToLower().Substring(0,5) == "autex")
            {
                String extension = timeEvent.Link.Substring(timeEvent.Link.Length-4);
                switch (extension)
                {
                    case ".jpg":
                    case "jpeg":
                    case ".png":
                    case ".bmp":
                    case ".gif":
                        imgPreview.Source = getImage(timeEvent.Link);
                        break;

                    default:
                        break;
                }
            }

            //Allow loading local images from web history events.
            else if (timeEvent.Id.ToLower().Substring(0,5) == "autwh")
            {
                String extension = timeEvent.Link.Substring(timeEvent.Link.Length-4);

                switch (extension)
                {
                    case ".jpg":
                    case "jpeg":
                    case ".png":
                    case ".bmp":
                    case ".gif":
                        //Chop the protocol off the front.
                        String filepath = timeEvent.Link.Substring(4);
                        imgPreview.Source = getImage(filepath);
                        break;

                    default:
                        break;
                }
            }

        }


        private void OpenContent_Click(object sender, RoutedEventArgs e)
        {

        }

        private BitmapImage getImage(string filepath)
        {
            //Image previews aren't critical, so fail silently if the image is not found, URI malformed etc
            try
            {
            filepath = Uri.UnescapeDataString(Directory.GetCurrentDirectory()+"/datasets"+filepath);

            Uri uri = new Uri(filepath);

            
                BitmapImage image = new BitmapImage(uri);
                return image;
            }
            catch (Exception)
            {
                //If file not found, fail silently
                return null;
            }
            

            
        }
        

    }
}
