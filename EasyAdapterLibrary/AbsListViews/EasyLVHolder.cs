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
using EasyAdapterLibrary.Helpers;
using Android.Util;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Views.Animations;
using Android.Content.Res;

namespace EasyAdapterLibrary.AbsListViews
{
    public class EasyLVHolder : Java.Lang.Object, AbsListView<EasyLVHolder>
    {

        /**
         * findViewById后保存View集合
         */
        private SparseArray<View> mViews = new SparseArray<View>();
        private SparseArray<View> mConvertViews = new SparseArray<View>();

        private View mConvertView;
        protected int mPosition;
        protected int mLayoutId;
        protected Context mContext;

        public EasyLVHolder(Context context, int position, ViewGroup parent, int layoutId)
        {
            this.mConvertView = mConvertViews.Get(layoutId);
            this.mPosition = position;
            this.mContext = context;
            this.mLayoutId = layoutId;
            if (mConvertView == null)
            {
                mConvertView = LayoutInflater.From(context).Inflate(layoutId, parent, false);
                mConvertViews.Put(layoutId, mConvertView);
                mConvertView.Tag = this;
            }
        }

        public EasyLVHolder()
        {
        }

        public BVH get<BVH>(Context context, int position, View convertView, ViewGroup parent, int layoutId) where BVH : EasyLVHolder
        {
            if (convertView == null)
            {
                return (BVH)new EasyLVHolder(context, position, parent, layoutId);
            }
            else
            {
                EasyLVHolder holder = (EasyLVHolder)convertView.Tag;
                if (holder.mLayoutId != layoutId)
                {
                    return (BVH)new EasyLVHolder(context, position, parent, layoutId);
                }
                holder.setPosition(position);
                return (BVH)holder;
            }
        }

        /**
         * 获取item布局
         * @return
         */
        public View getConvertView()
        {
            return mConvertViews.ValueAt(0);
        }

        public View getConvertView(int layoutId)
        {
            return mConvertViews.Get(layoutId);
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

        public void setPosition(int mPosition)
        {
            this.mPosition = mPosition;
        }

        public int getLayoutId()
        {
            return mLayoutId;
        }

        public EasyLVHolder setText(int viewId, String value)
        {
            TextView view = getView<TextView>(viewId);
            view.Text = value;
            return this;
        }

        public EasyLVHolder setTextColor(int viewId, int color)
        {
            TextView view = getView<TextView>(viewId);
            view.SetTextColor(ColorStateList.ValueOf(new Android.Graphics.Color(color)));
            return this;
        }

        public EasyLVHolder setTextColorRes(int viewId, int colorRes)
        {
            TextView view = getView<TextView>(viewId);
            view.SetTextColor(mContext.Resources.GetColor(colorRes, null));
            return this;
        }

        public EasyLVHolder setImageResource(int viewId, int imgResId)
        {
            ImageView view = getView<ImageView>(viewId);
            view.SetImageResource(imgResId);
            return this;
        }

        public EasyLVHolder setBackgroundColor(int viewId, int color)
        {
            View view = getView<View>(viewId);
            view.SetBackgroundColor(new Android.Graphics.Color(color));
            return this;
        }

        public EasyLVHolder setBackgroundColorRes(int viewId, int colorRes)
        {
            View view = getView<View>(viewId);
            view.SetBackgroundResource(colorRes);
            return this;
        }

        public EasyLVHolder setImageDrawable(int viewId, Drawable drawable)
        {
            ImageView view = getView<ImageView>(viewId);
            view.SetImageDrawable(drawable);
            return this;
        }

        public EasyLVHolder setImageDrawableRes(int viewId, int drawableRes)
        {
            Drawable drawable = mContext.Resources.GetDrawable(drawableRes, null);
            return setImageDrawable(viewId, drawable);
        }

        public EasyLVHolder setImageUrl(int viewId, String imgUrl)
        {
            // TO DO Use Glide/Picasso/ImageLoader/Fresco
            return this;
        }

        public EasyLVHolder setImageBitmap(int viewId, Bitmap imgBitmap)
        {
            ImageView view = getView<ImageView>(viewId);
            view.SetImageBitmap(imgBitmap);
            return this;
        }

        public EasyLVHolder setVisible(int viewId, bool visible)
        {
            View view = getView<View>(viewId);
            view.Visibility = visible ? ViewStates.Visible : ViewStates.Gone;
            return this;
        }

        public EasyLVHolder setVisible(int viewId, ViewStates visible)
        {
            View view = getView<View>(viewId);
            view.Visibility = visible;
            return this;
        }

        public EasyLVHolder setTag(int viewId, Java.Lang.Object tag)
        {
            View view = getView<View>(viewId);
            view.Tag = tag;
            return this;
        }

        public EasyLVHolder setTag(int viewId, int key, Java.Lang.Object tag)
        {
            View view = getView<View>(viewId);
            view.SetTag(key, tag);
            return this;
        }

        public EasyLVHolder setChecked(int viewId, bool check)
        {
            ICheckable view = (ICheckable)getView<View>(viewId);
            view.Checked = check;
            return this;
        }

        public EasyLVHolder setAlpha(int viewId, float value)
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

        public EasyLVHolder setTypeface(int viewId, Typeface typeface)
        {
            TextView view = getView<TextView>(viewId);
            view.Typeface = typeface;
            view.PaintFlags = (view.PaintFlags | PaintFlags.SubpixelText);
            return this;
        }

        public EasyLVHolder setTypeface(Typeface typeface, params int[] viewIds)
        {
            foreach (int viewId in viewIds)
            {
                TextView view = getView<TextView>(viewId);
                view.Typeface = typeface;
                view.PaintFlags = (view.PaintFlags | PaintFlags.SubpixelText);
            }
            return this;
        }

        public EasyLVHolder setOnClickListener(int viewId, View.IOnClickListener listener)
        {
            View view = getView<View>(viewId);
            view.SetOnClickListener(listener);
            return this;
        }
    }

}