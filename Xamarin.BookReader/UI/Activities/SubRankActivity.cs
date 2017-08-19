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

        //@Bind(Resource.Id.indicatorSubRank)
        RVPIndicator mIndicator;
        //@Bind(Resource.Id.viewpagerSubRank)
        ViewPager mViewPager;

        private List<Fragment> mTabContents;
        private FragmentPagerAdapter mAdapter;
        private List<String> mDatas;

        public override int getLayoutId()
        {
            return Resource.Layout.activity_sub_rank;
        }

        public override void initToolBar()
        {
            week = Intent.GetStringExtra(INTENT_WEEK);
            month = Intent.GetStringExtra(INTENT_MONTH);
            all = Intent.GetStringExtra(INTENT_ALL);

            title = Intent.GetStringExtra(INTENT_TITLE).split(" ")[0];
            mCommonToolbar.SetTitle(title);
            mCommonToolbar.SetNavigationIcon(Resource.Drawable.ab_back);
        }

        public override void bindViews()
        {
        }

        public override void initDatas()
        {
            mDatas = Arrays.asList(Resources.GetStringArray(Resource.Array.sub_rank_tabs));

            mTabContents = new ArrayList<>();
            mTabContents.add(SubRankFragment.newInstance(week));
            mTabContents.add(SubRankFragment.newInstance(month));
            mTabContents.add(SubRankFragment.newInstance(all));

            //mAdapter = new FragmentPagerAdapter(getSupportFragmentManager()) {
            //    @Override
            //    public int getCount() {
            //        return mTabContents.Count();
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
            mViewPager.SetAdapter(mAdapter);
            mViewPager.setOffscreenPageLimit(3);
            mIndicator.setViewPager(mViewPager, 0);
        }
    }
}