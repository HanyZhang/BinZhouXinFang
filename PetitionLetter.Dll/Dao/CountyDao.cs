using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinYuan.Lib;
using PetitionLetter.Dll.Model;

namespace PetitionLetter.Dll.Dao
{
    public class CountyDao : GenericDaoBase<PetitionLetterEntities, County>
    {
        protected override IOrderedQueryable<County> defaultOrderQuery()
        {
            return database.County.OrderBy(t => t.Id);
        }

        public List<County> getByLevel(int level)
        {
            return database.County.Where(t => t.Level == level).ToList();
        }

        //获得直属下级
        public List<County> getChildren(string pid)
        {
            return database.County.Where(t => t.ParentId == pid).ToList();
        }

        public List<County> getAllChildren(string pid)
        {
            List<County> list = new List<County>();
            var q = database.County.Where(t => t.ParentId == pid).ToList();
            if (q.Count == 0) return q;
            list.AddRange(q);
            if (q.First().Level == (int)Level.Village) return list;
            foreach (var item in q)
            {
                list.AddRange(getAllChildren(item.Id));
            }
            return list;
        }
    }
}
