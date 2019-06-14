using System;
using System.Collections.Generic;
using System.Text;

namespace ClassifiedAds.Domain.Entities
{
    public class Session : Entity<Guid>
    {
        public string Name { get; set; }
        public string Presenter { get; set; }
        public int  Duration { get; set; }
        public string Level { get; set; }
        public string Abstract { get; set; }
        public Guid EventId { get; set; }
        public IList<Voter> Voters { get; set; }
    }
}
