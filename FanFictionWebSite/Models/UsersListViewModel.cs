using FanFictionWebSite.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FanFictionWebSite.Models
{
    public class UsersListViewModel
    {
        public List<UserFilter> UserFilters { get; set; }
    }

    public class UserFilter
    {
        public User UserToFilter { get; set; }
        public Boolean IsSelected { get; set; }
    }
}
