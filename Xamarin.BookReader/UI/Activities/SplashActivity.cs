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
using Android.Support.V7.App;
using System.Threading.Tasks;
using Android.Content.PM;

namespace Xamarin.BookReader.UI.Activities
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, MainLauncher = true)]
    public class SplashActivity : AppCompatActivity
    {
        TextView tvSkip;
        private bool flag = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_splash);
            tvSkip = FindViewById<TextView>(Resource.Id.tvSkip);

            tvSkip.PostDelayed(goHome, 2000);
            tvSkip.Click += (sender, e) => goHome();
        }

        private /*synchronized*/ void goHome()
        {
            if (!flag)
            {
                flag = true;
                StartActivity(new Intent(this, typeof(MainActivity)));
                Finish();
            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            flag = true;
            tvSkip.RemoveCallbacks(goHome);
        }
    }
}