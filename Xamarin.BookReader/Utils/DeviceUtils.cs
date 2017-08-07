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
    public class DeviceUtils
    {
        private const string TAG = DeviceUtils.class.getSimpleName();
    private const string CMCC_ISP = "46000";//中国移动
    private const string CMCC2_ISP = "46002";//中国移动
    private const string CU_ISP = "46001";//中国联通
    private const string CT_ISP = "46003";//中国电信

    /**
     * 获取设备的系统版本号
     */
    public static int getDeviceSDK() {
        int sdk = Build.VERSION.SDK_INT;
        return sdk;
    }

    /**
     * 获取设备的型号
     */
    public static string getDeviceName() {
        string model = Build.MODEL;
        return model;
    }

    public static string getIMSI(Context context) {
        TelephonyManager telephonyManager = (TelephonyManager) context.getSystemService(Context.TELEPHONY_SERVICE);
        string IMSI = telephonyManager.getSubscriberId();
        return IMSI;
    }

    public static string getIMEI(Context context) {
        TelephonyManager telephonyManager = (TelephonyManager) context.getSystemService(Context.TELEPHONY_SERVICE);
        string IMEI = telephonyManager.getDeviceId();
        return IMEI;
    }

    /**
     * 获取手机网络运营商类型
     *
     * @param context
     * @return
     */
    public static string getPhoneISP(Context context) {
        if (context == null) {
            return "";
        }
        TelephonyManager manager = (TelephonyManager) context.getSystemService(Context.TELEPHONY_SERVICE);
        string teleCompany = "";
        string np = manager.getNetworkOperator();

        if (np != null) {
            if (np.equals(CMCC_ISP) || np.equals(CMCC2_ISP)) {
                teleCompany = "中国移动";
            } else if (np.startsWith(CU_ISP)) {
                teleCompany = "中国联通";
            } else if (np.startsWith(CT_ISP)) {
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
    public static DisplayMetrics getDisplayMetrics(Context context) {
        DisplayMetrics dm = context.getResources().getDisplayMetrics();
        return dm;
    }

    /**
     * 获取/打印屏幕信息
     *
     * @param context
     * @return
     */
    public static DisplayMetrics printDisplayInfo(Context context) {
        DisplayMetrics dm = getDisplayMetrics(context);
        StringBuilder sb = new StringBuilder();
        sb.append("\ndensity         :").append(dm.density);
        sb.append("\ndensityDpi      :").append(dm.densityDpi);
        sb.append("\nheightPixels    :").append(dm.heightPixels);
        sb.append("\nwidthPixels     :").append(dm.widthPixels);
        sb.append("\nscaledDensity   :").append(dm.scaledDensity);
        sb.append("\nxdpi            :").append(dm.xdpi);
        sb.append("\nydpi            :").append(dm.ydpi);
        LogUtils.i(TAG, sb.toString());
        return dm;
    }

    /**
     * 获取系统当前可用内存大小
     *
     * @param context
     * @return
     */
    @TargetApi(Build.VERSION_CODES.CUPCAKE)
    public static string getAvailMemory(Context context) {
        ActivityManager am = (ActivityManager) context.getSystemService(Context.ACTIVITY_SERVICE);
        ActivityManager.MemoryInfo mi = new ActivityManager.MemoryInfo();
        am.getMemoryInfo(mi);
        return Formatter.formatFileSize(context, mi.availMem);// 将获取的内存大小规格化
    }


    /**
     * 获取 MAC 地址
     * 须配置android.permission.ACCESS_WIFI_STATE权限
     */
    public static string getMacAddress(Context context) {
        //wifi mac地址
        WifiManager wifi = (WifiManager) context.getSystemService(Context.WIFI_SERVICE);
        WifiInfo info = wifi.getConnectionInfo();
        string mac = info.getMacAddress();
        LogUtils.i(TAG, " MAC：" + mac);
        return mac;
    }

    /**
     * 获取 开机时间
     */
    public static string getBootTimeString() {
        long ut = SystemClock.elapsedRealtime() / 1000;
        int h = (int) ((ut / 3600));
        int m = (int) ((ut / 60) % 60);
        LogUtils.i(TAG, h + ":" + m);
        return h + ":" + m;
    }
    }
}