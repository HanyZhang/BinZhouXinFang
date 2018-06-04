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

namespace XinFangAndroid.Shared
{
    public enum EncryptionType
    {
        xts128 = 0,// = "aes-128-xts",
        xts256 = 1// = "aes-256-xts"
    }
}
