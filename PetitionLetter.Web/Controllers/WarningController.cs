using PetitionLetter.Dll.Dao;
using PetitionLetter.Dll.Model;
using PetitionLetter.Dll.Util;
using PetitionLetter.Web.WebCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetitionLetter.Web.Controllers
{
    public class WarningController : Controller
    {
        // GET: Warning
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Detail()
        {
            string idStr = Request["warningId"];
            if (string.IsNullOrEmpty(idStr))
                throw new ApplicationException("未传入指定参数");
            int warningId = int.Parse(idStr);
            WarningDao dao = new WarningDao();
            Warning warning = dao.get(warningId);
            dao.Dispose();

            if (warning == null)
                throw new ApplicationException("未找到该预警信息" + warningId);
            List<string> list = StringUtil.ImageSplit(warning.Image);
            ViewData["ImageList"] = list;
            return View(warning);
        }

        public ActionResult getList()
        {
            User user = TheApp.currentUser;
            if (user == null) return Redirect("/Home/Login");
            int pagesize = int.Parse(Request["rows"]);
            int pagenum = int.Parse(Request["page"]);
            WarningDao dao = new WarningDao();
            List<Warning> list = dao.getWarningList(user, pagesize, pagenum);
            int total = dao.getCount(user);
            dao.Dispose();            
            JsonData data = new JsonData();
            data.rows = list;
            data.total = total;
            return Json(data);
        }

        [HttpPost]
        public ActionResult WarningFlowPost(WarningFlow flow)
        {
            User user = TheApp.currentUser;
            if (user == null) return Redirect("/Home/Login");
            MyJsonResult result = new MyJsonResult();
            flow.UserId = user.Id;
            flow.Time = DateTime.Now;
            WarningFlowDao dao = new WarningFlowDao();
            dao.addWarningFlow(flow);
            dao.Dispose();
            result.success = true;
            return Json(result);
        }
    }
}