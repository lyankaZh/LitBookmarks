﻿using Domain_Logic.Abstract;
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

        public ActionResult GetAllBookmarks (string searchText = null)
        {
            List<BookmarkViewModel> allBookmarks = GetBookmarksByUserId();
            if (searchText != null)
            {
                allBookmarks = allBookmarks.Where(x => x.Book.ToLower().Contains(searchText.ToLower())).ToList();
            }
            return View("AllBookmarksView", allBookmarks);
        }

        public ActionResult ShowMyBookmarks(string searchText = null)
        {
            List<BookmarkViewModel> myBookmarks = GetBookmarksByUserId(User.Identity.GetUserId());
            if (searchText != null)
            {
                myBookmarks = myBookmarks.Where(x => x.Book == searchText).ToList();
            }
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
                    Genres = bookmark.Genres,
                    AmountOfLikes = bookmark.Likers.Count,
                    IsLiked = bookmark.Likers.Count(x => x.Id == User.Identity.GetUserId()) == 1,
                    Comments = _unitOfWork.CommentRepository.Get(x => x.Bookmark.BookmarkId == bookmark.BookmarkId).ToList()

                });
            }
            return bookmarkModels.OrderByDescending(x => x.Date).ToList();
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
            return RedirectToAction("ShowMyBookmarks", "Bookmark");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var bookmark = _unitOfWork.BookmarkRepository.GetById(id);
            _unitOfWork.BookmarkRepository.Delete(bookmark);
            _unitOfWork.Save();
            return RedirectToAction("ShowMyBookmarks", "Bookmark");
        }

   
        public ActionResult EditView(int id)
        {
            Bookmark bookmark = _unitOfWork.BookmarkRepository.GetById(id);
            BookmarkViewModel model = new BookmarkViewModel();
            model.BookmarkId = bookmark.BookmarkId;
            model.Name = bookmark.Name;
            model.Book = bookmark.Book;
            model.Author = bookmark.Author;
            model.Description = bookmark.Description;
            model.Date = bookmark.Date;
            model.BookmarkOwner = bookmark.BookmarkOwner;
            return View("EditBookmarkView", model);
        }

        [HttpPost]
        public ActionResult Edit(BookmarkViewModel model)
        {
            if (ModelState.IsValid)
            {
                Bookmark bookmark = _unitOfWork.BookmarkRepository.GetById(model.BookmarkId);
                if(model.Name != null)
                {
                    bookmark.Name = model.Name;
                }

                if (model.Book != null)
                {
                    bookmark.Book = model.Book;
                }

                if(model.Author != null)
                {
                    bookmark.Author = model.Author;
                }

                if (model.Description != null)
                {

                    bookmark.Description = model.Description;
                }
              
                _unitOfWork.BookmarkRepository.Update(bookmark);
                _unitOfWork.Save();
                 
                return RedirectToAction("ShowMyBookmarks","Bookmark");
            }
            return RedirectToAction("EditView", model.BookmarkId);
        }
    }
}