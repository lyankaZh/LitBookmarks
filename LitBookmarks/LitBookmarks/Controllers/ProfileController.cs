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

        

        public List<UserViewModel> GetFollowers(string userId)
        {
            var currentUser = _unitOfWork.UserRepository.GetById(userId);
            var followers = new List<UserViewModel>();
            foreach (var user in _unitOfWork.UserRepository.Get())
            {
                if (user.Following.Contains(currentUser))
                {
                    followers.Add(
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
                            FavoriteGenres = user.FavoriteGenres,
                            BookmarksAmount = user.Bookmarks.Count,
                            FollowingAmount = user.Following.Count,
                            FollowersAmount = (from u in _unitOfWork.UserRepository.Get()
                                               where u.Following.Contains(user)
                                               select u).Count(),
                            IsFollowing = currentUser.Following.Contains(user),
                            ReturnUrl = "/Profile/ShowMyFollowers"
                        });
                }
            }
            return followers;
        }

        public ActionResult ShowMyFollowers()
        {
            var followers = GetFollowers(User.Identity.GetUserId());
            ViewBag.Title = "My Followers";
            return View("FollowView", followers);
        }

        [ChildActionOnly]
        public ActionResult ShowFollowersOfUserById(string id)
        {
            var followers = GetFollowers(id);
            return PartialView("FollowView", followers);
        }

        [ChildActionOnly]
        public ActionResult ShowFollowingOfUserById(string id)
        {
            var following = GetFollowing(id);
            return PartialView("FollowView", following);
        }

      

        public List<UserViewModel> GetFollowing(string userId)
        {
            var currentUser = _unitOfWork.UserRepository.GetById(userId);
            var following = new List<UserViewModel>();
            foreach (var user in currentUser.Following)
            {
                following.Add(
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
                        FavoriteGenres = user.FavoriteGenres,
                        BookmarksAmount = user.Bookmarks.Count,
                        FollowingAmount = user.Following.Count,
                        FollowersAmount = (from u in _unitOfWork.UserRepository.Get()
                                           where u.Following.Contains(user)
                                           select u).Count(),
                        IsFollowing = true,
                        ReturnUrl = "/Profile/ShowFollowing"
                    });
            }
            return following;
        }

        public ActionResult ShowFollowing()
        {
            var following = GetFollowing(User.Identity.GetUserId());
            ViewBag.Title = "I follow";
            return View("FollowView", following);
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

        public ActionResult Follow(UserViewModel user)
        {
            var currentUser = _unitOfWork.UserRepository.GetById(User.Identity.GetUserId());
            var userToFollow = _unitOfWork.UserRepository.GetById(user.Id);
            currentUser.Following.Add(userToFollow);
            _unitOfWork.UserRepository.Update(currentUser);
            _unitOfWork.Save();
            return new RedirectResult(user.ReturnUrl);
        }

        public ActionResult Unfollow(UserViewModel user)
        {
            var currentUser = _unitOfWork.UserRepository.GetById(User.Identity.GetUserId());
            var userToFollow = _unitOfWork.UserRepository.GetById(user.Id);
            currentUser.Following.Remove(userToFollow);
            _unitOfWork.UserRepository.Update(currentUser);
            _unitOfWork.Save();
            return new RedirectResult(user.ReturnUrl);
        }

        public ActionResult Like(int bookmarkId)
        {
            throw new NotImplementedException();
        }

        public ActionResult Unlike(int bookmarkId)
        {
            throw new NotImplementedException();
        }

       private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }


        public ActionResult ShowAllUsers()
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
                        ReturnUrl = "/Profile/ShowAllUsers"
                    });
            }

            ViewBag.Title = "All users";
            return View("FollowView", allUsers);
        }

        public ActionResult ShowAnotherUserProfile(UserViewModel user)
        {
            return View("AnotherUserViewModel", user);
        }

       
    }
}