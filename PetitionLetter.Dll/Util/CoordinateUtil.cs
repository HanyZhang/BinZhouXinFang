using PetitionLetter.Dll.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetitionLetter.Dll.Util
{
    public class CoordinateUtil
    {
        public static string getCoordinateString(List<Coordinate> list)
        {
            string result = "";
            if (list == null || list.Count == 0) return result;
            foreach (var item in list)
            {
                result += item.lat + "," + item.lng + "|";
            }
            result = result.Remove(result.Length - 1);
            return result;
        }

        public static List<Coordinate> getCoordinateList(string coordinateStr)
        {
            List<Coordinate> result = new List<Coordinate>();
            if (string.IsNullOrEmpty(coordinateStr)) return result;
            string[] coordinates = coordinateStr.Split('|');
            if (coordinates.Length == 0) return result;
            foreach (string item in coordinates)
            {
                Coordinate model = new Coordinate();
                string[] coordinate = item.Split(',');
                if (coordinate.Length != 2) throw new ApplicationException("坐标格式不正确");
                model.lat = coordinate[0];
                model.lng = coordinate[1];
                result.Add(model);
            }
            return result;
        }
    }
}
