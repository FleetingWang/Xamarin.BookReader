using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.BookReader.Views.RecyclerViews.Adapters;
using Xamarin.BookReader.Views.RecyclerViews;
using Xamarin.BookReader.Utils;
using Android.Support.V7.Widget;
using Android.Support.V4.Content;
using System;
using Xamarin.BookReader.Views.RecyclerViews.Swipes;

namespace Xamarin.BookReader.Bases
{
    public abstract class BaseRVActivity<T> : BaseActivity, IOnLoadMoreListener, IOnRefreshListener, RecyclerArrayAdapter<T>.OnItemClickListener where T : class
    {
        protected EasyRecyclerView mRecyclerView;
        protected RecyclerArrayAdapter<T> mAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mRecyclerView = FindViewById<EasyRecyclerView>(Resource.Id.recyclerview);
        }

        protected int start = 0;
        protected int limit = 20;

        protected void initAdapter(bool refreshable, bool loadmoreable)
        {
            if (mAdapter != null)
            {
                mAdapter.setOnItemClickListener(this);
                var errorView = mAdapter.setError(Resource.Layout.common_error_view);
                errorView.Click += (sender, e) =>
                {
                    mAdapter.resumeMore();
                };
                if (loadmoreable)
                {
                    mAdapter.setMore(Resource.Layout.common_more_view, this);
                    mAdapter.setNoMore(Resource.Layout.common_nomore_view);
                }
                if (refreshable && mRecyclerView != null)
                {
                    mRecyclerView.setRefreshListener(this);
                }
            }

            if (mRecyclerView != null)
            {
                mRecyclerView.setLayoutManager(new LinearLayoutManager(this));
                mRecyclerView.setItemDecoration(ContextCompat.GetColor(this, Resource.Color.common_divider_narrow), 1, 0, 0);
                mRecyclerView.setAdapterWithProgress(mAdapter);
            }
        }

        protected void initAdapter(RecyclerArrayAdapter<T> adapter, bool refreshable, bool loadmoreable)
        {
            mAdapter = adapter;
            initAdapter(refreshable, loadmoreable);
        }

        // createInstance
        //public Object createInstance(Class cls) {
        //    Object obj;
        //    try {
        //        Constructor c1 = cls.getDeclaredConstructor(Context.Class);
        //        c1.setAccessible(true);
        //        obj = c1.newInstance(mContext);
        //    } catch (Exception e) {
        //        obj = null;
        //    }
        //    return obj;
        //}

        public void onLoadMore()
        {
            if (!NetworkUtils.isConnected(ApplicationContext))
            {
                mAdapter.pauseMore();
                return;
            }
        }

        protected void loaddingError()
        {
            mAdapter.clear();
            mAdapter.pauseMore();
            mRecyclerView.setRefreshing(false);
        }

        public void onItemClick(int position)
        {

        }

        public void onRefresh()
        {
            start = 0;
            if (!NetworkUtils.isConnected(ApplicationContext))
            {
                mAdapter.pauseMore();
                return;
            }
        }
    }
}