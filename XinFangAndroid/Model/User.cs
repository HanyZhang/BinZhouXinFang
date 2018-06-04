using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace XinFangAndroid.Model
{
    public partial class User
    {
        public int Id { get; set; }
        public string LoginCode { get; set; }
        public string Password { get; set; }
        public int Role { get; set; }
        public string CountyId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
    }
}