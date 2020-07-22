using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FanFictionWebSite.Entities
{
    public class Like
    {
        public int Id { get; set; }
        public User Author { get; set; }
        public Chapter Chapter { get; set; }
    }
}
