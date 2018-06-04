using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RestSharp;
using XinFangAndroid.Adapter;
using XinFangAndroid.Common;

namespace XinFangAndroid
{
    [Activity(Label = "NoticeListActivity")]
    public class NoticeListActivity : Activity
    {
        int count = 1;
        private List<Notice> data;
        private Context context;
        private NoticeAdapter adapter;
        private ListView lv_test;
        private string userId;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.notice_list_layout);
            userId= UserPreferences.GetString("LoginToken", "userId");
            if (string.IsNullOrEmpty(userId))
            {
                Intent intent = new Intent(this, typeof(LoginActivity));
                StartActivity(intent);
                return;
            }
            string serverUrl=this.GetString(Resource.String.serverUrl);
            var client = new RestClient(serverUrl+ "/API/getNoticeList");//创建客户端请求  
            var request = new RestRequest(Method.POST);
            request.AddParameter("pagenum", "1");
            request.AddParameter("pagesize", "10");
            request.AddParameter("userId", userId);
            request.AddParameter("type",1);

            RestResponse response = (RestResponse)client.Execute(request);  
            IRestResponse<MyJsonResult> responseJson = client.Deserialize<MyJsonResult>(response); //这两句获取json字符串,并反序列化  
            MyJsonResult result = responseJson.Data;
            if(!result.success)
            {
                Toast.MakeText(this, "请求失败"+result.message, ToastLength.Short).Show();
                return;
            }
            data =(List<Notice>)result.data;
            adapter = new NoticeAdapter(data, this);

            lv_test = FindViewById<ListView>(Resource.Id.notice_list);
            
            lv_test.Adapter = adapter;


            lv_test.ItemClick += (s, e) =>
            {
                OnClick(e.Position);
            };
        }
        
        private async System.Threading.Tasks.Task<List<Notice>> GetNoticesAsync(int pagenum, int pagesize, string userId, string type)
        {
            string url = "/API/getNoticeList";
            IDictionary<string, string> routeParames = new Dictionary<string, string>();
            routeParames.Add("pagenum", pagenum.ToString());
            routeParames.Add("pagesize", pagesize.ToString());
            routeParames.Add("userId", userId);
            routeParames.Add("type", type);
            var result = await WebRequest.SendPostRequestBasedOnHttpClient(url, routeParames);
            var data = (JsonObject)result;
            if (!(bool)data["success"])
            {
                return null;
            }
            else
            {
                List<Notice> list = new List<Notice>();
                object noticeDatas = data["data"];
                return list;
            }
        }
        public void OnClick(int position)
        {
            /*
            position--;
            Toast.MakeText(this, $"这条新闻有" + data[position].Pv + "次浏览量", ToastLength.Short).Show();
            */
        }
    }
}