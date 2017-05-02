using Domain_Logic.Concrete;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace Domain_Logic.Entities
{
    public class AppUserManager : UserManager<User>
    {

        public AppUserManager(IUserStore<User> store) : base(store)
        {
        }
        public static AppUserManager Create(
        IdentityFactoryOptions<AppUserManager> options,
        IOwinContext context)
        {
            BookmarkDbContext db = context.Get<BookmarkDbContext>();
            AppUserManager manager = new AppUserManager(new UserStore<User>(db));
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = true
            };

            manager.UserValidator = new UserValidator<User>(manager)
            {
                RequireUniqueEmail = true
            };

            return manager;
        }
    }
}
