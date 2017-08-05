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
using Android.Annotation;
using Android.Support.V4.Content;
using Android.Graphics;
using AndroidResource = Android.Resource;

namespace Xamarin.BookReader.Utils
{
    public class StatusBarCompat
    {
        private static int INVALID_VAL = -1;

        [TargetApi(Value = (int)BuildVersionCodes.Lollipop)]
        public static View compat(Activity activity, int statusColor)
        {
            int color = ContextCompat.GetColor(activity, Resource.Color.colorPrimaryDark);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                if (statusColor != INVALID_VAL)
                {
                    color = statusColor;
                }
                activity.Window.SetStatusBarColor(new Color(color));
                return null;
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat
                    && Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
            {
                ViewGroup contentView = (ViewGroup)activity.FindViewById(AndroidResource.Id.Content);
                if (statusColor != INVALID_VAL)
                {
                    color = statusColor;
                }
                View statusBarView = contentView.GetChildAt(0);
                if (statusBarView != null && statusBarView.MeasuredHeight == getStatusBarHeight(activity))
                {
                    statusBarView.SetBackgroundColor(new Color(color));
                    return statusBarView;
                }
                statusBarView = new View(activity);
                ViewGroup.LayoutParams lp = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                        getStatusBarHeight(activity));
                statusBarView.SetBackgroundColor(new Color(color));
                contentView.AddView(statusBarView, lp);
                return statusBarView;
            }
            return null;

        }

        public static void compat(Activity activity)
        {
            compat(activity, INVALID_VAL);
        }


        public static int getStatusBarHeight(Context context)
        {
            int result = 0;
            int resourceId = context.Resources.GetIdentifier("status_bar_height", "dimen", "android");
            if (resourceId > 0)
            {
                result = context.Resources.GetDimensionPixelSize(resourceId);
            }
            return result;
        }
    }
}