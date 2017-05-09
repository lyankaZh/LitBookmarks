using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain_Logic.Entities;

namespace LitBookmarks.Models
{
    public class ProfileViewModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string AboutMyself { get; set; }
        public string LastActivityDateTime { get; set; }
        public string Email { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }

        public List<Genre> FavoriteGenres { get; set; }
        public List<AllGenresCheckBox> AllGenres { get; set; }
    }
}