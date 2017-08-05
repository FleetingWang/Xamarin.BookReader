using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using static Java.Lang.Thread;
using Android.Content.PM;
using Java.Lang;
using Xamarin.BookReader.Utils;
using Process = Android.OS.Process;
using Java.IO;
using Java.Text;
using Java.Util;
using static Android.Content.PM.PackageManager;
using Java.Lang.Reflect;
using Xamarin.BookReader.Services;

namespace Xamarin.BookReader.Bases
{
    public class CrashHandler: Java.Lang.Object, IUncaughtExceptionHandler
    {
        //系统默认的UncaughtException处理类
    private IUncaughtExceptionHandler mDefaultHandler;
    //CrashHandler实例
    private static CrashHandler INSTANCE;
    //程序的Context对象
    private Context mContext;
    //用来存储设备信息和异常信息
    private Dictionary<string, string> infos = new Dictionary<string, string>();

    private CrashHandler() {
    }

    /**
     * 获取CrashHandler实例 ,单例模式
     */
    public static CrashHandler getInstance() {
        if (INSTANCE == null)
            INSTANCE = new CrashHandler();
        return INSTANCE;
    }

    /**
     * 初始化
     *
     * @param context
     */
    public void init(Context context) {
        mContext = context;
        //获取系统默认的UncaughtException处理器
        mDefaultHandler = Thread.DefaultUncaughtExceptionHandler;
        //设置该CrashHandler为程序的默认处理器
        Thread.DefaultUncaughtExceptionHandler = this;
    }

    /**
     * 当UncaughtException发生时会转入该函数来处理
     */
    public void UncaughtException(Thread thread, Throwable ex) {
        if (!handleException(ex) && mDefaultHandler != null) {
            //如果用户没有处理则让系统默认的异常处理器来处理
            mDefaultHandler.UncaughtException(thread, ex);
        } else {
            DownloadBookService.cancel(); // 取消任务
            LogUtils.i("取消下载任务");
            new Thread(() => {
                Looper.Prepare();
                ToastUtils.showSingleToast("哎呀，程序发生异常啦...");
                Looper.Loop();
            }).Start();

            try {
                Thread.Sleep(3000);
            } catch (InterruptedException e) {
                LogUtils.e("CrashHandler.InterruptedException--->" + e.ToString());
            }
            //退出程序
            Process.KillProcess(Process.MyPid());
            JavaSystem.Exit(1);
        }
    }

    /**
     * 自定义错误处理,收集错误信息 发送错误报告等操作均在此完成.
     *
     * @param ex
     * @return true:如果处理了该异常信息;否则返回false.
     */
    private bool handleException(Throwable ex) {
        if (ex == null) {
            return false;
        }
        //收集设备参数信息
        collectDeviceInfo(mContext);
        //保存日志文件
        saveCrashInfo2File(ex);
        return true;
    }

    /**
     * 收集设备参数信息
     *
     * @param ctx
     */
    public void collectDeviceInfo(Context ctx) {
        try {
            PackageManager pm = ctx.PackageManager;
            PackageInfo pi = pm.GetPackageInfo(ctx.PackageName, PackageInfoFlags.Activities);
            if (pi != null) {
                string versionName = pi.VersionName == null ? "null" : pi.VersionName;
                string versionCode = pi.VersionCode + "";
                infos.Add("versionName", versionName);
                infos.Add("versionCode", versionCode);
            }
        } catch (NameNotFoundException e) {
            LogUtils.e("CrashHandleran.NameNotFoundException---> error occured when collect package info", e);
        }
        Field[] fields = Class.FromType(typeof(Build)).GetDeclaredFields();
        foreach (Field field in fields) {
            try {
                field.Accessible = true;
                infos.Add(field.Name, field.Get(null).ToString());
            } catch (Exception e) {
                LogUtils.e("CrashHandler.NameNotFoundException---> an error occured when collect crash info", e);
            }
        }
    }

    /**
     * 保存错误信息到文件中
     *
     * @param ex
     * @return 返回文件名称
     */
    private string saveCrashInfo2File(Throwable ex) {

        StringBuffer sb = new StringBuffer();
        sb.Append("---------------------sta--------------------------");
        foreach (var entry in infos) {
            string key = entry.Key;
            string value = entry.Value;
            sb.Append(key + "=" + value + "\n");
        }

        Writer writer = new StringWriter();
        PrintWriter printWriter = new PrintWriter(writer);
        ex.PrintStackTrace(printWriter);
        Throwable cause = ex.Cause;
        while (cause != null) {
            cause.PrintStackTrace(printWriter);
            cause = cause.Cause;
        }
        printWriter.Close();
        string result = writer.ToString();
        sb.Append(result);
        sb.Append("--------------------end---------------------------");
        LogUtils.e(sb.ToString());
        SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
        string fileName = format.Format(new Date()) + ".log";
        File file = new File(FileUtils.createRootPath(mContext) + "/log/" + fileName);
        FileUtils.createFile(file);
        FileUtils.writeFile(file.AbsolutePath, sb.ToString());
        // uploadCrashMessage(sb.ToString());
        return null;
    }
    }
}