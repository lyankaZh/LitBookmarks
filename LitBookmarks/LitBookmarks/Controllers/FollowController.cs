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
                            ReturnUrl = "/Follow/ShowMyFollowers"
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
                        ReturnUrl = "/Follow/ShowFollowing"
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
    }
}