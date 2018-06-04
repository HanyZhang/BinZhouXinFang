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
    public class NoticeController : Controller
    {
        // GET: Notice
        public ActionResult Release()
        {
            return View();
        }

        public ActionResult ReceivedIndex()
        {
            return View();
        }

        public ActionResult ReleasedIndex()
        {
            return View();
        }

        public ActionResult PeopleIndex()
        {
            string type = Request["type"];
            string noticeIdStr = Request["NoticeId"];
            if (string.IsNullOrEmpty(noticeIdStr)) throw new ApplicationException("NoticeId is empty");
            if (string.IsNullOrEmpty(type)) throw new ApplicationException("type is empty");
            int noticeId = int.Parse(noticeIdStr);
            int status = int.Parse(type);
            UserDao dao = new UserDao();
            List<User> users = dao.getReadOrUnRead(noticeId, status);
            dao.Dispose();
            string title;
            if (status == NoticeStatus.Unread)
                title = "未读人员列表";
            else if (status == NoticeStatus.Read)
                title = "已读人员列表";
            else
                throw new ApplicationException("无此状态");
            ViewBag.Title = title;
            return View(users);
        }

        public ActionResult ReceiveDetail()
        {
            string idStr = Request["id"];
            if (string.IsNullOrEmpty(idStr))
                throw new ApplicationException("未传入指定参数");
            int id = int.Parse(idStr);
            NoticeReceiverDao dao = new NoticeReceiverDao();
            NoticeReceiver item = dao.get(id);
            if (item == null) throw new ApplicationException("未找到该通知信息" + id);
            if (item.Status == NoticeStatus.Unread)
                dao.setToRead(id);
            dao.Dispose();
            return View(item);
        }

        public ActionResult ReleaseDetail()
        {
            string idStr = Request["id"];
            if (string.IsNullOrEmpty(idStr))
                throw new ApplicationException("未传入指定参数");
            int id = int.Parse(idStr);
            NoticeDao dao = new NoticeDao();
            Notice notice = dao.get(id);
            dao.Dispose();
            if (notice == null) throw new ApplicationException("未找到该通知信息" + id);
            return View(notice);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ReleasePost(string title, string content)
        {
            MyJsonResult result = new MyJsonResult();
            User user = TheApp.currentUser;
            if (user == null)
                return Redirect("/Home/Login");
            if (user.Role != Role.Admin)
            {
                result.message = "您不是管理员，没有权限发布通知！";
                return Json(result);
            }
            if (user.theCounty.Level == (int)Level.Town)
            {
                result.message = "您是乡镇管理员无法发布通知！";
                return Json(result);
            }
            if (string.IsNullOrEmpty(title))
            {
                result.message = "请输入通知标题！";
                return Json(result);
            }
            if (string.IsNullOrEmpty(content))
            {
                result.message = "请输入通知内容！";
                return Json(result);
            }

            Notice notice = new Notice();
            notice.Title = title;
            notice.Content = content;
            notice.Sender = user.Id;
            notice.CreateTime = DateTime.Now;
            
            NoticeDao dao = new NoticeDao();
            dao.Release(notice);
            dao.Dispose();
            result.success = true;
            return Json(result);
        }

        [HttpPost]
        //我收到的通知
        public ActionResult getReceiveList(NoticeCondition condition)
        {
            User user = TheApp.currentUser;
            if (user == null)
                return Redirect("/Home/Login");
            int pagesize = int.Parse(Request["rows"]);
            int pagenum = int.Parse(Request["page"]);
            if (condition == null) condition = new NoticeCondition();
            NoticeReceiverDao dao = new NoticeReceiverDao();
            List<NoticeReceiver> list = dao.getReceiveNotice(user.Id, pagenum, pagesize, condition);
            int total = dao.getReceiveCount(user.Id);
            dao.Dispose();
            JsonData data = new JsonData();
            data.rows = list;
            data.total = total;
            return Json(data);
        }

        [HttpPost]
        //我发布的通知
        public ActionResult getReleaseList(NoticeCondition condition)
        {
            User user = TheApp.currentUser;
            if (user == null)
                return Redirect("/Home/Login");
            int pagesize = int.Parse(Request["rows"]);
            int pagenum = int.Parse(Request["page"]);
            if (condition == null) condition = new NoticeCondition();
            NoticeDao dao = new NoticeDao();
            int total;
            List<Notice> list = dao.getRelease(user.Id, pagenum, pagesize, out total, condition);
            dao.Dispose();
            JsonData data = new JsonData();
            data.rows = list;
            data.total = total;
            return Json(data);
        }
    }
}