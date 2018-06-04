using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Content;
using Android.Util;
using XinFangAndroid.Common;
using Android.Runtime;

namespace XinFangAndroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        ImageButton today, important, work, remote, report, video, announce, publicity, contact;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            string userId = UserPreferences.GetString("LoginToken", "userId");
            if (string.IsNullOrEmpty(userId))
            {
                Intent intent = new Intent(this, typeof(LoginActivity));
                StartActivity(intent);
                return;
            }
            

            SetContentView(Resource.Layout.activity_main);
            announce = (ImageButton)FindViewById(Resource.Id.announce);//通知公告
            announce.Click += delegate
            {
                Intent intent = new Intent(this, typeof(NoticeListActivity));
                StartActivity(intent);

            };
            remote = (ImageButton)FindViewById(Resource.Id.remote);//视频
            remote.Click += delegate
            {
                Intent intent = new Intent(this, typeof(JoinActivity));
                StartActivity(intent);
                
            };


            //Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            //SetSupportActionBar(toolbar);

            //FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            //         fab.Click += FabOnClick;
            /*
            Button loginBtn = FindViewById<Button>(Resource.Id.loginBtn);
            loginBtn.Click +=delegate{
                Intent intent = new Intent(this, typeof(LoginActivity));
                //intent.PutExtra("name", "阳光数码");
                StartActivity(intent);
                Log.Debug("main", "转向login");
            };*/
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_logout)
            {
                //注销登录
                Context context = this;
                Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(context);
                builder.SetTitle("提示");
                builder.SetMessage("确认要注销登录吗？");
                builder.SetCancelable(true);
                builder.SetPositiveButton("确认", delegate
                {
                    Intent intent = new Intent(this, typeof(LoginActivity));
                    StartActivity(intent);//返回登录界面
                    this.Finish();//主界面结束
                });
                builder.SetNegativeButton("取消", delegate
                {
                    //Toast.MakeText(this, "点击了取消", ToastLength.Short).Show();
                });
                Android.App.AlertDialog dialog = builder.Create();
                dialog.Show();
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }
        
        DateTime? lastBackKeyDownTime;//记录上次按下Back的时间
        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back && e.Action == KeyEventActions.Down)//监听Back键
            {
                if (!lastBackKeyDownTime.HasValue || DateTime.Now - lastBackKeyDownTime.Value > new TimeSpan(0, 0, 2))
                {
                    Toast.MakeText(this, "再按一次退出程序", ToastLength.Short).Show();
                    lastBackKeyDownTime = DateTime.Now;
                }
                else
                {
                    Finish();
                }
                return true;
            }
            return base.OnKeyDown(keyCode, e);
        }
    }
}

