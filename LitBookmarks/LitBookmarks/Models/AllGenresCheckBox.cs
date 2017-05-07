using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain_Logic.Entities;

namespace LitBookmarks.Models
{
    public class AllGenresCheckBox
    {
        public Genre Genre { get; set; }
        public bool Selected { get; set; }
    }
}