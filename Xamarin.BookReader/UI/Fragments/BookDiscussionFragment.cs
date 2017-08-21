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
using Xamarin.BookReader.UI.EasyAdapters;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Xamarin.BookReader.Utils;
using Xamarin.BookReader.Datas;

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
            initAdapter(new BookDiscussionAdapter(Activity), true, true);
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
            getBookDisscussionList(block, sort, distillate, 0, limit);
        }
        public override void onLoadMore()
        {
            getBookDisscussionList(block, sort, distillate, start, limit);
        }
        void getBookDisscussionList(String block, String sort, String distillate, int start, int limit)
        {
            BookApi.Instance.getBookDisscussionList(block, "all", sort, "all", start.ToString(), limit.ToString(), distillate)
                .SubscribeOn(DefaultScheduler.Instance)
                .ObserveOn(Application.SynchronizationContext)
                .Subscribe(data => {
                    bool isRefresh = start == 0 ? true : false;
                    showBookDisscussionList(data.posts, isRefresh);
                }, e => {
                    LogUtils.e("BookDiscussionFragment", e.ToString());
                    showError();
                }, () => {
                    LogUtils.i("BookDiscussionFragment", "complete");
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