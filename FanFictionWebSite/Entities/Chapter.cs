using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FanFictionWebSite.Entities
{
    public class Chapter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }

        public int Number { get; set; }
        public FanFiction FanFiction { get; set; }

        public List<Like> Likes { get; set; }
    }
}
