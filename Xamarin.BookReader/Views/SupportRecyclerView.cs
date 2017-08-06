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

namespace Xamarin.BookReader.Views
{
    public class SupportRecyclerView : RecyclerView
    {
        private View emptyView;
        private AdapterDataObserver emptyObserver;
        //private AdapterDataObserver emptyObserver = new AdapterDataObserver() {
        //    @Override
        //    public void onChanged() {
        //        LogUtils.i("adapter changed");
        //        Adapter adapter = getAdapter();
        //        if (adapter != null && emptyView != null) {
        //            if (adapter.getItemCount() == 0) {
        //                LogUtils.i("adapter visible");
        //                emptyView.setVisibility(View.VISIBLE);
        //                SupportRecyclerView.this.setVisibility(View.GONE);
        //            } else {
        //                LogUtils.i("adapter gone");
        //                emptyView.setVisibility(View.GONE);
        //                SupportRecyclerView.this.setVisibility(View.VISIBLE);
        //            }
        //        }

        //    }
        //};

        public SupportRecyclerView(Context context) : base(context)
        {

        }

        public SupportRecyclerView(Context context, IAttributeSet attrs) : base(context, attrs)
        {

        }

        public SupportRecyclerView(Context context, IAttributeSet attrs, int defStyle)
                : base(context, attrs, defStyle)
        {

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
    }
}