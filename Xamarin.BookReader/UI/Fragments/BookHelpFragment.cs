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
    [Register("xamarin.bookreader.ui.fragments.BookHelpFragment")]
    public class BookHelpFragment : BaseRVFragment<BookHelpList.HelpsBean>
    {
        private String sort = Constant.SortType.Default.GetEnumDescription();
        private String distillate = Constant.Distillate.All.GetEnumDescription();

        public override int LayoutResId => Resource.Layout.common_easy_recyclerview;

        public override void InitDatas()
        {
            MessageBus.Default.Register<SelectionEvent>(initCategoryList);
        }
        public override void ConfigViews()
        {
            initAdapter(new BookHelpAdapter(Activity), true, true);
            onRefresh();
        }
        public void showBookHelpList(List<BookHelpList.HelpsBean> list, bool isRefresh)
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
                distillate = e.distillate.GetEnumDescription();
                onRefresh();
            });
        }
        public override void onRefresh()
        {
            base.onRefresh();
            getBookHelpList(sort, distillate, 0, limit);
        }
        public override void onLoadMore()
        {
            getBookHelpList(sort, distillate, start, limit);
        }
        void getBookHelpList(String sort, String distillate, int start, int limit)
        {
            BookApi.Instance.getBookHelpList("all", sort, start.ToString(), limit.ToString(), distillate)
                .SubscribeOn(DefaultScheduler.Instance)
                .ObserveOn(Application.SynchronizationContext)
                .Subscribe(data => {
                    bool isRefresh = start == 0 ? true : false;
                    showBookHelpList(data.helps, isRefresh);
                }, e => {
                    LogUtils.e("BookHelpFragment", e.ToString());
                    showError();
                }, () => {
                    LogUtils.i("BookHelpFragment", "complete");
                    complete();
                });
        }

        public override void onItemClick(int position)
        {
            BookHelpList.HelpsBean data = mAdapter.getItem(position);
            BookHelpDetailActivity.startActivity(Activity, data._id);
        }
        public override void OnDestroyView()
        {
            base.OnDestroyView();
            MessageBus.Default.DeRegister<SelectionEvent>(initCategoryList);
        }
    }
}