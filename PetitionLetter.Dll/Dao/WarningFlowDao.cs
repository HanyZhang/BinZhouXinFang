using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BinYuan.Lib;
using PetitionLetter.Dll.Model;

namespace PetitionLetter.Dll.Dao
{
    public class WarningFlowDao : GenericDaoBase<PetitionLetterEntities, WarningFlow>
    {
        protected override IOrderedQueryable<WarningFlow> defaultOrderQuery()
        {
            return database.WarningFlow.OrderBy(t => t.Id);
        }

        public void addWarningFlow(WarningFlow model)
        {
            if (model == null) return;
            Warning warning = database.Warning.Find(model.WarningId);
            if (warning == null) return;
            database.WarningFlow.Add(model);
            database.SaveChanges();
            warning.Status = model.Status;
            database.Entry(warning).State = System.Data.Entity.EntityState.Modified;
            database.SaveChanges();
        }
    }
}