using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Web.Profile;
using System.Web.Security;
using Domain_Logic.Concrete;
using Domain_Logic.Entities;
using LitBookmarks.Models;
using LitBookmarks.Profile;
using Microsoft.AspNet.Identity;

using Domain_Logic.Abstract;
using Microsoft.AspNet.Identity.Owin;


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
        public ActionResult MyProfile()
           {
          var currentUser = _unitOfWork.UserRepository.GetById(User.Identity.GetUserId());
          ProfileViewModel profile = new ProfileViewModel();
               profile.Id = currentUser.Id;
            profile.Age = currentUser.Age;
            profile.FirstName = currentUser.FirstName;
            profile.LastName = currentUser.LastName;
            profile.AboutMyself = currentUser.AboutMyself;
            profile.Email = currentUser.Email;
            profile.FavoriteGenres = currentUser.FavoriteGenres;
            profile.LastActivityDateTime = currentUser.LastActivityDateTime;
               profile.ImageData = currentUser.ImageData;
            var checkBoxes = new List<AllGenresCheckBox>();
            
            for (int i = 0; i < _unitOfWork.GenreRepository.Get().ToList().Count; i++)
            {
                checkBoxes.Add(new AllGenresCheckBox()
                { 
                    Genre = _unitOfWork.GenreRepository.Get().ToList()[i]
                });
            }
           
            profile.AllGenres = checkBoxes;
         
            return View("MyProfile",profile);
        }

        [HttpPost]
        public ActionResult AddFavoriteGenre(ProfileViewModel profile)
        {
            if (ModelState.IsValid)
            {
                var currentUser = _unitOfWork.UserRepository.GetById(User.Identity.GetUserId());
                for (int i = 0; i < profile.AllGenres.Count; i++)
                {
                    if (profile.AllGenres[i].Selected)
                    {
                        currentUser.FavoriteGenres.Add(_unitOfWork.GenreRepository.GetById(profile.AllGenres[i].Genre.GenreId));
                    }
                }
               _unitOfWork.UserRepository.Update(currentUser);
                _unitOfWork.Save();
            }
            return Redirect("MyProfile");
        }

        public FileContentResult GetImage(string id)
        {
            var user = _unitOfWork.UserRepository.Get().FirstOrDefault(p => p.Id == id);
            if (user != null)
            {
                return File(user.ImageData, user.ImageMimeType);
            }
            else
            {
                return null;
            }
        }

        public ActionResult Edit()
        {
            User user = UserManager.FindByIdAsync(User.Identity.GetUserId()).Result;
            return View("EditMyProfileView", user);
        }

        [HttpPost]
        public ActionResult Edit(User model, HttpPostedFileBase image = null)
        {
            if (ModelState.IsValid)
            {
                User user = _unitOfWork.UserRepository.GetById(model.Id);
                var amountOfUsersWithSameNick =
                    (from u in _unitOfWork.UserRepository.Get()
                     where u.UserName == model.UserName && u.UserName != user.UserName
                     select u).Count();
                if (amountOfUsersWithSameNick >= 1)
                {
                    ModelState.AddModelError("", "Such nickname already exists");
                    return RedirectToAction("Edit");
                }
                var amountOfUsersWithSameEmail =
                    (from u in _unitOfWork.UserRepository.Get()
                     where u.Email == model.Email && u.Email != user.Email
                     select u).Count();

                if (amountOfUsersWithSameEmail >= 1)
                {
                    ModelState.AddModelError("", "Such email already exists");
                    return RedirectToAction("Edit");
                }

                user.UserName = model.UserName;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.Age = model.Age;
                user.LastActivityDateTime = DateTime.Now.ToLongDateString();
                if (!string.IsNullOrEmpty(model.AboutMyself))
                {
                    user.AboutMyself = model.AboutMyself;
                }
                else
                {
                    user.AboutMyself = null;
                }
                if (image != null)
                {
                    user.ImageMimeType = image.ContentType;
                    user.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(user.ImageData, 0, image.ContentLength);
                }
                _unitOfWork.UserRepository.Update(user);
                _unitOfWork.Save();
                return RedirectToAction("MyProfile");
            }
            return Edit();
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

        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }


    }
}