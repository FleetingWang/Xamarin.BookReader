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

namespace Xamarin.BookReader.UI.Activities
{
    public class TopRankActivity: BaseActivity
    {
        //@Bind(Resource.Id.elvFeMale)
        ExpandableListView elvFeMale;
        //@Bind(Resource.Id.elvMale)
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

        }

        public override void initToolBar()
        {
            mCommonToolbar.SetTitle("排行榜");
            mCommonToolbar.SetNavigationIcon(Resource.Drawable.ab_back);
        }
        public override void initDatas()
        {
            maleAdapter = new TopRankAdapter(this, maleGroups, maleChilds);
            femaleAdapter = new TopRankAdapter(this, femaleGroups, femaleChilds);
            maleAdapter.setItemClickListener(new ClickListener());
            femaleAdapter.setItemClickListener(new ClickListener());
        }
        public override void configViews()
        {
            showDialog();
            elvMale.SetAdapter(maleAdapter);
            elvFeMale.SetAdapter(femaleAdapter);

            mPresenter.attachView(this);
            mPresenter.getRankList();
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
            List<RankingList.MaleBean> collapse = new ArrayList<>();
            for (RankingList.MaleBean bean : list)
            {
                if (bean.collapse)
                { // 折叠
                    collapse.add(bean);
                }
                else
                {
                    maleGroups.add(bean);
                    maleChilds.add(new ArrayList<RankingList.MaleBean>());
                }
            }
            if (collapse.Count() > 0)
            {
                maleGroups.add(new RankingList.MaleBean("别人家的排行榜"));
                maleChilds.add(collapse);
            }
            maleAdapter.notifyDataSetChanged();
        }

        private void updateFemale(RankingList rankingList)
        {
            List<RankingList.MaleBean> list = rankingList.female;
            List<RankingList.MaleBean> collapse = new ArrayList<>();
            for (RankingList.MaleBean bean : list)
            {
                if (bean.collapse)
                { // 折叠
                    collapse.add(bean);
                }
                else
                {
                    femaleGroups.add(bean);
                    femaleChilds.add(new ArrayList<RankingList.MaleBean>());
                }
            }
            if (collapse.Count() > 0)
            {
                femaleGroups.add(new RankingList.MaleBean("别人家的排行榜"));
                femaleChilds.add(collapse);
            }
            femaleAdapter.notifyDataSetChanged();
        }
        public void showError()
        {

        }
        public void complete()
        {
            dismissDialog();
        }
        class ClickListener: IOnRvItemClickListener<RankingList.MaleBean> {

            public void onItemClick(View view, int position, RankingList.MaleBean data) {
                if (data.monthRank == null) {
                    SubOtherHomeRankActivity.startActivity(mContext, data._id, data.title);
                } else {
                    SubRankActivity.startActivity(mContext, data._id, data.monthRank, data.totalRank, data.title);
                }
            }
        }
    }
}