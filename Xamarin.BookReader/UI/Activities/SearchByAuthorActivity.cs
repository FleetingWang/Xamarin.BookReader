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
using Xamarin.BookReader.UI.EasyAdapters;
using Xamarin.BookReader.Datas;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Xamarin.BookReader.Utils;
using Android.Content.PM;

namespace Xamarin.BookReader.UI.Activities
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class SearchByAuthorActivity : BaseRVActivity<SearchDetail.SearchBooks>
    {
        public static String INTENT_AUTHOR = "author";

        public static void startActivity(Context context, String author)
        {
            context.StartActivity(new Intent(context, typeof(SearchByAuthorActivity))
                    .PutExtra(INTENT_AUTHOR, author));
        }

        private String author = "";

        public override int getLayoutId()
        {
            return Resource.Layout.activity_common_recyclerview;
        }

        public override void initToolBar()
        {
            author = Intent.GetStringExtra(INTENT_AUTHOR);
            mCommonToolbar.Title = (author);
            mCommonToolbar.SetNavigationIcon(Resource.Drawable.ab_back);
        }
        public override void initDatas()
        {
            initAdapter(new SearchAdapter(this), false, false);
        }
        public override void configViews()
        {
            getSearchResultList(author);
        }
        void getSearchResultList(String author)
        {
            BookApi.Instance.searchBooksByAuthor(author)
                .SubscribeOn(DefaultScheduler.Instance)
                .ObserveOn(Application.SynchronizationContext)
                .Subscribe(data => {
                    showSearchResultList(data.books);
                }, e => {
                    LogUtils.e("SearchByAuthorActivity", e.ToString());
                    showError();
                }, () => {
                    LogUtils.i("SearchByAuthorActivity", "complete");
                    complete();
                });
        }

        public override void onItemClick(int position)
        {
            SearchDetail.SearchBooks data = mAdapter.getItem(position);
            BookDetailActivity.startActivity(this, data._id);
        }
        public void showSearchResultList(List<BooksByTag.TagBook> list)
        {
            List<SearchDetail.SearchBooks> mList = new List<SearchDetail.SearchBooks>();
            foreach (BooksByTag.TagBook book in list)
            {
                mList.Add(new SearchDetail.SearchBooks(book._id, book.title, book.author, book.cover, book.retentionRatio, book.latelyFollower));
            }
            mAdapter.clear();
            mAdapter.addAll(mList);
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