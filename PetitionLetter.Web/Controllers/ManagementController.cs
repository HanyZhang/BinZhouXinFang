using BinYuan.Lib;
using Newtonsoft.Json;
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
    public class ManagementController : Controller
    {
        // GET: Management
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Map()
        {
            return View();
        }

        public ActionResult UserIndex()
        {
            return View();
        }

        public ActionResult CategoryIndex()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MapPost(string overlays, string countyId)
        {
            MyJsonResult result = new MyJsonResult();
            if (string.IsNullOrEmpty(countyId))
            {
                result.message = "请选择乡镇";
                return Json(result);
            }
            if (string.IsNullOrEmpty(overlays))
            {
                result.message = "请在地图中选取行政规划";
                return Json(result);
            }
            List<Coordinate> list = JsonConvert.DeserializeObject<List<Coordinate>>(overlays);
            if (list == null || list.Count == 0)
                throw new ApplicationException("解析失败");
            string coordinate = CoordinateUtil.getCoordinateString(list);
            TownCoordinateDao dao = new TownCoordinateDao();
            dao.save(new TownCoordinate() { CountyId = countyId, Coordinate = coordinate });
            dao.Dispose();
            result.success = true;
            return Json(result);
        }

        /// <summary>
        /// 县区下拉表数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCountyList()
        {
            CountyDao dao = new CountyDao();
            List<County> countys = dao.getByLevel((int)Level.County);
            dao.Dispose();
            return Json(countys, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 乡镇下拉表数据
        /// </summary>
        /// <param name="county"></param>
        /// <returns></returns>
        public ActionResult GetTown(string county)
        {
            if (string.IsNullOrEmpty(county)) throw new ApplicationException("未选择县区");
            CountyDao dao = new CountyDao();
            List<County> list = dao.getChildren(county);
            dao.Dispose();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 类别下拉表数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCategorySelect()
        {
            CategoryDao dao = new CategoryDao();
            List<Category> list = dao.getAll(null);
            dao.Dispose();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 渠道下拉表数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetChannelList()
        {
            List<IdName> list = new List<IdName>();
            list.Add(new IdName() { Id = (int)PetitionChannel.Online, Name = "网上信访" });
            list.Add(new IdName() { Id = (int)PetitionChannel.Visit, Name = "来访" });
            list.Add(new IdName() { Id = (int)PetitionChannel.Letter, Name = "来信" });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 走访方式下拉表数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetVisitList()
        {
            List<IdName> list = new List<IdName>();
            list.Add(new IdName() { Id = (int)Visit.City, Name = "来市访" });
            list.Add(new IdName() { Id = (int)Visit.Province, Name = "到省访" });
            list.Add(new IdName() { Id = (int)Visit.Capital, Name = "进京访" });
            list.Add(new IdName() { Id = (int)Visit.NoVisit, Name = "非访" });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangePassword(string oldPwd, string newPwd) {
            User user = TheApp.currentUser;
            if (user == null) return Redirect("/Home/Login");
            MyJsonResult result = new MyJsonResult();
            if (string.IsNullOrEmpty(oldPwd))
            {
                result.message = "请输入旧密码";
                return Json(result);
            }
            if (string.IsNullOrEmpty(newPwd))
            {
                result.message = "请输入新密码";
                return Json(result);
            }
            if (user.Password.ToLower() != oldPwd.ToLower())
            {
                result.message = "旧密码不正确";
                return Json(result);
            }
            UserDao dao = new UserDao();
            dao.changePassword(user.Id, oldPwd, newPwd);
            dao.Dispose();
            result.success = true;
            return Json(result);
        }

        [HttpPost]
        public ActionResult getUserList(UserCondition condition)
        {
            if (condition == null) condition = new UserCondition();
            int pagesize = int.Parse(Request["rows"]);
            int pagenum = int.Parse(Request["page"]);
            int count;
            UserDao dao = new UserDao();
            List<User> list = dao.getList(condition, pagesize, pagenum, out count);
            dao.Dispose();
            JsonData data = new JsonData();
            data.rows = list;
            data.total = count;
            return Json(data);
        }

        [HttpPost]
        public ActionResult ResetPassword(int UserId)
        {
            User user = TheApp.currentUser;
            if (user == null) return Redirect("/Home/Login");
            MyJsonResult result = new MyJsonResult();
            if (user.Role != Role.Admin || (user.Role == Role.Admin && user.theCounty.Level != (int)Level.City))
            {
                result.message = "操作失败，您没有权限执行该操作";
                return Json(result);
            }
            if (UserId == 0)
            {
                result.message = "请选择需要重置密码的用户";
                return Json(result);
            }
            UserDao dao = new UserDao();
            bool r = dao.resetPassword(UserId);
            dao.Dispose();
            if (!r)
            {
                result.message = "重置失败，请稍后再试";
                return Json(result);
            }
            result.success = true;
            return Json(result);
        }

        [HttpPost]
        public ActionResult DeleteUserPost(int UserId)
        {
            User user = TheApp.currentUser;
            if (user == null) return Redirect("/Home/Login");
            MyJsonResult result = new MyJsonResult();
            if (user.Role != Role.Admin || (user.Role == Role.Admin && user.theCounty.Level != (int)Level.City))
            {
                result.message = "操作失败，您没有权限执行该操作";
                return Json(result);
            }
            if (UserId == 0)
            {
                result.message = "请选择要删除的用户";
                return Json(result);
            }
            UserDao dao = new UserDao();
            bool r = dao.removeUser(UserId);
            dao.Dispose();
            if (!r)
            {
                result.message = "删除失败，请稍后再试";
                return Json(result);
            }
            result.success = true;
            return Json(result);
        }

        [HttpPost]
        public ActionResult getCategoryList()
        {
            CategoryDao dao = new CategoryDao();
            List<Category>list = dao.getAll(null);
            dao.Dispose();
            JsonData data = new JsonData();
            data.rows = list;
            data.total = list.Count;
            return Json(data);
        }

        [HttpPost]
        public ActionResult CreateCategoryPost(Category model)
        {
            MyJsonResult result = new MyJsonResult();
            if (model == null)
            {
                result.message = "请填写类别";
                return Json(result);
            }
            CategoryDao dao = new CategoryDao();
            dao.addOrUpdate(model);
            dao.Dispose();
            result.success = true;
            return Json(result);            
        }

        [HttpPost]
        public ActionResult DeleteCategoryPost(int Id)
        {
            MyJsonResult result = new MyJsonResult();
            if (Id == 0)
            {
                result.message = "请选择要删除的类别";
                return Json(result);
            }
            CategoryDao dao = new CategoryDao();
            dao.remove(Id);
            dao.Dispose();
            result.success = true;
            return Json(result);
        }
    }
}