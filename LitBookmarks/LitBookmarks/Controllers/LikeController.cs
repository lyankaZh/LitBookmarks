using System.Web.Mvc;
using Domain_Logic.Abstract;
using Domain_Logic.Entities;
using LitBookmarks.Models;
using Microsoft.AspNet.Identity;

namespace LitBookmarks.Controllers
{
    public class LikeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public LikeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ActionResult Like(BookmarkViewModel model)
        {
            Bookmark bookmark = _unitOfWork.BookmarkRepository.GetById(model.BookmarkId);
            User user = _unitOfWork.UserRepository.GetById(User.Identity.GetUserId());
            user.LikedBookmarks.Add(bookmark);
            _unitOfWork.UserRepository.Update(user);
            _unitOfWork.Save();

            var returnUrl = Request.UrlReferrer == null ? "Profile/MyProfile" : 
                Request.UrlReferrer.PathAndQuery;

            return Redirect(returnUrl);
        }

        public ActionResult Unlike(BookmarkViewModel model)
        {
            Bookmark bookmark = _unitOfWork.BookmarkRepository.GetById(model.BookmarkId);
            User user = _unitOfWork.UserRepository.GetById(User.Identity.GetUserId());
            user.LikedBookmarks.Remove(bookmark);
            _unitOfWork.UserRepository.Update(user);
            _unitOfWork.Save();

            var returnUrl = Request.UrlReferrer == null ? "Profile/MyProfile" :
               Request.UrlReferrer.PathAndQuery;
            return Redirect(returnUrl);
        }
    }
}