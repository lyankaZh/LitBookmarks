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
            profile.Age = currentUser.Age;
            profile.FirstName = currentUser.FirstName;
            profile.LastName = currentUser.LastName;
            profile.AboutMyself = currentUser.AboutMyself;
            profile.Email = currentUser.Email;
            profile.FavoriteGenres = currentUser.FavoriteGenres;
            profile.LastActivityDateTime = currentUser.LastActivityDateTime;
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

        public ActionResult ShowMyBookmarks()
        {
            return View();
        }

        public ActionResult ShowMyFollowers()
        {
            var currentUser = _unitOfWork.UserRepository.GetById(User.Identity.GetUserId());
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
                       FollowingAmount = user.Following.Count,
                       FollowersAmount = (from u in _unitOfWork.UserRepository.Get()
                                          where u.Following.Contains(user)
                                          select u).Count(),
                       IsFollowing = currentUser.Following.Contains(user),
                       ReturnUrl = "/Profile/ShowMyFollowers"
                   });
                }
            }

            ViewBag.Title = "My Followers";
            return View("FollowView", followers);
        }

        public ActionResult ShowFollowing()
        {
            var currentUser = _unitOfWork.UserRepository.GetById(User.Identity.GetUserId());
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
                       FollowingAmount = user.Following.Count,
                       FollowersAmount = (from u in _unitOfWork.UserRepository.Get()
                                          where u.Following.Contains(user)
                                          select u).Count(),
                       IsFollowing = true,
                       ReturnUrl = "/Profile/ShowFollowing"
                   });
            }
            
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

        public ActionResult ShowAllBookmarks()
        {
            return View();
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

    }
}