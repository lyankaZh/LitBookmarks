using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain_Logic.Entities;
using LitBookmarks.Models;
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

            return View("MyProfile", profile);
        }

        [HttpPost]
        public ActionResult AddFavoriteGenre(List<AllGenresCheckBox> profile)
        {
            if (ModelState.IsValid)
            {
                var currentUser = _unitOfWork.UserRepository.GetById(User.Identity.GetUserId());
                for (int i = 0; i < profile.Count; i++)
                {
                    if (profile[i].Selected)
                    {
                        currentUser.FavoriteGenres.Add(
                            _unitOfWork.GenreRepository.GetById(profile[i].Genre.GenreId));
                    }
                }
                _unitOfWork.UserRepository.Update(currentUser);
                _unitOfWork.Save();
            }
            return View("MyProfile");
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

        [HttpPost]
        public ActionResult Delete(string id)
        {
            var user = _unitOfWork.UserRepository.GetById(id);
            if (user != null)
            {

                //TO DO - unsubscribe only from future excursions
                if (_unitOfWork.UserRepository.GetById(id).Following.Count > 0)
                {
                    TempData["deleteTravellerErrorMessage"] =
                        "Before deleting profile unsubscribe from all users";
                    return RedirectToAction("MyProfile");
                }
            }

            HttpContext.GetOwinContext().Authentication.SignOut();
            _unitOfWork.UserRepository.Delete(user);
            _unitOfWork.Save();

            return new RedirectResult("/Account/Login");
        }

       
      
        public FileContentResult GetImage(string id)
        {
            var user = _unitOfWork.UserRepository.GetById(id);
            if (user != null)
            {
                return File(user.ImageData, user.ImageMimeType);
            }
            else
            {
                return null;
            }
        }




        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }


        

        public ActionResult ShowAllUsers(string searchText = null)
        {
            var currentUser = _unitOfWork.UserRepository.GetById(User.Identity.GetUserId());
            var allUsers = new List<UserViewModel>();
            
            
            foreach (var user in _unitOfWork.UserRepository.Get().Where(x => x != currentUser && x.UserName != "Admin"))
            {
                allUsers.Add(
                    new UserViewModel()
                    {
                        Id = user.Id,
                        AboutMyself = user.AboutMyself,
                        Age = user.Age,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        UserName = user.UserName,
                        ImageData = user.ImageData,
                        ImageMimeType = user.ImageMimeType,
                        BookmarksAmount = user.Bookmarks.Count,
                        FavoriteGenres = user.FavoriteGenres,
                        FollowingAmount = user.Following.Count,
                        FollowersAmount = (from u in _unitOfWork.UserRepository.Get()
                                           where u.Following.Contains(user)
                                           select u).Count(),
                        IsFollowing = currentUser.Following.Contains(user),
                    });
            }
            if (searchText != null)
            {
                allUsers = allUsers.Where(x => x.UserName.ToLower().Contains(searchText.ToLower())).ToList();
            }

            ViewBag.Title = "All users";
            return View("FollowView", allUsers);
        }

        public ActionResult ShowAnotherUserProfile(UserViewModel user)
        {
            return View("AnotherUserView", user);
        }
    }
}