using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetitionLetter.Dll.Dao;
using PetitionLetter.Dll.Model;
using PetitionLetter.Dll.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetitionLetter.Test.Dao
{
    [TestClass]
    public class UserDaoTest
    {
        [TestMethod]
        public void addUser()
        {
            UserDao dao = new UserDao();
            dao.generateUser();
            dao.Dispose();
        }

        [TestMethod]
        public void test()
        {
            UserDao d = new UserDao();
            User u = d.get(79);
            d.Dispose();
            WarningDao dao = new WarningDao();
            //var q = dao.getWarningList(u);
            dao.Dispose();
        }
    }
}
