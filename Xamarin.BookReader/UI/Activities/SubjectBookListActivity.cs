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

namespace Xamarin.BookReader.UI.Activities
{
    public class SubjectBookListActivity : BaseActivity, IOnRvItemClickListener<String>
    {
        //@Bind(Resource.Id.indicatorSubject)
        RVPIndicator mIndicator;
        //@Bind(Resource.Id.viewpagerSubject)
        ViewPager mViewPager;
        //@Bind(Resource.Id.rsvTags)
        ReboundScrollView rsvTags;

        //@Bind(Resource.Id.rvTags)
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
            throw new NotImplementedException();
        }

        public override void initToolBar()
        {
            mCommonToolbar.SetTitle(Resource.String.subject_book_list);
            mCommonToolbar.SetNavigationIcon(Resource.Drawable.ab_back);
        }
        public override void initDatas()
        {
            mDatas = Arrays.asList(Resources.GetStringArray(Resource.Array.subject_tabs));

            mTabContents = new ArrayList<>();
            mTabContents.add(SubjectFragment.newInstance("", 0));
            mTabContents.add(SubjectFragment.newInstance("", 1));
            mTabContents.add(SubjectFragment.newInstance("", 2));

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
            mIndicator.setViewPager(mViewPager, 0);

            rvTags.HasFixedSize = (true);
            rvTags.SetLayoutManager(new LinearLayoutManager(this));
            rvTags.AddItemDecoration(new SupportDividerItemDecoration(this, LinearLayoutManager.VERTICAL));
            mTagAdapter = new SubjectTagsAdapter(this, mTagList);
            mTagAdapter.setItemClickListener(this);
            rvTags.SetAdapter(mTagAdapter);

            mPresenter.attachView(this);
            mPresenter.getBookListTags();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_subject, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.menu_tags) {
            if (isVisible(rsvTags)) {
                hideTagGroup();
            } else {
                showTagGroup();
            }
            return true;
            } else if (item.ItemId == Resource.Id.menu_my_book_list) {
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
            mTagAdapter.notifyDataSetChanged();
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
            MessageBus.Default.post(new TagEvent(currentTag));
        }

        private void showTagGroup()
        {
            if (mTagList.IsEmpty())
            {
                ToastUtils.showToast(GetString(Resource.String.network_error_tips));
                return;
            }
            Animation mShowAction = new TranslateAnimation(Animation.RELATIVE_TO_SELF, 0.0f,
                    Animation.RELATIVE_TO_SELF, 0.0f,
                    Animation.RELATIVE_TO_SELF, -1.0f,
                    Animation.RELATIVE_TO_SELF, 0.0f);
            mShowAction.setDuration(400);
            rsvTags.startAnimation(mShowAction);
            rsvTags.Visibility = (View.VISIBLE);
        }

        private void hideTagGroup()
        {
            Animation mHiddenAction = new TranslateAnimation(Animation.RELATIVE_TO_SELF, 0.0f,
                    Animation.RELATIVE_TO_SELF, 0.0f,
                    Animation.RELATIVE_TO_SELF, 0.0f,
                    Animation.RELATIVE_TO_SELF, -1.0f);
            mHiddenAction.setDuration(400);
            rsvTags.startAnimation(mHiddenAction);
            rsvTags.Visibility = (View.GONE);
        }
    }
}