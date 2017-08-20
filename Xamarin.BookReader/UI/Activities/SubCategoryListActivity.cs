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
using Xamarin.BookReader.Views;
using Android.Support.V4.View;
using Xamarin.BookReader.Managers;
using Xamarin.BookReader.Models;
using Android.Text;
using Android.Support.V4.App;

namespace Xamarin.BookReader.UI.Activities
{
    public class SubCategoryListActivity : BaseActivity
    {
        public static String INTENT_CATE_NAME = "name";
        public static String INTENT_GENDER = "gender";
        private String cate = "";
        private String gender = "";

        private String currentMinor = "";

        RVPIndicator mIndicator;
        ViewPager mViewPager;

        private List<Fragment> mTabContents;
        private FragmentPagerAdapter mAdapter;
        private List<String> mDatas;

        private List<String> mMinors = new List<String>();
        private ListPopupWindow mListPopupWindow;
        private MinorAdapter minorAdapter;
        private String[] types = new String[] {
            Constant.CateType.New.ToString(),
            Constant.CateType.Hot.ToString(),
            Constant.CateType.Reputation.ToString(),
            Constant.CateType.Over.ToString()
        };

        private IMenuItem menuItem = null;

        public static void startActivity(Context context, String name, String gender)
        {
            Intent intent = new Intent(context, typeof(SubCategoryListActivity));
            intent.PutExtra(INTENT_CATE_NAME, name);
            intent.PutExtra(INTENT_GENDER, gender);
            context.StartActivity(intent);
        }

        public override int getLayoutId()
        {
            return Resource.Layout.activity_sub_category_list;
        }
        public override void bindViews()
        {
            mIndicator = FindViewById<RVPIndicator>(Resource.Id.indicatorSub);
            mViewPager = FindViewById<ViewPager>(Resource.Id.viewpagerSub);
        }
        public override void initToolBar()
        {
            cate = Intent.GetStringExtra(INTENT_CATE_NAME);
            if (menuItem != null)
            {
                menuItem.SetTitle(cate);
            }
            gender = Intent.GetStringExtra(INTENT_GENDER);
            mCommonToolbar.Title = (cate);
            mCommonToolbar.SetNavigationIcon(Resource.Drawable.ab_back);
        }

        public override void initDatas()
        {
            mDatas = Resources.GetStringArray(Resource.Array.sub_tabs).ToList();

            //TODO: mPresenter.attachView(this);
            //TODO: mPresenter.getCategoryListLv2();

            mTabContents = new List<Fragment>();
            mTabContents.Add(SubCategoryFragment.newInstance(cate, "", gender, Constant.CateType.New));
            mTabContents.Add(SubCategoryFragment.newInstance(cate, "", gender, Constant.CateType.Hot));
            mTabContents.Add(SubCategoryFragment.newInstance(cate, "", gender, Constant.CateType.Reputation));
            mTabContents.Add(SubCategoryFragment.newInstance(cate, "", gender, Constant.CateType.Over));

            mAdapter = new CustomFragmentPagerAdapter(SupportFragmentManager, mTabContents);
        }

        public override void configViews()
        {
            mIndicator.setTabItemTitles(mDatas);
            mViewPager.Adapter = mAdapter;
            mViewPager.OffscreenPageLimit = (4);
            mIndicator.setViewPager(mViewPager, 0);
            mViewPager.PageSelected += (sender, e) =>
            {
                EventManager.refreshSubCategory(currentMinor, types[e.Position]);
            };
        }

        public void showCategoryList(CategoryListLv2 data)
        {
            mMinors.Clear();
            mMinors.Add(cate);
            if (gender.Equals(Constant.Gender.Male.ToString()))
            {
                foreach (CategoryListLv2.MaleBean bean in data.male)
                {
                    if (cate.Equals(bean.major))
                    {
                        mMinors.AddRange(bean.mins);
                        break;
                    }
                }
            }
            else
            {
                foreach (CategoryListLv2.MaleBean bean in data.female)
                {
                    if (cate.Equals(bean.major))
                    {
                        mMinors.AddRange(bean.mins);
                        break;
                    }
                }
            }
            minorAdapter = new MinorAdapter(this, mMinors);
            minorAdapter.setChecked(0);
            currentMinor = "";
            EventManager.refreshSubCategory(currentMinor, Constant.CateType.New.ToString());
        }

        public void showError()
        {

        }

        public void complete()
        {
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_sub_category, menu);
            menuItem = menu.FindItem(Resource.Id.menu_major);
            if (!TextUtils.IsEmpty(cate))
            {
                menuItem.SetTitle(cate);
            }
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.menu_major)
            {
                showMinorPopupWindow();
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void showMinorPopupWindow()
        {
            if (mMinors.Count() > 0 && minorAdapter != null)
            {
                if (mListPopupWindow == null)
                {
                    mListPopupWindow = new ListPopupWindow(this);
                    mListPopupWindow.SetAdapter(minorAdapter);
                    mListPopupWindow.Width = (ViewGroup.LayoutParams.MatchParent);
                    mListPopupWindow.Height = (ViewGroup.LayoutParams.WrapContent);
                    mListPopupWindow.AnchorView = (mCommonToolbar);
                    mListPopupWindow.Modal = (true);
                    mListPopupWindow.ItemClick += (sender, e) =>
                    {
                        var position = e.Position;
                        minorAdapter.SetChecked(position);
                        if (position > 0)
                        {
                            currentMinor = mMinors[position];
                        }
                        else
                        {
                            currentMinor = "";
                        }
                        int current = mViewPager.CurrentItem;
                        EventManager.refreshSubCategory(currentMinor, types[current]);
                        mListPopupWindow.Dismiss();
                        mCommonToolbar.Title = (mMinors[position]);
                    };
                }
                mListPopupWindow.Show();
            }
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