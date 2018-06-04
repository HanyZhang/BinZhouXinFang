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
    public class PetitionController : Controller
    {
        // GET: Petition
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(string Date)
        {
            if (string.IsNullOrEmpty(Date)) throw new ApplicationException("Date为空");
            ViewBag.Date = Date;
            return View();
        }

        public ActionResult DailyIndex()
        {
            return View();
        }

        public ActionResult Statistics(PetitionCondition condition)
        {
            if (condition == null)
                condition = new PetitionCondition();
            if (condition.StartTime == null && condition.EndTime == null)
                condition.Daily = DateTime.Now.Date;
            PetitionDao dao = new PetitionDao();
            int total_Qi, total_Ren;
            var list = dao.getStatistics(out total_Qi, out total_Ren, condition);
            dao.Dispose();
            string dateString;
            if (condition.StartTime == null && condition.EndTime == null)
                dateString = DateTime.Now.ToString("MM月dd日");
            else
            {
                var startTime = condition.StartTime == null ? " " : condition.StartTime.Value.ToString("MM月dd日");
                var endTime = condition.EndTime == null ? " " : condition.EndTime.Value.ToString("MM月dd日");
                dateString = startTime + "-" + endTime;
            }
            ViewBag.DateString = dateString;
            ViewBag.Total_Qi = total_Qi;
            ViewBag.Total_Ren = total_Ren;
            return View(list);
        }

        [HttpPost]
        public ActionResult getList(PetitionCondition condition)
        {
            if (condition == null) condition = new PetitionCondition();
            int pagesize = int.Parse(Request["rows"]);
            int pagenum = int.Parse(Request["page"]);
            PetitionDao dao = new PetitionDao();
            int count;
            var list = dao.getList(condition, pagenum, pagesize, out count);
            dao.Dispose();
            JsonData data = new JsonData();
            data.rows = list;
            data.total = count;
            return Json(data);
        }

        [HttpPost]
        public ActionResult getDailyPetitionList(string date)
        {
            User user = TheApp.currentUser;
            if (user == null) return Redirect("/Home/Login");
            int pagesize = int.Parse(Request["rows"]);
            int pagenum = int.Parse(Request["page"]);
            PetitionCondition pc = new PetitionCondition();
            if (string.IsNullOrEmpty(date)) throw new ApplicationException("date is empty");
            pc.Daily = Convert.ToDateTime(date).Date;
            int count;
            PetitionDao dao = new PetitionDao();
            var list = dao.getList(pc, pagenum, pagesize, out count);
            dao.Dispose();
            JsonData data = new JsonData();
            data.rows = list;
            data.total = count;
            return Json(data);
        }

        [HttpPost]
        public ActionResult createPost(Petition model)
        {
            User user = TheApp.currentUser;
            if (user == null) return Redirect("/Home/Login");
            //todo: 判断权限，只有市县级信访局管理员才能填写日报
            MyJsonResult result = new MyJsonResult();
            if (model == null)
            {
                result.message = "请填写日报记录";
                return Json(result);
            }
            model.PetitionDate = DateTime.Now.Date;
            model.ProblemAddress = user.CountyId;
            PetitionDao dao = new PetitionDao();
            dao.addOrUpdate(model);
            dao.Dispose();
            result.success = true;
            return Json(result);
        }

        [HttpPost]
        public ActionResult deletePost(int PetitionId)
        {
            User user = TheApp.currentUser;
            if (user == null) return Redirect("/Home/Login");
            //todo: 判断权限，只有市县级信访局管理员才能填写日报
            MyJsonResult result = new MyJsonResult();
            if (PetitionId == 0)
            {
                result.message = "请选择要删除的日报记录";
                return Json(result);
            }
            PetitionDao dao = new PetitionDao();
            dao.delete(PetitionId);
            dao.Dispose();
            result.success = true;
            return Json(result);
        }

        [HttpPost]
        public ActionResult getDailyList()
        {
            User user = TheApp.currentUser;
            if (user == null) return Redirect("/Home/Login");
            PetitionDao dao = new PetitionDao();
            List<DailyPetition> list = dao.getDaily(user.CountyId);
            dao.Dispose();
            JsonData data = new JsonData();
            data.total = list.Count;
            data.rows = list;
            return Json(data);
        }
    }
}