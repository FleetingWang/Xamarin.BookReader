using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Xamarin.BookReader.Utils;
using Android.Annotation;
using Android.Graphics;
using Android.Support.V4.Content;
using Xamarin.BookReader.Views.Loading;
using AndroidResource = Android.Resource;

namespace Xamarin.BookReader.Bases
{
    public abstract class BaseActivity : AppCompatActivity
    {
        public Toolbar mCommonToolbar;

        protected Context mContext;
        protected int statusBarColor = 0;
        protected View statusBarView = null;
        private bool mNowMode;
        private CustomDialog dialog;//进度条

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(getLayoutId());
            if (statusBarColor == 0)
            {
                statusBarView = StatusBarCompat.compat(this, ContextCompat.GetColor(this, Resource.Color.colorPrimaryDark));
            }
            else if (statusBarColor != -1)
            {
                statusBarView = StatusBarCompat.compat(this, statusBarColor);
            }
            transparent19and20();
            mContext = this;
            // TODO: ButterKnife ButterKnife.bind(this);
            // TODO:Component setupActivityComponent(ReaderApplication.getsInstance().getAppComponent());
            mCommonToolbar = FindViewById<Toolbar>(Resource.Id.common_toolbar);
            if (mCommonToolbar != null)
            {
                initToolBar();
                SetSupportActionBar(mCommonToolbar);
            }
            initDatas();
            configViews();
            mNowMode = SharedPreferencesUtil.getInstance().getBoolean(Constant.ISNIGHT);
        }

        protected void transparent19and20()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat
                    && Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
            {
                //透明状态栏
                Window.AddFlags(WindowManagerFlags.TranslucentStatus);
            }
        }

        [TargetApi(Value = (int)BuildVersionCodes.Lollipop)]
        private void toolbarSetElevation(float elevation)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                mCommonToolbar.Elevation = elevation;
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (SharedPreferencesUtil.getInstance().getBoolean(Constant.ISNIGHT, false) != mNowMode)
            {
                if (SharedPreferencesUtil.getInstance().getBoolean(Constant.ISNIGHT, false))
                {
                    AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightYes;
                }
                else
                {
                    AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
                }
                Recreate();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            //ButterKnife.unbind(this);
            dismissDialog();
        }

        public abstract int getLayoutId();

        //protected abstract void setupActivityComponent(AppComponent appComponent);

        public abstract void initToolBar();

        public abstract void initDatas();

        /**
         * 对各种控件进行设置、适配、填充数据
         */
        public abstract void configViews();

        protected void gone(View[] views)
        {
            if (views != null && views.Length > 0)
            {
                foreach (View view in views)
                {
                    if (view != null)
                    {
                        view.Visibility = ViewStates.Gone;
                    }
                }
            }
        }

        protected void visible(View[] views)
        {
            if (views != null && views.Length > 0)
            {
                foreach (View view in views)
                {
                    if (view != null)
                    {
                        view.Visibility = ViewStates.Visible;
                    }
                }
            }
        }

        protected bool isVisible(View view)
        {
            return view.Visibility == ViewStates.Visible;
        }

        // dialog
        public CustomDialog getDialog()
        {
            if (dialog == null)
            {
                dialog = CustomDialog.instance(this);
                dialog.SetCancelable(true);
            }
            return dialog;
        }

        public void hideDialog()
        {
            if (dialog != null)
                dialog.Hide();
        }

        public void showDialog()
        {
            getDialog().Show();
        }

        public void dismissDialog()
        {
            if (dialog != null)
            {
                dialog.Dismiss();
                dialog = null;
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == AndroidResource.Id.Home)
            {
                Finish();
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        protected void hideStatusBar()
        {
            WindowManagerLayoutParams attrs = Window.Attributes;
            attrs.Flags |= WindowManagerFlags.Fullscreen;
            Window.Attributes = attrs;
            if (statusBarView != null)
            {
                statusBarView.SetBackgroundColor(Color.Transparent);
            }
        }

        protected void showStatusBar()
        {
            WindowManagerLayoutParams attrs = Window.Attributes;
            attrs.Flags &= ~WindowManagerFlags.Fullscreen;
            Window.Attributes = attrs;
            if (statusBarView != null)
            {
                statusBarView.SetBackgroundColor(Color.Transparent);
            }
            if (statusBarView != null)
            {
                statusBarView.SetBackgroundColor(new Color(statusBarColor));
            }
        }
    }
}