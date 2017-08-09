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
using Android.Views.InputMethods;

namespace Xamarin.BookReader.Utils
{
    /// <summary>
    /// 软键盘工具类
    /// </summary>
    [TargetApi(Value = (int)BuildVersionCodes.Cupcake)]
    public class IMEUtils
    {
        /**
     * 切换键盘显示/隐藏状态
     *
     * @param context
     */
        public static void toggleSoftInput(Context context)
        {
            InputMethodManager imm = (InputMethodManager)context.GetSystemService(Context.InputMethodService);
            imm.ToggleSoftInput(0, HideSoftInputFlags.NotAlways);
        }

        /**
         * 显示键盘
         *
         * @param view
         * @return
         */
        public static bool showSoftInput(View view)
        {
            InputMethodManager imm = (InputMethodManager)view.Context.GetSystemService(Context.InputMethodService);
            return imm.ShowSoftInput(view, ShowFlags.Forced);
        }

        public static bool showSoftInput(Activity activity)
        {
            View view = activity.CurrentFocus;
            if (view != null)
            {
                InputMethodManager imm = (InputMethodManager)view.Context.GetSystemService(
                        Context.InputMethodService);
                return imm.ShowSoftInput(view, ShowFlags.Forced);
            }
            return false;
        }

        /**
         * 隐藏键盘
         *
         * @param view
         * @return
         */
        public static bool hideSoftInput(View view)
        {
            InputMethodManager imm = (InputMethodManager)view.Context.GetSystemService(Context.InputMethodService);
            return imm.HideSoftInputFromWindow(view.WindowToken, 0);
        }

        public static bool hideSoftInput(Activity activity)
        {
            if (activity.CurrentFocus != null)
            {
                InputMethodManager imm = (InputMethodManager)activity.GetSystemService(Context.InputMethodService);
                return imm.HideSoftInputFromWindow(activity.CurrentFocus.WindowToken, 0);
            }
            return false;
        }

        /**
         * 判断键盘是否打开
         *
         * @param context
         * @return
         */
        public static bool isActive(Context context)
        {
            InputMethodManager imm = (InputMethodManager)context.GetSystemService(Context.InputMethodService);
            return imm.IsActive;
        }
    }
}