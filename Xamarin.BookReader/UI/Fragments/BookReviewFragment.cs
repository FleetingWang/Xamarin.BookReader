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
using Xamarin.BookReader.Models.Support;
using Xamarin.BookReader.UI.Activities;
using Xamarin.BookReader.UI.EasyAdapters;
using Xamarin.BookReader.Datas;
using System.Reactive.Concurrency;
using Xamarin.BookReader.Utils;
using System.Reactive.Linq;
using Xamarin.BookReader.Extensions;

namespace Xamarin.BookReader.UI.Fragments
{
    /// <summary>
    /// 书评区Fragment
    /// </summary>
    [Register("xamarin.bookreader.ui.fragments.BookReviewFragment")]
    public class BookReviewFragment : BaseRVFragment<BookReviewList.ReviewsBean>
    {
        private String sort = Constant.SortType.Default.GetEnumDescription();
        private String type = Constant.BookType.ALL.GetEnumDescription();
        private String distillate = Constant.Distillate.All.GetEnumDescription();

        public override int LayoutResId => Resource.Layout.common_easy_recyclerview;

        public override void InitDatas()
        {
            MessageBus.Default.Register<SelectionEvent>(initCategoryList);
        }
        public override void ConfigViews()
        {
            initAdapter(new BookReviewAdapter(Activity), true, true);
            onRefresh();
        }
        public void showBookReviewList(List<BookReviewList.ReviewsBean> list, bool isRefresh)
        {
            if (isRefresh)
            {
                mAdapter.clear();
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
                mRecyclerView.setRefreshing(true);
                sort = e.sort.GetEnumDescription();
                type = e.type.GetEnumDescription();
                distillate = e.distillate.GetEnumDescription();
                start = 0;
                getBookReviewList(sort, type, distillate, start, limit);
            });
        }
        public override void onRefresh()
        {
            base.onRefresh();
            getBookReviewList(sort, type, distillate, start, limit);
        }
        public override void onLoadMore()
        {
            getBookReviewList(sort, type, distillate, start, limit);
        }
        void getBookReviewList(String sort, String type, String distillate, int start, int limit)
        {
            BookApi.Instance.getBookReviewList("all", sort, type, start.ToString(), limit.ToString(), distillate)
                .SubscribeOn(DefaultScheduler.Instance)
                .ObserveOn(Application.SynchronizationContext)
                .Subscribe(data => {
                    bool isRefresh = start == 0 ? true : false;
                    showBookReviewList(data.reviews, isRefresh);
                }, e => {
                    LogUtils.e("BookReviewFragment", e.ToString());
                    showError();
                }, () => {
                    LogUtils.i("BookReviewFragment", "complete");
                    complete();
                });
        }

        public override void onItemClick(int position)
        {
            BookReviewList.ReviewsBean data = mAdapter.getItem(position);
            BookReviewDetailActivity.startActivity(Activity, data._id);
        }
        public override void OnDestroyView()
        {
            base.OnDestroyView();
            MessageBus.Default.DeRegister<SelectionEvent>(initCategoryList);
        }
    }
}