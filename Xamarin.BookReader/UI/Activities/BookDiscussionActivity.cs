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
        public static String INTENT_DIS = "isDis";
        public static void startActivity(Context context, bool isDiscussion)
        {
            context.StartActivity(new Intent(context, typeof(BookDetailCommunityActivity))
                .PutExtra(INTENT_DIS, isDiscussion));
        }
        private bool mIsDiscussion;
        SelectionLayout slOverall;

        public override int getLayoutId()
        {
            return Resource.Layout.activity_community_book_discussion;
        }
        public override void bindViews()
        {
            slOverall = FindViewById<SelectionLayout>(Resource.Id.slOverall);
        }

        public override void initToolBar()
        {
            mIsDiscussion = Intent.GetBooleanExtra(INTENT_DIS, false);
            if (mIsDiscussion)
            {
                mCommonToolbar.Title = ("综合讨论区");
            }
            else
            {
                mCommonToolbar.Title = ("原创区");
            }
            mCommonToolbar.SetNavigationIcon(Resource.Drawable.ab_back);
        }

        public override void configViews()
        {
            BookDiscussionFragment fragment = BookDiscussionFragment.newInstance(mIsDiscussion ? "ramble" : "original");
            SupportFragmentManager.BeginTransaction().Replace(Resource.Id.fragmentCO, fragment).Commit();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_community, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        protected override List<List<string>> getTabList()
        {
            return list1;
        }
    }
}