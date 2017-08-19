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

namespace Xamarin.BookReader.UI.Activities
{
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
        public override void bindViews()
        {
            throw new NotImplementedException();
        }
        public override void initToolBar()
        {
            author = Intent.GetStringExtra(INTENT_AUTHOR);
            mCommonToolbar.Title = (author);
            mCommonToolbar.SetNavigationIcon(Resource.Drawable.ab_back);
        }
        public override void initDatas()
        {
            initAdapter(SearchAdapter, false, false);
        }
        public override void configViews()
        {
            //TODO: mPresenter.getSearchResultList(author);
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