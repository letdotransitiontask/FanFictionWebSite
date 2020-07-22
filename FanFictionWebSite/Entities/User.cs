using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FanFictionWebSite.Entities
{
    public class User : IdentityUser
    {
        public List<Mark> Marks { get; set; }
        public List<Like> Likes { get; set; }
        public bool Blocked { get; set; }
    }
}
