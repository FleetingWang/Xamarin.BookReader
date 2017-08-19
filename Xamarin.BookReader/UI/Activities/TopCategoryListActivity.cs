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

namespace Xamarin.BookReader.UI.Activities
{
    public class TopCategoryListActivity: BaseActivity
    {
        //@Bind(R.id.rvMaleCategory)
        RecyclerView mRvMaleCategory;
        //@Bind(R.id.rvFemaleCategory)
        RecyclerView mRvFeMaleCategory;

        private TopCategoryListAdapter mMaleCategoryListAdapter;
        private TopCategoryListAdapter mFemaleCategoryListAdapter;
        private List<CategoryList.MaleBean> mMaleCategoryList = new List<CategoryList.MaleBean>();
        private List<CategoryList.MaleBean> mFemaleCategoryList = new List<CategoryList.MaleBean>();

        public override int getLayoutId()
        {
            return R.layout.activity_top_category_list;
        }
        public override void bindViews()
        {
            throw new NotImplementedException();
        }

        public override void initToolBar()
        {
            mCommonToolbar.setTitle(getString(R.string.category));
            mCommonToolbar.setNavigationIcon(R.drawable.ab_back);
        }
        public override void initDatas()
        {
        }
        public override void configViews()
        {
            showDialog();
            mRvMaleCategory.setHasFixedSize(true);
            mRvMaleCategory.setLayoutManager(new GridLayoutManager(this, 3));
            mRvMaleCategory.addItemDecoration(new SupportGridItemDecoration(this));
            mRvFeMaleCategory.setHasFixedSize(true);
            mRvFeMaleCategory.setLayoutManager(new GridLayoutManager(this, 3));
            mRvFeMaleCategory.addItemDecoration(new SupportGridItemDecoration(this));
            mMaleCategoryListAdapter = new TopCategoryListAdapter(mContext, mMaleCategoryList, new ClickListener(Constant.Gender.MALE));
            mFemaleCategoryListAdapter = new TopCategoryListAdapter(mContext, mFemaleCategoryList, new ClickListener(Constant.Gender.FEMALE));
            mRvMaleCategory.setAdapter(mMaleCategoryListAdapter);
            mRvFeMaleCategory.setAdapter(mFemaleCategoryListAdapter);

            mPresenter.attachView(this);
            mPresenter.getCategoryList();
        }

        public void showCategoryList(CategoryList data)
        {
            mMaleCategoryList.clear();
            mFemaleCategoryList.clear();
            mMaleCategoryList.addAll(data.male);
            mFemaleCategoryList.addAll(data.female);
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

        class ClickListener: IOnRvItemClickListener<CategoryList.MaleBean> {

            private String gender;

            public ClickListener(String gender) {
                this.gender = gender;
            }

            public void onItemClick(View view, int position, CategoryList.MaleBean data) {
                SubCategoryListActivity.startActivity(mContext, data.name, gender);
            }
        }

    }
}