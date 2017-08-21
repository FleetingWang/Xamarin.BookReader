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
using Xamarin.BookReader.Models;
using Xamarin.BookReader.Bases;
using DSoft.Messaging;
using Xamarin.BookReader.UI.Activities;
using Xamarin.BookReader.Models.Support;
using Xamarin.BookReader.UI.EasyAdapters;
using Xamarin.BookReader.Datas;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Xamarin.BookReader.Utils;

namespace Xamarin.BookReader.UI.Fragments
{
    /// <summary>
    /// 书籍详情 讨论列表Fragment
    /// </summary>
    public class BookDetailDiscussionFragment : BaseRVFragment<DiscussionList.PostsBean>
    {
        public static String BUNDLE_ID = "bookId";
        public static BookDetailDiscussionFragment newInstance(String id)
        {
            BookDetailDiscussionFragment fragment = new BookDetailDiscussionFragment();
            Bundle bundle = new Bundle();
            bundle.PutString(BUNDLE_ID, id);
            fragment.Arguments = bundle;
            return fragment;
        }

        private String bookId;

        private String sort = Constant.SortType.Default.ToString();

        public override int LayoutResId => Resource.Layout.common_easy_recyclerview;

        public override void InitDatas()
        {
            MessageBus.Default.Register<SelectionEvent>(initCategoryList);
            bookId = Arguments.GetString(BUNDLE_ID);
        }

        public override void ConfigViews()
        {
            initAdapter(new BookDiscussionAdapter(Activity), true, true);
            onRefresh();
        }

        public void showBookDetailDiscussionList(List<DiscussionList.PostsBean> list, bool isRefresh)
        {
            if (isRefresh)
            {
                mAdapter.clear();
                start = 0;
            }
            mAdapter.addAll(list);
            start = start + list.Count();
        }
        public void showError()
        {
            loaddingError();
        }
        public void complete()
        {
            mRecyclerView.setRefreshing(false);
        }

        public void initCategoryList(object sender, MessageBusEvent evnt)
        {
            var e = evnt as SelectionEvent;
            Activity.RunOnUiThread(() =>
            {
                if (UserVisibleHint)
                {
                    mRecyclerView.setRefreshing(true);
                    sort = e.sort.ToString();
                    onRefresh();
                }
            });
        }

        public override void onRefresh()
        {
            base.onRefresh();
            getBookDetailDiscussionList(bookId, sort, 0, limit);
        }
        public override void onLoadMore()
        {
            getBookDetailDiscussionList(bookId, sort, start, limit);
        }
        void getBookDetailDiscussionList(String bookId, String sort, int start, int limit)
        {
            BookApi.Instance.getBookDetailDisscussionList(bookId, sort, "normal,vote", start.ToString(), limit.ToString())
                .SubscribeOn(DefaultScheduler.Instance)
                .ObserveOn(Application.SynchronizationContext)
                .Subscribe(data => {
                    bool isRefresh = start == 0 ? true : false;
                    showBookDetailDiscussionList(data.posts, isRefresh);
                }, e => {
                    LogUtils.e("BookDetailDiscussionFragment", e.ToString());
                    showError();
                }, () => {
                    LogUtils.i("BookDetailDiscussionFragment", "complete");
                    complete();
                });
        }

        public override void onItemClick(int position)
        {
            DiscussionList.PostsBean data = mAdapter.getItem(position);
            BookDiscussionDetailActivity.startActivity(Activity, data._id);
        }
        public override void OnDestroyView()
        {
            base.OnDestroyView();
            MessageBus.Default.DeRegister<SelectionEvent>(initCategoryList);
        }
    }
}