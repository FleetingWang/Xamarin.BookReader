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

namespace Xamarin.BookReader.UI.Activities
{
    /// <summary>
    /// 书评区
    /// </summary>
    public class BookReviewActivity : BaseCommunitActivity
    {
        public static void startActivity(Context context) {
            context.StartActivity(new Intent(context, typeof(BookReviewActivity)));
        }

        //@Bind(R.id.slOverall)
        SelectionLayout slOverall;

        public override int getLayoutId()
        {
            return R.layout.activity_community_book_review;
        }
        public override void bindViews()
        {

        }

        public override void initToolBar()
        {
            mCommonToolbar.setTitle("书评区");
            mCommonToolbar.setNavigationIcon(R.drawable.ab_back);
        }
        public override void configViews()
        {
        }

        protected override List<List<string>> getTabList()
        {
            return list2;
        }
    }
}