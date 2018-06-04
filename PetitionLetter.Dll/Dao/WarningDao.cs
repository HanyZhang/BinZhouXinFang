using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinYuan.Lib;
using PetitionLetter.Dll.Model;

namespace PetitionLetter.Dll.Dao
{
    public class WarningDao : GenericDaoBase<PetitionLetterEntities, Warning>
    {
        protected override IOrderedQueryable<Warning> defaultOrderQuery()
        {
            return database.Warning.OrderBy(t => t.Id);
        }

        public List<Warning> getWarningList(User currentUser, int pagesize, int pagenum)
        {
            if (currentUser == null) throw new ApplicationException("当前用户为空");
            if (currentUser.theCounty.Level == (int)Level.City)
                return database.Warning.OrderBy(t => t.Id).ToList();
            List<Warning> result = new List<Warning>();
            List<County> list = getVillage(currentUser.CountyId);
            if (list.Count == 0) return result;
            List<string> villages = list.Select(t => t.Id).ToList();
            var users = database.User.Where(t => villages.Contains(t.CountyId)).Select(t => t.Id).ToList();
            if (users.Count == 0) return result;
            result = database.Warning.Where(t => users.Contains(t.UserId)).OrderBy(t => t.Id).Skip((pagenum - 1) * pagesize).Take(pagesize).ToList();
            foreach (var item in result)
            {
                loadForeignKey(item);
            }
            return result;
        }

        public int getCount(User currentUser)
        {
            if (currentUser == null) throw new ApplicationException("当前用户为空");
            if (currentUser.theCounty.Level == (int)Level.City)
                return database.Warning.Count();
            List<County> list = getVillage(currentUser.CountyId);
            if (list.Count == 0) return 0;
            List<string> villages = list.Select(t => t.Id).ToList();
            var users = database.User.Where(t => villages.Contains(t.CountyId)).Select(t => t.Id).ToList();
            if (users.Count == 0) return 0;
            return database.Warning.Count(t => users.Contains(t.UserId));
        }

        public List<County> getVillage(string parentId)
        {
            List<County> list = database.County.Where(t => t.ParentId == parentId).ToList();
            if (list.Count == 0) return list;
            if (list.First().Level == (int)Level.Village)
                return list;
            List<County> result = new List<County>();
            foreach (var item in list)
            {
                result.AddRange(getVillage(item.Id));
            }
            return result;
        }

        public override void loadForeignKey(Warning entity)
        {
            if (entity == null) return;
            var user = database.User.Where(t => t.Id == entity.UserId).FirstOrDefault();
            entity.Uploader = database.County.Where(t => t.Id == user.CountyId).Select(t => t.Name).FirstOrDefault() + "信息员";
        }

         
    }
}
