using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PetitionLetter.Dll.Util
{
    public class StringUtil
    {
        public static List<string> ImageSplit(string imageStr)
        {
            List<string> result = new List<string>();
            if (string.IsNullOrEmpty(imageStr)) return result;
            string[] imageArr = imageStr.Split('|');
            if (imageArr.Length == 0) return result;
            
            foreach (var item in imageArr)
            {
                result.Add(item);
            }
            return result;
        }

        public static string DayOfWeek(DayOfWeek day)
        {
            switch (day) {
                case System.DayOfWeek.Sunday: return "星期日";
                case System.DayOfWeek.Monday: return "星期一";
                case System.DayOfWeek.Tuesday: return "星期二";
                case System.DayOfWeek.Wednesday: return "星期三";
                case System.DayOfWeek.Thursday: return "星期四";
                case System.DayOfWeek.Friday: return "星期五";
                case System.DayOfWeek.Saturday: return "星期六";
                default: throw new ApplicationException("error enum value");
            }
        }
    }
}
