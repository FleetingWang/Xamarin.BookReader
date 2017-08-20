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
using EasyAdapterLibrary.AbsListViews;
using Xamarin.BookReader.Models.Support;
using Android.Text;
using Android.Text.Style;
using Android.Support.V4.Content;
using Android.Graphics;

namespace Xamarin.BookReader.UI.Adapters
{
    public class BookMarkAdapter : EasyLVAdapter<BookMark>
    {
        public BookMarkAdapter(Context context, List<BookMark> list)
            : base(context, list, Resource.Layout.item_read_mark)
        {

        }

        public override void convert(EasyLVHolder holder, int position, BookMark item)
        {
            TextView tv = holder.getView<TextView>(Resource.Id.tvMarkItem);

            SpannableString spanText = new SpannableString((position + 1) + ". " + item.title + ": ");
            spanText.SetSpan(new ForegroundColorSpan(new Color(ContextCompat.GetColor(mContext, Resource.Color.light_coffee))),
                    0, spanText.Length(), SpanTypes.InclusiveExclusive);

            tv.Text = spanText.ToString();

            if (item.desc != null)
            {
                tv.Append(item.desc
                        .Replace("　", "")
                        .Replace(" ", "")
                        .Replace("\n", ""));
            }
        }
    }
}