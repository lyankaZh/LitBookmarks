using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Domain_Logic.Concrete;
using System.Web.Mvc;
using Domain_Logic.Abstract;
using Domain_Logic.Entities;
using LitBookmarks.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace LitBookmarks.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        public ActionResult Register(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel details, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                //returnUrl = "/Excursion/GetExcursions";

                returnUrl = "/Profile/MyProfile";


            }
            if (ModelState.IsValid)
            {
                User user = await UserManager.FindAsync(details.Username, details.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid name or password.");
                }
                else
                {
                    ClaimsIdentity ident = await UserManager.CreateIdentityAsync(user,
                    DefaultAuthenticationTypes.ApplicationCookie);
                    AuthManager.SignOut();
                    AuthManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = false
                    }, ident);
                    return Redirect(returnUrl);
                }
            }
            ViewBag.returnUrl = returnUrl;
            return View(details);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Register(RegisterViewModel model, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = "/Profile/MyProfile";
            }
            if (ModelState.IsValid)
            {
                User user = await UserManager.FindByNameAsync(model.Username);
                if (user == null)
                {
                    IdentityResult creationResult = await UserManager.CreateAsync(
                        new User
                        {
                            UserName = model.Username,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Email = model.Email,
                            Age = model.Age,                           
                            AboutMyself = model.About
                        },
                        model.Password);
                    if (creationResult.Succeeded)
                    {
                        return new RedirectResult(returnUrl);
                    }
                    AddErrorsFromResult(creationResult);
                    return View(model);
                }              
                return new RedirectResult(returnUrl);
            }
            ViewBag.returnUrl = returnUrl;
            return View(model);
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public RedirectResult LogOut()
        {
            AuthManager.SignOut();
            return new RedirectResult("/Account/Login");
        }

        private IAuthenticationManager AuthManager => HttpContext.GetOwinContext().Authentication;

        private AppUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
    }
}