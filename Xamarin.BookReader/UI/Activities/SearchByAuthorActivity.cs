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

        public static void startActivity(Context context, String author) {
            context.StartActivity(new Intent(context, typeof(SearchByAuthorActivity))
                    .PutExtra(INTENT_AUTHOR, author));
        }

        private String author = "";

        public override int getLayoutId()
        {
            return R.layout.activity_common_recyclerview;
        }
        public override void bindViews()
        {
            throw new NotImplementedException();
        }
        public override void initToolBar()
        {
            author = getIntent().getStringExtra(INTENT_AUTHOR);
            mCommonToolbar.setTitle(author);
            mCommonToolbar.setNavigationIcon(R.drawable.ab_back);
        }
        public override void initDatas()
        {
            initAdapter(SearchAdapter, false, false);
        }
        public override void configViews()
        {
            attachView(this);
            getSearchResultList(author);
        }

        public override void onItemClick(int position)
        {
            SearchDetail.SearchBooks data = mAdapter.getItem(position);
            BookDetailActivity.startActivity(this, data._id);
        }
        public void showSearchResultList(List<BooksByTag.TagBook> list)
        {
            List<SearchDetail.SearchBooks> mList = new ArrayList<>();
            for (BooksByTag.TagBook book : list)
            {
                mList.add(new SearchDetail.SearchBooks(book._id, book.title, book.author, book.cover, book.retentionRatio, book.latelyFollower));
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