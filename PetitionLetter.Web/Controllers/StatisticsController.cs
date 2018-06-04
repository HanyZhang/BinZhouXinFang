using BinYuan.Lib;
using PetitionLetter.Dll.Dao;
using PetitionLetter.Dll.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetitionLetter.Web.Controllers
{
    public class StatisticsController : Controller
    {
        // GET: Statistics
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Map()
        {
            return View();
        }

        public ActionResult BarGraph()
        {
            return View();
        }

        public ActionResult PieChart()
        {
            return View();
        }

        public ActionResult getYear()
        {
            int year = DateTime.Now.Year;
            List<IdName> list = new List<IdName>();
            list.Add(new IdName() { Id = year, Name = year.ToString() });
            for (int i = 0; i < 30; i++)
            {
                year--;
                list.Add(new IdName() { Id = year, Name = year.ToString() });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getCounty(int? year)
        {
            if (year == null) year = DateTime.Now.Year;
            CountyDao dao = new CountyDao();
            var county = dao.getByLevel((int)Level.County);
            dao.Dispose();
            if (county.Count == 0) throw new ApplicationException("未找到县区");
            List<ChartData> list = new List<ChartData>();
            PetitionDao dao2 = new PetitionDao();
            foreach (var item in county)
            {
                ChartData chart = new ChartData();
                chart.name = item.Name;
                chart.value = dao2.getCountByParentAndYear(item.Id, year.Value);
                list.Add(chart);
            }
            dao2.Dispose();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getCategory(int? year)
        {
            if (year == null) year = DateTime.Now.Year;
            CategoryDao dao = new CategoryDao();
            var category = dao.getAll(null).ToList();
            dao.Dispose();
            if (category.Count == 0) throw new ApplicationException("信访类别为空");
            List<ChartData> list = new List<ChartData>();
            PetitionDao dao2 = new PetitionDao();
            foreach (var item in category)
            {
                ChartData chart = new ChartData();
                chart.name = item.Name;
                chart.value = dao2.getCountByCategoryAndYear(item.Id, year.Value);
                list.Add(chart);
            }
            dao2.Dispose();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getxAxis(int? type)
        {
            if (type == null) throw new ApplicationException("统计类型不正确");
            List<string> list = new List<string>();
            if (type == StatisticsType.County)      //县区
            {
                CountyDao dao = new CountyDao();
                list = dao.getByLevel((int)Level.County).Select(t => t.Name).ToList();
                dao.Dispose();
            }
            else if (type == StatisticsType.Category)     //类别
            {
                CategoryDao dao = new CategoryDao();
                list = dao.getAll(null).Select(t => t.Name).ToList();
                dao.Dispose();
            }
            else
                throw new ApplicationException("未知类型");
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult getCount(string countyId)
        {
            MyJsonResult result = new MyJsonResult();
            if (string.IsNullOrEmpty(countyId))
            {
                result.message = "未选择县区或乡镇";
                return Json(result);
            }
            PetitionDao dao = new PetitionDao();
            int count = dao.getCountByParentAndYear(countyId, 0);
            dao.Dispose();
            result.success = true;
            result.data = count;
            return Json(result);
        }
    }
}