using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain_Logic.Entities
{
    public class Bookmark
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookmarkId { get; set; }
        public string Book { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public string Name { get; set; }
        public virtual User BookmarkOwner { get; set; }
        public virtual List<Genre> Genres { get; set; }
        public virtual List<Comment> Comments { get; set; }
        public virtual List<Like> Likes { get; set; }
    }
}
