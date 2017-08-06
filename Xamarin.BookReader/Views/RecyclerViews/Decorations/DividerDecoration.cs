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
using Android.Graphics.Drawables;
using Android.Graphics;
using Xamarin.BookReader.Views.RecyclerViews.Adapters;

namespace Xamarin.BookReader.Views.RecyclerViews.Decorations
{
    public class DividerDecoration : RecyclerView.ItemDecoration
    {
        private ColorDrawable mColorDrawable;
        private int mHeight;
        private int mPaddingLeft;
        private int mPaddingRight;
        private bool mDrawLastItem = true;
        private bool mDrawHeaderFooter = false;

        public DividerDecoration(int color, int height)
        {
            this.mColorDrawable = new ColorDrawable(new Color(color));
            this.mHeight = height;
        }
        public DividerDecoration(int color, int height, int paddingLeft, int paddingRight)
        {
            this.mColorDrawable = new ColorDrawable(new Color(color));
            this.mHeight = height;
            this.mPaddingLeft = paddingLeft;
            this.mPaddingRight = paddingRight;
        }

        public void setDrawLastItem(bool mDrawLastItem)
        {
            this.mDrawLastItem = mDrawLastItem;
        }

        public void setDrawHeaderFooter(bool mDrawHeaderFooter)
        {
            this.mDrawHeaderFooter = mDrawHeaderFooter;
        }

        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            int position = parent.GetChildAdapterPosition(view);
            int orientation = 0;
            int headerCount = 0, footerCount = 0;
            var adapterObj = parent.GetAdapter();
            var adapterType = adapterObj.GetType();
            if (adapterType.GetGenericTypeDefinition() == typeof(RecyclerArrayAdapter<>))
            {
                headerCount = (int)adapterType.GetMethod("GetHeaderCount").Invoke(adapterObj, null);
                footerCount = (int)adapterType.GetMethod("GetFooterCount").Invoke(adapterObj, null);
            }

            RecyclerView.LayoutManager layoutManager = parent.GetLayoutManager();
            if (layoutManager is StaggeredGridLayoutManager)
            {
                orientation = ((StaggeredGridLayoutManager)layoutManager).Orientation;
            }
            else if (layoutManager is GridLayoutManager)
            {
                orientation = ((GridLayoutManager)layoutManager).Orientation;
            }
            else if (layoutManager is LinearLayoutManager)
            {
                orientation = ((LinearLayoutManager)layoutManager).Orientation;
            }

            if (position >= headerCount && position < parent.GetAdapter().ItemCount - footerCount || mDrawHeaderFooter)
            {
                if (orientation == OrientationHelper.Vertical)
                {
                    outRect.Bottom = mHeight;
                }
                else
                {
                    outRect.Right = mHeight;
                }
            }
        }

        public void onDrawOver(Canvas c, RecyclerView parent, RecyclerView.State state)
        {


            int orientation = 0;
            int headerCount = 0, footerCount = 0, dataCount;
            var adapterObj = parent.GetAdapter();
            var adapterType = adapterObj.GetType();
            if (adapterType.GetGenericTypeDefinition() == typeof(RecyclerArrayAdapter<>))
            {
                headerCount = (int)adapterType.GetMethod("GetHeaderCount").Invoke(adapterObj, null);
                footerCount = (int)adapterType.GetMethod("GetFooterCount").Invoke(adapterObj, null);
                dataCount = (int)adapterType.GetMethod("GetCount").Invoke(adapterObj, null);
            }
            else
            {
                dataCount = parent.GetAdapter().ItemCount;
            }
            int dataStartPosition = headerCount;
            int dataEndPosition = headerCount + dataCount;


            RecyclerView.LayoutManager layoutManager = parent.GetLayoutManager();
            if (layoutManager is StaggeredGridLayoutManager)
            {
                orientation = ((StaggeredGridLayoutManager)layoutManager).Orientation;
            }
            else if (layoutManager is GridLayoutManager)
            {
                orientation = ((GridLayoutManager)layoutManager).Orientation;
            }
            else if (layoutManager is LinearLayoutManager)
            {
                orientation = ((LinearLayoutManager)layoutManager).Orientation;
            }
            int start, end;
            if (orientation == OrientationHelper.Vertical)
            {
                start = parent.PaddingLeft + mPaddingLeft;
                end = parent.Width - parent.PaddingRight - mPaddingRight;
            }
            else
            {
                start = parent.PaddingTop + mPaddingLeft;
                end = parent.Height - parent.PaddingBottom - mPaddingRight;
            }

            int childCount = parent.ChildCount;
            for (int i = 0; i < childCount; i++)
            {
                View child = parent.GetChildAt(i);
                int position = parent.GetChildAdapterPosition(child);

                if (position >= dataStartPosition && position < dataEndPosition - 1//数据项除了最后一项
                        || (position == dataEndPosition - 1 && mDrawLastItem)//数据项最后一项
                        || (!(position >= dataStartPosition && position < dataEndPosition) && mDrawHeaderFooter)//header&footer且可绘制
                        )
                {

                    if (orientation == OrientationHelper.Vertical)
                    {
                        RecyclerView.LayoutParams layoutParams = (RecyclerView.LayoutParams)child.LayoutParameters;
                        int top = child.Bottom + layoutParams.BottomMargin;
                        int bottom = top + mHeight;
                        mColorDrawable.SetBounds(start, top, end, bottom);
                        mColorDrawable.Draw(c);
                    }
                    else
                    {
                        RecyclerView.LayoutParams layoutParams = (RecyclerView.LayoutParams)child.LayoutParameters;
                        int left = child.Right + layoutParams.RightMargin;
                        int right = left + mHeight;
                        mColorDrawable.SetBounds(left, start, right, end);
                        mColorDrawable.Draw(c);
                    }
                }
            }
        }
    }
}