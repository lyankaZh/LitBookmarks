using System;
using System.Collections.Generic;
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

namespace LitBookmarks.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        UnitOfWork unitOfWork = new UnitOfWork();
        public ActionResult MyProfile()
        {
          var currentUser = unitOfWork.UserRepository.GetById(User.Identity.GetUserId());
          ProfileViewModel profile = new ProfileViewModel();
            profile.Age = currentUser.Age;
            profile.FirstName = currentUser.FirstName;
            profile.LastName = currentUser.LastName;
            profile.AboutMyself = currentUser.AboutMyself;
            profile.Email = currentUser.Email;
            profile.FavoriteGenres = currentUser.FavoriteGenres;
            profile.LastActivityDateTime = currentUser.LastActivityDateTime;
            var checkBoxes = new List<AllGenresCheckBox>();
            
            for (int i = 0; i < unitOfWork.GenreRepository.Get().ToList().Count; i++)
            {
                checkBoxes.Add(new AllGenresCheckBox()
                { 
                    Genre = unitOfWork.GenreRepository.Get().ToList()[i]
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
                var currentUser = unitOfWork.UserRepository.GetById(User.Identity.GetUserId());
                for (int i = 0; i < profile.AllGenres.Count; i++)
                {
                    if (profile.AllGenres[i].Selected)
                    {
                        currentUser.FavoriteGenres.Add(unitOfWork.GenreRepository.GetById(profile.AllGenres[i].Genre.GenreId));
                    }
                }
               unitOfWork.UserRepository.Update(currentUser);
                unitOfWork.Save();
            }
            return Redirect("MyProfile");
        }
    }
}