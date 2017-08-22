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
using Xamarin.BookReader.Helpers;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Xamarin.BookReader.Utils;

namespace Xamarin.BookReader.UI.Fragments
{
    /// <summary>
    /// 主题书单
    /// </summary>
    [Register("xamarin.bookreader.ui.fragments.SubjectFragment")]
    public class SubjectFragment : BaseRVFragment<BookLists.BookListsBean>
    {
        public static String BUNDLE_TAG = "tag";
        public static String BUNDLE_TAB = "tab";

        public String currendTag;
        public int currentTab;

        public String duration = "";
        public String sort = "";

        public static SubjectFragment newInstance(String tag, int tab)
        {
            SubjectFragment fragment = new SubjectFragment();
            Bundle bundle = new Bundle();
            bundle.PutString(BUNDLE_TAG, tag);
            bundle.PutInt(BUNDLE_TAB, tab);
            fragment.Arguments = (bundle);
            return fragment;
        }
        public override int LayoutResId => Resource.Layout.common_easy_recyclerview;

        public override void InitDatas()
        {
            MessageBus.Default.Register<TagEvent>(initCategoryList);
            currentTab = Arguments.GetInt(BUNDLE_TAB);
            switch (currentTab)
            {
                case 0:
                    duration = "last-seven-days";
                    sort = "collectorCount";
                    break;
                case 1:
                    duration = "all";
                    sort = "created";
                    break;
                case 2:
                default:
                    duration = "all";
                    sort = "collectorCount";
                    break;
            }
        }
        public override void ConfigViews()
        {
            initAdapter(new SubjectBookListAdapter(Activity), true, true);
            onRefresh();
        }
        public void showBookList(List<BookLists.BookListsBean> bookLists, bool isRefresh)
        {
            if (isRefresh)
            {
                mAdapter.clear();
                start = 0;
            }
            mAdapter.addAll(bookLists);
            start = start + bookLists.Count();
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
            var e = evnt as TagEvent;
            Activity.RunOnUiThread(() =>
            {
                currendTag = e.tag.ToString();
                if (UserVisibleHint)
                {
                    getBookLists(duration, sort, 0, limit, currendTag, Settings.UserChooseSex.ToString());
                }
            });
        }

        public override void onItemClick(int position)
        {
            SubjectBookListDetailActivity.startActivity(Activity, mAdapter.getItem(position));
        }
        public override void onRefresh()
        {
            base.onRefresh();
            getBookLists(duration, sort, 0, limit, currendTag, Settings.UserChooseSex.ToString());
        }
        public override void onLoadMore()
        {
            getBookLists(duration, sort, start, limit, currendTag, Settings.UserChooseSex.ToString());
        }
        void getBookLists(String duration, String sort, int start, int limit, String tag, String gender)
        {
            BookApi.Instance.getBookLists(duration, sort, start.ToString(), limit.ToString(), tag, gender)
                .SubscribeOn(DefaultScheduler.Instance)
                .ObserveOn(Application.SynchronizationContext)
                .Subscribe(data => {
                    showBookList(data.bookLists, start == 0 ? true : false);
                    if (data.bookLists == null || !data.bookLists.Any())
                    {
                        ToastUtils.showSingleToast("暂无相关书单");
                    }
                }, e => {
                    LogUtils.e("GirlBookDiscussionFragment", e.ToString());
                    showError();
                }, () => {
                    LogUtils.i("GirlBookDiscussionFragment", "complete");
                    complete();
                });
        }

        public override void OnDestroyView()
        {
            MessageBus.Default.DeRegister<TagEvent>(initCategoryList);
            base.OnDestroyView();
        }
    }
}