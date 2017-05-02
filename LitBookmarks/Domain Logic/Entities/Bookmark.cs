using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain_Logic.Entities
{
    public class Bookmark
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookmarkId { get; set; }
        public string Book { get; set; }
        public string Author { get; set; }
        public int Likes { get; set; }
        public DateTime Date { get; set; }
        public virtual User BookmarkOwner { get; set; }
        public virtual List<Genre> Genres { get; set; }
        public virtual List<Comment> Comments { get; set; }
        

    }
}
