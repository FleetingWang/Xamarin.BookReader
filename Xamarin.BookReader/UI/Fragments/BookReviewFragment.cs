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

namespace Xamarin.BookReader.UI.Fragments
{
    /// <summary>
    /// 书评区Fragment
    /// </summary>
    public class BookReviewFragment : BaseRVFragment<BookReviewList.ReviewsBean>
    {
        private String sort = Constant.SortType.Default.ToString();
        private String type = Constant.BookType.ALL.ToString();
        private String distillate = Constant.Distillate.All.ToString();

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
                sort = e.sort.ToString();
                type = e.type.ToString();
                distillate = e.distillate.ToString();
                start = 0;
                //TODO: mPresenter.getBookReviewList(sort, type, distillate, start, limit);
            });
        }
        public override void onRefresh()
        {
            base.onRefresh();
            //TODO: mPresenter.getBookReviewList(sort, type, distillate, start, limit);
        }
        public override void onLoadMore()
        {
            //TODO: mPresenter.getBookReviewList(sort, type, distillate, start, limit);
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