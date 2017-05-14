using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Domain_Logic.Abstract;
using LitBookmarks.Models;
using Microsoft.AspNet.Identity;

namespace LitBookmarks.Controllers
{
    public class FollowController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public FollowController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                        GetUserViewModelByUserId(user.Id)
                       );
                }
            }
            return followers;
        }

        public ActionResult ShowMyFollowers(string searchText = null)
        {
            var followers = GetFollowers(User.Identity.GetUserId());
            ViewBag.Title = "My Followers";
            if (searchText != null)
            {
                followers = followers.Where(x => x.UserName.ToLower().Contains(searchText.ToLower())).ToList();
            }
            return View("FollowView", followers);
        }


        public ActionResult ShowFollowersOfUserById(string id, string searchText = null)
        {
            var followers = GetFollowers(id);
            if (searchText != null)
            {
                followers = followers.Where(x => x.UserName.ToLower().Contains(searchText.ToLower())).ToList();
            }
            return PartialView("FollowView", followers);
        }


        public ActionResult ShowFollowingOfUserById(string id, string searchText = null)
        {
            var following = GetFollowing(id);
            return PartialView("FollowView", following);    
        }


        private UserViewModel GetUserViewModelByUserId(string userId)
        {
            var user = _unitOfWork.UserRepository.GetById(userId);
            return new UserViewModel()
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

            };
        }

        public List<UserViewModel> GetFollowing(string userId)
        {
            var currentUser = _unitOfWork.UserRepository.GetById(userId);
            var following = new List<UserViewModel>();
            foreach (var user in currentUser.Following)
            {
                following.Add(GetUserViewModelByUserId(user.Id));
            }
            return following;
        }

        public ActionResult ShowFollowing(string searchText = null)
        {
            var following = GetFollowing(User.Identity.GetUserId());
            ViewBag.Title = "I follow";
            if (searchText != null)
            {
                following = following.Where(x => x.UserName.ToLower().Contains(searchText.ToLower())).ToList();
            }
            return View("FollowView", following);
        }

        public ActionResult Follow(UserViewModel user)
        {
            var currentUser = _unitOfWork.UserRepository.GetById(User.Identity.GetUserId());
            var userToFollow = _unitOfWork.UserRepository.GetById(user.Id);
            currentUser.Following.Add(userToFollow);
            _unitOfWork.UserRepository.Update(currentUser);
            _unitOfWork.Save();

            var returnUrl = Request.UrlReferrer == null ? "Profile/MyProfile" :
                    Request.UrlReferrer.PathAndQuery;

            return Redirect(returnUrl);
        }

        public ActionResult Unfollow(UserViewModel user)
        {
            var currentUser = _unitOfWork.UserRepository.GetById(User.Identity.GetUserId());
            var userToFollow = _unitOfWork.UserRepository.GetById(user.Id);
            currentUser.Following.Remove(userToFollow);
            _unitOfWork.UserRepository.Update(currentUser);
            _unitOfWork.Save();
            var returnUrl = Request.UrlReferrer == null ? "Profile/MyProfile" :
                Request.UrlReferrer.PathAndQuery;

            return Redirect(returnUrl);
        }
    }
}