using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight
{
    [Serializable]
    public class SerializableTimelineEvent:TimelineLibrary.TimelineEvent
    {
        public SerializableTimelineEvent():base()
        {
        }
    }
}
