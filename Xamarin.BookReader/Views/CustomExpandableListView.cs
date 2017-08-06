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
using Android.Util;

namespace Xamarin.BookReader.Views
{
    public class CustomExpandableListView : ExpandableListView
    {
        public CustomExpandableListView(Context context, IAttributeSet attrs) : base(context, attrs)
        {

        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            int expandSpec = MeasureSpec.MakeMeasureSpec(int.MaxValue >> 2, MeasureSpecMode.AtMost);

            base.OnMeasure(widthMeasureSpec, expandSpec);
        }
    }
}