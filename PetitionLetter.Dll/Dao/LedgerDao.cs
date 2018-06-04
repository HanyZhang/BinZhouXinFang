using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinYuan.Lib;
using PetitionLetter.Dll.Model;

namespace PetitionLetter.Dll.Dao
{
    public class LedgerDao : GenericDaoBase<PetitionLetterEntities, Ledger>
    {
        protected override IOrderedQueryable<Ledger> defaultOrderQuery()
        {
            return database.Ledger.OrderBy(t => t.Id);
        }

        public List<Ledger> getList(County county, int pagesize, int pagenum)
        {
            List<Ledger> list = new List<Ledger>();
            var q = defaultOrderQuery();
            if (county == null) return list;
            switch (county.Level) {
                case (int)Level.City:
                    list = q.Skip((pagenum - 1) * pagesize).Take(pagesize).ToList();
                    break;
                case (int)Level.County:
                    List<string> counties = database.County.Where(t => t.ParentId == county.Id).Select(t => t.Id).ToList();
                    list = q.Where(t => t.CountyId == county.Id || counties.Contains(t.CountyId)).Skip((pagenum - 1) * pagesize).Take(pagesize).ToList();
                    break;
                case (int)Level.Town:
                    list = q.Where(t => t.CountyId == county.Id).Skip((pagenum - 1) * pagesize).Take(pagesize).ToList();
                    break;
                default:
                    throw new ApplicationException("无权限查看台账");
            }
            if (list.Count == 0) return list;
            foreach (var item in list)
            {
                loadForeignKey(item);
            }
            return list;
        }

        public int getCout(County county)
        {
            if (county == null) return 0;
            switch (county.Level) {
                case (int)Level.City:
                    return database.Ledger.Count();
                case (int)Level.County:
                    List<string> counties = database.County.Where(t => t.ParentId == county.Id).Select(t => t.Id).ToList();
                    return database.Ledger.Count(t => t.CountyId == county.Id || counties.Contains(county.Id));
                case (int)Level.Town:
                    return database.Ledger.Count(t => t.CountyId == county.Id);
                default:
                    return 0;
            }
        }

        public override void loadForeignKey(Ledger entity)
        {
            if (entity == null) return;
            LedgerLeader leader = database.LedgerLeader.Where(t => t.LedgerId == entity.Id && t.Type == (int)LeaderType.Leader).FirstOrDefault();
            LedgerLeader director = database.LedgerLeader.Where(t => t.LedgerId == entity.Id && t.Type == (int)LeaderType.Director).FirstOrDefault();
            if (leader == null) return;
            if (director == null) return;
            entity.LeaderStr = leader.Name + "," + leader.Duty + "," + leader.Phone;
            entity.DirectorStr = director.Name + "," + director.Duty + "," + director.Phone;
            entity.DateString = entity.PetitionDate.ToString("yyyy-MM-dd");
        }

        public void add(Ledger model, LedgerLeader Leader, LedgerLeader Director)
        {
            if (model.Id == 0)
                database.Ledger.Add(model);
            else
                database.Entry(model).State = System.Data.Entity.EntityState.Modified;
            database.SaveChanges();

            var list = database.LedgerLeader.Where(t => t.LedgerId == model.Id).ToList();
            if (list.Count != 0)
                database.LedgerLeader.RemoveRange(list);

            Leader.LedgerId = model.Id;
            database.LedgerLeader.Add(Leader);
            database.SaveChanges();

            Director.LedgerId = model.Id;
            database.LedgerLeader.Add(Director);
            database.SaveChanges();
        }

        public void delete(int id)
        {
            if (id == 0) return;
            var list = database.LedgerLeader.Where(t => t.LedgerId == id).ToList();
            database.LedgerLeader.RemoveRange(list);
            database.SaveChanges();

            Ledger ledger = database.Ledger.Find(id);
            if (ledger == null) return;
            database.Ledger.Remove(ledger);
            database.SaveChanges();
        }
    }
}
