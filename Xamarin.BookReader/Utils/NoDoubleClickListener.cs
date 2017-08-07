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
using Java.Util;

namespace Xamarin.BookReader.Utils
{
    /// <summary>
    /// 防止重复点击的点击监听
    /// </summary>
    public abstract class NoDoubleClickListener : Java.Lang.Object, View.IOnClickListener
    {
        public int MIN_CLICK_DELAY_TIME = 1000;
        private long lastClickTime = 0;

        public void OnClick(View v)
        {
            long currentTime = Calendar.Instance.TimeInMillis;
            if (currentTime - lastClickTime > MIN_CLICK_DELAY_TIME)
            {
                lastClickTime = currentTime;
                onNoDoubleClick(v);
            }
        }
        protected abstract void onNoDoubleClick(View view);
    }
}