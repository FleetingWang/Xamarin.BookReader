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
using Android.Support.V7.Widget;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using AndroidResource = Android.Resource;

namespace Xamarin.BookReader.Views
{
    public class SupportDividerItemDecoration : RecyclerView.ItemDecoration
    {
        private static int[] ATTRS = new int[]{
            AndroidResource.Attribute.ListDivider
    };

        public static int HORIZONTAL_LIST = LinearLayoutManager.Horizontal;

        public static int VERTICAL_LIST = LinearLayoutManager.Vertical;

        private Drawable mDivider;

        private int mOrientation;

        public SupportDividerItemDecoration(Context context, int orientation)
            : this(context, orientation, false)
        {

        }

        public SupportDividerItemDecoration(Context context, int orientation, bool dash)
        {
            if (dash)
            {
                mDivider = ContextCompat.GetDrawable(context, Resource.Drawable.shape_common_dash_divide);
            }
            else
            {
                TypedArray a = context.ObtainStyledAttributes(ATTRS);
                mDivider = a.GetDrawable(0);
                a.Recycle();
            }
            setOrientation(orientation);
        }


        public void setOrientation(int orientation)
        {
            if (orientation != HORIZONTAL_LIST && orientation != VERTICAL_LIST)
            {
                throw new ArgumentOutOfRangeException("invalid orientation");
            }
            mOrientation = orientation;
        }

        [Obsolete]
        public override void OnDraw(Canvas c, RecyclerView parent)
        {
            if (mOrientation == VERTICAL_LIST)
            {
                drawVertical(c, parent);
            }
            else
            {
                drawHorizontal(c, parent);
            }
        }


        public void drawVertical(Canvas c, RecyclerView parent)
        {
            int left = parent.PaddingLeft;
            int right = parent.Width - parent.PaddingRight;

            int childCount = parent.ChildCount;
            for (int i = 0; i < childCount; i++)
            {
                View child = parent.GetChildAt(i);
                RecyclerView v = new RecyclerView(parent.Context);
                RecyclerView.LayoutParams layoutParams = (RecyclerView.LayoutParams)child
                       .LayoutParameters;
                int top = child.Bottom + layoutParams.BottomMargin;
                int bottom = top + mDivider.IntrinsicHeight;
                mDivider.SetBounds(left, top, right, bottom);
                mDivider.Draw(c);
            }
        }

        public void drawHorizontal(Canvas c, RecyclerView parent)
        {
            int top = parent.PaddingTop;
            int bottom = parent.Height - parent.PaddingBottom;

            int childCount = parent.ChildCount;
            for (int i = 0; i < childCount; i++)
            {
                View child = parent.GetChildAt(i);
                RecyclerView.LayoutParams layoutParams = (RecyclerView.LayoutParams)child
                       .LayoutParameters;
                int left = child.Right + layoutParams.RightMargin;
                int right = left + mDivider.IntrinsicHeight;
                mDivider.SetBounds(left, top, right, bottom);
                mDivider.Draw(c);
            }
        }

        [Obsolete]
        public override void GetItemOffsets(Rect outRect, int itemPosition, RecyclerView parent)
        {
            if (mOrientation == VERTICAL_LIST)
            {
                outRect.Set(0, 0, 0, mDivider.IntrinsicHeight);
            }
            else
            {
                outRect.Set(0, 0, mDivider.IntrinsicWidth, 0);
            }
        }
    }
}