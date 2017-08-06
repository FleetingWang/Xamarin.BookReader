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
using Android.Net;
using Android.Telephony;
using Android.Net.Wifi;
using Android.Provider;

namespace Xamarin.BookReader.Utils
{
    public static class NetworkUtils
    {
        public const int NETWORK_WIFI = 1;    // wifi network
        public const int NETWORK_4G = 4;    // "4G" networks
        public const int NETWORK_3G = 3;    // "3G" networks
        public const int NETWORK_2G = 2;    // "2G" networks
        public const int NETWORK_UNKNOWN = 5;    // unknown network
        public const int NETWORK_NO = -1;   // no network

        private const int NETWORK_TYPE_GSM = 16;
        private const int NETWORK_TYPE_TD_SCDMA = 17;
        private const int NETWORK_TYPE_IWLAN = 18;

        /**
         * 打开网络设置界面
         * <p>3.0以下打开设置界面</p>
         *
         * @param context 上下文
         */
        public static void openWirelessSettings(Context context)
        {
            if (Build.VERSION.SdkInt > BuildVersionCodes.GingerbreadMr1) //  10
            {
                context.StartActivity(new Intent(Settings.ActionSettings));
            }
            else
            {
                context.StartActivity(new Intent(Settings.ActionWirelessSettings));
            }
        }

        /**
         * 获取活动网络信息
         *
         * @param context 上下文
         * @return NetworkInfo
         */
        private static NetworkInfo getActiveNetworkInfo(Context context)
        {
            ConnectivityManager cm = (ConnectivityManager)context
                    .GetSystemService(Context.ConnectivityService);
            return cm.ActiveNetworkInfo;
        }

        /**
         * 判断网络是否可用
         * <p>需添加权限 {@code <uses-permission android:name="android.permission
         * .ACCESS_NETWORK_STATE"/>}</p>
         *
         * @param context 上下文
         * @return {@code true}: 可用<br>{@code false}: 不可用
         */
        public static bool isAvailable(Context context)
        {
            NetworkInfo info = getActiveNetworkInfo(context);
            return info != null && info.IsAvailable;
        }

        /**
         * 判断网络是否连接
         * <p>需添加权限 {@code <uses-permission android:name="android.permission
         * .ACCESS_NETWORK_STATE"/>}</p>
         *
         * @param context 上下文
         * @return {@code true}: 是<br>{@code false}: 否
         */
        public static bool isConnected(Context context)
        {
            NetworkInfo info = getActiveNetworkInfo(context);
            return info != null && info.IsConnected;
        }

        /**
         * 判断网络是否是4G
         * <p>需添加权限 {@code <uses-permission android:name="android.permission
         * .ACCESS_NETWORK_STATE"/>}</p>
         *
         * @param context 上下文
         * @return {@code true}: 是<br>{@code false}: 不是
         */
        public static bool is4G(Context context)
        {
            NetworkInfo info = getActiveNetworkInfo(context);
            TelephonyManager tm = TelephonyManager.FromContext(context);
            return info != null && info.IsAvailable && tm.NetworkType == NetworkType.Lte;
        }

        /**
         * 判断wifi是否连接状态
         * <p>需添加权限 {@code <uses-permission android:name="android.permission
         * .ACCESS_NETWORK_STATE"/>}</p>
         *
         * @param context 上下文
         * @return {@code true}: 连接<br>{@code false}: 未连接
         */
        public static bool isWifiConnected(Context context)
        {
            ConnectivityManager cm = (ConnectivityManager)context
                    .GetSystemService(Context.ConnectivityService);
            return cm != null && cm.ActiveNetworkInfo != null
                    && cm.ActiveNetworkInfo.Type == ConnectivityType.Wifi;
        }

        /**
         * 获取移动网络运营商名称
         * <p>如中国联通、中国移动、中国电信</p>
         *
         * @param context 上下文
         * @return 移动网络运营商名称
         */
        public static String getNetworkOperatorName(Context context)
        {
            TelephonyManager tm = (TelephonyManager)context
                    .GetSystemService(Context.TelephonyService);
            return tm != null ? tm.NetworkOperatorName : null;
        }

        /**
         * 获取移动终端类型
         *
         * @param context 上下文
         * @return 手机制式
         * <ul>
         * <li>{@link TelephonyManager#PHONE_TYPE_NONE } : 0 手机制式未知</li>
         * <li>{@link TelephonyManager#PHONE_TYPE_GSM  } : 1 手机制式为GSM，移动和联通</li>
         * <li>{@link TelephonyManager#PHONE_TYPE_CDMA } : 2 手机制式为CDMA，电信</li>
         * <li>{@link TelephonyManager#PHONE_TYPE_SIP  } : 3</li>
         * </ul>
         */
        public static int getPhoneType(Context context)
        {
            TelephonyManager tm = (TelephonyManager)context
                    .GetSystemService(Context.TelephonyService);
            return tm != null ? (int)tm.PhoneType : -1;
        }


        /**
         * 获取当前的网络类型(WIFI,2G,3G,4G)
         * <p>需添加权限 {@code <uses-permission android:name="android.permission
         * .ACCESS_NETWORK_STATE"/>}</p>
         *
         * @param context 上下文
         * @return 网络类型
         * <ul>
         * <li>{@link #NETWORK_WIFI   } = 1;</li>
         * <li>{@link #NETWORK_4G     } = 4;</li>
         * <li>{@link #NETWORK_3G     } = 3;</li>
         * <li>{@link #NETWORK_2G     } = 2;</li>
         * <li>{@link #NETWORK_UNKNOWN} = 5;</li>
         * <li>{@link #NETWORK_NO     } = -1;</li>
         * </ul>
         */
        public static int getNetWorkType(Context context)
        {
            int netType = NETWORK_NO;
            NetworkInfo info = getActiveNetworkInfo(context);
            if (info != null && info.IsAvailable)
            {

                if (info.Type == ConnectivityType.Wifi)
                {
                    netType = NETWORK_WIFI;
                }
                else if (info.Type == ConnectivityType.Mobile)
                {
                    TelephonyManager tm = TelephonyManager.FromContext(context);


                    switch (tm.NetworkType)
                    {

                        case NetworkType.Gsm:
                        case NetworkType.Gprs:
                        case NetworkType.Cdma:
                        case NetworkType.Edge:
                        case NetworkType.OneXrtt:
                        case NetworkType.Iden:
                            netType = NETWORK_2G;
                            break;

                        case NetworkType.TdScdma:
                        case NetworkType.EvdoA:
                        case NetworkType.Umts:
                        case NetworkType.Evdo0:
                        case NetworkType.Hsdpa:
                        case NetworkType.Hsupa:
                        case NetworkType.Hspa:
                        case NetworkType.EvdoB:
                        case NetworkType.Ehrpd:
                        case NetworkType.Hspap:
                            netType = NETWORK_3G;
                            break;

                        case NetworkType.Iwlan:
                        case NetworkType.Lte:
                            netType = NETWORK_4G;
                            break;
                        default:

                            String subtypeName = info.SubtypeName;
                            if (subtypeName.Equals("TD-SCDMA", StringComparison.InvariantCultureIgnoreCase)
                                    || subtypeName.Equals("WCDMA", StringComparison.InvariantCultureIgnoreCase)
                                    || subtypeName.Equals("CDMA2000", StringComparison.InvariantCultureIgnoreCase))
                            {
                                netType = NETWORK_3G;
                            }
                            else
                            {
                                netType = NETWORK_UNKNOWN;
                            }
                            break;
                    }
                }
                else
                {
                    netType = NETWORK_UNKNOWN;
                }
            }
            return netType;
        }

        /**
         * 获取当前的网络类型(WIFI,2G,3G,4G)
         * <p>依赖上面的方法</p>
         *
         * @param context 上下文
         * @return 网络类型名称
         * <ul>
         * <li>NETWORK_WIFI   </li>
         * <li>NETWORK_4G     </li>
         * <li>NETWORK_3G     </li>
         * <li>NETWORK_2G     </li>
         * <li>NETWORK_UNKNOWN</li>
         * <li>NETWORK_NO     </li>
         * </ul>
         */
        public static String getNetWorkTypeName(Context context)
        {
            switch (getNetWorkType(context))
            {
                case NETWORK_WIFI:
                    return "NETWORK_WIFI";
                case NETWORK_4G:
                    return "NETWORK_4G";
                case NETWORK_3G:
                    return "NETWORK_3G";
                case NETWORK_2G:
                    return "NETWORK_2G";
                case NETWORK_NO:
                    return "NETWORK_NO";
                default:
                    return "NETWORK_UNKNOWN";
            }
        }

        /**
         * 获取当前连接wifi的名称
         *
         * @return
         */
        public static String getConnectWifiSsid(Context context)
        {
            if (isWifiConnected(context))
            {
                WifiManager wifiManager = (WifiManager)context.GetSystemService(Context.WifiService);
                WifiInfo wifiInfo = wifiManager.ConnectionInfo;
                return wifiInfo.SSID;
            }
            return null;
        }

        /**
         * 获取当前连接wifi的名称
         *
         * @return
         */
        public static String getConnectWifiIp(Context context)
        {
            if (isWifiConnected(context))
            {
                WifiManager wifiManager = (WifiManager)context.GetSystemService(Context.WifiService);
                WifiInfo wifiInfo = wifiManager.ConnectionInfo;
                int ipAddress = wifiInfo.IpAddress;
                if (ipAddress == 0)
                {
                    return null;
                }
                return ((ipAddress & 0xff) + "." + (ipAddress >> 8 & 0xff) + "."
                        + (ipAddress >> 16 & 0xff) + "." + (ipAddress >> 24 & 0xff));
            }
            return null;
        }
    }
}