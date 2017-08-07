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
    public enum BookStatus
    {
        NO_PRE_PAGE,
        NO_NEXT_PAGE,

        PRE_CHAPTER_LOAD_FAILURE,
        NEXT_CHAPTER_LOAD_FAILURE,

        LOAD_SUCCESS
    }
}