using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartQADoc
{
    public class ChartQA
    {
        public long ChartQASer;

        public string user;

        public DateTime dateTime;

        public string comment;

        //This is a list of all the RadiationHstrySer associated with a ChartQA entry.
        //A Radiation History is an entry for each treatment of a beam.
        public List<long> radiationHstrySerList;

        //this is a list of unique radiationSer that are associated with this ChartQA object's RadiationHstry objects
        //so, in other words, the RadiationSer for the beams in the plan.
        public List<long> radiationSerList;

        public ChartQA()
        {
            radiationHstrySerList = new List<long>();
            radiationSerList = new List<long>();
        }
    }
}
