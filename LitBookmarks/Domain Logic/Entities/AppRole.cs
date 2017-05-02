using Microsoft.AspNet.Identity.EntityFramework;

namespace Domain_Logic.Entities
{
    public class AppRole:IdentityRole
    {
        public AppRole() : base() { }
        public AppRole(string name) : base(name) { }
    }
}
