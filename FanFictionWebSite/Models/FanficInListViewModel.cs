using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FanFictionWebSite.Models
{
    public class FanficInListViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
        public double Rating { get; set; }
        public int Comments { get; set; }
        public string Category { get; set; }
    }
}
