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

using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;

namespace Xamarin.BookReader.Views
{
    public class DrawableCenterButton : TextView
    {
        public DrawableCenterButton(Context context, IAttributeSet attrs,
                                int defStyle) : base(context, attrs, defStyle)
        {

        }

        public DrawableCenterButton(Context context, IAttributeSet attrs) : base(context, attrs)
        {

        }

        public DrawableCenterButton(Context context) : base(context)
        {

        }

        protected override void OnDraw(Canvas canvas)
        {
            Drawable[] drawables = GetCompoundDrawables();
            if (drawables != null)
            {
                Drawable drawableLeft = drawables[0];
                if (drawableLeft != null)
                {
                    float textWidth = Paint.MeasureText(Text);
                    int drawablePadding = CompoundDrawablePadding;
                    int drawableWidth = 0;
                    drawableWidth = drawableLeft.IntrinsicWidth;
                    float bodyWidth = textWidth + drawableWidth + drawablePadding;
                    canvas.Translate((Width - bodyWidth) / 11 * 5, 0);
                }
            }
            base.OnDraw(canvas);
        }
    }
}