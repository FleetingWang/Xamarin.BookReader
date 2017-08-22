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
using Xamarin.BookReader.UI.Fragments;
using Android.Content.PM;
using AndroidApp = Android.App;

namespace Xamarin.BookReader.UI.Activities
{
    [AndroidApp.Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class SubRankActivity : BaseActivity
    {
        public static String INTENT_WEEK = "_id";
        public static String INTENT_MONTH = "month";
        public static String INTENT_ALL = "all";
        public static String INTENT_TITLE = "title";

        public static void startActivity(Context context, String week, String month, String all, String title)
        {
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

        RVPIndicator mIndicator;
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

            title = Intent.GetStringExtra(INTENT_TITLE).Split(' ')[0];
            mCommonToolbar.Title = (title);
            mCommonToolbar.SetNavigationIcon(Resource.Drawable.ab_back);
        }

        public override void bindViews()
        {
            mIndicator = FindViewById<RVPIndicator>(Resource.Id.indicatorSubRank);
            mViewPager = FindViewById<ViewPager>(Resource.Id.viewpagerSubRank);
        }

        public override void initDatas()
        {
            mDatas = Resources.GetStringArray(Resource.Array.sub_rank_tabs).ToList();

            mTabContents = new List<Fragment>();
            mTabContents.Add(SubRankFragment.newInstance(week));
            mTabContents.Add(SubRankFragment.newInstance(month));
            mTabContents.Add(SubRankFragment.newInstance(all));
            mAdapter = new CustomFragmentPagerAdapter(SupportFragmentManager, mTabContents);
        }

        public override void configViews()
        {
            mIndicator.setTabItemTitles(mDatas);
            mViewPager.Adapter = (mAdapter);
            mViewPager.OffscreenPageLimit = (3);
            mIndicator.setViewPager(mViewPager, 0);
        }

        class CustomFragmentPagerAdapter : FragmentPagerAdapter
        {
            private List<Fragment> _mTabContents;
            public CustomFragmentPagerAdapter(FragmentManager fm, List<Fragment> mTabContents)
                : base(fm)
            {
                _mTabContents = mTabContents;
            }

            public override int Count => _mTabContents.Count();

            public override Fragment GetItem(int position)
            {
                return _mTabContents[position];
            }
        }
    }
}