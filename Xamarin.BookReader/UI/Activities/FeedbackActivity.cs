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
    public class FeedbackActivity: BaseActivity
    {
        public static void startActivity(Context context)
        {
            context.StartActivity(new Intent(context, typeof(FeedbackActivity)));
        }

        //@Bind(R.id.feedbackView)
        ProgressWebView feedbackView;

        public override int getLayoutId()
        {
            return R.layout.activity_feedback;
        }

        public override void initToolBar()
        {
            mCommonToolbar.setTitle("反馈建议");
            mCommonToolbar.setNavigationIcon(R.drawable.ab_back);
        }

        public override void bindViews()
        {

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