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

        // @Inject
        // MainActivityPresenter mPresenter;

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

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            mIndicator = FindViewById<RVPIndicator>(Resource.Id.indicator);
            mViewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
        }

        public override void initDatas()
        {
            // TODO: startService(new Intent(this, DownloadBookService.class));
            // TODO: mTencent = Tencent.createInstance("1105670298", MainActivity.this);

            mDatas = Resources.GetStringArray(Resource.Array.home_tabs).ToList();
            mTabContents = new List<Fragment>();
            mTabContents.Add(new RecommendFragment());
            mTabContents.Add(new CommunityFragment());
            mTabContents.Add(new FindFragment());

            // TODO: mAdapter
            mAdapter = null;
        }

        public override void configViews()
        {
            mIndicator.setTabItemTitles(mDatas);
            mViewPager.Adapter = (mAdapter);
            mViewPager.OffscreenPageLimit = (3);
            mIndicator.setViewPager(mViewPager, 0);

            //TODO: mPresenter.attachView(this);

            mIndicator.PostDelayed(() => {
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
                //TODO: !SettingManager.getInstance().isUserChooseSex() && 
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
                if (popupWindow == null) {
                    popupWindow = new LoginPopupWindow(this);
                    popupWindow.setLoginTypeListener(this);
                }
                popupWindow.ShowAtLocation(mCommonToolbar, GravityFlags.Center, 0, 0);
                break;
            case Resource.Id.action_my_message:
                if (popupWindow == null) {
                    popupWindow = new LoginPopupWindow(this);
                    popupWindow.setLoginTypeListener(this);
                }
                popupWindow.ShowAtLocation(mCommonToolbar, GravityFlags.Center, 0, 0);
                break;
            case Resource.Id.action_sync_bookshelf:
                showDialog();
                // TODO：mPresenter.syncBookShelf();
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
                if (SharedPreferencesUtil.getInstance().getBoolean(Constant.ISNIGHT, false)) {
                    SharedPreferencesUtil.getInstance().putBoolean(Constant.ISNIGHT, false);
                    AppCompatDelegate.DefaultNightMode = (AppCompatDelegate.ModeNightNo);
                } else {
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

        public void onLogin(ImageView view, string type)
        {
            throw new NotImplementedException();
        }
    }
}

