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

namespace XinFangAndroid.Adapter
{
    public class MyJsonResult
    {
        public string message { get; set; }
        public bool success { get; set; }
        public object data { get; set; }
    }
}