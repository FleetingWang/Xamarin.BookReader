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
    /// <summary>
    /// 别人家的排行榜
    /// </summary>
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
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
                    LogUtils.e("SubOtherHomeRankActivity", e.ToString());
                    showError();
                }, () => {
                    LogUtils.i("SubOtherHomeRankActivity", "complete");
                    complete();
                });
        }
    }
}