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
    public class APIController : Controller
    {
        // GET: API
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login()
        {
            MyJsonResult result = new MyJsonResult();
            string username = Request.Form["username"];
            string password = Request.Form["password"];
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                result.message = "账号和密码不能为空";
                return Json(result);
            }
            UserDao dao = new UserDao();
            User user = dao.login(username, password);
            dao.Dispose();
            if (user == null)
            {
                result.message = "账号或密码错误";
                return Json(result);
            }
            result.success = true;
            result.data = user.Id;
            return Json(result);
        }

        [HttpPost]
        public ActionResult getNoticeList(int pagenum, int pagesize, string userId, string type)
        {
            List<XinFangAndroid.Adapter.Notice> andNoticeList = new List<XinFangAndroid.Adapter.Notice>();
            MyJsonResult result = new MyJsonResult();
            pagenum = pagenum == 0 ? 1 : pagenum;
            pagesize = pagesize == 0 ? 5 : pagesize;
            //string type = Request.Form["type"];     //通知类型：1: 我发布的 2: 我收到的
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(type))
            {
                result.message = "参数错误";
                return Json(result);
            }
            int count;
            if (int.Parse(type) == 1)
            {
                NoticeDao dao = new NoticeDao();
                List<Notice> list = dao.getRelease(int.Parse(userId), pagenum, pagesize, out count, null);
                foreach(var item in list)
                {
                    XinFangAndroid.Adapter.Notice note = new XinFangAndroid.Adapter.Notice(item.Id,item.Sender,item.Title,item.Content,item.CreateTime);
                    andNoticeList.Add(note);
                }
                dao.Dispose();
                result.success = true;
                result.data = andNoticeList;
                return Json(result);
            }
            else if (int.Parse(type) == 2)
            {
                NoticeReceiverDao dao = new NoticeReceiverDao();
                List<NoticeReceiver> list = dao.getReceiveNotice(int.Parse(userId), pagenum, pagesize, null);
                dao.Dispose();
                result.success = true;
                result.data = list;
                return Json(result);
            }
            else
            {
                throw new ApplicationException("类型错误");
            }
        }

        [HttpPost]
        public ActionResult setToRead(int id)
        {
            if (id == 0) return null;
            NoticeReceiverDao dao = new NoticeReceiverDao();
            dao.setToRead(id);
            dao.Dispose();
            return null;
        }
        [HttpPost]
        public ActionResult GetUsers(int id)
        {
            int count;
            UserDao dao = new UserDao();
            var list= dao.getList(null, 10, 1, out count);
            dao.Dispose();
            MyJsonResult result = new MyJsonResult();
            result.success = true;
            result.data = list;
            return Json(result);
        }
    }
}