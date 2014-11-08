using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using TimelineLibrary;

namespace Insight
{
    static class SerializableEventHelper
    {
        public static SerializableTimelineEvent convertToSerializable(TimelineEvent timeEvent)
        {
            SerializableTimelineEvent newEvent = new SerializableTimelineEvent();

            newEvent.Id = timeEvent.Id;
            newEvent.EventColor = timeEvent.EventColor;
            newEvent.Description = timeEvent.Description;
            newEvent.StartDate = timeEvent.StartDate;
            newEvent.EndDate = timeEvent.EndDate;
            newEvent.IsDuration = timeEvent.IsDuration;
            newEvent.Link = timeEvent.Link;
            newEvent.Title = timeEvent.Title;

            return newEvent;
        }

        public static TimelineEvent convertFromSerializable(SerializableTimelineEvent timeEvent)
        {
            TimelineEvent newEvent = new TimelineEvent();

            newEvent.Id = timeEvent.Id;
            newEvent.EventColor = timeEvent.EventColor;
            newEvent.Description = timeEvent.Description;
            newEvent.StartDate = timeEvent.StartDate;
            newEvent.EndDate = timeEvent.EndDate;
            newEvent.IsDuration = timeEvent.IsDuration;
            newEvent.Link = timeEvent.Link;
            newEvent.Title = timeEvent.Title;

            return newEvent;

        }

        public static void saveCustomEventsToXML(List<TimelineEvent> timeEvents)
        {
            List<SerializableTimelineEvent> outputList = timeEvents.ConvertAll<SerializableTimelineEvent>(new Converter<TimelineEvent, SerializableTimelineEvent>(convertToSerializable));

            try
            {
                using (Stream stream = File.Open("customEvents.xml", FileMode.Create))
                {
                    XmlSerializer xmls = new XmlSerializer(typeof(List<SerializableTimelineEvent>));
                    xmls.Serialize(stream, outputList);
                }
            }
            catch (IOException IOerr)
            {
                //Don't handle this error for debug reasons, just throw it again.
                throw IOerr;
            }
        }

        public static List<TimelineEvent> loadCustomEventsFromXML()
        {

            List<SerializableTimelineEvent> timeEvents = null;
            List<TimelineEvent> outputEvents = null;

            try
            {
                using (Stream stream = File.Open("customEvents.xml", FileMode.Open))
                {
                    XmlSerializer xmls = new XmlSerializer(typeof(List<SerializableTimelineEvent>));
                    XmlReader xmlr = XmlReader.Create(stream);
                    timeEvents = (List<SerializableTimelineEvent>)xmls.Deserialize(xmlr);
                }

                outputEvents = timeEvents.ConvertAll<TimelineEvent>(new Converter<SerializableTimelineEvent, TimelineEvent>(convertFromSerializable));
            }
            catch (IOException IOerr)
            {
                //Don't handle this error for debug reasons, just throw it again.
                throw IOerr;
            }



            return outputEvents;
        }
    }

    [Serializable]
    public class SerializableTimelineEvent
    {
        public string Id{ get; set; }
        public string EventColor { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsDuration { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }
    }

        
}
