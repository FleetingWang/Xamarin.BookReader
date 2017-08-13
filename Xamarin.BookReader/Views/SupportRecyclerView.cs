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
using Android.Support.V7.Widget;

using Android.Util;
using Xamarin.BookReader.Utils;

namespace Xamarin.BookReader.Views
{
    public class SupportRecyclerView : RecyclerView
    {
        private View emptyView;
        private AdapterDataObserver emptyObserver;

        public SupportRecyclerView(Context context) : base(context)
        {
            CommonConstructor();
        }

        public SupportRecyclerView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            CommonConstructor();
        }

        public SupportRecyclerView(Context context, IAttributeSet attrs, int defStyle)
                : base(context, attrs, defStyle)
        {
            CommonConstructor();
        }

        private void CommonConstructor()
        {
            emptyObserver = new CustomAdapterDataObserver(this);
        }

        public void setAdapter(Adapter adapter)
        {
            Adapter oldAdapter = GetAdapter();
            if (oldAdapter != null && emptyObserver != null)
            {
                oldAdapter.UnregisterAdapterDataObserver(emptyObserver);
            }
            base.SetAdapter(adapter);

            if (adapter != null)
            {
                adapter.RegisterAdapterDataObserver(emptyObserver);
            }
            emptyObserver.OnChanged();
        }

        /**
         * set view when no content item
         *
         * @param emptyView visiable view when items is empty
         */
        public void setEmptyView(View emptyView)
        {
            this.emptyView = emptyView;
        }

        class CustomAdapterDataObserver : AdapterDataObserver
        {
            private SupportRecyclerView supportRecyclerView;

            public CustomAdapterDataObserver(SupportRecyclerView supportRecyclerView)
            {
                this.supportRecyclerView = supportRecyclerView;
            }

            public override void OnChanged()
            {
                LogUtils.i("adapter changed");
                Adapter adapter = supportRecyclerView.GetAdapter();
                if (adapter != null && supportRecyclerView.emptyView != null)
                {
                    if (adapter.ItemCount == 0)
                    {
                        LogUtils.i("adapter visible");
                        supportRecyclerView.emptyView.Visibility = ViewStates.Visible;
                        supportRecyclerView.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        LogUtils.i("adapter gone");
                        supportRecyclerView.emptyView.Visibility = ViewStates.Gone;
                        supportRecyclerView.Visibility = ViewStates.Visible;
                    }
                }
            }
        }
    }
}