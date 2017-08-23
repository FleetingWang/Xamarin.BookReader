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
using Newtonsoft.Json;
using Xamarin.BookReader.UI.EasyAdapters;
using Xamarin.BookReader.Datas;
using System.Reactive.Concurrency;
using Xamarin.BookReader.Utils;
using System.Reactive.Linq;
using Android.Content.PM;

namespace Xamarin.BookReader.UI.Activities
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class BookSourceActivity : BaseRVActivity<BookSource>
    {
        public static String INTENT_BOOK_ID = "bookId";

        public static void startActivityForResult(Activity activity, String bookId, int reqId)
        {
            activity.StartActivityForResult(new Intent(activity, typeof(BookSourceActivity))
                    .PutExtra(INTENT_BOOK_ID, bookId), reqId);
        }
        private String bookId = "";


        public override int getLayoutId()
        {
            return Resource.Layout.activity_common_recyclerview;
        }

        public override void initToolBar()
        {
            bookId = Intent.GetStringExtra(INTENT_BOOK_ID);
            mCommonToolbar.Title = ("选择来源");
            mCommonToolbar.SetNavigationIcon(Resource.Drawable.ab_back);
        }
        public override void initDatas()
        {
            initAdapter(new BookSourceAdapter(this), false, false);
        }
        public override void configViews()
        {
            getBookSource("summary", bookId);

            new AlertDialog.Builder(this)
                .SetMessage("换源功能暂未实现，后续更新...")
                .SetPositiveButton("OK", (sender, e) =>
                {
                    var dialog = sender as AlertDialog;
                    dialog?.Dismiss();
                })
            .Create().Show();
        }

        void getBookSource(String viewSummary, String book)
        {
            BookApi.Instance.getBookSource(viewSummary, book)
                .SubscribeOn(DefaultScheduler.Instance)
                .ObserveOn(Application.SynchronizationContext)
                .Subscribe(data => {
                    if (data != null)
                    {
                        showBookSource(data);
                    }
                }, e => {
                    LogUtils.e("BookSourceActivity", e.ToString());
                }, () => {

                });
        }

        public override void onItemClick(int position)
        {
            BookSource data = mAdapter.getItem(position);
            Intent intent = new Intent();
            intent.PutExtra("source", JsonConvert.SerializeObject(data));
            SetResult(Result.Ok, intent);
            Finish();
        }

        public void showBookSource(List<BookSource> list)
        {
            mAdapter.clear();
            mAdapter.addAll(list);
        }

        public void showError()
        {
            loaddingError();
        }

        public void complete()
        {
            mRecyclerView.setRefreshing(false);
        }


    }
}