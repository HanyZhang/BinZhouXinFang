using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinYuan.Lib;
using PetitionLetter.Dll.Model;

namespace PetitionLetter.Dll.Dao
{
    public class CategoryDao : GenericDaoBase<PetitionLetterEntities, Category>
    {
        protected override IOrderedQueryable<Category> defaultOrderQuery()
        {
            return database.Category.OrderBy(t => t.Id);
        }

        public void addOrUpdate(Category model)
        {
            if (model == null) return;
            if (model.Id == 0)
                database.Category.Add(model);
            else
                database.Entry(model).State = System.Data.Entity.EntityState.Modified;
            database.SaveChanges();
        }

        public void remove(int id)
        {
            Category category = database.Category.Find(id);
            if (category == null) return;
            List<Petition> list = database.Petition.Where(t => t.CategoryId == category.Id).ToList();
            if (list.Count != 0)
                database.Petition.RemoveRange(list);
            database.Category.Remove(category);
            database.SaveChanges();
        }
    }
}
