using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FanFictionWebSite.Entities
{
    public class FanFiction
    {
        public List<Mark> Marks { get; set; }
        public User Author { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<FanFictionBadge> FanFictionBadges { get; set; }

        public List<Chapter> Chapters { get; set; }
        public Category Category { get; set; }

        public int Id { get; set; }

        public List<Comment> Comments { get; set; }
        
    }
}
