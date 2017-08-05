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

namespace Xamarin.BookReader.Utils
{
    public class ToastUtils
    {
        private static Toast mToast;
        private static Context context = AppUtils.getAppContext();

        /********************** 非连续弹出的Toast ***********************/
        public static void showSingleToast(int resId)
        { //R.string.**
            getSingleToast(resId, ToastLength.Short).Show();
        }

        public static void showSingleToast(String text)
        {
            getSingleToast(text, ToastLength.Short).Show();
        }

        public static void showSingleLongToast(int resId)
        {
            getSingleToast(resId, ToastLength.Long).Show();
        }

        public static void showSingleLongToast(String text)
        {
            getSingleToast(text, ToastLength.Long).Show();
        }

        /*********************** 连续弹出的Toast ************************/
        public static void showToast(int resId)
        {
            getToast(resId, ToastLength.Short).Show();
        }

        public static void showToast(String text)
        {
            getToast(text, ToastLength.Short).Show();
        }

        public static void showLongToast(int resId)
        {
            getToast(resId, ToastLength.Long).Show();
        }

        public static void showLongToast(String text)
        {
            getToast(text, ToastLength.Long).Show();
        }

        public static Toast getSingleToast(int resId, ToastLength duration)
        { // 连续调用不会连续弹出，只是替换文本
            return getSingleToast(context.Resources.GetText(resId).ToString(), duration);
        }

        public static Toast getSingleToast(String text, ToastLength duration)
        {
            if (mToast == null)
            {
                mToast = Toast.MakeText(context, text, duration);
            }
            else
            {
                mToast.SetText(text);
            }
            return mToast;
        }

        public static Toast getToast(int resId, ToastLength duration)
        { // 连续调用会连续弹出
            return getToast(context.Resources.GetText(resId).ToString(), duration);
        }

        public static Toast getToast(String text, ToastLength duration)
        {
            return Toast.MakeText(context, text, duration);
        }
    }
}