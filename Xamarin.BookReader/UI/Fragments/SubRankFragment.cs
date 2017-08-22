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
using Xamarin.BookReader.UI.EasyAdapters;
using Xamarin.BookReader.Datas;
using System.Reactive.Concurrency;
using Xamarin.BookReader.Utils;
using System.Reactive.Linq;

namespace Xamarin.BookReader.UI.Fragments
{
    /// <summary>
    /// 二级排行榜
    /// </summary>
    [Register("xamarin.bookreader.ui.fragments.SubRankFragment")]
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
            initAdapter(new SubCategoryAdapter(Activity), true, false);
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
            getRankList(id);
        }
        void getRankList(String id)
        {
            BookApi.Instance.getRanking(id)
                .SubscribeOn(DefaultScheduler.Instance)
                .ObserveOn(Application.SynchronizationContext)
                .Subscribe(data => {
                    List<Rankings.RankingBean.BooksBean> books = data.ranking.books;

                    BooksByCats cats = new BooksByCats();
                    cats.books = new List<BooksByCats.BooksBean>();
                    foreach (Rankings.RankingBean.BooksBean bean in books)
                    {
                        cats.books.Add(new BooksByCats.BooksBean(bean._id, bean.cover, bean.title, bean.author, bean.cat, bean.shortIntro, bean.latelyFollower, bean.retentionRatio));
                    }
                    showRankList(cats);
                }, e => {
                    LogUtils.e("SubRankFragment", e.ToString());
                    showError();
                }, () => {
                    LogUtils.i("SubRankFragment", "complete");
                    complete();
                });
        }

    }
}