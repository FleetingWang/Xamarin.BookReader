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

namespace Xamarin.BookReader.UI.Activities
{
    /// <summary>
    /// 书籍详情 社区
    /// </summary>
    public class BookDetailCommunityActivity : BaseActivity
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

        //@Bind(R.id.indicatorSubRank)
        RVPIndicator mIndicator;
        //@Bind(R.id.viewpagerSubRank)
        ViewPager mViewPager;

        private List<Fragment> mTabContents;
        private FragmentPagerAdapter mAdapter;
        private List<String> mDatas;

        private AlertDialog dialog;
        private int[] select = new int[] { 0, 0 };

        public override int getLayoutId()
        {
            return R.layout.activity_book_detail_community;
        }

        public override void bindViews()
        {
            throw new NotImplementedException();
        }

        public override void initToolBar()
        {
            bookId = getIntent().getStringExtra(INTENT_ID);
            title = getIntent().getStringExtra(INTENT_TITLE);
            index = getIntent().getIntExtra(INTENT_INDEX, 0);
            mCommonToolbar.setTitle(title);
            mCommonToolbar.setNavigationIcon(R.drawable.ab_back);
        }

        public override void initDatas()
        {
            mDatas = Arrays.asList(getResources().getStringArray(R.array.bookdetail_community_tabs));

            mTabContents = new ArrayList<>();
            mTabContents.add(BookDetailDiscussionFragment.newInstance(bookId));
            mTabContents.add(BookDetailReviewFragment.newInstance(bookId));

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
            mViewPager.setOffscreenPageLimit(2);
            mIndicator.setViewPager(mViewPager, index);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            getMenuInflater().inflate(R.menu.menu_community, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.getItemId() == R.id.action_sort)
            {
                showSortDialog();
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void showSortDialog() {
        int check = select[mViewPager.getCurrentItem()];
        dialog = new AlertDialog.Builder(this)
                .setTitle("排序")
                //.setSingleChoiceItems(new String[]{"默认排序", "最新发布", "最多评论"},
                //        checked, new DialogInterface.OnClickListener() {
                //            @Override
                //            public void onClick(DialogInterface dialog, int which) {
                //                if (select[mViewPager.getCurrentItem()] != which) {
                //                    select[mViewPager.getCurrentItem()] = which;
                //                    switch (which) {
                //                        case 0:
                //                            EventBus.getDefault().post(new SelectionEvent(Constant.SortType.DEFAULT));
                //                            break;
                //                        case 1:
                //                            EventBus.getDefault().post(new SelectionEvent(Constant.SortType.CREATED));
                //                            break;
                //                        case 2:
                //                            EventBus.getDefault().post(new SelectionEvent(Constant.SortType.COMMENT_COUNT));
                //                            break;
                //                        default:
                //                            break;
                //                    }
                //                }
                //                dialog.dismiss();
                //            }
                //        })
                .setNegativeButton("取消", null)
                .create();
        dialog.show();
    }
    }
}