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
using Xamarin.BookReader.UI.Adapters;
using Xamarin.BookReader.Utils;
using System.Reactive.Concurrency;
using Xamarin.BookReader.Datas;
using System.Reactive.Linq;
using Android.Content.PM;
using Xamarin.BookReader.Extensions;

namespace Xamarin.BookReader.UI.Activities
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
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
            mMaleCategoryListAdapter = new TopCategoryListAdapter(mContext, mMaleCategoryList, new ClickListener(this, Constant.Gender.Male.GetEnumDescription()));
            mFemaleCategoryListAdapter = new TopCategoryListAdapter(mContext, mFemaleCategoryList, new ClickListener(this, Constant.Gender.Female.GetEnumDescription()));
            mRvMaleCategory.SetAdapter(mMaleCategoryListAdapter);
            mRvFeMaleCategory.SetAdapter(mFemaleCategoryListAdapter);

            getCategoryList();
        }
        void getCategoryList()
        {
            BookApi.Instance.getCategoryList()
                .SubscribeOn(DefaultScheduler.Instance)
                .ObserveOn(Application.SynchronizationContext)
                .Subscribe(data => {
                    if (data != null)
                    {
                        showCategoryList(data);
                    }
                }, e => {
                    LogUtils.e("TopCategoryListActivity", e.ToString());
                    showError();
                }, () => {
                    LogUtils.i("TopCategoryListActivity", "complete");
                    complete();
                });
        }

        public void showCategoryList(CategoryList data)
        {
            mMaleCategoryList.Clear();
            mFemaleCategoryList.Clear();
            mMaleCategoryList.AddRange(data.male);
            mFemaleCategoryList.AddRange(data.female);
            mMaleCategoryListAdapter.NotifyDataSetChanged();
            mFemaleCategoryListAdapter.NotifyDataSetChanged();
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