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
using Android.Util;
using Android.Graphics;
using Android.Views.Animations;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Com.Bumptech.Glide;
using Com.Bumptech.Glide.Load.Resource.Bitmap;
using Android.Content.Res;

namespace Xamarin.BookReader.Views.RecyclerViews.Adapters
{
    public abstract class BaseViewHolder<M>: RecyclerView.ViewHolder where M:class
    {
        protected BaseViewHolder<M> holder;

    private int mLayoutId;
    protected Context mContext;
    private View mConvertView;

    private SparseArray<View> mViews = new SparseArray<View>();

    public BaseViewHolder(View itemView)
            :base(itemView)
    {
        holder = this;
        mConvertView = itemView;
        mContext = mConvertView.Context;
    }

    public BaseViewHolder(ViewGroup parent, int res)
            :base(LayoutInflater.From(parent.Context).Inflate(res, parent, false))
        {
        holder = this;
        mConvertView = ItemView;
        mLayoutId = res;
        mContext = mConvertView.Context;
    }

    public virtual void setData(M item) {

    }

    protected Context getContext() {
        return mContext == null ? (mContext = ItemView.Context) : mContext;
    }

    public View getView(int viewId)
        {
            return getView<View>(viewId);
        }

    public V getView<V>(int viewId) where V:View {
        View view = mViews.Get(viewId);
        if (view == null) {
            view = mConvertView.FindViewById(viewId);
            mViews.Put(viewId, view);
        }
        return (V) view;
    }

    public int getLayoutId() {
        return mLayoutId;
    }

    /**
     * 获取item布局
     *
     * @return
     */
    public View getItemView() {
        return mConvertView;
    }

    public BaseViewHolder<M> setOnItemViewClickListener(View.IOnClickListener listener) {
        mConvertView.SetOnClickListener(listener);
        return this;
    }

    public BaseViewHolder<M> setText(int viewId, String value) {
        TextView view = getView<TextView>(viewId);
        view.Text = value;
        return this;
    }

    public BaseViewHolder<M> setTextColor(int viewId, int color) {
        TextView view = getView<TextView>(viewId);
        view.SetTextColor(ColorStateList.ValueOf(new Color(color)));
        return this;
    }

    public BaseViewHolder<M> setTextColorRes(int viewId, int colorRes) {
        TextView view = getView<TextView>(viewId);
        view.SetTextColor(ColorStateList.ValueOf(new Color(ContextCompat.GetColor(mContext, colorRes))));
        return this;
    }

    public BaseViewHolder<M> setImageResource(int viewId, int imgResId) {
        ImageView view = getView<ImageView>(viewId);
        view.SetImageResource(imgResId);
        return this;
    }

    public BaseViewHolder<M> setBackgroundColor(int viewId, int color) {
        View view = getView(viewId);
        view.SetBackgroundColor(new Color(color));
        return this;
    }

    public BaseViewHolder<M> setBackgroundColorRes(int viewId, int colorRes) {
        View view = getView(viewId);
        view.SetBackgroundResource(colorRes);
        return this;
    }

    public BaseViewHolder<M> setImageDrawable(int viewId, Drawable drawable) {
        ImageView view = getView<ImageView>(viewId);
        view.SetImageDrawable(drawable);
        return this;
    }

    public BaseViewHolder<M> setImageDrawableRes(int viewId, int drawableRes) {
        Drawable drawable = ContextCompat.GetDrawable(mContext, drawableRes);
        return setImageDrawable(viewId, drawable);
    }

    public BaseViewHolder<M> setImageUrl(int viewId, String imgUrl) {
        ImageView view = getView<ImageView>(viewId);
        Glide.With(mContext).Load(imgUrl).Into(view);
        return this;
    }

    public BaseViewHolder<M> setImageUrl(int viewId, String imgUrl, int placeHolderRes) {
        ImageView view = getView<ImageView>(viewId);
        Glide.With(mContext).Load(imgUrl).Placeholder(placeHolderRes).Into(view);
        return this;
    }

    public BaseViewHolder<M> setCircleImageUrl(int viewId, String imgUrl, int placeHolderRes) {
        ImageView view = getView<ImageView>(viewId);
            Glide.With(mContext).Load(imgUrl)
                .Placeholder(placeHolderRes)
                .Transform(new GlideRoundTransform(mContext))
                .Into(view);
            return this;
    }

    public BaseViewHolder<M> setRoundImageUrl(int viewId, String imgUrl, int placeHolderRes) {
        ImageView view = getView<ImageView>(viewId);
        Glide.With(mContext).Load(imgUrl)
                .Placeholder(placeHolderRes)
                .Transform(new GlideRoundTransform(mContext))
                .Into(view);
        return this;
    }

    public BaseViewHolder<M> setImageBitmap(int viewId, Bitmap imgBitmap) {
        ImageView view = getView<ImageView>(viewId);
        view.SetImageBitmap(imgBitmap);
        return this;
    }

    public BaseViewHolder<M> setVisible(int viewId, bool visible) {
        View view = getView(viewId);
        view.Visibility = visible ? ViewStates.Visible : ViewStates.Gone;
        return this;
    }

    public BaseViewHolder<M> setVisible(int viewId, int visible) {
        View view = getView(viewId);
        view.Visibility = ViewStates.Visible;
        return this;
    }

    public BaseViewHolder<M> setTag(int viewId, Java.Lang.Object tag) {
        View view = getView(viewId);
        view.Tag = tag;
        return this;
    }

    public BaseViewHolder<M> setTag(int viewId, int key, Java.Lang.Object tag) {
        View view = getView(viewId);
        view.SetTag(key, tag);
        return this;
    }

    public BaseViewHolder<M> setChecked(int viewId, bool check) {
        ICheckable view = getView(viewId) as ICheckable;
        view.Checked = check;
        return this;
    }

    public BaseViewHolder<M> setAlpha(int viewId, float value) {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb) {
            getView<View>(viewId).Alpha = value;
        } else {
            AlphaAnimation alpha = new AlphaAnimation(value, value);
            alpha.Duration = 0;
            alpha.FillAfter = true;
            getView<View>(viewId).StartAnimation(alpha);
        }
        return this;
    }

    public BaseViewHolder<M> setTypeface(int viewId, Typeface typeface) {
        TextView view = getView<TextView>(viewId);
        view.Typeface = typeface;
        view.PaintFlags = view.PaintFlags | PaintFlags.SubpixelText;
        return this;
    }

    public BaseViewHolder<M> setTypeface(Typeface typeface, int[] viewIds) {
        foreach (int viewId in viewIds) {
            TextView view = getView<TextView>(viewId);
                view.Typeface = typeface;
                view.PaintFlags = view.PaintFlags | PaintFlags.SubpixelText;
            }
        return this;
    }

    public BaseViewHolder<M> setOnClickListener(int viewId, View.IOnClickListener listener) {
        View view = getView(viewId);
        view.SetOnClickListener(listener);
        return this;
    }
    }
}