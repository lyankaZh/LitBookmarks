using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain_Logic.Abstract;
using Domain_Logic.Entities;
using Microsoft.AspNet.Identity;

namespace LitBookmarks.Controllers
{
    public class CommentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

      
        public ActionResult ShowCommentField(int bookmarkId)
        {
            TempData["commentField"+bookmarkId] = "show";
            var returnUrl = Request.UrlReferrer == null ? "Profile/MyProfile" :
                Request.UrlReferrer.PathAndQuery;

            return Redirect(returnUrl);
        }

        public ActionResult Cancel()
        {
            var returnUrl = Request.UrlReferrer == null ? "Profile/MyProfile" :
                Request.UrlReferrer.PathAndQuery;

            return Redirect(returnUrl);
        }

        public ActionResult AddComment(string text, int bookmark)
        {
            var comment = new Comment
            {
                Text = text,
                Bookmark = _unitOfWork.BookmarkRepository.GetById(bookmark),
                Author = _unitOfWork.UserRepository.GetById(User.Identity.GetUserId())
            };
            _unitOfWork.CommentRepository.Insert(comment);
            _unitOfWork.Save();
            var returnUrl = Request.UrlReferrer == null ? "Profile/MyProfile" :
              Request.UrlReferrer.PathAndQuery;

            return Redirect(returnUrl);
        }

        public ActionResult EditComment()
        {
            throw new NotImplementedException();
        }

        public ActionResult DeleteComment()
        {
            throw new NotImplementedException();
        }
    }
}