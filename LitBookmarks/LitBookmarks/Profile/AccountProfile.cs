using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Web.Profile;
using System.Web.Security;
using Domain_Logic.Entities;

namespace LitBookmarks.Profile
{
        public class AccountProfile : ProfileBase
        {
            public static AccountProfile CurrentUser(string username)
            {            
                    return Create(username) as AccountProfile;                                          
            }

            public string FullName
            {
                get { return ((string)(base["FullName"])); }
                set { base["FullName"] = value; Save(); }
            }
        public string Age
        {
            get { return ((string)(base["Age"])); }
            set { base["Age"] = value; Save(); }
        }
        public string AboutMySelf
        {
            get { return ((string)(base["AboutMySelf"])); }
            set { base["AboutMySelf"] = value; Save(); }
        }

        public List<Genre> Genres
        {
            get { return ((List<Genre>)(base["Genres"])); }
            set { base["Genres"] = value; Save(); }
        }
        // add additional properties here

    }
}