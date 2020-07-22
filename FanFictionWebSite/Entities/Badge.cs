using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FanFictionWebSite.Entities
{
    public class Badge
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public List<FanFictionBadge> FanFictionBadges { get; set; }
    }
}
