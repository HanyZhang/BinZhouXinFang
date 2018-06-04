using PetitionLetter.Web.WebCode;
using System.Web;
using System.Web.Mvc;

namespace PetitionLetter.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ActionFilters());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
