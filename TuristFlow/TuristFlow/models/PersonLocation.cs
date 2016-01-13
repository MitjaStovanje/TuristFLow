using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuristFlow.models
{
   public class PersonLocation
    {

            public PersonLocation()
            {
                this.People = new HashSet<Person>();
            }
            public string id { get; set; }
            public int  PersonLocationID { get; set; }
            public string LAT { get; set; }
            public string LOT { get; set; }
            public int FlowFlowID { get; set; }

            public virtual ICollection<Person> People { get; set; }
            public virtual Flow Flow { get; set; }
        }
    }
