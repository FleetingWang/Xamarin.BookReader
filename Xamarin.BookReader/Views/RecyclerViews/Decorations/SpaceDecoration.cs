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
using Android.Graphics;
using Xamarin.BookReader.Views.RecyclerViews.Adapters;

namespace Xamarin.BookReader.Views.RecyclerViews.Decorations
{
    public class SpaceDecoration : RecyclerView.ItemDecoration
    {
        private int halfSpace;
        private int headerCount = -1;
        private int footerCount = int.MaxValue;
        private bool mPaddingEdgeSide = true;
        private bool mPaddingStart = true;
        private bool mPaddingHeaderFooter = false;


        public SpaceDecoration(int space)
        {
            this.halfSpace = space / 2;
        }

        public void setPaddingEdgeSide(bool mPaddingEdgeSide)
        {
            this.mPaddingEdgeSide = mPaddingEdgeSide;
        }

        public void setPaddingStart(bool mPaddingStart)
        {
            this.mPaddingStart = mPaddingStart;
        }

        public void setPaddingHeaderFooter(bool mPaddingHeaderFooter)
        {
            this.mPaddingHeaderFooter = mPaddingHeaderFooter;
        }

        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            int position = parent.GetChildAdapterPosition(view);
            int spanCount = 0;
            int orientation = 0;
            int spanIndex = 0;
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
                spanCount = ((StaggeredGridLayoutManager)layoutManager).SpanCount;
                spanIndex = ((StaggeredGridLayoutManager.LayoutParams)view.LayoutParameters).SpanIndex;
            }
            else if (layoutManager is GridLayoutManager)
            {
                orientation = ((GridLayoutManager)layoutManager).Orientation;
                spanCount = ((GridLayoutManager)layoutManager).SpanCount;
                spanIndex = ((GridLayoutManager.LayoutParams)view.LayoutParameters).SpanIndex;
            }
            else if (layoutManager is LinearLayoutManager)
            {
                orientation = ((LinearLayoutManager)layoutManager).Orientation;
                spanCount = 1;
                spanIndex = 0;
            }

            /**
             * 普通Item的尺寸
             */
            if ((position >= headerCount && position < parent.GetAdapter().ItemCount - footerCount))
            {
                GravityFlags gravity;
                if (spanIndex == 0 && spanCount > 1) gravity = GravityFlags.Left;
                else if (spanIndex == spanCount - 1 && spanCount > 1) gravity = GravityFlags.Right;
                else if (spanCount == 1) gravity = GravityFlags.FillHorizontal;
                else
                {
                    gravity = GravityFlags.Center;
                }
                if (orientation == OrientationHelper.Vertical)
                {
                    switch (gravity)
                    {
                        case GravityFlags.Left:
                            if (mPaddingEdgeSide)
                                outRect.Left = halfSpace * 2;
                            outRect.Right = halfSpace;
                            break;
                        case GravityFlags.Right:
                            outRect.Left = halfSpace;
                            if (mPaddingEdgeSide)
                                outRect.Right = halfSpace * 2;
                            break;
                        case GravityFlags.FillHorizontal:
                            if (mPaddingEdgeSide)
                            {
                                outRect.Left = halfSpace * 2;
                                outRect.Right = halfSpace * 2;
                            }
                            break;
                        case GravityFlags.Center:
                            outRect.Left = halfSpace;
                            outRect.Right = halfSpace;
                            break;
                    }
                    if (position - headerCount < spanCount && mPaddingStart) outRect.Top = halfSpace * 2;
                    outRect.Bottom = halfSpace * 2;
                }
                else
                {
                    switch (gravity)
                    {
                        case GravityFlags.Left:
                            if (mPaddingEdgeSide)
                                outRect.Bottom = halfSpace * 2;
                            outRect.Top = halfSpace;
                            break;
                        case GravityFlags.Right:
                            outRect.Bottom = halfSpace;
                            if (mPaddingEdgeSide)
                                outRect.Top = halfSpace * 2;
                            break;
                        case GravityFlags.FillHorizontal:
                            if (mPaddingEdgeSide)
                            {
                                outRect.Left = halfSpace * 2;
                                outRect.Right = halfSpace * 2;
                            }
                            break;
                        case GravityFlags.Center:
                            outRect.Bottom = halfSpace;
                            outRect.Top = halfSpace;
                            break;
                    }
                    if (position - headerCount < spanCount && mPaddingStart) outRect.Left = halfSpace * 2;
                    outRect.Right = halfSpace * 2;
                }
            }
            else
            {//只有HeaderFooter进到这里
                if (mPaddingHeaderFooter)
                {//并且需要padding Header&Footer
                    if (orientation == OrientationHelper.Vertical)
                    {
                        if (mPaddingEdgeSide)
                        {
                            outRect.Left = halfSpace * 2;
                            outRect.Right = halfSpace * 2;
                        }
                        outRect.Top = halfSpace * 2;
                    }
                    else
                    {
                        if (mPaddingEdgeSide)
                        {
                            outRect.Top = halfSpace * 2;
                            outRect.Bottom = halfSpace * 2;
                        }
                        outRect.Left = halfSpace * 2;
                    }
                }
            }
        }
    }
}