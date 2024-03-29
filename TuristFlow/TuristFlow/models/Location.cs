﻿ using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuristFlow.models
{
    public class Location {

        public string Id { get; set; }
        public string LAT { get; set; }
        public string LOT { get; set; }
        public string City { get; set; }
        public string Types { get; set; }
        public int Radius { get; set; }
        public string Name { get; set; }
        public int FlowFlowID { get; set; }

        public virtual Flow Flow { get; set; }

    }
}
