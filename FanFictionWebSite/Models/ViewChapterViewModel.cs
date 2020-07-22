using FanFictionWebSite.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FanFictionWebSite.Models
{
    public class ViewChapterViewModel
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
        public int FanficId { get; set; }
        public int ChapterNumber { get; set; }
        public bool IsLast { get; set; }
        public string CommentContent { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Like> Likes { get; set; }
        public bool IsRated { get; set; }
        public bool IsLiked { get; set; }
        public int Rating { get; set; }
    }
}
