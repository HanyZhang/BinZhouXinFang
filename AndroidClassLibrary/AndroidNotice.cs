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

namespace AndroidClassLibrary
{
    public class AndroidNotice
    {
        public int Id { get; set; }
        public int Sender { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public System.DateTime CreateTime { get; set; }
        public AndroidNotice(string title, string content, DateTime createTime)
        {
            this.Title = title;
            this.Content = content;
            this.CreateTime = createTime;
        }
    }
    public class NoticeAdapter : BaseAdapter
    {
        private List<AndroidNotice> list;
        private Context context;
        public override int Count
        {
            get
            {
                return list.Count;
            }
        }

        public NoticeAdapter(List<AndroidNotice> list, Context context)
        {
            this.list = list;
            this.context = context;
        }


        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            convertView = LayoutInflater.From(context).Inflate(Resource.Layout.notice_list, parent, false);
            TextView title = convertView.FindViewById<TextView>(Resource.Id.tv_title);
            TextView pv = convertView.FindViewById<TextView>(Resource.Id.tv_pv);
            pv.Text = list[position].CreateTime.ToString("MM-dd");
            title.Text = list[position].Title;
            return convertView;

        }
    }
}