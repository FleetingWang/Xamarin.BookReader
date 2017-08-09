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

namespace Xamarin.BookReader.Views.ReadViews
{
    public interface IOnReadStateChangeListener
    {
        void onChapterChanged(int chapter);

        void onPageChanged(int chapter, int page);

        void onLoadChapterFailure(int chapter);

        void onCenterClick();

        void onFlip();
    }
}