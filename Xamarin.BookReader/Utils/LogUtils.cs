using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Text;
using Java.Lang;
using Java.IO;
using Android.Util;
using Java.Util;

namespace Xamarin.BookReader.Utils
{
    public class LogUtils
    {
        private static bool LOG_SWITCH = true; // 日志文件总开关
    private static bool LOG_TO_FILE = false; // 日志写入文件开关
    private static string LOG_TAG = "BookReader"; // 默认的tag
    private static char LOG_TYPE = 'v';// 输入日志类型，v代表输出所有信息,w则只输出警告...
    private static int LOG_SAVE_DAYS = 7;// sd卡中日志文件的最多保存天数

    private static SimpleDateFormat LOG_FORMAT = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");// 日志的输出格式
    private static SimpleDateFormat FILE_SUFFIX = new SimpleDateFormat("yyyy-MM-dd");// 日志文件格式
    private static string LOG_FILE_PATH; // 日志文件保存路径
    private static string LOG_FILE_NAME;// 日志文件保存名称

    public static void init(Context context) { // 在Application中初始化
        LOG_FILE_PATH = Environment.ExternalStorageDirectory.Path + File.Separator + ReaderApplication.getsInstance().PackageName;
        LOG_FILE_NAME = "Log";
    }

    /****************************
     * Warn
     *********************************/
    public static void w(Object msg) {
        w(LOG_TAG, msg);
    }

    public static void w(string tag, Object msg) {
        w(tag, msg, null);
    }

    public static void w(string tag, Object msg, Throwable tr) {
        log(tag, msg.ToString(), tr, 'w');
    }

    /***************************
     * Error
     ********************************/
    public static void e(Object msg) {
        e(LOG_TAG, msg);
    }

    public static void e(string tag, Object msg) {
        e(tag, msg, null);
    }

    public static void e(string tag, Object msg, Throwable tr) {
        log(tag, msg.ToString(), tr, 'e');
    }
    public static void e(Exception msg) {
        e(LOG_TAG, msg);
    }

    public static void e(string tag, Exception msg) {
        e(tag, msg, null);
    }

    public static void e(string tag, Exception msg, Throwable tr) {
        log(tag, msg.ToString(), tr, 'e');
    }

    /***************************
     * Debug
     ********************************/
    public static void d(Object msg) {
        d(LOG_TAG, msg);
    }

    public static void d(string tag, Object msg) {// 调试信息
        d(tag, msg, null);
    }

    public static void d(string tag, Object msg, Throwable tr) {
        log(tag, msg.ToString(), tr, 'd');
    }

    /****************************
     * Info
     *********************************/
    public static void i(Object msg) {
        i(LOG_TAG, msg);
    }

    public static void i(string tag, Object msg) {
        i(tag, msg, null);
    }

    public static void i(string tag, Object msg, Throwable tr) {
        log(tag, msg.ToString(), tr, 'i');
    }

    /**************************
     * Verbose
     ********************************/
    public static void v(Object msg) {
        v(LOG_TAG, msg);
    }

    public static void v(string tag, Object msg) {
        v(tag, msg, null);
    }

    public static void v(string tag, Object msg, Throwable tr) {
        log(tag, msg.ToString(), tr, 'v');
    }

    /**
     * 根据tag, msg和等级，输出日志
     *
     * @param tag
     * @param msg
     * @param level
     */
    private static void log(string tag, string msg, Throwable tr, char level) {
        if (LOG_SWITCH) {
            if ('e' == level && ('e' == LOG_TYPE || 'v' == LOG_TYPE)) { // 输出错误信息
                Log.Error(tag, createMessage(msg), tr);
            } else if ('w' == level && ('w' == LOG_TYPE || 'v' == LOG_TYPE)) {
                Log.Warn(tag, createMessage(msg), tr);
            } else if ('d' == level && ('d' == LOG_TYPE || 'v' == LOG_TYPE)) {
                Log.Debug(tag, createMessage(msg), tr);
            } else if ('i' == level && ('d' == LOG_TYPE || 'v' == LOG_TYPE)) {
                Log.Info(tag, createMessage(msg), tr);
            } else {
                Log.Verbose(tag, createMessage(msg), tr);
            }
            if (LOG_TO_FILE)
                log2File(String.ValueOf(level), tag, msg + tr == null ? "" : "\n" + Log.GetStackTraceString(tr));
        }
    }

    private static string getFunctionName() {
        StackTraceElement[] sts = Thread.CurrentThread().GetStackTrace();
        if (sts == null) {
            return null;
        }
        foreach (StackTraceElement st in sts) {
            if (st.IsNativeMethod) {
                continue;
            }
            if (st.ClassName.Equals(Class.FromType(typeof(Thread)).Name)) {
                continue;
            }
            if (st.FileName.Equals("LogUtils.java")) {
                continue;
            }
            return "[" + Thread.CurrentThread().Name + "("
                    + Thread.CurrentThread().Id + "): " + st.FileName
                    + ":" + st.LineNumber + "]";
        }
        return null;
    }

    private static string createMessage(string msg) {
        string functionName = getFunctionName();
        string message = (functionName == null ? msg
                : (functionName + " - " + msg));
        return message;
    }

    /**
     * 打开日志文件并写入日志
     *
     * @return
     **/
    private /*synchronized*/ static void log2File(string mylogtype, string tag, string text) {
        Date nowtime = new Date();
        string date = FILE_SUFFIX.Format(nowtime);
        string dateLogContent = LOG_FORMAT.Format(nowtime) + ":" + mylogtype + ":" + tag + ":" + text; // 日志输出格式
        File destDir = new File(LOG_FILE_PATH);
        if (!destDir.Exists()) {
            destDir.Mkdirs();
        }
        File file = new File(LOG_FILE_PATH, LOG_FILE_NAME + date);
        try {
            FileWriter filerWriter = new FileWriter(file, true);
            BufferedWriter bufWriter = new BufferedWriter(filerWriter);
            bufWriter.Write(dateLogContent);
            bufWriter.NewLine();
            bufWriter.Close();
            filerWriter.Close();
        } catch (IOException e) {
            e.PrintStackTrace();
        }
    }

    /**
     * 删除指定的日志文件
     */
    public static void delFile() {// 删除日志文件
        string needDelFiel = FILE_SUFFIX.Format(getDateBefore());
        File file = new File(LOG_FILE_PATH, needDelFiel + LOG_FILE_NAME);
        if (file.Exists()) {
            file.Delete();
        }
    }

    /**
     * 得到LOG_SAVE_DAYS天前的日期
     *
     * @return
     */
    private static Date getDateBefore() {
        Date nowtime = new Date();
        Calendar now = Calendar.Instance;
        now.Time = nowtime;
        now.Set(CalendarField.Date, now.Get(CalendarField.Date) - LOG_SAVE_DAYS);
        return now.Time;
    }
    }
}