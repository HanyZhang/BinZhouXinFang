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
    public class UserDao : GenericDaoBase<PetitionLetterEntities, User>
    {
        private string InitialPassword = "123456";
        protected override IOrderedQueryable<User> defaultOrderQuery()
        {
            return database.User.OrderBy(t => t.Id);
        }

        public User login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new ApplicationException("用户名和密码不能为空");
            User user = database.User.Where(t => t.LoginCode == username && t.Password == password).FirstOrDefault();
            if (user != null)
                loadForeignKey(user);
            return user;
        }

        public override void loadForeignKey(User entity)
        {
            if (entity == null) return;
            var county = database.County.Find(entity.CountyId);
            if (county == null) return;
            entity.theCounty = county;
            entity.CountyName = county.Name;
        }

        public void generateUser()
        {
            var list = database.County.Where(t => t.Level == (int)Level.City || t.Level == (int)Level.County).ToList();
            foreach (var item in list)
            {
                User user = new User();
                user.LoginCode = item.Id;
                user.Password = MD5Util.EncryptWithMD5(InitialPassword);
                user.Role = Role.Admin;
                user.CountyId = item.Id;
                database.User.Add(user);
            }
            database.SaveChanges();
        }

        public void changePassword(int id, string oldPwd, string newPwd)
        {
            if (string.IsNullOrEmpty(oldPwd) || string.IsNullOrEmpty(newPwd)) return;
            User user = database.User.Find(id);
            if (user == null) return;
            if (user.Password.ToLower() != oldPwd.ToLower()) return;
            user.Password = newPwd;
            database.Entry(user).State = System.Data.Entity.EntityState.Modified;
            database.SaveChanges();
        }

        public List<User> getList(UserCondition condition, int pagesize, int pagenum, out int count)
        {
            IQueryable<User> q = defaultOrderQuery();
            if (condition != null)
            {
                if (!string.IsNullOrEmpty(condition.loginCode))
                    q = q.Where(t => t.LoginCode.Contains(condition.loginCode));
            }
            count = q.Count();
            var list = q.Skip((pagenum - 1) * pagesize).Take(pagesize).ToList();
            if (list.Count == 0) return list;
            foreach (var item in list)
            {
                loadForeignKey(item);
            }
            return list;
        }
        
        public bool resetPassword(int userId)
        {
            if (userId == 0) return false;
            User user = database.User.Find(userId);
            if (user == null) return false;
            user.Password = MD5Util.EncryptWithMD5(InitialPassword);
            database.Entry(user).State = System.Data.Entity.EntityState.Modified;
            database.SaveChanges();
            return true;
        }

        public List<User> getReadOrUnRead(int noticeId, int status)
        {
            List<User> result = new List<User>();
            if (noticeId == 0) return result;
            var model = database.NoticeReceiver.Where(t => t.NoticeId == noticeId && t.Status == status).Select(t => t.Receiver).ToList();
            if (model.Count == 0) return result;
            result = database.User.Where(t => model.Contains(t.Id)).ToList();
            if (result.Count == 0) return result;
            foreach (var item in result)
                loadForeignKey(item);
            return result;
        }

        public bool removeUser(int userId)
        {
            if (userId == 0) return false;
            User user = database.User.Find(userId);
            if (user == null) return false;

            var warning = database.Warning.Where(t => t.UserId == userId).ToList();
            if (warning.Count != 0)
                database.Warning.RemoveRange(warning);

            var noticeReceiver = database.NoticeReceiver.Where(t => t.Receiver == userId).ToList();
            if (noticeReceiver.Count != 0)
                database.NoticeReceiver.RemoveRange(noticeReceiver);

            var notice = database.Notice.Where(t => t.Sender == userId).ToList();
            if (notice.Count != 0)
            {
                var noticeId = notice.Select(t => t.Id).ToList();
                var q = database.NoticeReceiver.Where(t => noticeId.Contains(t.NoticeId)).ToList();
                if (q.Count != 0)
                    database.NoticeReceiver.RemoveRange(q);
                database.Notice.RemoveRange(notice);
            }
            database.SaveChanges();
            return true;
        }
    }

    public class UserCondition
    {
        public string loginCode { get; set; }
    }
}
