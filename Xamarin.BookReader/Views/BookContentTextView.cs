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
using Android.Text;
using Android.Text.Style;
using Android.Util;
using Xamarin.BookReader.Utils;
using Android.Text.Method;
using Android.Graphics;
using Xamarin.BookReader.UI.Activities;

namespace Xamarin.BookReader.Views
{
    [Register("xamarin.bookreader.views.BookContentTextView")]
    public class BookContentTextView : TextView
    {
        public BookContentTextView(Context context) : this(context, null)
        {

        }

        public BookContentTextView(Context context, IAttributeSet attrs) : this(context, attrs, 0)
        {

        }

        public BookContentTextView(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {

        }

        public void setText(String text)
        {
            int startIndex = 0;
            while (true)
            {

                int start = text.IndexOf("《");
                int end = text.IndexOf("》");
                if (start < 0 || end < 0)
                {
                    Append(text.Substring(startIndex));
                    break;
                }

                Append(text.Substring(startIndex, start));

                SpannableString spanableInfo = new SpannableString(text.Substring(start, end + 1));
                spanableInfo.SetSpan(new Clickable(Context, spanableInfo.ToString()), 0, end + 1 - start, SpanTypes.ExclusiveExclusive);
                Append(spanableInfo);
                //setMovementMethod()该方法必须调用，否则点击事件不响应
                MovementMethod = LinkMovementMethod.Instance;
                text = text.Substring(end + 1);

                LogUtils.e(spanableInfo.ToString());
            }
        }

        class Clickable : ClickableSpan
        {
            private String name;
            private Context context;
            public Clickable(Context context, String name) : base()
            {
                this.context = context;
                this.name = name;
            }

            public override void UpdateDrawState(TextPaint ds)
            {
                base.UpdateDrawState(ds);
                ds.Color = new Color(ContextCompat.GetColor(context, Resource.Color.light_coffee));
                ds.UnderlineText = false;
            }

            public override void OnClick(View v)
            {
                SearchActivity.startActivity(context, name.Replace("》", "").Replace("《", ""));
            }
        }
    }
}