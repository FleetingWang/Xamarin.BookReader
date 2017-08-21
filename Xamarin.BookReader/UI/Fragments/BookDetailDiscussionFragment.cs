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
            // TODO: mPresenter.getBookDetailDiscussionList(bookId, sort, 0, limit);
        }
        public override void onLoadMore()
        {
            // TODO: mPresenter.getBookDetailDiscussionList(bookId, sort, start, limit);
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