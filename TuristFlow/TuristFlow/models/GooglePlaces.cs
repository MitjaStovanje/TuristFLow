using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuristFlow.models
{
    class GooglePlaces
    {
        public string status { get; set; }
        public results[] results { get; set; }
    }

    public class results
    {
        public geometry geometry { get; set; }
        public string name { get; set; }
        public string icon { get; set; }
        public string place_id { get; set; }
        public string[] types { get; set; }
    }

    public class geometry
    {
        public location location { get; set; }
    }

    public class location
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }
}
