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
using Xamarin.BookReader.UI.Listeners;
using Android.Support.V7.Widget;
using Android.Support.V4.View;
using Xamarin.BookReader.Views;
using Xamarin.BookReader.Models;
using Android.Support.V4.App;
using DSoft.Messaging;
using Xamarin.BookReader.Models.Support;
using Xamarin.BookReader.Utils;
using Android.Views.Animations;
using Xamarin.BookReader.UI.Adapters;
using Xamarin.BookReader.UI.Fragments;
using Xamarin.BookReader.Datas;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using AppApplication = Android.App.Application;
using Android.Content.PM;
using AndroidApp = Android.App;

namespace Xamarin.BookReader.UI.Activities
{
    [AndroidApp.Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class SubjectBookListActivity : BaseActivity, IOnRvItemClickListener<String>
    {
        RVPIndicator mIndicator;
        ViewPager mViewPager;
        ReboundScrollView rsvTags;
        RecyclerView rvTags;

        private SubjectTagsAdapter mTagAdapter;
        private List<BookListTags.DataBean> mTagList = new List<BookListTags.DataBean>();

        private List<Fragment> mTabContents;
        private FragmentPagerAdapter mAdapter;
        private List<String> mDatas;

        private String currentTag = "";

        public static void startActivity(Context context)
        {
            Intent intent = new Intent(context, typeof(SubjectBookListActivity));
            context.StartActivity(intent);
        }

        public override int getLayoutId()
        {
            return Resource.Layout.activity_subject_book_list_tag;
        }
        public override void bindViews()
        {
            mIndicator = FindViewById<RVPIndicator>(Resource.Id.indicatorSubject);
            mViewPager = FindViewById<ViewPager>(Resource.Id.viewpagerSubject);
            rsvTags = FindViewById<ReboundScrollView>(Resource.Id.rsvTags);
            rvTags = FindViewById<RecyclerView>(Resource.Id.rvTags);
        }

        public override void initToolBar()
        {
            mCommonToolbar.SetTitle(Resource.String.subject_book_list);
            mCommonToolbar.SetNavigationIcon(Resource.Drawable.ab_back);
        }
        public override void initDatas()
        {
            mDatas = Resources.GetStringArray(Resource.Array.subject_tabs).ToList();

            mTabContents = new List<Fragment>();
            mTabContents.Add(SubjectFragment.newInstance("", 0));
            mTabContents.Add(SubjectFragment.newInstance("", 1));
            mTabContents.Add(SubjectFragment.newInstance("", 2));
            mAdapter = new CustomFragmentPagerAdapter(SupportFragmentManager, mTabContents);
        }
        public override void configViews()
        {
            mIndicator.setTabItemTitles(mDatas);
            mViewPager.Adapter = (mAdapter);
            mIndicator.setViewPager(mViewPager, 0);

            rvTags.HasFixedSize = (true);
            rvTags.SetLayoutManager(new LinearLayoutManager(this));
            rvTags.AddItemDecoration(new SupportDividerItemDecoration(this, LinearLayoutManager.Vertical));
            mTagAdapter = new SubjectTagsAdapter(this, mTagList);
            mTagAdapter.setItemClickListener(this);
            rvTags.SetAdapter(mTagAdapter);

            getBookListTags();
        }
        void getBookListTags()
        {
            BookApi.Instance.getBookListTags()
                .SubscribeOn(DefaultScheduler.Instance)
                .ObserveOn(AppApplication.SynchronizationContext)
                .Subscribe(data => {
                    showBookListTags(data);
                }, e => {
                    LogUtils.e("SubjectBookListActivity", e.ToString());
                    showError();
                }, () => {
                    LogUtils.i("SubjectBookListActivity", "complete");
                    complete();
                });
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_subject, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.menu_tags)
            {
                if (isVisible(rsvTags))
                {
                    hideTagGroup();
                }
                else
                {
                    showTagGroup();
                }
                return true;
            }
            else if (item.ItemId == Resource.Id.menu_my_book_list)
            {
                StartActivity(new Intent(this, typeof(MyBookListActivity)));
            }
            return base.OnOptionsItemSelected(item);
        }

        public override void OnBackPressed()
        {
            if (isVisible(rsvTags))
            {
                hideTagGroup();
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public void showBookListTags(BookListTags data)
        {
            mTagList.Clear();
            mTagList.AddRange(data.data);
            mTagAdapter.NotifyDataSetChanged();
        }

        public void showError()
        {

        }
        public void complete()
        {
        }

        public void onItemClick(View view, int position, string data)
        {
            hideTagGroup();
            currentTag = data;
            MessageBus.Default.Post(new TagEvent(currentTag));
        }

        private void showTagGroup()
        {
            if (!mTagList.Any())
            {
                ToastUtils.showToast(GetString(Resource.String.network_error_tips));
                return;
            }
            Animation mShowAction = new TranslateAnimation(Dimension.RelativeToSelf, 0.0f,
                    Dimension.RelativeToSelf, 0.0f,
                    Dimension.RelativeToSelf, -1.0f,
                    Dimension.RelativeToSelf, 0.0f);
            mShowAction.Duration = (400);
            rsvTags.StartAnimation(mShowAction);
            rsvTags.Visibility = ViewStates.Visible;
        }

        private void hideTagGroup()
        {
            Animation mHiddenAction = new TranslateAnimation(Dimension.RelativeToSelf, 0.0f,
                    Dimension.RelativeToSelf, 0.0f,
                    Dimension.RelativeToSelf, 0.0f,
                    Dimension.RelativeToSelf, -1.0f);
            mHiddenAction.Duration = (400);
            rsvTags.StartAnimation(mHiddenAction);
            rsvTags.Visibility = ViewStates.Gone;
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