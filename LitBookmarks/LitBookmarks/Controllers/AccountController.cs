using Domain_Logic.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain_Logic.Entities;

namespace LitBookmarks.Controllers
{
    public class AccountController : Controller
    {
        // GET: Login
        public void Index()
        {
            using (BookmarkDbContext ctx= new BookmarkDbContext() )
            {
                ctx.Genres.Add(new Genre
                {
                    Name = "Adventures"
                });
                ctx.SaveChanges();

            }
        }

        public void Login()
        {           
        }
    }
}