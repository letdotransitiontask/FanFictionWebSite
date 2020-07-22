using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FanFictionWebSite.Models
{
    public class ChapterCreatingInputModel
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public string Content { get; set; }
        public int Number { get; set; }
        public int FanficId { get; set; }
        public bool IsUpdating { get; set; }
    }
}
