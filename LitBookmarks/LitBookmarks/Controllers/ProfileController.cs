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
            return View();
        }

        public ActionResult ShowFollowing()
        {
            return View();
        }

        public ActionResult ShowAllBookmarks()
        {
            return View();
        }

       
       
    }
}