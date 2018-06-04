using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinYuan.Lib;
using PetitionLetter.Dll.Model;

namespace PetitionLetter.Dll.Dao
{
    public class NoticeDao : GenericDaoBase<PetitionLetterEntities, Notice>
    {
        protected override IOrderedQueryable<Notice> defaultOrderQuery()
        {
            return database.Notice.OrderByDescending(t => t.Id);
        }

        public void Release(Notice notice)
        {
            if (notice == null) throw new ApplicationException("通知为空");
            database.Notice.Add(notice);
            database.SaveChanges();
            User user = database.User.Where(t => t.Id == notice.Sender).FirstOrDefault();
            if (user == null) throw new ApplicationException("用户为空");
            List<string> list = database.County.Where(t => t.ParentId == user.CountyId).Select(t => t.Id).ToList();
            if (list == null || list.Count == 0) return;
            var receivers = database.User.Where(t => list.Contains(t.CountyId)).ToList();
            foreach (var item in receivers)
            {
                NoticeReceiver detail = new NoticeReceiver();
                detail.NoticeId = notice.Id;
                detail.Receiver = item.Id;
                database.NoticeReceiver.Add(detail);
            }
            database.SaveChanges();
        }

        public override void loadForeignKey(Notice entity)
        {
            if (entity == null) return;
            entity.CreateTimeString = entity.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
            var sender = database.User.Where(t => t.Id == entity.Sender).FirstOrDefault();
            if (sender == null) return;
            entity.SenderName = database.County.Where(t => t.Id == sender.CountyId).Select(t => t.Name).FirstOrDefault() + "信访办";
            entity.Read = database.NoticeReceiver.Count(t => t.NoticeId == entity.Id && t.Status == NoticeStatus.Read);
            entity.Unread = database.NoticeReceiver.Count(t => t.NoticeId == entity.Id && t.Status == NoticeStatus.Unread);
        }
        
        public List<Notice> getRelease(int userId, int pagenum, int pagesize, out int count, NoticeCondition condition)
        {
            if (userId == 0) throw new ApplicationException("用户ID不对");
            IQueryable<Notice> q = defaultOrderQuery();
            if (condition != null)
            {
                if (condition.StartTime != null) q = q.Where(t => t.CreateTime >= condition.StartTime);
                if (condition.EndTime != null) q = q.Where(t => t.CreateTime <= condition.EndTime.Value.AddDays(1));
                if (!string.IsNullOrEmpty(condition.KeyWord)) q = q.Where(t => t.Title.Contains(condition.KeyWord) || t.Content.Contains(condition.KeyWord));
            }
            q = q.Where(t => t.Sender == userId);
            count = q.Count();
            List<Notice> list = q.Skip((pagenum - 1) * pagesize).Take(pagesize).ToList();
            //List<Notice> list = database.Notice.Where(t => t.Sender == userId).OrderBy(t => t.Id).Skip((pagenum - 1) * pagesize).Take(pagesize).ToList();
            if (list.Count == 0) return list;
            foreach (var item in list)
            {
                loadForeignKey(item);
            }
            return list;
        }

        //public List<Notice> getReceive(int userId, int pagenum, int pagesize, NoticeCondition condition)
        //{
        //    if (userId == 0) throw new ApplicationException("用户ID不对");
        //    IQueryable<Notice> q = defaultOrderQuery();
        //    if (condition != null)
        //    {
        //        if (condition.StartTime != null) q = q.Where(t => t.CreateTime >= condition.StartTime);
        //        if (condition.EndTime != null) q = q.Where(t => t.CreateTime <= condition.EndTime.Value.AddDays(1));
        //        if (!string.IsNullOrEmpty(condition.KeyWord)) q = q.Where(t => t.Title.Contains(condition.KeyWord) || t.Content.Contains(condition.KeyWord));
        //    }
        //    List<int> notice = database.NoticeReceiver.Where(t => t.Receiver == userId).Select(t => t.NoticeId).Distinct().ToList();
        //    q = q.Where(t => notice.Contains(t.Id));
        //    List<Notice> list = q.Skip((pagenum - 1) * pagesize).Take(pagesize).ToList();
        //    if (list.Count == 0) return list;
        //    foreach (var item in list)
        //    {
        //        loadForeignKey(item);
        //    }
        //    return list;
        //}

        public int getReleaseCount(int userId)
        {
            if (userId == 0) throw new ApplicationException("用户ID不对");
            return database.Notice.Count(t => t.Sender == userId);
        }

    }

    public class NoticeCondition
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string KeyWord { get; set; }
        public int Status { get; set; }
    }
}
