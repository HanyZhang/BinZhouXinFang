using PetitionLetter.Dll.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetitionLetter.Web.WebCode
{
    public class ActionFilters : ActionFilterAttribute 
    {
        static string[] freeModules = { "api" };
        static string[] freeUrl = { "/home/login", "/home/loginpost", "/home/logout" };
        const string loginUrl = "/home/login";
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string path = filterContext.RequestContext.HttpContext.Request.Url.LocalPath.ToLower();
            if (path == "/") return;
            if (freeUrl.Any(t => path.Contains(t))) return;

            string controller = filterContext.Controller.GetType().Name.ToLower();
            if (freeModules.Any(t => controller.StartsWith(t))) return;
            
            var Session = filterContext.HttpContext.Session;
            if (Session == null)
            {
                filterContext.HttpContext.Response.Redirect(loginUrl);
                return;
            }
            User user = TheApp.currentUser;
            if (user == null)
            {
                filterContext.HttpContext.Response.Redirect(loginUrl);
                return;
            }
        }
    }
}