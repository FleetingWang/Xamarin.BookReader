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
using Xamarin.BookReader.Models.Support;
using DSoft.Messaging;
using Xamarin.BookReader.UI.Activities;

namespace Xamarin.BookReader.UI.Fragments
{
    /// <summary>
    /// 综合讨论区Fragment
    /// </summary>
    public class BookDiscussionFragment : BaseRVFragment<DiscussionList.PostsBean>
    {
        private static String BUNDLE_BLOCK = "block";

        public static BookDiscussionFragment newInstance(String block)
        {
            BookDiscussionFragment fragment = new BookDiscussionFragment();
            Bundle bundle = new Bundle();
            bundle.PutString(BUNDLE_BLOCK, block);
            fragment.Arguments = (bundle);
            return fragment;
        }

        private String block = "ramble";
        private String sort = Constant.SortType.Default.ToString();
        private String distillate = Constant.Distillate.All.ToString();

        public override int LayoutResId => Resource.Layout.common_easy_recyclerview;

        public override void InitDatas()
        {
            block = Arguments.GetString(BUNDLE_BLOCK);
            MessageBus.Default.Register<SelectionEvent>(initCategoryList);
        }

        public override void ConfigViews()
        {
            initAdapter(/*new BookDiscussionAdapter(),*/ true, true);
            onRefresh();
        }
        public void showBookDisscussionList(List<DiscussionList.PostsBean> list, bool isRefresh)
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
                mRecyclerView.setRefreshing(true);
                sort = e.sort.ToString();
                distillate = e.distillate.ToString();
                onRefresh();
            });
        }
        public override void onRefresh()
        {
            base.onRefresh();
            //TODO: mPresenter.getBookDisscussionList(block, sort, distillate, 0, limit);
        }
        public override void onLoadMore()
        {
            //TODO: mPresenter.getBookDisscussionList(block, sort, distillate, start, limit);
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