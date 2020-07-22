using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FanFictionWebSite.Entities
{
    public class Mark
    {
        public int Id { get; set; }
        public User User { get; set; }
        public FanFiction Fanfic { get; set; }
        public int Value { get; set; }
    }
}
