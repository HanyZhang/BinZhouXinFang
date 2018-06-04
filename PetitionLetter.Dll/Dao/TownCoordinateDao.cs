using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinYuan.Lib;
using PetitionLetter.Dll.Model;

namespace PetitionLetter.Dll.Dao
{
    public class TownCoordinateDao : GenericDaoBase<PetitionLetterEntities, TownCoordinate>
    {
        protected override IOrderedQueryable<TownCoordinate> defaultOrderQuery()
        {
            return database.TownCoordinate.OrderBy(t => t.Id);
        }

        public TownCoordinate getByCountyId(string countyId)
        {
            if (string.IsNullOrEmpty(countyId)) throw new ApplicationException("乡镇不能为空");
            return database.TownCoordinate.Where(t => t.CountyId == countyId).FirstOrDefault();
        }

        public void save(TownCoordinate model)
        {
            if (model == null) return;
            var old = database.TownCoordinate.Where(t => t.CountyId == model.CountyId).ToList();
            if (old.Count != 0)
            {
                database.TownCoordinate.RemoveRange(old);
                database.SaveChanges();
            }
            database.TownCoordinate.Add(model);
            database.SaveChanges();
        }
    }
}
