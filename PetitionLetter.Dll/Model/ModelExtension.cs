using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetitionLetter.Dll.Model
{
    public class ModelExtension
    {

    }

    public enum Level { Illegal = 0, Province = 1, City = 2, County = 3, Town = 4, Village = 5 };
    public enum LeaderType { Illegal = 0, Leader = 1, Director = 2 };
    public enum PetitionChannel { Illegal = 0, Online = 1, Visit = 2, Letter = 3 };
    // 1：来市    2：到省    3：进京    4：非访
    public enum Visit { City = 1, Province = 2, Capital = 3, NoVisit = 4 };
    // 0:未处理   1:已批示    2:已转交   3:转交处理完成   4:已上报
    public enum WarningStatus { Untreated = 0, Instructed = 1, Transferred = 2, TransferCompleted = 3, Reported = 4 };
    public enum DailyStatus { NotReported = 0, Reported = 1 };

    public class Role
    {
        //todo: 人员角色需修改
        //系统管理员 信访办领导（市、县、镇） 信访办管理员（市、县、镇） 村信息员
        public static int Admin = 1;
        public static int Messager = 2;
    }

    public class NoticeStatus
    {
        public static int Unread = 0;
        public static int Read = 1;
    }

    public partial class User
    {
        public County theCounty { get; set; }
        public string CountyName { get; set; }
    }

    public partial class Notice
    {
        public string SenderName { get; set; }
        public string CreateTimeString { get; set; }
        public int Read { get; set; }
        public int Unread { get; set; }
    }

    public partial class NoticeReceiver
    {
        public string StatusName { get; set; }
        public string Title { get; set; }
        public string CreateTime { get; set; }
        public Notice theNotice { get; set; }
        public string Sender { get; set; }
    }

    public class ChartData
    {
        public int value { get; set; }
        public string name { get; set; }
    }

    public class StatisticsType
    {
        public static int County = 1;
        public static int Category = 2;
    }

    public class Coordinate
    {
        public string lng { get; set; }
        public string lat { get; set; }
    }

    public partial class Warning
    {
        public string Uploader { get; set; }
    }

    public partial class Ledger
    {
        public string DateString { get; set; }
        public string LeaderStr { get; set; }
        public string DirectorStr { get; set; }
    }

    public partial class Petition
    {
        public string DateString { get; set; }
        public string VisitString { get; set; }
    }

    public class PetitionStatistics
    {
        public int NoVisit_Qi { get; set; }
        public int NoVisit_Ren { get; set; }
        public int Capital_Qi { get; set; }
        public int Capital_Ren { get; set; }
        public int Province_Qi { get; set; }
        public int Province_Ren { get; set; }
        public int City_Qi { get; set; }
        public int City_Ren { get; set; }
        public string CountyName { get; set; }
        public string CountyId { get; set; }
    }

    public class DailyPetition
    {
        public string PetitionDate { get; set; }
        public int Status { get; set; }
        public string DayOfWeek { get; set; }
    }
}
