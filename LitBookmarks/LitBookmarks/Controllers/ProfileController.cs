using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain_Logic.Abstract;

namespace LitBookmarks.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProfileController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

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