using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using XinFangAndroid.Model;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using XinFangAndroid.Common;
using Newtonsoft.Json;
using System.IO;

namespace XinFangAndroid
{
    [Activity(Label = "LoginActivity")]
    public class LoginActivity : Activity
    {
        EditText userName, password;
        Button loginBtn;
        CheckBox rememberPass;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            ClearLoginToken("LoginToken");
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layout_login);
            userName = (EditText)FindViewById(Resource.Id.userNameET);
            password = (EditText)FindViewById(Resource.Id.passwordET);
            loginBtn = (Button)FindViewById(Resource.Id.loginBtn);
            rememberPass = (CheckBox)FindViewById(Resource.Id.remember_pass);
            string preRemeberPass = UserPreferences.GetString("MySharedPrefs", "rememberPass");
            if(!string.IsNullOrEmpty(preRemeberPass))
            {
                rememberPass.Checked = true;
                string userNameStr = UserPreferences.GetString("MySharedPrefs", "userName");
                string passwordStr = UserPreferences.GetString("MySharedPrefs", "password");
                userName.Text = userNameStr;
                password.Text = passwordStr;
            }
            loginBtn.Click += LoginBtn_Click;
            // Create your application here
        }
        private void ClearLoginToken(string perName)
        {
            UserPreferences.DeleteString(perName, "userId");
        }
        private void SetSharedPrefs()
        {
             if(rememberPass.Checked)
            {
                UserPreferences.SetString("MySharedPrefs", "rememberPass", "true");
                UserPreferences.SetString("MySharedPrefs", "userName", userName.Text);
                UserPreferences.SetString("MySharedPrefs", "password", password.Text);
            }
            else
            {
                UserPreferences.DeleteString("MySharedPrefs", "rememberPass");
                UserPreferences.DeleteString("MySharedPrefs", "userName");
                UserPreferences.DeleteString("MySharedPrefs", "password");
            }

        }
        private async void LoginBtn_Click(object sender, EventArgs e)
        {
            string userName = this.userName.Text;
            string password = this.password.Text;
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                Toast.MakeText(this, "用户名和密码必填", ToastLength.Short).Show();
            }
            else
            {
                this.loginBtn.Text = "登录中...";
                this.loginBtn.Enabled = false;
                string url = "/API/Login";
                IDictionary<string, string> routeParames = new Dictionary<string, string>();
                routeParames.Add("username", userName);
                routeParames.Add("password", CommonHelper.EncryptWithMD5(password));
                var result = await WebRequest.SendPostRequestBasedOnHttpClient(url, routeParames);
                var data = (JsonObject)result;
                if((bool)data["success"])
                {
                    SetSharedPrefs();
                    Toast.MakeText(this, "登录成功", ToastLength.Short).Show();
                    object userId = data["data"];
                    UserPreferences.SetString("LoginToken", "userId", userId.ToString());
                    Intent intent = new Intent(this, typeof(MainActivity));
                    StartActivity(intent);//打开主页面
                    this.Finish();//登录页面结束
                }
                else
                {
                    Toast.MakeText(this, "用户名或密码错误", ToastLength.Short).Show();
                    this.loginBtn.Text = "登录";
                    this.loginBtn.Enabled = true;
                }
                
            }
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