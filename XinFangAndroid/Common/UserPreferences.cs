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

namespace XinFangAndroid.Common
{
    public static class UserPreferences
    {
        public static void DeleteString(string preName, string key)
        {
            var prefs = Application.Context.GetSharedPreferences(preName, FileCreationMode.Private);
            prefs.Edit().Remove(key).Commit();
        }

        public static string GetString(string preName, string key)
        {
            var prefs = Application.Context.GetSharedPreferences(preName, FileCreationMode.Private);
            return prefs.GetString(key, "");
        }

        public static void SetString(string preName, string key, string value)
        {
            var prefs = Application.Context.GetSharedPreferences(preName, FileCreationMode.Private);
            var prefsEditor = prefs.Edit();

            prefsEditor.PutString(key, value);
            prefsEditor.Commit();
        }
    }
}