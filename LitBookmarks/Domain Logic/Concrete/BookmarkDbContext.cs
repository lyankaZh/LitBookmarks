using System.Collections.Generic;
using System.Data.Entity;
using Domain_Logic.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Domain_Logic.Concrete
{
    public class BookmarkDbContext:IdentityDbContext<User>
    {
        public BookmarkDbContext(): base("BbbDb") 
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
            var about = "I know nothing";

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
                        Age = age,
                        AboutMyself = about,
                        Following = new List<User>()
                    }, password);
                user = userManager.FindByName(userName);
            }

            var user2 = userManager.FindByName("user2");
            if (user2 == null)
            {
                userManager.Create(
                    new User
                    {
                        UserName = "user2",
                        FirstName = "Ned",
                        LastName = "Stark",
                        Email = "user2@gmail.com",
                        Age = 30,
                        AboutMyself = "Winter is Coming",
                        Following = new List<User>()
                    }, "123qqq");
                user2 = userManager.FindByName("user2");
            }

            var user3 = userManager.FindByName("user3");
            if (user3 == null)
            {
                userManager.Create(
                    new User
                    {
                        UserName = "user3",
                        FirstName = "Cersei",
                        LastName = "Lannister",
                        Email = "user3@gmail.com",
                        Age = 30,
                        AboutMyself = "Lannisters always pay their debts",
                        Following = new List<User>()
                    }, "123qqq");
                user3 = userManager.FindByName("user3");
            }

            user.Following.Add(user2);
            user3.Following.Add(user);


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
