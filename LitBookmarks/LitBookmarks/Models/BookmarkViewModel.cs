using Domain_Logic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LitBookmarks.Models
{
    public class BookmarkViewModel
    {
        public int BookmarkId { get; set; }
        public string Book { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Date { get; set; }
        public string Name { get; set; }
        public List<Genre> Genres { get; set; }
        public User BookmarkOwner { get; set; }
    }
}