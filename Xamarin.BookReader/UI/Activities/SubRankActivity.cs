using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.BookReader.Bases;
using Android.Support.V4.View;
using Xamarin.BookReader.Views;
using Android.Support.V4.App;

namespace Xamarin.BookReader.UI.Activities
{
    public class SubRankActivity: BaseActivity
    {
        public static String INTENT_WEEK = "_id";
        public static String INTENT_MONTH = "month";
        public static String INTENT_ALL = "all";
        public static String INTENT_TITLE = "title";

        public static void startActivity(Context context, String week, String month, String all, String title) {
            context.StartActivity(new Intent(context, typeof(SubRankActivity))
                    .PutExtra(INTENT_WEEK, week)
                    .PutExtra(INTENT_MONTH, month)
                    .PutExtra(INTENT_ALL, all)
                    .PutExtra(INTENT_TITLE, title));
        }


        private String week;
        private String month;
        private String all;
        private String title;

        //@Bind(R.id.indicatorSubRank)
        RVPIndicator mIndicator;
        //@Bind(R.id.viewpagerSubRank)
        ViewPager mViewPager;

        private List<Fragment> mTabContents;
        private FragmentPagerAdapter mAdapter;
        private List<String> mDatas;

        public override int getLayoutId()
        {
            return R.layout.activity_sub_rank;
        }

        public override void initToolBar()
        {
            week = getIntent().getStringExtra(INTENT_WEEK);
            month = getIntent().getStringExtra(INTENT_MONTH);
            all = getIntent().getStringExtra(INTENT_ALL);

            title = getIntent().getStringExtra(INTENT_TITLE).split(" ")[0];
            mCommonToolbar.setTitle(title);
            mCommonToolbar.setNavigationIcon(R.drawable.ab_back);
        }

        public override void bindViews()
        {
        }

        public override void initDatas()
        {
            mDatas = Arrays.asList(getResources().getStringArray(R.array.sub_rank_tabs));

            mTabContents = new ArrayList<>();
            mTabContents.add(SubRankFragment.newInstance(week));
            mTabContents.add(SubRankFragment.newInstance(month));
            mTabContents.add(SubRankFragment.newInstance(all));

            //mAdapter = new FragmentPagerAdapter(getSupportFragmentManager()) {
            //    @Override
            //    public int getCount() {
            //        return mTabContents.size();
            //    }

            //    @Override
            //    public Fragment getItem(int position) {
            //        return mTabContents.get(position);
            //    }
            //};
        }

        public override void configViews()
        {
            mIndicator.setTabItemTitles(mDatas);
            mViewPager.setAdapter(mAdapter);
            mViewPager.setOffscreenPageLimit(3);
            mIndicator.setViewPager(mViewPager, 0);
        }
    }
}