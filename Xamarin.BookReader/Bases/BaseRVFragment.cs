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
using Xamarin.BookReader.Views.RecyclerViews.Adapters;
using Xamarin.BookReader.Views.RecyclerViews.Swipes;
using Xamarin.BookReader.Views.RecyclerViews;
using Android.Support.V7.Widget;
using Android.Support.V4.Content;

namespace Xamarin.BookReader.Bases
{
    public abstract class BaseRVFragment<T> : BaseFragment,
        IOnLoadMoreListener, IOnRefreshListener,
        RecyclerArrayAdapter<T>.OnItemClickListener
        where T : class
    {
        protected EasyRecyclerView mRecyclerView;
        protected RecyclerArrayAdapter<T> mAdapter;

        protected int start = 0;
        protected int limit = 20;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle state)
        {
            var parent = base.OnCreateView(inflater, container, state);
            mRecyclerView = parent.FindViewById<EasyRecyclerView>(Resource.Id.recyclerview);
            return parent;
        }

        protected void initAdapter(bool refreshable, bool loadmoreable)
        {
            if (mRecyclerView != null)
            {
                mRecyclerView.setLayoutManager(new LinearLayoutManager(Activity));
                mRecyclerView.setItemDecoration(ContextCompat.GetColor(Activity, Resource.Color.common_divider_narrow), 1, 0, 0);
                mRecyclerView.setAdapterWithProgress(mAdapter);
            }

            if (mAdapter != null)
            {
                mAdapter.setOnItemClickListener(this);
                mAdapter.setError(Resource.Layout.common_error_view).Click += (sender, e) =>
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
        }

        protected void initAdapter(RecyclerArrayAdapter<T> adapter, bool refreshable, bool loadmoreable)
        {
            mAdapter = adapter;
            initAdapter(refreshable, loadmoreable);
        }

        public virtual void onItemClick(int position)
        {
        }

        public virtual void onLoadMore()
        {
        }

        public virtual void onRefresh()
        {
            mRecyclerView.setRefreshing(true);
        }

        protected void loaddingError()
        {
            if (mAdapter.getCount() < 1)
            { // 说明缓存也没有加载，那就显示errorview，如果有缓存，即使刷新失败也不显示error
                mAdapter.clear();
            }
            mAdapter.pauseMore();
            mRecyclerView.setRefreshing(false);
            mRecyclerView.showTipViewAndDelayClose("似乎没有网络哦");
        }
    }
}