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
using Java.Lang;
using Android.Content.Res;

namespace Xamarin.BookReader.Utils
{
    public class AppUtils
    {
        private static Context mContext;
        private static Thread mUiThread;

        private static Handler sHandler = new Handler(Looper.MainLooper);


        public static void init(Context context)
        {
            mContext = context;
            mUiThread = Thread.CurrentThread();
        }

        public static Context getAppContext()
        {
            return mContext;
        }

        public static AssetManager getAssets()
        {
            return mContext.Assets;
        }

        public static Resources getResource()
        {
            return mContext.Resources;
        }

        public static bool isUIThread()
        {
            return Thread.CurrentThread() == mUiThread;
        }

        public static void runOnUI(Runnable r)
        {
            sHandler.Post(r);
        }

        public static void runOnUIDelayed(Runnable r, long delayMills)
        {
            sHandler.PostDelayed(r, delayMills);
        }

        public static void removeRunnable(Runnable r)
        {
            if (r == null)
            {
                sHandler.RemoveCallbacksAndMessages(null);
            }
            else
            {
                sHandler.RemoveCallbacks(r);
            }
        }
    }
}