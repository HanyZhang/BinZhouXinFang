using PetitionLetter.Dll.Dao;
using PetitionLetter.Dll.Model;
using PetitionLetter.Web.WebCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace PetitionLetter.Web.Controllers
{
    public class LedgerController : Controller
    {
        // GET: Ledger
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult getList()
        {
            User user = TheApp.currentUser;
            if (user == null) return Redirect("/Home/Login");
            int pagesize = int.Parse(Request["rows"]);
            int pagenum = int.Parse(Request["page"]);
            JsonData result = new JsonData();
            LedgerDao dao = new LedgerDao();
            List<Ledger> list = dao.getList(user.theCounty, pagesize, pagenum);
            int count = dao.getCout(user.theCounty);
            dao.Dispose();
            result.rows = list;
            result.total = count;
            return Json(result);
        }

        [HttpPost]
        public ActionResult CreatePost(Ledger model)
        {
            User user = TheApp.currentUser;
            if (user == null) return Redirect("/Home/Login");
            MyJsonResult result = new MyJsonResult();
            if (model == null)
            {
                result.message = "请正确填写信息";
                return Json(result);
            }
            if (string.IsNullOrEmpty(model.LeaderStr))
            {
                result.message = "请填写包家领导信息";
                return Json(result);
            }
            if (string.IsNullOrEmpty(model.DirectorStr))
            {
                result.message = "请填写维稳负责人信息";
                return Json(result);
            }
            LedgerLeader leader = JsonConvert.DeserializeObject<LedgerLeader>(model.LeaderStr);
            LedgerLeader director = JsonConvert.DeserializeObject<LedgerLeader>(model.DirectorStr);
            if (leader == null || director == null) throw new ApplicationException("解析失败");
            model.CountyId = user.CountyId;
            LedgerDao dao = new LedgerDao();
            dao.add(model, leader, director);
            dao.Dispose();
            result.success = true;
            return Json(result);
        }

        [HttpPost]
        public ActionResult deletePost(int ledgerId)
        {
            MyJsonResult result = new MyJsonResult();
            if (ledgerId == 0)
            {
                result.message = "请选择要删除的数据";
                return Json(result);
            }
            LedgerDao dao = new LedgerDao();
            dao.delete(ledgerId);
            dao.Dispose();
            result.success = true;
            return Json(result);
        }
    }
}