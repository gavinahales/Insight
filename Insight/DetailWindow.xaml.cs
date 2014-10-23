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
        String eventPrefix;
        TimelineEvent currentEvent;

        public DetailWindow(TimelineEvent timeEvent)
        {
            InitializeComponent();

            currentEvent = timeEvent;

            String eventType;
            eventPrefix = timeEvent.Id.ToLower().Substring(0, 5);

            switch (eventPrefix)
            {
                case "autwh":
                    eventType = "Web History (Autopsy)";
                    break;
                case "autad":
                    eventType = "Attached Device (Autopsy)";
                    btnOpenContent.Visibility = System.Windows.Visibility.Hidden;
                    break;
                case "autip":
                    eventType = "Installed Program (Autopsy)";
                    btnOpenContent.Visibility = System.Windows.Visibility.Hidden;
                    break;
                case "autex":
                    eventType = "EXIF Metadata (Autopsy)";
                    break;
                case "auttm":
                    eventType = "File Type Mismatch";
                    btnOpenContent.Visibility = System.Windows.Visibility.Hidden;
                    break;
                default:
                    eventType = "Unknown";
                    btnOpenContent.Visibility = System.Windows.Visibility.Hidden;
                    break;
            }

            lblModified.Content = (String)(timeEvent.StartDate.ToShortDateString() + " " + timeEvent.StartDate.ToShortTimeString());
            lblAccessed.Content = "Not Available";
            lblLink.Content = timeEvent.Link;
            lblEventType.Content = eventType;

            //Load preview image
            if (eventPrefix == "autex")
            {
                String extension = timeEvent.Link.Substring(timeEvent.Link.Length - 4);
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
            else if (eventPrefix == "autwh")
            {
                String extension = timeEvent.Link.Substring(timeEvent.Link.Length - 4);

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

            switch (eventPrefix)
            {
                case "autwh":
                    String url = currentEvent.Link;
                    try
                    {
                        System.Diagnostics.Process.Start(url);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Sorry, this web page cannot be loaded. The URL may be malformatted.");
                    }
                    break;

                case "autex":

                    //Check that what we are opening is definitely an image
                    String extension = currentEvent.Link.Substring(currentEvent.Link.Length - 4);
                    switch (extension)
                    {
                        case ".jpg":
                        case "jpeg":
                        case ".png":
                        case ".bmp":
                        case ".gif":
                            String filepath = currentEvent.Link;
                            filepath = Uri.UnescapeDataString(Directory.GetCurrentDirectory() + "/datasets" + filepath);
                            try
                            {
                                System.Diagnostics.Process.Start(filepath);
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Sorry, this image cannot be opened. The file might not exist.");
                            }
                            break;

                        default:
                            break;
                    }

                    break;

                default:
                    break;
            }


        }

        private BitmapImage getImage(string filepath)
        {
            //Image previews aren't critical, so fail silently if the image is not found, URI malformed etc
            try
            {
                filepath = Uri.UnescapeDataString(Directory.GetCurrentDirectory() + "/datasets" + filepath);

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
