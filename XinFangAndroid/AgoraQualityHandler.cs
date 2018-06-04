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
using DT.Xamarin.Agora;

namespace XinFangAndroid
{
    public class AgoraQualityHandler: IRtcEngineEventHandler
    {
        private JoinActivity _context;

        public AgoraQualityHandler(JoinActivity activity)
        {
            _context = activity;
        }

        public override void OnLastmileQuality(int p0)
        {
            _context.OnLastmileQuality(p0);
        }
    }
}