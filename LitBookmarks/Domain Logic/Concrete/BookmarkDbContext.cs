using System.Data.Entity;
using Domain_Logic.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Domain_Logic.Concrete
{
    public class BookmarkDbContext:IdentityDbContext<User>
    {
        public BookmarkDbContext(): base("BookmarkDb") 
        {
            Database.SetInitializer(new BookmarkDbInitializer());
        }

        public static BookmarkDbContext Create()
        {
            return new BookmarkDbContext();
        }

        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Genre> Genres { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().ToTable("Users");
        }
    }


    public class BookmarkDbInitializer : DropCreateDatabaseIfModelChanges<BookmarkDbContext>
    {
        protected override void Seed(BookmarkDbContext context)
        {

            AppUserManager userManager = new AppUserManager(new UserStore<User>(context));
            AppRoleManager roleManager = new AppRoleManager(new RoleStore<AppRole>(context));

            var userName = "user";
            var password = "123qqq";
            var email = "user@gmail.com";
            var age = 20;
            var firstName = "John";
            var lastName = "Snow";

            var user = userManager.FindByName(userName);
            if (user == null)
            {
                userManager.Create(
                    new User
                    {
                        UserName = userName,
                        FirstName = firstName,
                        LastName = lastName,
                        Email = email,
                        Age = age
                    }, password);
            }

            if (!roleManager.RoleExists("Admin"))
            {
                roleManager.Create(new AppRole("Admin"));
            }

            var admin = userManager.FindByName("Admin");
            if (admin == null)
            {
                userManager.Create(
                    new User
                    {
                        UserName = "Admin",
                        FirstName = "Admin",
                        LastName = "Admin",
                        Email = "admin@gmail.com",
                        Age = 30
                    }, "123qqq");
                admin = userManager.FindByName("Admin");
            }

            if (!userManager.IsInRole(admin.Id, "Admin"))
            {
                userManager.AddToRole(admin.Id, "Admin");
            }
            base.Seed(context);
        }
    }
}
