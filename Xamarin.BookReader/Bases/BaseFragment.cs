using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Xamarin.BookReader.Views.Loading;
using Activity = Android.App.Activity;

namespace Xamarin.BookReader.Bases
{
    public abstract class BaseFragment : Fragment
    {
        protected View parentView;
        protected FragmentActivity activity;
        protected LayoutInflater inflater;

        protected Context mContext;

        private CustomDialog dialog;

        public abstract int getLayoutResId();

        // TODO: AppComponent protected abstract void setupActivityComponent(AppComponent appComponent);

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle state)
        {
            parentView = inflater.Inflate(getLayoutResId(), container, false);
            activity = getSupportActivity();
            mContext = activity;
            this.inflater = inflater;
            return parentView;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            // TODO: ButterKnife.bind(this, view);
            // TODO: setupActivityComponent(ReaderApplication.getsInstance().getAppComponent());
            attachView();
            initDatas();
            configViews();
        }

        public abstract void attachView();

        public abstract void initDatas();

        /**
         * 对各种控件进行设置、适配、填充数据
         */
        public abstract void configViews();

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            if (context is FragmentActivity fragmentActivity)
            {
                activity = fragmentActivity;
            }
        }

        public override void OnDetach()
        {
            base.OnDetach();
            this.activity = null;
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            // TODO: ButterKnife.unbind(this);
        }

        public FragmentActivity getSupportActivity()
        {
            return base.Activity;
        }

        public Context getApplicationContext()
        {
            return this.activity == null ? (Activity == null ? null : Activity
                    .ApplicationContext) : this.activity.ApplicationContext;
        }

        protected LayoutInflater getLayoutInflater()
        {
            return inflater;
        }

        protected View getParentView()
        {
            return parentView;
        }

        public CustomDialog getDialog()
        {
            if (dialog == null)
            {
                dialog = CustomDialog.instance(Activity);
                dialog.SetCancelable(false);
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
    }
}