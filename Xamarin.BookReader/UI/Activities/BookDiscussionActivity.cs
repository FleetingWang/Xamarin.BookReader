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
using Android.Support.V7.App;

namespace Xamarin.BookReader.UI.Activities
{
    /// <summary>
    /// 综合讨论区
    /// </summary>
    public class BookDiscussionActivity : BaseCommunitActivity
    {
        public static String INTENT_ID = "bookId";
        public static String INTENT_TITLE = "title";
        public static String INTENT_INDEX = "index";

        public static void startActivity(Context context, String bookId, String title, int index)
        {
            context.StartActivity(new Intent(context, typeof(BookDetailCommunityActivity))
                .PutExtra(INTENT_ID, bookId)
                .PutExtra(INTENT_TITLE, title)
                .PutExtra(INTENT_INDEX, index));
        }

        private String bookId;
        private String title;
        private int index;

        RVPIndicator mIndicator;
        ViewPager mViewPager;

        private List<Fragment> mTabContents;
        private FragmentPagerAdapter mAdapter;
        private List<String> mDatas;

        private AlertDialog dialog;
        private int[] select = new int[] { 0, 0 };


        public override int getLayoutId()
        {
            return Resource.Layout.activity_book_detail_community;
        }
        public override void bindViews()
        {
            mIndicator = FindViewById<RVPIndicator>(Resource.Id.indicatorSubRank);
            mViewPager = FindViewById<ViewPager>(Resource.Id.viewpagerSubRank);
        }

        public override void initToolBar()
        {
            bookId = Intent.GetStringExtra(INTENT_ID);
            title = Intent.GetStringExtra(INTENT_TITLE);
            index = Intent.GetIntExtra(INTENT_INDEX, 0);
            mCommonToolbar.Title = (title);
            mCommonToolbar.SetNavigationIcon(Resource.Drawable.ab_back);
        }
        public override void initDatas()
        {
            mDatas = Resources.GetStringArray(Resource.Array.bookdetail_community_tabs).ToList();

            mTabContents = new List<Fragment>();
            mTabContents.Add(BookDetailDiscussionFragment.newInstance(bookId));
            mTabContents.Add(BookDetailReviewFragment.newInstance(bookId));
            mAdapter = new CustomFragmentPagerAdapter(this, SupportFragmentManager);
        }

        public override void configViews()
        {
            mIndicator.setTabItemTitles(mDatas);
            mViewPager.Adapter = (mAdapter);
            mViewPager.OffscreenPageLimit = (2);
            mIndicator.setViewPager(mViewPager, index);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_community, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.action_sort)
            {
                showSortDialog();
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void showSortDialog()
        {
            int check = select[mViewPager.CurrentItem];
            dialog = new AlertDialog.Builder(this)
                    .SetTitle("排序")
                    //.setSingleChoiceItems(new String[]{"默认排序", "最新发布", "最多评论"},
                    //        checked, new DialogInterface.OnClickListener() {
                    //            @Override
                    //            public void onClick(DialogInterface dialog, int which) {
                    //                if (select[mViewPager.CurrentItem] != which) {
                    //                    select[mViewPager.CurrentItem] = which;
                    //                    switch (which) {
                    //                        case 0:
                    //                            MessageBus.Default.post(new SelectionEvent(Constant.SortType.DEFAULT));
                    //                            break;
                    //                        case 1:
                    //                            MessageBus.Default.post(new SelectionEvent(Constant.SortType.CREATED));
                    //                            break;
                    //                        case 2:
                    //                            MessageBus.Default.post(new SelectionEvent(Constant.SortType.COMMENT_COUNT));
                    //                            break;
                    //                        default:
                    //                            break;
                    //                    }
                    //                }
                    //                dialog.Dismiss();
                    //            }
                    //        })
                    .SetNegativeButton("取消", (sender, e) => { })
                    .Create();
            dialog.Show();
        }

        protected override List<List<string>> getTabList()
        {
            throw new NotImplementedException();
        }

        private class CustomFragmentPagerAdapter : FragmentPagerAdapter
        {
            private BookDiscussionActivity bookDiscussionActivity;
            private FragmentManager supportFragmentManager;

            public CustomFragmentPagerAdapter(BookDiscussionActivity bookDiscussionActivity, FragmentManager supportFragmentManager)
                : base(supportFragmentManager)
            {
                this.bookDiscussionActivity = bookDiscussionActivity;
                this.supportFragmentManager = supportFragmentManager;
            }

            public override int Count => bookDiscussionActivity.mTabContents.Count();

            public override Fragment GetItem(int position)
            {
                return bookDiscussionActivity.mTabContents[position];
            }
        }
    }
}