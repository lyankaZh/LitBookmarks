using System.Data.Entity;
using Domain_Logic.Entities;
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

           
            base.Seed(context);
        }
    }
}
