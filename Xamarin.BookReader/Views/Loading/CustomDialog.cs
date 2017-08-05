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
using Android.Support.V4.Content;

namespace Xamarin.BookReader.Views.Loading
{
    public class CustomDialog: Dialog
    {
        public CustomDialog(Context context)
            :this(context, 0)
        {
        }

        public CustomDialog(Context context, int themeResId)
            :base(context, themeResId)
        {
        }

        public static CustomDialog instance(Activity activity)
        {
            LoadingView v = (LoadingView)View.Inflate(activity, Resource.Layout.common_progress_view, null);
            v.setColor(ContextCompat.GetColor(activity, Resource.Color.reader_menu_bg_color));
            CustomDialog dialog = new CustomDialog(activity, Resource.Style.loading_dialog);
            dialog.SetContentView(v,
                    new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                            ViewGroup.LayoutParams.MatchParent)
            );
            return dialog;
        }
    }
}