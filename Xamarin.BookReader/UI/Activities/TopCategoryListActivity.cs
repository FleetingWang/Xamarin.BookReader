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
using Android.Support.V7.Widget;
using Xamarin.BookReader.Models;
using Xamarin.BookReader.UI.Listeners;
using Xamarin.BookReader.Views;

namespace Xamarin.BookReader.UI.Activities
{
    public class TopCategoryListActivity : BaseActivity
    {
        RecyclerView mRvMaleCategory;
        RecyclerView mRvFeMaleCategory;

        private TopCategoryListAdapter mMaleCategoryListAdapter;
        private TopCategoryListAdapter mFemaleCategoryListAdapter;
        private List<CategoryList.MaleBean> mMaleCategoryList = new List<CategoryList.MaleBean>();
        private List<CategoryList.MaleBean> mFemaleCategoryList = new List<CategoryList.MaleBean>();

        public override int getLayoutId()
        {
            return Resource.Layout.activity_top_category_list;
        }
        public override void bindViews()
        {
            mRvMaleCategory = FindViewById<RecyclerView>(Resource.Id.rvMaleCategory);
            mRvFeMaleCategory = FindViewById<RecyclerView>(Resource.Id.rvFemaleCategory);
        }

        public override void initToolBar()
        {
            mCommonToolbar.Title = (GetString(Resource.String.category));
            mCommonToolbar.SetNavigationIcon(Resource.Drawable.ab_back);
        }
        public override void initDatas()
        {
        }
        public override void configViews()
        {
            showDialog();
            mRvMaleCategory.HasFixedSize = (true);
            mRvMaleCategory.SetLayoutManager(new GridLayoutManager(this, 3));
            mRvMaleCategory.AddItemDecoration(new SupportGridItemDecoration(this));
            mRvFeMaleCategory.HasFixedSize = (true);
            mRvFeMaleCategory.SetLayoutManager(new GridLayoutManager(this, 3));
            mRvFeMaleCategory.AddItemDecoration(new SupportGridItemDecoration(this));
            mMaleCategoryListAdapter = new TopCategoryListAdapter(mContext, mMaleCategoryList, new ClickListener(this, Constant.Gender.Male.ToString()));
            mFemaleCategoryListAdapter = new TopCategoryListAdapter(mContext, mFemaleCategoryList, new ClickListener(this, Constant.Gender.Female.ToString()));
            mRvMaleCategory.SetAdapter(mMaleCategoryListAdapter);
            mRvFeMaleCategory.SetAdapter(mFemaleCategoryListAdapter);

            //TODO: mPresenter.getCategoryList();
        }

        public void showCategoryList(CategoryList data)
        {
            mMaleCategoryList.Clear();
            mFemaleCategoryList.Clear();
            mMaleCategoryList.AddRange(data.male);
            mFemaleCategoryList.AddRange(data.female);
            mMaleCategoryListAdapter.notifyDataSetChanged();
            mFemaleCategoryListAdapter.notifyDataSetChanged();
        }

        public void showError()
        {

        }
        public void complete()
        {
            dismissDialog();
        }

        class ClickListener : IOnRvItemClickListener<CategoryList.MaleBean>
        {
            private String gender;
            private TopCategoryListActivity topCategoryListActivity;

            public ClickListener(TopCategoryListActivity topCategoryListActivity, String gender)
            {
                this.topCategoryListActivity = topCategoryListActivity;
                this.gender = gender;
            }

            public void onItemClick(View view, int position, CategoryList.MaleBean data)
            {
                SubCategoryListActivity.startActivity(topCategoryListActivity, data.name, gender);
            }
        }

    }
}