using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FanFictionWebSite.Entities
{
    public class Comment
    {
        public User Author { get; set; }
        public FanFiction FanFiction { get; set; }
        public string Content { get; set; }
        public int Id { get; set; }
    }
}
