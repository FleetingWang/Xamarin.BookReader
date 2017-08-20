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
using Xamarin.BookReader.Bases;
using Xamarin.BookReader.UI.Listeners;
using Xamarin.BookReader.Models;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Xamarin.BookReader.Views;
using Xamarin.BookReader.UI.Adapters;

namespace Xamarin.BookReader.UI.Activities
{
    public class BooksByTagActivity : BaseActivity,
        IOnRvItemClickListener<BooksByTag.TagBook>
    {
        SwipeRefreshLayout refreshLayout;
        RecyclerView mRecyclerView;
        private LinearLayoutManager linearLayoutManager;

        private BooksByTagAdapter mAdapter;
        private List<BooksByTag.TagBook> mList = new List<BooksByTag.TagBook>();

        private String tag;
        private int current = 0;

        public override int getLayoutId()
        {
            return Resource.Layout.activity_books_by_tag;
        }

        public override void bindViews()
        {
            refreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.refreshLayout);
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerview);
        }

        public override void initToolBar()
        {
            mCommonToolbar.Title = (Intent.GetStringExtra("tag"));
            mCommonToolbar.SetNavigationIcon(Resource.Drawable.ab_back);
        }

        public override void initDatas()
        {
            tag = Intent.GetStringExtra("tag");
        }

        public override void configViews()
        {
            refreshLayout.SetOnRefreshListener(new RefreshListener(this));

            mRecyclerView.HasFixedSize = (true);
            linearLayoutManager = new LinearLayoutManager(this);
            mRecyclerView.SetLayoutManager(linearLayoutManager);
            mRecyclerView.AddItemDecoration(new SupportDividerItemDecoration(mContext, LinearLayoutManager.Vertical));
            mAdapter = new BooksByTagAdapter(mContext, mList, this);
            mRecyclerView.SetAdapter(mAdapter);
            mRecyclerView.AddOnScrollListener(new RefreshListener(this));

            //TODO: mPresenter.attachView(this);
            //TODO: mPresenter.getBooksByTag(tag, current + "", (current + 10) + "");
        }

        public void showBooksByTag(List<BooksByTag.TagBook> list, bool isRefresh)
        {
            if (isRefresh)
                mList.Clear();
            mList.AddRange(list);
            current = mList.Count();
            mAdapter.NotifyDataSetChanged();
        }

        public void onLoadComplete(bool isSuccess, String msg)
        {
            refreshLayout.Refreshing = (false);
        }



        public void onItemClick(View view, int position, BooksByTag.TagBook data)
        {
            StartActivity(new Intent(this, typeof(BookDetailActivity))
                .PutExtra("bookId", data._id));
        }
        public void showError()
        {

        }
        public void complete()
        {
            refreshLayout.Refreshing = (false);
        }

        class RefreshListener : RecyclerView.OnScrollListener, SwipeRefreshLayout.IOnRefreshListener
        {
            private BooksByTagActivity booksByTagActivity;

            public RefreshListener(BooksByTagActivity booksByTagActivity)
            {
                this.booksByTagActivity = booksByTagActivity;
            }

            public void OnRefresh()
            {
                booksByTagActivity.current = 0;
                //TODO: BooksByTagPresenter getBooksByTag(booksByTagActivity.tag, booksByTagActivity.current + "", "10");
            }

            public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
            {
                base.OnScrolled(recyclerView, dx, dy);

                int lastVisibleItemPosition = booksByTagActivity.linearLayoutManager.FindLastVisibleItemPosition();
                if (lastVisibleItemPosition + 1 == booksByTagActivity.mAdapter.ItemCount)
                { // 滑到倒数第二项就加载更多

                    bool isRefreshing = booksByTagActivity.refreshLayout.Refreshing;
                    if (isRefreshing)
                    {
                        booksByTagActivity.mAdapter.NotifyItemRemoved(booksByTagActivity.mAdapter.ItemCount);
                        return;
                    }
                    //TODO: BooksByTagPresenter getBooksByTag(booksByTagActivity.tag, booksByTagActivity.current + "", "10");
                }
            }

        }
    }
}