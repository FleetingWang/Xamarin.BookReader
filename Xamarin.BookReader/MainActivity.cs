using Android.Widget;
using Android.OS;
using Xamarin.BookReader.Bases;
using System;
using Xamarin.BookReader.Views;
using Android.Support.V4.View;
using Android.Support.V4.App;
using System.Collections.Generic;
using Activity = Android.App.ActivityAttribute;
using System.Linq;
using Android.Views;
using Android.Content;
using Xamarin.BookReader.Utils;
using Android.Support.V7.App;
using Xamarin.BookReader.UI.Fragments;
using Settings = Xamarin.BookReader.Helpers.Settings;
using Xamarin.BookReader.Services;
using Xamarin.BookReader.Models;
using Xamarin.BookReader.Managers;
using Xamarin.BookReader.Datas;
using System.Reactive.Linq;
using Android.Support.V7.View.Menu;
using System.Reactive.Concurrency;

namespace Xamarin.BookReader
{
    [Activity(Label = "Xamarin.BookReader", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : BaseActivity, LoginPopupWindow.LoginTypeListener
    {
        RVPIndicator mIndicator;
        ViewPager mViewPager;

        private List<Fragment> mTabContents;
        private FragmentPagerAdapter mAdapter;
        private List<String> mDatas;

        // 退出时间
        private long currentBackPressedTime = 0;
        // 退出间隔
        private static int BACK_PRESSED_INTERVAL = 2000;

        private LoginPopupWindow popupWindow;
        //TODO: public static Tencent mTencent;
        //TODO: public IUiListener loginListener;
        private GenderPopupWindow genderPopupWindow;

        public override void initToolBar()
        {
            mCommonToolbar.SetLogo(Resource.Mipmap.logo);
            Title = "";
        }

        public override int getLayoutId() => Resource.Layout.activity_main;

        public override void bindViews()
        {
            mIndicator = FindViewById<RVPIndicator>(Resource.Id.indicator);
            mViewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
        }

        public override void initDatas()
        {
            StartService(new Intent(this, typeof(DownloadBookService)));
            // TODO: mTencent = Tencent.createInstance("1105670298", MainActivity.this);

            mDatas = Resources.GetStringArray(Resource.Array.home_tabs).ToList();
            mTabContents = new List<Fragment>();
            mTabContents.Add(new RecommendFragment());
            mTabContents.Add(new CommunityFragment());
            mTabContents.Add(new FindFragment());

            mAdapter = new CustomFragmentPagerAdapter(SupportFragmentManager, mTabContents);
        }

        public override void configViews()
        {
            mIndicator.setTabItemTitles(mDatas);
            mViewPager.Adapter = (mAdapter);
            mViewPager.OffscreenPageLimit = (3);
            mIndicator.setViewPager(mViewPager, 0);

            mIndicator.PostDelayed(() =>
            {
                showChooseSexPopupWindow();
            }, 500);
        }

        public void showChooseSexPopupWindow()
        {
            if (genderPopupWindow == null)
            {
                genderPopupWindow = new GenderPopupWindow(this);
            }
            if (
                !Settings.IsUserChooseSex &&
                    !genderPopupWindow.IsShowing)
            {
                genderPopupWindow.ShowAtLocation(mCommonToolbar, GravityFlags.Center, 0, 0);
            }
        }

        public void setCurrentItem(int position)
        {
            mViewPager.CurrentItem = (position);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            switch (id)
            {
                case Resource.Id.action_search:
                    StartActivity(new Intent(this, typeof(MainActivity)));
                    break;
                case Resource.Id.action_login:
                    if (popupWindow == null)
                    {
                        popupWindow = new LoginPopupWindow(this);
                        popupWindow.setLoginTypeListener(this);
                    }
                    popupWindow.ShowAtLocation(mCommonToolbar, GravityFlags.Center, 0, 0);
                    break;
                case Resource.Id.action_my_message:
                    if (popupWindow == null)
                    {
                        popupWindow = new LoginPopupWindow(this);
                        popupWindow.setLoginTypeListener(this);
                    }
                    popupWindow.ShowAtLocation(mCommonToolbar, GravityFlags.Center, 0, 0);
                    break;
                case Resource.Id.action_sync_bookshelf:
                    showDialog();
                    syncBookShelf();
                    /* if (popupWindow == null) {
                         popupWindow = new LoginPopupWindow(this);
                         popupWindow.setLoginTypeListener(this);
                     }
                     popupWindow.showAtLocation(mCommonToolbar, GravityFlags.Center, 0, 0);*/
                    break;
                case Resource.Id.action_scan_local_book:
                    // TODO：ScanLocalBookActivity.startActivity(this);
                    break;
                case Resource.Id.action_wifi_book:
                    // TODO：WifiBookActivity.startActivity(this);
                    break;
                case Resource.Id.action_feedback:
                    // TODO：FeedbackActivity.startActivity(this);
                    break;
                case Resource.Id.action_night_mode:
                    if (SharedPreferencesUtil.getInstance().getBoolean(Constant.ISNIGHT, false))
                    {
                        SharedPreferencesUtil.getInstance().putBoolean(Constant.ISNIGHT, false);
                        AppCompatDelegate.DefaultNightMode = (AppCompatDelegate.ModeNightNo);
                    }
                    else
                    {
                        SharedPreferencesUtil.getInstance().putBoolean(Constant.ISNIGHT, true);
                        AppCompatDelegate.DefaultNightMode = (AppCompatDelegate.ModeNightYes);
                    }
                    Recreate();
                    break;
                case Resource.Id.action_settings:
                    // TODO：SettingActivity.startActivity(this);
                    break;
                default:
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        /// <summary>
        /// 双击退出
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public override bool DispatchKeyEvent(KeyEvent e)
        {
            if (e.Action == KeyEventActions.Down
                && e.KeyCode == Keycode.Back)
            {
                if (Java.Lang.JavaSystem.CurrentTimeMillis() - currentBackPressedTime > BACK_PRESSED_INTERVAL)
                {
                    currentBackPressedTime = Java.Lang.JavaSystem.CurrentTimeMillis();
                    ToastUtils.showToast(GetString(Resource.String.exit_tips));
                    return true;
                }
                else
                {
                    Finish(); // 退出
                }
            }
            else if (e.KeyCode == Keycode.Menu)
            {
                return true;
            }
            return base.DispatchKeyEvent(e);
        }

        protected override bool OnPrepareOptionsPanel(View view, IMenu menu)
        {
            if (menu != null)
            {
                var menuBuilder = menu as MenuBuilder;
                if (menuBuilder != null)
                {
                    try
                    {
                        menuBuilder.SetOptionalIconsVisible(true);
                    }
                    catch (Java.Lang.Exception e)
                    {
                        e.PrintStackTrace();
                    }
                }
            }
            return base.OnPrepareOptionsPanel(view, menu);
        }

        public void syncBookShelfCompleted()
        {
            dismissDialog();
            EventManager.refreshCollectionList();
        }

        public void onLogin(ImageView view, string type)
        {
            if (type.Equals("QQ"))
            {
                //if (!mTencent.isSessionValid())
                //{
                //    if (loginListener == null) loginListener = new BaseUIListener();
                //    mTencent.login(this, "all", loginListener);
                //}
            }
            //4f45e920ff5d1a0e29d997986cd97181
        }

        public void showError()
        {
            ToastUtils.showSingleToast("同步异常");
            dismissDialog();
        }

        private void syncBookShelf()
        {
            List<Recommend.RecommendBooks> list = CollectionsManager.getInstance().getCollectionList();
            List<IObservable<BookMixAToc.MixToc>> observables = new List<IObservable<BookMixAToc.MixToc>>();
            if (list != null && list.Any())
            {
                foreach (var bean in list)
                {
                    if (!bean.isFromSD)
                    {
                        IObservable<BookMixAToc.MixToc> fromNetWork = BookApi.Instance.getBookMixAToc(bean._id, "chapters")
                            .Select(s => s.mixToc);
                        observables.Add(fromNetWork);
                    }
                }
            }
            else
            {
                ToastUtils.showSingleToast("书架空空如也...");
                syncBookShelfCompleted();
                return;
            }
            Observable.Merge(observables)
                //.SubscribeOn(Scheduler.io)
                //.ObserveOn(AndroidSchedulers.MainThread())
                .Subscribe(data => {
                    String lastChapter = data.chapters[data.chapters.Count() - 1].title;
                    CollectionsManager.getInstance().setLastChapterAndLatelyUpdate(data.book, lastChapter, data.chaptersUpdated);
                }, e => {
                    LogUtils.e("onError: " + e);
                    showError();
                }, () => {
                    syncBookShelfCompleted();
                });
        }

        class CustomFragmentPagerAdapter : FragmentPagerAdapter
        {
            private List<Fragment> _mTabContents;
            public CustomFragmentPagerAdapter(FragmentManager fm, List<Fragment> mTabContents)
                : base(fm)
            {
                _mTabContents = mTabContents;
            }

            public override int Count => _mTabContents.Count();

            public override Fragment GetItem(int position)
            {
                return _mTabContents[position];
            }
        }
    }
}

