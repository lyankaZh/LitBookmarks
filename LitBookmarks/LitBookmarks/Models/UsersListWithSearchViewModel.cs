using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LitBookmarks.Models
{
    public class UsersListWithSearchViewModel
    {
        public List<UserViewModel> Users { get; set; }
        public string SearchText {get; set; }
    }
}