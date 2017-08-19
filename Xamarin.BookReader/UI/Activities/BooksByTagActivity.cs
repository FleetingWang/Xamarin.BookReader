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

namespace Xamarin.BookReader.UI.Activities
{
    public class BooksByTagActivity : BaseActivity,
        IOnRvItemClickListener<BooksByTag.TagBook>
    {
        //@Bind(R.id.refreshLayout)
        SwipeRefreshLayout refreshLayout;
        //@Bind(R.id.recyclerview)
        RecyclerView mRecyclerView;
        private LinearLayoutManager linearLayoutManager;

        private BooksByTagAdapter mAdapter;
        private List<BooksByTag.TagBook> mList = new ArrayList<>();

        private String tag;
        private int current = 0;

        public override int getLayoutId()
        {
            return R.layout.activity_books_by_tag;
        }

        public override void bindViews()
        {

        }

        public override void initToolBar()
        {
            mCommonToolbar.setTitle(getIntent().getStringExtra("tag"));
            mCommonToolbar.setNavigationIcon(R.drawable.ab_back);
        }

        public override void initDatas()
        {
            tag = getIntent().getStringExtra("tag");
        }

        public override void configViews()
        {
            refreshLayout.setOnRefreshListener(new RefreshListener());

            mRecyclerView.setHasFixedSize(true);
            linearLayoutManager = new LinearLayoutManager(this);
            mRecyclerView.setLayoutManager(linearLayoutManager);
            mRecyclerView.addItemDecoration(new SupportDividerItemDecoration(mContext, LinearLayoutManager.VERTICAL));
            mAdapter = new BooksByTagAdapter(mContext, mList, this);
            mRecyclerView.setAdapter(mAdapter);
            mRecyclerView.addOnScrollListener(new RefreshListener());

            mPresenter.attachView(this);
            mPresenter.getBooksByTag(tag, current + "", (current + 10) + "");
        }

        public void showBooksByTag(List<BooksByTag.TagBook> list, boolean isRefresh)
        {
            if (isRefresh)
                mList.clear();
            mList.addAll(list);
            current = mList.size();
            mAdapter.notifyDataSetChanged();
        }

        public void onLoadComplete(boolean isSuccess, String msg)
        {
            refreshLayout.setRefreshing(false);
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
            refreshLayout.setRefreshing(false);
        }

        class RefreshListener : RecyclerView.OnScrollListener, SwipeRefreshLayout.IOnRefreshListener
        {
            public void OnRefresh()
            {
                current = 0;
                mPresenter.getBooksByTag(tag, current + "", "10");
            }

            public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
            {
                base.OnScrolled(recyclerView, dx, dy);

                int lastVisibleItemPosition = linearLayoutManager.findLastVisibleItemPosition();
                if (lastVisibleItemPosition + 1 == mAdapter.getItemCount())
                { // 滑到倒数第二项就加载更多

                    boolean isRefreshing = refreshLayout.isRefreshing();
                    if (isRefreshing)
                    {
                        mAdapter.notifyItemRemoved(mAdapter.getItemCount());
                        return;
                    }
                    mPresenter.getBooksByTag(tag, current + "", "10");
                }
            }

        }
    }
}