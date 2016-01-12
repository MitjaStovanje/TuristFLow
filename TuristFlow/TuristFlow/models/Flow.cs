using System.Collections.Generic;

namespace TuristFlow.models
{
    public class Flow
    {
        public Flow()
        {
            this.Locations = new HashSet<Location>();
            this.PersonLocations = new HashSet<PersonLocation>();
        }

        public int FlowID { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Location> Locations { get; set; }
        public virtual ICollection<PersonLocation> PersonLocations { get; set; }
    }
}