using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LitBookmarks.Controllers
{
    public class ProfileController : Controller
    {
        // GET: Profile
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowMyBookmarks()
        {
            return View();
        }

        public ActionResult ShowMyFollowers()
        {
            return View();
        }

        public ActionResult ShowFollowing()
        {
            return View();
        }

        public ActionResult ShowAllBookmarks()
        {
            return View();
        }
    }
}