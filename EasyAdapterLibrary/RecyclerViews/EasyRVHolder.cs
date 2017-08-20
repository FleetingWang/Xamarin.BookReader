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
using EasyAdapterLibrary.Helpers;
using Android.Util;
using Android.Support.V4.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Views.Animations;
using Com.Bumptech.Glide;
using Com.Bumptech.Glide.Load.Resource.Bitmap;

namespace EasyAdapterLibrary.RecyclerViews
{

    public class EasyRVHolder : RecyclerView.ViewHolder, RecyclerView<EasyRVHolder>
    {

        private SparseArray<View> mViews = new SparseArray<View>();

        private View mConvertView;
        private int mLayoutId;
        protected Context mContext;
        public event EventHandler Click;

        public EasyRVHolder(Context context, int layoutId, View itemView)
                : base(itemView)
        {
            this.mContext = context;
            this.mLayoutId = layoutId;
            mConvertView = itemView;
            mConvertView.Tag = this;
            mConvertView.Click += (sender, e) => {
                Click?.Invoke(sender, e);
            };
        }

        public V getView<V>(int viewId) where V : View
        {
            View view = mViews.Get(viewId);
            if (view == null)
            {
                view = mConvertView.FindViewById(viewId);
                mViews.Put(viewId, view);
            }
            return (V)view;
        }

        public int getLayoutId()
        {
            return mLayoutId;
        }

        /**
         * 获取item布局
         *
         * @return
         */
        public View getItemView()
        {
            return mConvertView;
        }

        public EasyRVHolder setOnItemViewClickListener(View.IOnClickListener listener)
        {
            mConvertView.SetOnClickListener(listener);
            return this;
        }

        public EasyRVHolder setText(int viewId, String value)
        {
            TextView view = getView<TextView>(viewId);
            view.Text = value;
            return this;
        }

        public EasyRVHolder setTextColor(int viewId, int color)
        {
            TextView view = getView<TextView>(viewId);
            view.SetTextColor(ColorStateList.ValueOf(new Android.Graphics.Color(color)));
            return this;
        }

        public EasyRVHolder setTextColorRes(int viewId, int colorRes)
        {
            TextView view = getView<TextView>(viewId);
            view.SetTextColor(ColorStateList.ValueOf(new Android.Graphics.Color(ContextCompat.GetColor(mContext, colorRes))));
            return this;
        }

        public EasyRVHolder setImageResource(int viewId, int imgResId)
        {
            ImageView view = getView<ImageView>(viewId);
            view.SetImageResource(imgResId);
            return this;
        }

        public EasyRVHolder setBackgroundColor(int viewId, int color)
        {
            View view = getView<View>(viewId);
            view.SetBackgroundColor(new Android.Graphics.Color(color));
            return this;
        }

        public EasyRVHolder setBackgroundColorRes(int viewId, int colorRes)
        {
            View view = getView<View>(viewId);
            view.SetBackgroundResource(colorRes);
            return this;
        }

        public EasyRVHolder setImageDrawable(int viewId, Drawable drawable)
        {
            ImageView view = getView<ImageView>(viewId);
            view.SetImageDrawable(drawable);
            return this;
        }

        public EasyRVHolder setImageDrawableRes(int viewId, int drawableRes)
        {
            Drawable drawable = ContextCompat.GetDrawable(mContext, drawableRes);
            return setImageDrawable(viewId, drawable);
        }

        public EasyRVHolder setImageUrl(int viewId, String imgUrl)
        {
            ImageView view = getView<ImageView>(viewId);
            Glide.With(mContext).Load(imgUrl).Into(view);
            return this;
        }

        public EasyRVHolder setImageUrl(int viewId, String imgUrl, int placeHolderRes)
        {
            ImageView view = getView<ImageView>(viewId);
            Glide.With(mContext).Load(imgUrl).Placeholder(placeHolderRes).Into(view);
            return this;
        }

        public EasyRVHolder setCircleImageUrl(int viewId, String imgUrl, int placeHolderRes)
        {
            ImageView view = getView<ImageView>(viewId);
            Glide.With(mContext).Load(imgUrl).Placeholder(placeHolderRes).Transform(new GlideCircleTransform(mContext)).Into(view);
            return this;
        }

        public EasyRVHolder setRoundImageUrl(int viewId, String imgUrl, int placeHolderRes)
        {
            ImageView view = getView<ImageView>(viewId);
            Glide.With(mContext).Load(imgUrl).Placeholder(placeHolderRes).Transform(new GlideRoundTransform(mContext)).Into(view);
            return this;
        }

        public EasyRVHolder setImageBitmap(int viewId, Bitmap imgBitmap)
        {
            ImageView view = getView<ImageView>(viewId);
            view.SetImageBitmap(imgBitmap);
            return this;
        }

        public EasyRVHolder setVisible(int viewId, bool visible)
        {
            View view = getView<View>(viewId);
            view.Visibility = (visible ? ViewStates.Visible : ViewStates.Gone);
            return this;
        }

        public EasyRVHolder setVisible(int viewId, ViewStates visible)
        {
            View view = getView<View>(viewId);
            view.Visibility = visible;
            return this;
        }

        public EasyRVHolder setTag(int viewId, Java.Lang.Object tag)
        {
            View view = getView<View>(viewId);
            view.Tag = tag;
            return this;
        }

        public EasyRVHolder setTag(int viewId, int key, Java.Lang.Object tag)
        {
            View view = getView<View>(viewId);
            view.SetTag(key, tag);
            return this;
        }

        public EasyRVHolder setChecked(int viewId, bool check)
        {
            ICheckable view = (ICheckable)getView<View>(viewId);
            view.Checked = check;
            return this;
        }

        public EasyRVHolder setAlpha(int viewId, float value)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb)
            {
                getView<View>(viewId).Alpha = value;
            }
            else
            {
                AlphaAnimation alpha = new AlphaAnimation(value, value);
                alpha.Duration = 0;
                alpha.FillAfter = true;
                getView<View>(viewId).StartAnimation(alpha);
            }
            return this;
        }

        public EasyRVHolder setTypeface(int viewId, Typeface typeface)
        {
            TextView view = getView<TextView>(viewId);
            view.Typeface = typeface;
            view.PaintFlags = (view.PaintFlags | PaintFlags.SubpixelText);
            return this;
        }

        public EasyRVHolder setTypeface(Typeface typeface, params int[] viewIds)
        {
            foreach (int viewId in viewIds)
            {
                TextView view = getView<TextView>(viewId);
                view.Typeface = typeface;
                view.PaintFlags = (view.PaintFlags | PaintFlags.SubpixelText);
            }
            return this;
        }

        public EasyRVHolder setOnClickListener(int viewId, View.IOnClickListener listener)
        {
            View view = getView<View>(viewId);
            view.SetOnClickListener(listener);
            return this;
        }
    }
}