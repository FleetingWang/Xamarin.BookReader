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
using Android.Content.PM;

namespace Xamarin.BookReader.UI.Activities
{
    /// <summary>
    /// 女生区
    /// </summary>
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class GirlBookDiscussionActivity : BaseCommunitActivity
    {
        public static void startActivity(Context context)
        {
            context.StartActivity(new Intent(context, typeof(GirlBookDiscussionActivity)));
        }

        public override int getLayoutId()
        {
            return Resource.Layout.activity_community_girl_book_discussion;
        }

        public override void initToolBar()
        {
            mCommonToolbar.Title = ("女生区");
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