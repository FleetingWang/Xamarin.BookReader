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
using Xamarin.BookReader.Utils;
using Xamarin.BookReader.Bases;
using Android.Support.V7.App;

namespace Xamarin.BookReader
{
    [Application(
        AllowBackup = true,
        Icon = "@mipmap/icon",
        Label = "@string/app_name",
        Theme = "@style/AppTheme.NoActionBar"
        )]
    public class ReaderApplication: Application
    {
        private static ReaderApplication sInstance;
        //TODO: 依赖注入 private AppComponent appComponent;

        // TODO: RefWatcher
        //private RefWatcher refWatcher;

        //public static RefWatcher getRefWatcher(Context context) {
        //    ReaderApplication application = (ReaderApplication) context.getApplicationContext();
        //    return application.refWatcher;
        //}

        public override void OnCreate() {
        base.OnCreate();
        //refWatcher = LeakCanary.install(this);
        sInstance = this;
        initCompoent();
        AppUtils.init(this);
        CrashHandler.getInstance().init(this);
        initPrefs();
        initNightMode();
        //initHciCloud();
    }

    public static ReaderApplication getsInstance() {
        return sInstance;
    }

    private void initCompoent() {
        //appComponent = DaggerAppComponent.builder()
        //        .bookApiModule(new BookApiModule())
        //        .appModule(new AppModule(this))
        //        .build();
    }

    //public AppComponent getAppComponent() {
    //    return appComponent;
    //}

    /**
     * 初始化SharedPreference
     */
    protected void initPrefs() {
        SharedPreferencesUtil.init(ApplicationContext, PackageName + "_preference", FileCreationMode.MultiProcess);
    }

    protected void initNightMode() {
        bool isNight = SharedPreferencesUtil.getInstance().getBoolean(Constant.ISNIGHT, false);
        LogUtils.d("isNight=" + isNight);
        if (isNight) {
            AppCompatDelegate.DefaultNightMode = (AppCompatDelegate.ModeNightYes);
        } else {
            AppCompatDelegate.DefaultNightMode = (AppCompatDelegate.ModeNightNo);
        }
    }

        // TODO: HciCloud
        //protected void initHciCloud() {
        //    InitParam initparam = new InitParam();
        //    String authDirPath = getFilesDir().getAbsolutePath();
        //    initparam.addParam(InitParam.AuthParam.PARAM_KEY_AUTH_PATH, authDirPath);
        //    initparam.addParam(InitParam.AuthParam.PARAM_KEY_AUTO_CLOUD_AUTH, "no");
        //    initparam.addParam(InitParam.AuthParam.PARAM_KEY_CLOUD_URL, "test.api.hcicloud.com:8888");
        //    initparam.addParam(InitParam.AuthParam.PARAM_KEY_DEVELOPER_KEY, "0a5e69f8fb1c019b2d87a17acf200889");
        //    initparam.addParam(InitParam.AuthParam.PARAM_KEY_APP_KEY, "0d5d5466");
        //    String logDirPath = FileUtils.createRootPath(this) + "/hcicloud";
        //    FileUtils.createDir(logDirPath);
        //    initparam.addParam(InitParam.LogParam.PARAM_KEY_LOG_FILE_PATH, logDirPath);
        //    initparam.addParam(InitParam.LogParam.PARAM_KEY_LOG_FILE_COUNT, "5");
        //    initparam.addParam(InitParam.LogParam.PARAM_KEY_LOG_FILE_SIZE, "1024");
        //    initparam.addParam(InitParam.LogParam.PARAM_KEY_LOG_LEVEL, "5");
        //    int errCode = HciCloudSys.hciInit(initparam.getStringConfig(), this);
        //    if (errCode != HciErrorCode.HCI_ERR_NONE) {
        //        LogUtils.e("HciCloud初始化失败" + errCode);
        //        return;
        //    }
        //    LogUtils.e("HciCloud初始化成功");
        //}
    }
}