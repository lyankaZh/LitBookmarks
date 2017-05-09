using Domain_Logic.Abstract;
using Domain_Logic.Concrete;
using LitBookmarks.Models;
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
            List<BookmarkViewModel> allBookmarks = new List<BookmarkViewModel>();

            foreach(var bookmark in _unitOfWork.BookmarkRepository.Get().ToList())
            {
                allBookmarks.Add(new BookmarkViewModel()
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
            return View("AllBookmarksView", allBookmarks);
        }

        //[HttpPost]
        //public ActionResult AddBookmak(BookmarkViewModel book)
        //{

        //}
    }
}