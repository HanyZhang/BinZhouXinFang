using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using DT.Xamarin.Agora;
using XinFangAndroid.Shared;

namespace XinFangAndroid
{
    [Activity(Label = "@string/app_name", ScreenOrientation = ScreenOrientation.Portrait)]
    public class JoinActivity : AppCompatActivity,ActivityCompat.IOnRequestPermissionsResultCallback
    {
        protected const int REQUEST_ID = 1;
        protected string[] REQUEST_PERMISSIONS = new string[] {
            Manifest.Permission.Camera,
            Manifest.Permission.WriteExternalStorage,
            Manifest.Permission.RecordAudio,
            Manifest.Permission.ModifyAudioSettings,
            Manifest.Permission.Internet,
            Manifest.Permission.AccessNetworkState
        };

        protected RtcEngine AgoraEngine;
        protected AgoraQualityHandler AgoraHandler;
        protected const string QualityFormat = "Current Connection - {0}";
        protected const string VersionFormat = " {0}";
        private View _layout;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.join);
            _layout = FindViewById(Resource.Id.joinLayout);
            CheckPermissions();//判断应用授权
            FindViewById<EditText>(Resource.Id.channelName).Text = AgoraSettings.Current.RoomName;
            FindViewById<EditText>(Resource.Id.encryptionKey).Text = AgoraSettings.Current.EncryptionPhrase;
            AgoraHandler = new AgoraQualityHandler(this);
            AgoraEngine = RtcEngine.Create(BaseContext, AgoraTestConstants.AgoraAPI, AgoraHandler);
            AgoraEngine.EnableWebSdkInteroperability(true);
            AgoraEngine.EnableLastmileTest();
            FindViewById<TextView>(Resource.Id.agora_version_text).Text = string.Format(VersionFormat, RtcEngine.SdkVersion);
        }

        protected bool CheckPermissions(bool requestPermissions = true)
        {
            var isGranted = REQUEST_PERMISSIONS.Select(permission => ContextCompat.CheckSelfPermission(this, permission) == (int)Permission.Granted).All(granted => granted);//Permission.Granted 0 许可，-1 拒绝
            if (requestPermissions && !isGranted)
            {
                //向用户申请授权
                ActivityCompat.RequestPermissions(this, REQUEST_PERMISSIONS, REQUEST_ID);
            }
            return isGranted;
        }
        /*
        //申请授权回调函数，但是似乎没用
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if (requestCode == REQUEST_ID)
            {
                // Received permission result for camera permission.
                // Check if the only required permission has been granted
                var isNoGranted = grantResults.Any(t => t == Permission.Denied);//是否存在拒绝的权限
                if ((grantResults.Length == REQUEST_PERMISSIONS.Length) && !isNoGranted)
                {
                    OnJoin(new View(this));
                }
                else
                {
                    Snackbar.Make(_layout, Resource.String.permissions_not_granted, Snackbar.LengthShort).Show();
                }
            }
            else
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
        }
        */

        [Java.Interop.Export("OnJoin")]
        public void OnJoin(View v)
        {
            AgoraSettings.Current.RoomName = FindViewById<EditText>(Resource.Id.channelName).Text;
            AgoraSettings.Current.EncryptionPhrase = FindViewById<EditText>(Resource.Id.encryptionKey).Text;
            CheckPermissionsAndStartCall();
        }

        private void CheckPermissionsAndStartCall()
        {
            if (CheckPermissions(false))
            {
                StartActivity(typeof(RoomActivity));
            }
            else
            {
                Snackbar.Make(_layout, Resource.String.permissions_not_granted, Snackbar.LengthShort).Show();
            }
        }
        /*
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.menu_share)
            {
                ShareActivity();
                return true;
            }
            else if (item.ItemId == Resource.Id.menu_settings)
            {
                StartActivity(typeof(SettingsActivity));
                return true;
            }
            else
            {
                return true;
            }
            //switch (item.ItemId)
            //{
            //    case Resource.Id.menu_settings:
            //        StartActivity(typeof(SettingsActivity));
            //        break;
            //    case Resource.Id.menu_share:
            //        ShareActivity();
            //        break;
            //}
            //return true;
        }

        private void ShareActivity()
        {
            Intent sendIntent = new Intent();
            sendIntent.SetAction(Intent.ActionSend);
            sendIntent.PutExtra(Intent.ExtraText, AgoraTestConstants.ShareString);
            sendIntent.SetType("text/plain");
            StartActivity(sendIntent);
        }
        */
        /// <summary>
        /// 网络连接质量
        /// </summary>
        /// <param name="p0"></param>
        internal void OnLastmileQuality(int p0)
        {
            if(IsFinishing)
            {
                return;
            }
            RunOnUiThread(() =>
            {
                var textQuality = FindViewById<TextView>(Resource.Id.quality_text);
                var imageQuality = FindViewById<ImageView>(Resource.Id.quality_image);
                string quality = string.Empty;
                switch (p0)
                {
                    case Constants.QualityExcellent:
                        quality = "Excellent";
                        imageQuality.SetImageResource(Resource.Drawable.ic_connection_5);
                        break;
                    case Constants.QualityGood:
                        quality = "Good";
                        imageQuality.SetImageResource(Resource.Drawable.ic_connection_4);
                        break;
                    case Constants.QualityPoor:
                        quality = "Poor";
                        imageQuality.SetImageResource(Resource.Drawable.ic_connection_3);
                        break;
                    case Constants.QualityBad:
                        quality = "Bad";
                        imageQuality.SetImageResource(Resource.Drawable.ic_connection_2);
                        break;
                    case Constants.QualityVbad:
                        quality = "Very Bad";
                        imageQuality.SetImageResource(Resource.Drawable.ic_connection_1);
                        break;
                    case Constants.QualityDown:
                        quality = "Down";
                        imageQuality.SetImageResource(Resource.Drawable.ic_connection_0);
                        break;
                    default:
                        quality = "Unknown";
                        imageQuality.SetImageResource(Resource.Drawable.ic_connection_0);
                        break;
                }
                textQuality.Text = string.Format(QualityFormat, quality);
            });
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (AgoraEngine != null)
            {
                RtcEngine.Destroy();
                AgoraEngine.Dispose();
                AgoraEngine = null;
            }
            //if (AgoraHandler != null)
            //{
            //    AgoraHandler.Dispose();
            //    AgoraHandler = null;
            //}
            //if (AgoraEngine != null)
            //{
                
            //    AgoraEngine.Dispose();
            //    AgoraEngine = null;
            //}
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(CurrentFocus.WindowToken, 0);
            return base.OnTouchEvent(e);
        }
    }
}