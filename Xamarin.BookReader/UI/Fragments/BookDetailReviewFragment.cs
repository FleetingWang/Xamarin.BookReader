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
using Xamarin.BookReader.Models;
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
    /// 书籍详情 书评列表Fragment
    /// </summary>
    public class BookDetailReviewFragment : BaseRVFragment<HotReview.Reviews>
    {
        public static String BUNDLE_ID = "bookId";

        public static BookDetailReviewFragment newInstance(String id)
        {
            BookDetailReviewFragment fragment = new BookDetailReviewFragment();
            Bundle bundle = new Bundle();
            bundle.PutString(BUNDLE_ID, id);
            fragment.Arguments = (bundle);
            return fragment;
        }

        private String bookId;

        private String sort = Constant.SortType.Default.ToString();
        private String type = Constant.BookType.ALL.ToString();

        public override int LayoutResId => Resource.Layout.common_easy_recyclerview;


        public override void InitDatas()
        {
            MessageBus.Default.Register<SelectionEvent>(initCategoryList);
            bookId = Arguments.GetString(BUNDLE_ID);
        }

        public override void ConfigViews()
        {
            initAdapter(new BookDetailReviewAdapter(Activity), true, true);
            onRefresh();
        }

        public void showBookDetailReviewList(List<HotReview.Reviews> list, bool isRefresh)
        {
            if (isRefresh)
            {
                mAdapter.clear();
            }
            mAdapter.addAll(list);
            if (list != null)
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
            getBookDetailReviewList(bookId, sort, 0, limit);
        }
        public override void onLoadMore()
        {
            base.onLoadMore();
            getBookDetailReviewList(sort, type, start, limit);
        }
        void getBookDetailReviewList(String bookId, String sort, int start, int limit)
        {
            BookApi.Instance.getBookDetailReviewList(bookId, sort, start.ToString(), limit.ToString())
                .SubscribeOn(DefaultScheduler.Instance)
                .ObserveOn(Application.SynchronizationContext)
                .Subscribe(data => {
                    bool isRefresh = start == 0 ? true : false;
                    showBookDetailReviewList(data.reviews, isRefresh);
                }, e => {
                    LogUtils.e("BookDetailReviewFragment", e.ToString());
                    showError();
                }, () => {
                    LogUtils.i("BookDetailReviewFragment", "complete");
                    complete();
                });
        }

        public override void onItemClick(int position)
        {
            BookReviewDetailActivity.startActivity(Activity, mAdapter.getItem(position)._id);
        }
        public override void OnDestroyView()
        {
            base.OnDestroyView();
            MessageBus.Default.DeRegister<SelectionEvent>(initCategoryList);
        }
    }
}