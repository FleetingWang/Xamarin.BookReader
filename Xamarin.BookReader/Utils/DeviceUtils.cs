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
using Android.Telephony;
using Android.Util;
using Android.Annotation;
using Android.Net.Wifi;
using Android.Text.Format;

namespace Xamarin.BookReader.Utils
{
    public class DeviceUtils
    {
        private const string TAG = "DeviceUtils"; // DeviceUtils.class.getSimpleName();
        private const string CMCC_ISP = "46000";//中国移动
        private const string CMCC2_ISP = "46002";//中国移动
        private const string CU_ISP = "46001";//中国联通
        private const string CT_ISP = "46003";//中国电信

        /**
         * 获取设备的系统版本号
         */
        public static int getDeviceSDK()
        {
            int sdk = (int)Build.VERSION.SdkInt;
            return sdk;
        }

        /**
         * 获取设备的型号
         */
        public static string getDeviceName()
        {
            string model = Build.Model;
            return model;
        }

        public static string getIMSI(Context context)
        {
            TelephonyManager telephonyManager = (TelephonyManager)context.GetSystemService(Context.TelephonyService);
            string IMSI = telephonyManager.SubscriberId;
            return IMSI;
        }

        public static string getIMEI(Context context)
        {
            TelephonyManager telephonyManager = (TelephonyManager)context.GetSystemService(Context.TelephonyService);
            string IMEI = telephonyManager.DeviceId;
            return IMEI;
        }

        /**
         * 获取手机网络运营商类型
         *
         * @param context
         * @return
         */
        public static string getPhoneISP(Context context)
        {
            if (context == null)
            {
                return "";
            }
            TelephonyManager manager = (TelephonyManager)context.GetSystemService(Context.TelephonyService);
            string teleCompany = "";
            string np = manager.NetworkOperator;

            if (np != null)
            {
                if (np.Equals(CMCC_ISP) || np.Equals(CMCC2_ISP))
                {
                    teleCompany = "中国移动";
                }
                else if (np.StartsWith(CU_ISP))
                {
                    teleCompany = "中国联通";
                }
                else if (np.StartsWith(CT_ISP))
                {
                    teleCompany = "中国电信";
                }
            }
            return teleCompany;
        }

        /**
         * 获取屏幕信息
         *
         * @param context
         * @return
         */
        public static DisplayMetrics getDisplayMetrics(Context context)
        {
            DisplayMetrics dm = context.Resources.DisplayMetrics;
            return dm;
        }

        /**
         * 获取/打印屏幕信息
         *
         * @param context
         * @return
         */
        public static DisplayMetrics printDisplayInfo(Context context)
        {
            DisplayMetrics dm = getDisplayMetrics(context);
            StringBuilder sb = new StringBuilder();
            sb.Append("\ndensity         :").Append(dm.Density);
            sb.Append("\ndensityDpi      :").Append(dm.DensityDpi);
            sb.Append("\nheightPixels    :").Append(dm.HeightPixels);
            sb.Append("\nwidthPixels     :").Append(dm.WidthPixels);
            sb.Append("\nscaledDensity   :").Append(dm.ScaledDensity);
            sb.Append("\nxdpi            :").Append(dm.Xdpi);
            sb.Append("\nydpi            :").Append(dm.Ydpi);
            LogUtils.i(TAG, sb.ToString());
            return dm;
        }

        /**
         * 获取系统当前可用内存大小
         *
         * @param context
         * @return
         */
        //@(Build.VERSION_CODES.CUPCAKE)
        [TargetApi(Value = (int)BuildVersionCodes.Cupcake)]
        public static string getAvailMemory(Context context)
        {
            ActivityManager am = (ActivityManager)context.GetSystemService(Context.ActivityService);
            ActivityManager.MemoryInfo mi = new ActivityManager.MemoryInfo();
            am.GetMemoryInfo(mi);
            return Formatter.FormatFileSize(context, mi.AvailMem);// 将获取的内存大小规格化
        }


        /**
         * 获取 MAC 地址
         * 须配置android.permission.ACCESS_WIFI_STATE权限
         */
        public static string getMacAddress(Context context)
        {
            //wifi mac地址
            WifiManager wifi = (WifiManager)context.GetSystemService(Context.WifiService);
            WifiInfo info = wifi.ConnectionInfo;
            string mac = info.MacAddress;
            LogUtils.i(TAG, " MAC：" + mac);
            return mac;
        }

        /**
         * 获取 开机时间
         */
        public static string getBootTimeString()
        {
            long ut = SystemClock.ElapsedRealtime() / 1000;
            int h = (int)((ut / 3600));
            int m = (int)((ut / 60) % 60);
            LogUtils.i(TAG, h + ":" + m);
            return h + ":" + m;
        }
    }
}