using PetitionLetter.Dll.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitionLetter.Web.WebCode
{
    public class TheApp
    {
        public static string AppName = "滨州市信访工作信息平台";
        private const string SessionUser = "CurrentUser";
        public static User currentUser
        {
            get
            {
                if (HttpContext.Current == null) return null;
                return HttpContext.Current.Session[SessionUser] as User;
            }
            set
            {
                HttpContext.Current.Session[SessionUser] = value;
            }
        }
    }
}