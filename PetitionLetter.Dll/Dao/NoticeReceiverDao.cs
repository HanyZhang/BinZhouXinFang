using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinYuan.Lib;
using PetitionLetter.Dll.Model;

namespace PetitionLetter.Dll.Dao
{
    public class NoticeReceiverDao : GenericDaoBase<PetitionLetterEntities, NoticeReceiver>
    {
        protected override IOrderedQueryable<NoticeReceiver> defaultOrderQuery()
        {
            return database.NoticeReceiver.OrderBy(t => t.Status).ThenBy(t => t.Id);
        }

        public List<NoticeReceiver> getReceiveNotice(int userId, int pagenum, int pagesize, NoticeCondition condition)
        {
            if (userId == 0) throw new ApplicationException("用户id不能为0");
            IQueryable<Notice> q = database.Notice.OrderBy(t => t.Id);
            if (condition != null)
            {
                if (condition.StartTime != null) q = q.Where(t => t.CreateTime >= condition.StartTime);
                if (condition.EndTime != null) q = q.Where(t => t.CreateTime <= condition.EndTime.Value.AddDays(1));
                if (!string.IsNullOrEmpty(condition.KeyWord)) q = q.Where(t => t.Title.Contains(condition.KeyWord) || t.Content.Contains(condition.KeyWord));
            }
            var m = q.Select(t => t.Id).ToList();
            List<NoticeReceiver> list = database.NoticeReceiver.Where(t => t.Receiver == userId && m.Contains(t.NoticeId)).OrderBy(t => t.Status).ThenBy(t => t.Id).Skip((pagenum - 1) * pagesize).Take(pagesize).ToList();
            if (list.Count == 0) return list;
            foreach (NoticeReceiver item in list)
            {
                loadForeignKey(item);
            }
            return list;
        }

        public int getReceiveCount(int userId)
        {
            if (userId == 0) throw new ApplicationException("用户id不对");
            return database.NoticeReceiver.Count(t => t.Receiver == userId);
        }

        public override void loadForeignKey(NoticeReceiver entity)
        {
            if (entity == null) return;
            Notice notice = database.Notice.Find(entity.NoticeId);
            if (notice == null) return;
            entity.theNotice = notice;
            entity.Title = notice.Title;
            entity.CreateTime = notice.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
            entity.StatusName = entity.Status == NoticeStatus.Unread ? "未读" : "已读";
            User sender = database.User.Find(notice.Sender);
            if (sender == null) return;
            County department = database.County.Find(sender.CountyId);
            if (department == null) return;
            entity.Sender = department.Name + "信访局";
        }

        public void setToRead(int id)
        {
            NoticeReceiver item = database.NoticeReceiver.Find(id);
            if (item == null) return;
            if (item.Status == NoticeStatus.Read) return;
            item.Status = NoticeStatus.Read;
            database.SaveChanges();
        }
    }
}
