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
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class FeedbackActivity: BaseActivity
    {
        public static void startActivity(Context context)
        {
            context.StartActivity(new Intent(context, typeof(FeedbackActivity)));
        }

        ProgressWebView feedbackView;

        public override int getLayoutId()
        {
            return Resource.Layout.activity_feedback;
        }

        public override void initToolBar()
        {
            mCommonToolbar.Title = ("反馈建议");
            mCommonToolbar.SetNavigationIcon(Resource.Drawable.ab_back);
        }

        public override void bindViews()
        {
            feedbackView = FindViewById<ProgressWebView>(Resource.Id.feedbackView);
        }

        public override void initDatas()
        {
        }

        public override void configViews()
        {
            feedbackView.loadUrl("https://github.com/JustWayward/BookReader/issues/new");
        }

    }
}