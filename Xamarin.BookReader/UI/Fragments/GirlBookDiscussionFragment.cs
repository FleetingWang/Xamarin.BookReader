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

namespace Xamarin.BookReader.UI.Fragments
{
    /// <summary>
    /// 女生区Fragment
    /// </summary>
    public class GirlBookDiscussionFragment : BaseRVFragment<DiscussionList.PostsBean>
    {
        private String sort = Constant.SortType.Default.ToString();
        private String distillate = Constant.Distillate.All.ToString();

        public override int LayoutResId => Resource.Layout.common_easy_recyclerview;

        public override void InitDatas()
        {
            MessageBus.Default.Register<SelectionEvent>(initCategoryList);
        }
        public override void ConfigViews()
        {
            initAdapter(/*new BookDiscussionAdapter(),*/ true, true);
            onRefresh();
        }
        public void showGirlBookDisscussionList(List<DiscussionList.PostsBean> list, bool isRefresh)
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
            //TODO: mPresenter.getGirlBookDisscussionList(sort, distillate, 0, limit);
        }
        public override void onLoadMore()
        {
            //TODO: mPresenter.getGirlBookDisscussionList(sort, distillate, start, limit);
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