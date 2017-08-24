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

using AndroidResource = Android.Resource;

namespace Xamarin.BookReader.Views
{
    [Register("xamarin.bookreader.views.SupportGridItemDecoration")]
    public class SupportGridItemDecoration : RecyclerView.ItemDecoration
    {
        private static int[] ATTRS = new int[] { AndroidResource.Attribute.ListDivider };
        private Drawable mDivider;

        public SupportGridItemDecoration(Context context)
        {
            TypedArray a = context.ObtainStyledAttributes(ATTRS);
            mDivider = a.GetDrawable(0);
            a.Recycle();
        }

        public override void OnDraw(Canvas c, RecyclerView parent, RecyclerView.State state)
        {

            drawHorizontal(c, parent);
            drawVertical(c, parent);

        }

        private int getSpanCount(RecyclerView parent)
        {
            // 列数
            int spanCount = -1;
            RecyclerView.LayoutManager layoutManager = parent.GetLayoutManager();
            if (layoutManager is GridLayoutManager)
            {

                spanCount = ((GridLayoutManager)layoutManager).SpanCount;
            }
            else if (layoutManager is StaggeredGridLayoutManager)
            {
                spanCount = ((StaggeredGridLayoutManager)layoutManager)
                        .SpanCount;
            }
            return spanCount;
        }

        public void drawHorizontal(Canvas c, RecyclerView parent)
        {
            int childCount = parent.ChildCount;
            for (int i = 0; i < childCount; i++)
            {
                View child = parent.GetChildAt(i);
                RecyclerView.LayoutParams layoutParams = (RecyclerView.LayoutParams)child
                       .LayoutParameters;
                int left = child.Left - layoutParams.LeftMargin;
                int right = child.Right + layoutParams.RightMargin
                        + mDivider.IntrinsicWidth;
                int top = child.Bottom + layoutParams.BottomMargin;
                int bottom = top + mDivider.IntrinsicHeight;
                mDivider.SetBounds(left, top, right, bottom);
                mDivider.Draw(c);
            }
        }

        public void drawVertical(Canvas c, RecyclerView parent)
        {
            int childCount = parent.ChildCount;
            for (int i = 0; i < childCount; i++)
            {
                View child = parent.GetChildAt(i);

                RecyclerView.LayoutParams layoutParams = (RecyclerView.LayoutParams)child
                       .LayoutParameters;
                int top = child.Top - layoutParams.TopMargin;
                int bottom = child.Bottom + layoutParams.BottomMargin;
                int left = child.Right + layoutParams.RightMargin;
                int right = left + mDivider.IntrinsicWidth;

                mDivider.SetBounds(left, top, right, bottom);
                mDivider.Draw(c);
            }
        }

        private bool isLastColum(RecyclerView parent, int pos, int spanCount,
                                    int childCount)
        {
            RecyclerView.LayoutManager layoutManager = parent.GetLayoutManager();
            if (layoutManager is GridLayoutManager)
            {
                if ((pos + 1) % spanCount == 0)// 如果是最后一列，则不需要绘制右边
                {
                    return true;
                }
            }
            else if (layoutManager is StaggeredGridLayoutManager)
            {
                int orientation = ((StaggeredGridLayoutManager)layoutManager)
                        .Orientation;
                if (orientation == StaggeredGridLayoutManager.Vertical)
                {
                    if ((pos + 1) % spanCount == 0)// 如果是最后一列，则不需要绘制右边
                    {
                        return true;
                    }
                }
                else
                {
                    childCount = childCount - childCount % spanCount;
                    if (pos >= childCount)// 如果是最后一列，则不需要绘制右边
                        return true;
                }
            }
            return false;
        }

        private bool isLastRaw(RecyclerView parent, int pos, int spanCount,
                                  int childCount)
        {
            RecyclerView.LayoutManager layoutManager = parent.GetLayoutManager();
            if (layoutManager is GridLayoutManager)
            {
                childCount = childCount - childCount % spanCount;
                if (pos >= childCount)// 如果是最后一行，则不需要绘制底部
                    return true;
            }
            else if (layoutManager is StaggeredGridLayoutManager)
            {
                int orientation = ((StaggeredGridLayoutManager)layoutManager)
                        .Orientation;
                // StaggeredGridLayoutManager 且纵向滚动
                if (orientation == StaggeredGridLayoutManager.Vertical)
                {
                    childCount = childCount - childCount % spanCount;
                    // 如果是最后一行，则不需要绘制底部
                    if (pos >= childCount)
                        return true;
                }
                else
                // StaggeredGridLayoutManager 且横向滚动
                {
                    // 如果是最后一行，则不需要绘制底部
                    if ((pos + 1) % spanCount == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        [Obsolete]
        public override void GetItemOffsets(Rect outRect, int itemPosition,
                                   RecyclerView parent)
        {
            int spanCount = getSpanCount(parent);
            int childCount = parent.GetAdapter().ItemCount;
            if (isLastRaw(parent, itemPosition, spanCount, childCount))// 如果是最后一行，则不需要绘制底部
            {
                outRect.Set(0, 0, mDivider.IntrinsicWidth, 0);
            }
            else if (isLastColum(parent, itemPosition, spanCount, childCount))// 如果是最后一列，则不需要绘制右边
            {
                outRect.Set(0, 0, 0, mDivider.IntrinsicHeight);
            }
            else
            {
                outRect.Set(0, 0, mDivider.IntrinsicWidth,
                        mDivider.IntrinsicHeight);
            }
        }
    }
}