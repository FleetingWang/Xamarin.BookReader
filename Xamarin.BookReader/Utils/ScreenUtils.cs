using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using AndroidResource = Android.Resource;
using Java.Lang;
using Java.Lang.Reflect;
using Android.Content.Res;
using Orientation = Android.Content.Res.Orientation;
using Android.Animation;
using Android.Provider;

namespace Xamarin.BookReader.Utils
{
    /// <summary>
    /// 屏幕亮度工具类
    /// </summary>
    public class ScreenUtils
    {
        public enum EScreenDensity
        {
            XXHDPI,    //超高分辨率    1080×1920
            XHDPI,    //超高分辨率    720×1280
            HDPI,    //高分辨率         480×800
            MDPI,    //中分辨率         320×480
        }

        public static EScreenDensity getDisply(Context context)
        {
            EScreenDensity eScreenDensity;
            //初始化屏幕密度
            DisplayMetrics dm = context.ApplicationContext.Resources.DisplayMetrics;
            int densityDpi = (int)dm.DensityDpi;

            if (densityDpi <= 160)
            {
                eScreenDensity = EScreenDensity.MDPI;
            }
            else if (densityDpi <= 240)
            {
                eScreenDensity = EScreenDensity.HDPI;
            }
            else if (densityDpi < 400)
            {
                eScreenDensity = EScreenDensity.XHDPI;
            }
            else
            {
                eScreenDensity = EScreenDensity.XXHDPI;
            }
            return eScreenDensity;
        }

        /**
         * 获取屏幕宽度
         *
         * @return
         */
        public static int getScreenWidth()
        {
            return AppUtils.getAppContext().Resources.DisplayMetrics.WidthPixels;
        }

        /**
         * 获取屏幕高度
         *
         * @return
         */
        public static int getScreenHeight()
        {
            return AppUtils.getAppContext().Resources.DisplayMetrics.HeightPixels;
        }

        /**
         * 将dp转换成px
         *
         * @param dp
         * @return
         */
        public static float dpToPx(float dp)
        {
            return dp * AppUtils.getAppContext().Resources.DisplayMetrics.Density;
        }

        public static int dpToPxInt(float dp)
        {
            return (int)(dpToPx(dp) + 0.5f);
        }

        /**
         * 将px转换成dp
         *
         * @param px
         * @return
         */
        public static float pxToDp(float px)
        {
            return px / AppUtils.getAppContext().Resources.DisplayMetrics.Density;
        }

        public static int pxToDpInt(float px)
        {
            return (int)(pxToDp(px) + 0.5f);
        }

        /**
         * 将px值转换为sp值
         *
         * @param pxValue
         * @return
         */
        public static float pxToSp(float pxValue)
        {
            return pxValue / AppUtils.getAppContext().Resources.DisplayMetrics.ScaledDensity;
        }

        /**
         * 将sp值转换为px值
         *
         * @param spValue
         * @return
         */
        public static float spToPx(float spValue)
        {
            return spValue * AppUtils.getAppContext().Resources.DisplayMetrics.ScaledDensity;
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

        public static int getActionBarSize(Context context)
        {
            TypedValue tv = new TypedValue();
            if (context.Theme.ResolveAttribute(AndroidResource.Attribute.ActionBarSize, tv, true))
            {
                int actionBarHeight = TypedValue.ComplexToDimensionPixelSize(tv.Data, context.Resources.DisplayMetrics);
                return actionBarHeight;
            }
            return 0;
        }

        private int getStatusBarHeight()
        {
            Class c = null;
            Object obj = null;
            Field field = null;
            int x = 0, sbar = 0;
            try
            {
                c = Class.ForName("com.android.internal.R$dimen");
                obj = c.NewInstance();
                field = c.GetField("status_bar_height");
                x = Integer.ParseInt(field.Get(obj).ToString());
                sbar = AppUtils.getAppContext().Resources.GetDimensionPixelSize(x);
            }
            catch (Exception e1)
            {
                e1.PrintStackTrace();
            }
            return sbar;
        }

        /**
         * 当前是否是横屏
         *
         * @param context context
         * @return boolean
         */
        public static bool isLandscape(Context context)
        {
            return context.Resources.Configuration.Orientation == Orientation.Landscape;
        }

        /**
         * 当前是否是竖屏
         *
         * @param context context
         * @return boolean
         */
        public static bool isPortrait(Context context)
        {
            return context.Resources.Configuration.Orientation == Orientation.Portrait;
        }

        /**
         * 调整窗口的透明度  1.0f,0.5f 变暗
         *
         * @param from    from>=0&&from<=1.0f
         * @param to      to>=0&&to<=1.0f
         * @param context 当前的activity
         */
        public static void dimBackground(float from, float to, Activity context)
        {
            Window window = context.Window;
            ValueAnimator valueAnimator = ValueAnimator.OfFloat(from, to);
            valueAnimator.SetDuration(500);
            //valueAnimator.addUpdateListener(
            //        new ValueAnimator.AnimatorUpdateListener() {
            //            @Override
            //            public void onAnimationUpdate(ValueAnimator animation) {
            //                WindowManager.LayoutParams params = window.Attributes;
            //                params.alpha = (Float) animation.getAnimatedValue();
            //                window.setAttributes(params);
            //            }
            //        });
            valueAnimator.Start();
        }

        /**
         * 判断是否开启了自动亮度调节
         *
         * @param activity
         * @return
         */
        public static bool isAutoBrightness(Activity activity)
        {
            bool isAutoAdjustBright = false;
            try
            {
                isAutoAdjustBright = Settings.System.GetInt(
                        activity.ContentResolver,
                        Settings.System.ScreenBrightnessMode) == (int)ScreenBrightness.ModeAutomatic;
            }
            catch (Settings.SettingNotFoundException e)
            {
                e.PrintStackTrace();
            }
            return isAutoAdjustBright;
        }

        /**
         * 关闭亮度自动调节
         *
         * @param activity
         */
        public static void stopAutoBrightness(Activity activity)
        {
            Settings.System.PutInt(activity.ContentResolver,
                    Settings.System.ScreenBrightnessMode,
                    (int)ScreenBrightness.ModeManual);
        }

        /**
         * 开启亮度自动调节
         *
         * @param activity
         */

        public static void startAutoBrightness(Activity activity)
        {
            Settings.System.PutInt(activity.ContentResolver,
                    Settings.System.ScreenBrightnessMode,
                    (int)ScreenBrightness.ModeAutomatic);
        }

        /**
         * 获得当前屏幕亮度值
         *
         * @param mContext
         * @return 0~100
         */
        public static float getScreenBrightness(Context mContext)
        {
            int screenBrightness = 255;
            try
            {
                screenBrightness = Settings.System.GetInt(mContext.ContentResolver, Settings.System.ScreenBrightness);
            }
            catch (Exception e)
            {
                e.PrintStackTrace();
            }
            return screenBrightness / 255.0F * 100;
        }

        /**
         * 设置当前屏幕亮度值
         *
         * @param paramInt 0~100
         * @param mContext
         */
        public static void saveScreenBrightness(int paramInt, Context mContext)
        {
            if (paramInt <= 5)
            {
                paramInt = 5;
            }
            try
            {
                float f = paramInt / 100.0F * 255;
                Settings.System.PutInt(mContext.ContentResolver, Settings.System.ScreenBrightness, (int)f);
            }
            catch (Exception e)
            {
                e.PrintStackTrace();
            }
        }

        /**
         * 设置Activity的亮度
         *
         * @param paramInt
         * @param mActivity
         */
        public static void setScreenBrightness(int paramInt, Activity mActivity)
        {
            if (paramInt <= 5)
            {
                paramInt = 5;
            }
            Window localWindow = mActivity.Window;
            WindowManagerLayoutParams localLayoutParams = localWindow.Attributes;
            float f = paramInt / 100.0F;
            localLayoutParams.ScreenBrightness = f;
            localWindow.Attributes = localLayoutParams;
        }
    }
}