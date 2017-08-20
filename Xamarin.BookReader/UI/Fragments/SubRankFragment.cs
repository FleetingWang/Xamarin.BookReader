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
using Xamarin.BookReader.UI.Activities;

namespace Xamarin.BookReader.UI.Fragments
{
    /// <summary>
    /// 二级排行榜
    /// </summary>
    public class SubRankFragment : BaseRVFragment<BooksByCats.BooksBean>
    {
        public static String BUNDLE_ID = "_id";

        public static SubRankFragment newInstance(String id)
        {
            SubRankFragment fragment = new SubRankFragment();
            Bundle bundle = new Bundle();
            bundle.PutString(BUNDLE_ID, id);
            fragment.Arguments = (bundle);
            return fragment;
        }

        private String id;

        public override int LayoutResId => Resource.Layout.common_easy_recyclerview;

        public override void InitDatas()
        {
            id = Arguments.GetString(BUNDLE_ID);
        }
        public override void ConfigViews()
        {
            initAdapter(/*new SubCategoryAdapter(),*/ true, false);
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
            BookDetailActivity.startActivity(Activity, mAdapter.getItem(position)._id);
        }
        public override void onRefresh()
        {
            base.onRefresh();
            //TODO: mPresenter.getRankList(id);
        }
    }
}