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
    [Register("xamarin.bookreader.views.CustomTextView")]
    public class CustomTextView : TextView
    {
        private String txt;

        public CustomTextView(Context context): this(context, null)
        {
            
        }

        public CustomTextView(Context context, IAttributeSet attrs): this(context, attrs, 0)
        {
            
        }

        public CustomTextView(Context context, IAttributeSet attrs, int defStyleAttr):base(context, attrs, defStyleAttr)
        {
            Gravity = (GravityFlags.Left);
        }
    }
}