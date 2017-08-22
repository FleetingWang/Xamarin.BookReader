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
using Xamarin.BookReader.UI.Listeners;
using Xamarin.BookReader.UI.Adapters;
using Xamarin.BookReader.Datas;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Xamarin.BookReader.Utils;
using Android.Content.PM;

namespace Xamarin.BookReader.UI.Activities
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class TopRankActivity : BaseActivity
    {
        ExpandableListView elvFeMale;
        ExpandableListView elvMale;

        private List<RankingList.MaleBean> maleGroups = new List<RankingList.MaleBean>();
        private List<List<RankingList.MaleBean>> maleChilds = new List<List<RankingList.MaleBean>>();
        private TopRankAdapter maleAdapter;

        private List<RankingList.MaleBean> femaleGroups = new List<RankingList.MaleBean>();
        private List<List<RankingList.MaleBean>> femaleChilds = new List<List<RankingList.MaleBean>>();
        private TopRankAdapter femaleAdapter;

        public static void startActivity(Context context)
        {
            Intent intent = new Intent(context, typeof(TopRankActivity));
            context.StartActivity(intent);
        }

        public override int getLayoutId()
        {
            return Resource.Layout.activity_top_rank;
        }

        public override void bindViews()
        {
            elvFeMale = FindViewById<ExpandableListView>(Resource.Id.elvFeMale);
            elvMale = FindViewById<ExpandableListView>(Resource.Id.elvMale);
        }

        public override void initToolBar()
        {
            mCommonToolbar.Title = ("排行榜");
            mCommonToolbar.SetNavigationIcon(Resource.Drawable.ab_back);
        }
        public override void initDatas()
        {
            maleAdapter = new TopRankAdapter(this, maleGroups, maleChilds);
            femaleAdapter = new TopRankAdapter(this, femaleGroups, femaleChilds);
            maleAdapter.setItemClickListener(new ClickListener(this));
            femaleAdapter.setItemClickListener(new ClickListener(this));
        }
        public override void configViews()
        {
            showDialog();
            elvMale.SetAdapter(maleAdapter);
            elvFeMale.SetAdapter(femaleAdapter);

            getRankList();
        }
        void getRankList()
        {
            BookApi.Instance.getRanking()
                .SubscribeOn(DefaultScheduler.Instance)
                .ObserveOn(Application.SynchronizationContext)
                .Subscribe(data => {
                    if (data != null)
                    {
                        showRankList(data);
                    }
                }, e => {
                    LogUtils.e("TopRankActivity", e.ToString());
                    showError();
                }, () => {
                    LogUtils.i("TopRankActivity", "complete");
                    complete();
                });
        }

        public void showRankList(RankingList rankingList)
        {
            maleGroups.Clear();
            femaleGroups.Clear();
            updateMale(rankingList);
            updateFemale(rankingList);
        }

        private void updateMale(RankingList rankingList)
        {
            List<RankingList.MaleBean> list = rankingList.male;
            List<RankingList.MaleBean> collapse = new List<RankingList.MaleBean>();
            foreach (RankingList.MaleBean bean in list)
            {
                if (bean.collapse)
                { // 折叠
                    collapse.Add(bean);
                }
                else
                {
                    maleGroups.Add(bean);
                    maleChilds.Add(new List<RankingList.MaleBean>());
                }
            }
            if (collapse.Count() > 0)
            {
                maleGroups.Add(new RankingList.MaleBean("别人家的排行榜"));
                maleChilds.Add(collapse);
            }
            maleAdapter.NotifyDataSetChanged();
        }

        private void updateFemale(RankingList rankingList)
        {
            List<RankingList.MaleBean> list = rankingList.female;
            List<RankingList.MaleBean> collapse = new List<RankingList.MaleBean>();
            foreach (RankingList.MaleBean bean in list)
            {
                if (bean.collapse)
                { // 折叠
                    collapse.Add(bean);
                }
                else
                {
                    femaleGroups.Add(bean);
                    femaleChilds.Add(new List<RankingList.MaleBean>());
                }
            }
            if (collapse.Count() > 0)
            {
                femaleGroups.Add(new RankingList.MaleBean("别人家的排行榜"));
                femaleChilds.Add(collapse);
            }
            femaleAdapter.NotifyDataSetChanged();
        }
        public void showError()
        {

        }
        public void complete()
        {
            dismissDialog();
        }
        class ClickListener : IOnRvItemClickListener<RankingList.MaleBean>
        {
            private Context mContext;
            public ClickListener(Context mContext)
            {
                this.mContext = mContext;
            }

            public void onItemClick(View view, int position, RankingList.MaleBean data)
            {
                if (data.monthRank == null)
                {
                    SubOtherHomeRankActivity.startActivity(mContext, data._id, data.title);
                }
                else
                {
                    SubRankActivity.startActivity(mContext, data._id, data.monthRank, data.totalRank, data.title);
                }
            }
        }
    }
}