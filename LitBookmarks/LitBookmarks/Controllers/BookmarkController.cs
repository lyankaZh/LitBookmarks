using Domain_Logic.Abstract;
using Domain_Logic.Concrete;
using Domain_Logic.Entities;
using LitBookmarks.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LitBookmarks.Controllers
{
    public class BookmarkController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookmarkController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ActionResult GetAllBookmarks ()
        {
            List<BookmarkViewModel> allBookmarks = GetBookmarksByUserId();
            return View("AllBookmarksView", allBookmarks);
        }

        public ActionResult ShowMyBookmarks()
        {
            List<BookmarkViewModel> myBookmarks = GetBookmarksByUserId(User.Identity.GetUserId());
            return View("MyBookmarks", myBookmarks);
        }
        
        public List<BookmarkViewModel> GetBookmarksByUserId(string userId = null)
        {
            List<BookmarkViewModel> bookmarkModels = new List<BookmarkViewModel>();
            var bookmarks = userId == null ? _unitOfWork.BookmarkRepository.Get().ToList() 
                : _unitOfWork.BookmarkRepository.Get(x => x.BookmarkOwner.Id == userId).ToList();

            foreach (var bookmark in bookmarks)
            {
               bookmarkModels.Add(new BookmarkViewModel()
                {
                    Author = bookmark.Author,
                    Book = bookmark.Book,
                    BookmarkId = bookmark.BookmarkId,
                    Description = bookmark.Description,
                    Date = bookmark.Date,
                    BookmarkOwner = bookmark.BookmarkOwner,
                    Genres = bookmark.Genres
                });
            }
            return bookmarkModels;
        }

        [ChildActionOnly]
        public ActionResult ShowBookmarksOfUserById(string id)
        {
            var bookmarksOfUser = GetBookmarksByUserId(id);
            return PartialView("_BookMarksView", bookmarksOfUser);
        }

        public ActionResult AddBookmark()
        {
            return View("AddBookmark");
        }

        [HttpPost]
        public ActionResult AddBookmark(BookmarkViewModel bookmark)
        {
            _unitOfWork.BookmarkRepository.Insert(new Bookmark()
            {
                Name = bookmark.Name,
                Book = bookmark.Book,
                Author = bookmark.Author,
                BookmarkOwner = _unitOfWork.UserRepository.GetById(User.Identity.GetUserId()),
                Description = bookmark.Description,
                Date = DateTime.Now.ToLongDateString()
            });
            _unitOfWork.Save();
            return View("MyBookmarks");
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            var bookmark = _unitOfWork.BookmarkRepository.GetById(id);
            _unitOfWork.BookmarkRepository.Delete(bookmark);
            _unitOfWork.Save();

            return new RedirectResult("MyBookmarks");
        }
    }
}