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

namespace Xamarin.BookReader.Bases
{
    public abstract class BaseFragment : Fragment
    {
        private CustomDialog dialog;

        public abstract int LayoutResId { get; }
        protected View ParentView { get; private set; }
        protected LayoutInflater LayoutInflater { get; private set; }
        // TODO: AppComponent protected abstract void setupActivityComponent(AppComponent appComponent);

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle state)
        {
            ParentView = inflater.Inflate(LayoutResId, container, false);
            LayoutInflater = inflater;
            return ParentView;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            //TODO: ButterKnife.bind(this, view);
            //TODO: setupActivityComponent(ReaderApplication.getsInstance().getAppComponent());
            InitDatas();
            ConfigViews();
        }

        public abstract void InitDatas();

        /**
         * 对各种控件进行设置、适配、填充数据
         */
        public abstract void ConfigViews();

        public CustomDialog GetDialog()
        {
            if (dialog == null)
            {
                dialog = CustomDialog.instance(Activity);
                dialog.SetCancelable(false);
            }
            return dialog;
        }

        public void HideDialog()
        {
            dialog?.Hide();
        }

        public void ShowDialog()
        {
            GetDialog().Show();
        }

        public void DismissDialog()
        {
            if (dialog != null)
            {
                dialog.Dismiss();
                dialog = null;
            }
        }

        protected void Gone(params View[] views)
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

        protected void Visible(params View[] views)
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

        protected bool IsViewVisible(View view)
        {
            return view.Visibility == ViewStates.Visible;
        }
    }
}