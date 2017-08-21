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

namespace Xamarin.BookReader.UI.Activities
{
    /// <summary>
    /// 别人家的排行榜
    /// </summary>
    public class SubOtherHomeRankActivity : BaseRVActivity<BooksByCats.BooksBean>
    {
        public static String BUNDLE_ID = "_id";
        public static String INTENT_TITLE = "title";
        private String id;
        private String title;

        public static void startActivity(Context context, String id, String title)
        {
            context.StartActivity(new Intent(context, typeof(SubOtherHomeRankActivity))
                    .PutExtra(INTENT_TITLE, title)
                    .PutExtra(BUNDLE_ID, id));
        }

        public override int getLayoutId()
        {
            return Resource.Layout.activity_subject_book_list_detail;
        }
        public override void bindViews()
        {

        }

        public override void initToolBar()
        {
            title = Intent.GetStringExtra(INTENT_TITLE).Split(' ')[0];
            id = Intent.GetStringExtra(BUNDLE_ID);

            mCommonToolbar.Title = (title);
            mCommonToolbar.SetNavigationIcon(Resource.Drawable.ab_back);
        }
        public override void initDatas()
        {
        }
        public override void configViews()
        {
            initAdapter(new SubCategoryAdapter(this), true, false);
            onRefresh();
        }
        public void showRankList(BooksByCats data)
        {
            mAdapter.clear();
            mAdapter.addAll(data.books);
        }
        public void showError()
        {
            loaddingError();
        }
        public void complete()
        {
            mRecyclerView.setRefreshing(false);
        }

        public override void onItemClick(int position)
        {
            BookDetailActivity.startActivity(this, mAdapter.getItem(position)._id);
        }
        public override void onRefresh()
        {
            base.onRefresh();
            //TODO：mPresenter.getRankList(id);
        }
    }
}