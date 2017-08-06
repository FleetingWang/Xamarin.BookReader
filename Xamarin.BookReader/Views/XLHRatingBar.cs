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
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Support.V4.Content;
using AndroidResource = Android.Resource;
using Android.Graphics;
using Xamarin.BookReader.Utils;

namespace Xamarin.BookReader.Views
{
    /// <summary>
    /// 自定义评分控件
    /// </summary>
    public class XLHRatingBar : LinearLayout
    {
        private int countNum;
        private int countSelected;
        private int stateResId;
        private float widthAndHeight;
        private float dividerWidth;
        private bool canEdit;
        private bool differentSize;

        public XLHRatingBar(Context context) : this(context, null)
        {

        }

        public XLHRatingBar(Context context, IAttributeSet attrs) : base(context, attrs)
        {

            TypedArray typedArray = context.ObtainStyledAttributes(attrs, Resource.Styleable.XlHRatingBar);
            countNum = typedArray.GetInt(Resource.Styleable.XlHRatingBar_starCount, 5);
            countSelected = typedArray.GetInt(Resource.Styleable.XlHRatingBar_countSelected, 0);
            canEdit = typedArray.GetBoolean(Resource.Styleable.XlHRatingBar_canEdit, false);
            differentSize = typedArray.GetBoolean(Resource.Styleable.XlHRatingBar_differentSize, false);
            widthAndHeight = typedArray.GetDimension(Resource.Styleable.XlHRatingBar_widthAndHeight, ScreenUtils.dpToPxInt(0));
            dividerWidth = typedArray.GetDimension(Resource.Styleable.XlHRatingBar_dividerWidth, ScreenUtils.dpToPxInt(0));
            stateResId = typedArray.GetResourceId(Resource.Styleable.XlHRatingBar_stateResId, -1);
            initView();
        }

        public int getCountNum()
        {
            return countNum;
        }

        public void setCountNum(int countNum)
        {
            this.countNum = countNum;
            initView();
        }

        public int getCountSelected()
        {
            return countSelected;
        }

        public void setCountSelected(int countSelected)
        {
            if (countSelected > countNum)
            {
                return;
            }
            this.countSelected = countSelected;
            initView();
        }


        private void initView()
        {
            RemoveAllViews();
            for (int i = 0; i < countNum; i++)
            {
                CheckBox cb = new CheckBox(Context);
                LayoutParams layoutParams;
                if (widthAndHeight == 0)
                {
                    layoutParams = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                }
                else
                {
                    layoutParams = new LayoutParams((int)widthAndHeight, (int)widthAndHeight);
                }
                if (differentSize && countNum % 2 != 0)
                {
                    Log.Error("xxx", layoutParams.Width + "");
                    int index = i;
                    if (index > countNum / 2)
                    {
                        index = countNum - 1 - index;
                    }
                    float scale = (index + 1) / (float)(countNum / 2 + 1);
                    layoutParams.Width = (int)(layoutParams.Width * scale);
                    layoutParams.Height = layoutParams.Width;
                }
                layoutParams.Gravity = GravityFlags.CenterVertical;
                if (i != 0 && i != countNum - 1)
                {
                    layoutParams.LeftMargin = (int)dividerWidth;
                    layoutParams.RightMargin = (int)dividerWidth;
                }
                else if (i == 0)
                {
                    layoutParams.RightMargin = (int)dividerWidth;
                }
                else if (i == countNum - 1)
                {
                    layoutParams.LeftMargin = (int)dividerWidth;
                }
                AddView(cb, layoutParams);
                cb.SetButtonDrawable(new ColorDrawable(new Color(ContextCompat.GetColor(Context, AndroidResource.Color.Transparent))));
                if (stateResId == -1)
                {
                    stateResId = Resource.Drawable.book_review_rating_bar_selector;
                }
                cb.SetBackgroundResource(stateResId);
                if (i + 1 <= countSelected)
                {
                    cb.Checked = true;
                }
                cb.Enabled = canEdit;
                cb.SetOnClickListener(new MyClickListener(this, i));
            }

        }

        private class MyClickListener : Java.Lang.Object, IOnClickListener
        {

            int position;
            private XLHRatingBar xLHRatingBar;

            public MyClickListener(int position) : base()
            {

                this.position = position;
            }

            public MyClickListener(XLHRatingBar xLHRatingBar, int position)
            {
                this.xLHRatingBar = xLHRatingBar;
                this.position = position;
            }

            public void OnClick(View v)
            {
                xLHRatingBar.countSelected = position + 1;

                for (int i = 0; i < xLHRatingBar.countNum; i++)
                {
                    CheckBox cb = (CheckBox)xLHRatingBar.GetChildAt(i);

                    if (i <= position)
                    {
                        cb.Checked = true;
                    }
                    else if (i > position)
                    {
                        cb.Checked = false;
                    }
                }
                if (xLHRatingBar.mOnRatingChangeListener != null)
                {
                    xLHRatingBar.mOnRatingChangeListener.onChange(xLHRatingBar.countSelected);
                }
            }
        }

        private OnRatingChangeListener mOnRatingChangeListener;

        public OnRatingChangeListener getOnRatingChangeListener()
        {
            return mOnRatingChangeListener;
        }

        public void setOnRatingChangeListener(OnRatingChangeListener onRatingChangeListener)
        {
            mOnRatingChangeListener = onRatingChangeListener;
        }

        public interface OnRatingChangeListener
        {
            void onChange(int countSelected);
        }
    }
}