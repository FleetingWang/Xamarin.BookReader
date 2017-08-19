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
using Xamarin.BookReader.Views;
using Android.Support.V4.View;

namespace Xamarin.BookReader.UI.Activities
{
    public class SubCategoryListActivity : BaseActivity
    {
        public static String INTENT_CATE_NAME = "name";
        public static String INTENT_GENDER = "gender";
        private String cate = "";
        private String gender = "";

        private String currentMinor = "";

        //@Bind(R.id.indicatorSub)
        RVPIndicator mIndicator;
        //@Bind(R.id.viewpagerSub)
        ViewPager mViewPager;

        private List<Fragment> mTabContents;
        private FragmentPagerAdapter mAdapter;
        private List<String> mDatas;

        private List<String> mMinors = new List<String>();
        private ListPopupWindow mListPopupWindow;
        private MinorAdapter minorAdapter;
        private String[] types = new String[] { Constant.CateType.NEW, Constant.CateType.HOT, Constant.CateType.REPUTATION, Constant.CateType.OVER };

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
            return R.layout.activity_sub_category_list;
        }
        public override void bindViews()
        {
            throw new NotImplementedException();
        }
        public override void initToolBar()
        {
            cate = getIntent().getStringExtra(INTENT_CATE_NAME);
            if (menuItem != null)
            {
                menuItem.setTitle(cate);
            }
            gender = getIntent().getStringExtra(INTENT_GENDER);
            mCommonToolbar.setTitle(cate);
            mCommonToolbar.setNavigationIcon(R.drawable.ab_back);
        }

        public override void initDatas()
        {
            mDatas = Arrays.asList(getResources().getStringArray(R.array.sub_tabs));

            mPresenter.attachView(this);
            mPresenter.getCategoryListLv2();

            mTabContents = new ArrayList<>();
            mTabContents.add(SubCategoryFragment.newInstance(cate, "", gender, Constant.CateType.NEW));
            mTabContents.add(SubCategoryFragment.newInstance(cate, "", gender, Constant.CateType.HOT));
            mTabContents.add(SubCategoryFragment.newInstance(cate, "", gender, Constant.CateType.REPUTATION));
            mTabContents.add(SubCategoryFragment.newInstance(cate, "", gender, Constant.CateType.OVER));

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
            mViewPager.setOffscreenPageLimit(4);
            mIndicator.setViewPager(mViewPager, 0);
            //mViewPager.addOnPageChangeListener(new ViewPager.OnPageChangeListener() {
            //    @Override
            //    public void onPageScrolled(int position, float positionOffset, int positionOffsetPixels) {

            //    }

            //    @Override
            //    public void onPageSelected(int position) {
            //        EventManager.refreshSubCategory(currentMinor, types[position]);
            //    }

            //    @Override
            //    public void onPageScrollStateChanged(int state) {

            //    }
            //});
        }

        public void showCategoryList(CategoryListLv2 data)
        {
            mMinors.clear();
            mMinors.add(cate);
            if (gender.equals(Constant.Gender.MALE))
            {
                for (CategoryListLv2.MaleBean bean : data.male)
                {
                    if (cate.equals(bean.major))
                    {
                        mMinors.addAll(bean.mins);
                        break;
                    }
                }
            }
            else
            {
                for (CategoryListLv2.MaleBean bean : data.female)
                {
                    if (cate.equals(bean.major))
                    {
                        mMinors.addAll(bean.mins);
                        break;
                    }
                }
            }
            minorAdapter = new MinorAdapter(this, mMinors);
            minorAdapter.setChecked(0);
            currentMinor = "";
            EventManager.refreshSubCategory(currentMinor, Constant.CateType.NEW);
        }

        public void showError()
        {

        }

        public void complete()
        {
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            getMenuInflater().inflate(R.menu.menu_sub_category, menu);
            menuItem = menu.findItem(R.id.menu_major);
            if (!TextUtils.isEmpty(cate))
            {
                menuItem.setTitle(cate);
            }
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.getItemId() == R.id.menu_major)
            {
                showMinorPopupWindow();
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void showMinorPopupWindow() {
            if (mMinors.size() > 0 && minorAdapter != null) {
                if (mListPopupWindow == null) {
                    mListPopupWindow = new ListPopupWindow(this);
                    mListPopupWindow.setAdapter(minorAdapter);
                    mListPopupWindow.setWidth(ViewGroup.LayoutParams.MATCH_PARENT);
                    mListPopupWindow.setHeight(ViewGroup.LayoutParams.WRAP_CONTENT);
                    mListPopupWindow.setAnchorView(mCommonToolbar);
                    mListPopupWindow.setModal(true);
                    //mListPopupWindow.setOnItemClickListener(new AdapterView.OnItemClickListener() {
                    //    @Override
                    //    public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                    //        minorAdapter.setChecked(position);
                    //        if (position > 0) {
                    //            currentMinor = mMinors.get(position);
                    //        } else {
                    //            currentMinor = "";
                    //        }
                    //        int current = mViewPager.getCurrentItem();
                    //        EventManager.refreshSubCategory(currentMinor, types[current]);
                    //        mListPopupWindow.dismiss();
                    //        mCommonToolbar.setTitle(mMinors.get(position));
                    //    }
                    //});
                }
                mListPopupWindow.show();
            }
        }


    }
}