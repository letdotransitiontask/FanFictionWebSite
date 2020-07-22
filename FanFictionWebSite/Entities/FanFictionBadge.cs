using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FanFictionWebSite.Entities
{
    public class FanFictionBadge
    {
        public int Id { get; set; }
        public int BadgeId { get; set; }
        public Badge Badge { get; set; }
        public int FanFictionId { get; set; }
        public FanFiction FanFiction { get; set; }
    }
}
