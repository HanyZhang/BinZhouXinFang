using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace XinFangAndroid.Common
{
    public class CommonHelper
    {
        public static string EncryptWithMD5(string source)
        {
            if (string.IsNullOrEmpty(source))
                throw new ApplicationException("加密字符串不能为空");
            MD5 md5 = MD5.Create();
            byte[] result = md5.ComputeHash(Encoding.UTF8.GetBytes(source));
            StringBuilder strbul = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strbul.Append(result[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位

            }
            return strbul.ToString();
        }
    }
}