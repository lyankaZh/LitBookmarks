using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Domain_Logic.Entities
{
    public class User:IdentityUser
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

        public string AboutMyself { get; set; }

        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }

        public virtual List<Genre> FavoriteGenres { get; set; }
        public virtual  List<Bookmark> Bookmarks { get; set; }

        public virtual List<Comment> Comments { get; set; }

    }
}
