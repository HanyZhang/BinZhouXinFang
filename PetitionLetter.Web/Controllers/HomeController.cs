using PetitionLetter.Dll.Dao;
using PetitionLetter.Dll.Model;
using PetitionLetter.Web.WebCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetitionLetter.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Welcome()
        {
            return View();
        }
        public ActionResult Index()
        {
            //User user = TheApp.currentUser;
            //if (user == null) return Redirect("/Home/Login");
            //ViewBag.UserName = user.theCounty.Name + "信访局";
            //ViewBag.AppName = TheApp.AppName;
            //ViewBag.Level = user.theCounty.Level;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            TheApp.currentUser = null;
            ViewBag.AppName = TheApp.AppName;
            return View();
        }

        public ActionResult LogOut()
        {
            TheApp.currentUser = null;
            return View("Login");
        }
        
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            MyJsonResult result = new MyJsonResult();
            if (string.IsNullOrEmpty(username))
            {
                result.message = "请输入用户名";
                return Json(result);
            }
            if (string.IsNullOrEmpty(password))
            {
                result.message = "请输入密码";
                return Json(result);
            }
            UserDao dao = new UserDao();
            User user = dao.login(username, password);
            dao.Dispose();
            if (user == null)
            {
                result.message = "用户名或密码不正确";
                return Json(result);
            }
            if (user.Role != Role.Admin)
            {
                result.message = "您不是管理员，无权登录本系统";
                return Json(result);
            }
            TheApp.currentUser = user;
            result.success = true;
            return Json(result);
        }
    }
}