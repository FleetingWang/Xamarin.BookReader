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
    /// 书荒求助区
    /// </summary>
    public class BookHelpActivity : BaseCommunitActivity
    {
        public static void startActivity(Context context) {
            context.StartActivity(new Intent(context, typeof(BookHelpActivity)));
        }
        public override int getLayoutId()
        {
            return Resource.Layout.activity_community_book_help;
        }
        public override void bindViews()
        {
        }

        public override void initToolBar()
        {
            mCommonToolbar.Title = ("书荒互助区");
            mCommonToolbar.SetNavigationIcon(Resource.Drawable.ab_back);
        }

        public override void configViews()
        {
        }

        protected override List<List<string>> getTabList()
        {
            return list1;
        }
    }
}