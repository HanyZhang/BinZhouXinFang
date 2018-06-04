using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinYuan.Lib;
using PetitionLetter.Dll.Model;
using PetitionLetter.Dll.Util;

namespace PetitionLetter.Dll.Dao
{
    public class PetitionDao : GenericDaoBase<PetitionLetterEntities, Petition>
    {
        protected override IOrderedQueryable<Petition> defaultOrderQuery()
        {
            return database.Petition.OrderBy(t => t.Id);
        }

        public int getCountByParentAndYear(string parentId, int year)
        {
            if (string.IsNullOrEmpty(parentId)) throw new ApplicationException("empty string");
            CountyDao dao = new CountyDao();
            List<string> list = dao.getAllChildren(parentId).Select(t => t.Id).ToList();
            dao.Dispose();
            if (year != 0)
                return database.Petition.Count(t => list.Contains(t.ProblemAddress) && t.PetitionDate.Year == year);
            else
                return database.Petition.Count(t => list.Contains(t.ProblemAddress));
        }

        public int getCountByCategoryAndYear(int category, int year)
        {
            return database.Petition.Count(t => t.CategoryId == category && t.PetitionDate.Year == year);
        }

        public List<Petition> getList(PetitionCondition condition, int pagenum, int pagesize, out int count)
        {
            IQueryable<Petition> q = getByCondition(condition);
            count = q.Count();
            var list = q.Skip((pagenum - 1) * pagesize).Take(pagesize).ToList();
            if (list.Count == 0) return list;
            foreach (var item in list)
            {
                loadForeignKey(item);
            }
            return list;
        }

        public void addOrUpdate(Petition model)
        {
            if (model.Id == 0)
                database.Petition.Add(model);
            else
                database.Entry(model).State = System.Data.Entity.EntityState.Modified;
            database.SaveChanges();
        }

        public List<PetitionStatistics> getStatistics(out int total_Qi, out int total_Ren, PetitionCondition condition)
        {
            List<PetitionStatistics> result = new List<PetitionStatistics>();
            var list = database.County.Where(t => t.Level == (int)Level.County).ToList();
            if (list.Count == 0) throw new ApplicationException("县区为空");
            foreach (var item in list)
            {
                PetitionStatistics model = new PetitionStatistics();
                condition.ProblemAddress = item.Id;
                model.City_Qi = getPetitionNumber((int)Visit.City, condition);
                model.City_Ren = getPeopleCount((int)Visit.City, condition);
                model.Province_Qi = getPetitionNumber((int)Visit.Province, condition);
                model.Province_Ren = getPeopleCount((int)Visit.Province, condition);
                model.Capital_Qi = getPetitionNumber((int)Visit.Capital, condition);
                model.Capital_Ren = getPeopleCount((int)Visit.Capital, condition);
                model.NoVisit_Qi = getPetitionNumber((int)Visit.NoVisit, condition);
                model.NoVisit_Ren = getPeopleCount((int)Visit.NoVisit, condition);
                model.CountyId = item.Id;
                model.CountyName = item.Name;
                result.Add(model);
            }
            result.Add(getSum(result, out total_Qi, out total_Ren));
            return result;
        }

        public List<DailyPetition> getDaily(string countyId)
        {
            List<DailyPetition> result = new List<DailyPetition>();
            if (string.IsNullOrEmpty(countyId)) return result;
            var today = DateTime.Now;
            for (int i = 0; i < 10; i++)
            {
                var day = today.AddDays(0 - i);
                DailyPetition daily = new DailyPetition();
                daily.PetitionDate = day.ToString("yyyy-MM-dd");
                daily.DayOfWeek = StringUtil.DayOfWeek(day.DayOfWeek);
                var item = database.Petition.Where(t => t.PetitionDate == day.Date && t.ProblemAddress == countyId).FirstOrDefault();
                daily.Status = item == null ? (int)DailyStatus.NotReported : (int)DailyStatus.Reported;
                result.Add(daily);
            }
            return result;
        }

        public override void loadForeignKey(Petition entity)
        {
            if (entity == null) return;
            entity.DateString = entity.PetitionDate.ToString("yyyy-MM-dd");
            switch (entity.VisitType)
            {
                case (int)Visit.City:
                    entity.VisitString = "来市访";
                    break;
                case (int)Visit.Province:
                    entity.VisitString = "到省访";
                    break;
                case (int)Visit.Capital:
                    entity.VisitString = "进京访";
                    break;
                case (int)Visit.NoVisit:
                    entity.VisitString = "非访";
                    break;
                default:
                    throw new ApplicationException("Unknown VisitType");
            }
        }

        /// <summary>
        /// 案件起数
        /// </summary>
        /// <param name="visit"></param>
        /// <param name="countyId"></param>
        /// <returns></returns>
        private int getPetitionNumber(int visit, PetitionCondition condition)
        {
            IQueryable<Petition> q = getByCondition(condition);
            q = q.Where(t => t.VisitType == visit);
            return q.Count();
            //var today = DateTime.Now.Date;
            //return database.Petition.Count(t => t.VisitType == visit && t.ProblemAddress == countyId && t.PetitionDate == today);
        }

        /// <summary>
        /// 案件人数
        /// </summary>
        /// <param name="visit"></param>
        /// <param name="countyId"></param>
        /// <returns></returns>
        private int getPeopleCount(int visit, PetitionCondition condition)
        {
            IQueryable<Petition> q = getByCondition(condition);
            q = q.Where(t => t.VisitType == visit);
            return q.Count() == 0 ? 0 : q.Sum(t => t.Count);
            //var q = database.Petition.Where(t => t.VisitType == visit && t.ProblemAddress == countyId && t.PetitionDate == today);
            //return q.Count() == 0 ? 0 : q.Sum(t => t.Count);
        }


        private PetitionStatistics getSum(List<PetitionStatistics> list, out int total_Qi, out int total_Ren)
        {
            PetitionStatistics model = new PetitionStatistics();
            total_Qi = 0;
            total_Ren = 0;
            if (list == null || list.Count == 0) return model;
            model.City_Qi = list.Sum(t => t.City_Qi);
            model.City_Ren = list.Sum(t => t.City_Ren);
            model.Province_Qi = list.Sum(t => t.Province_Qi);
            model.Province_Ren = list.Sum(t => t.Province_Ren);
            model.Capital_Qi = list.Sum(t => t.Capital_Qi);
            model.Capital_Ren = list.Sum(t => t.Capital_Ren);
            model.NoVisit_Qi = list.Sum(t => t.NoVisit_Qi);
            model.NoVisit_Ren = list.Sum(t => t.NoVisit_Ren);
            model.CountyName = "合计";
            total_Qi = model.City_Qi + model.Province_Qi + model.Capital_Qi + model.NoVisit_Qi;
            total_Ren = model.City_Ren + model.Province_Ren + model.Capital_Ren + model.NoVisit_Ren;
            return model;
        }

        private IQueryable<Petition> getByCondition(PetitionCondition condition)
        {
            IQueryable<Petition> q = defaultOrderQuery();
            if (condition == null) return q;
            if (condition.StartTime != null) q = q.Where(t => t.PetitionDate >= condition.StartTime.Value);
            if (condition.EndTime != null) q = q.Where(t => t.PetitionDate <= condition.EndTime.Value);
            if (!string.IsNullOrEmpty(condition.ProblemAddress)) q = q.Where(t => t.ProblemAddress == condition.ProblemAddress);
            if (condition.CategoryId != 0) q = q.Where(t => t.CategoryId == condition.CategoryId);
            if (condition.Daily != null) q = q.Where(t => t.PetitionDate == condition.Daily.Value);
            return q;
        }
    }

    public class PetitionCondition
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string ProblemAddress { get; set; }
        public int CategoryId { get; set; }
        public DateTime? Daily { get; set; }
    }
}
