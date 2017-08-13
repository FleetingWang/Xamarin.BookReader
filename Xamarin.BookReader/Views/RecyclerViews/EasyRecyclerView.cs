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
using Android.Content.Res;
using AndroidResource = Android.Resource;
using static Android.Support.V7.Widget.RecyclerView;
using Android.Views.Animations;
using Xamarin.BookReader.Views.RecyclerViews.Adapters;
using Xamarin.BookReader.Views.RecyclerViews.Decorations;
using Xamarin.BookReader.Utils;
using Xamarin.BookReader.Views.RecyclerViews.Swipes;

namespace Xamarin.BookReader.Views.RecyclerViews
{
    [Register("xamarin.bookreader.views.recyclerviews.EasyRecyclerView")]
    public class EasyRecyclerView : FrameLayout
    {
        private Context mContext;

        public static string TAG = "EasyRecyclerView";
        public static bool DEBUG = false;
        protected RecyclerView mRecycler;
        protected TextView tipView;
        protected ViewGroup mProgressView;
        protected ViewGroup mEmptyView;
        protected ViewGroup mErrorView;
        private int mProgressId;
        private int mEmptyId;
        private int mErrorId;

        protected bool mClipToPadding;
        protected int mPadding;
        protected int mPaddingTop;
        protected int mPaddingBottom;
        protected int mPaddingLeft;
        protected int mPaddingRight;
        protected int mScrollbarStyle;
        protected int mScrollbar;

        protected RecyclerView.OnScrollListener mInternalOnScrollListener;
        protected RecyclerView.OnScrollListener mExternalOnScrollListener;

        protected SwipeRefreshLayout mPtrLayout;
        protected IOnRefreshListener mRefreshListener;

        public List<RecyclerView.ItemDecoration> decorations = new List<RecyclerView.ItemDecoration>();


        public SwipeRefreshLayout getSwipeToRefresh()
        {
            return mPtrLayout;
        }

        public RecyclerView getRecyclerView()
        {
            return mRecycler;
        }

        public EasyRecyclerView(Context context)
                : this(context, null)
        {

        }

        public EasyRecyclerView(Context context, IAttributeSet attrs)
                : this(context, attrs, 0)
        {

        }

        public EasyRecyclerView(Context context, IAttributeSet attrs, int defStyle)
                : base(context, attrs, defStyle)
        {

            mContext = context;
            if (attrs != null)
                initAttrs(attrs);
            initView();
        }

        protected void initAttrs(IAttributeSet attrs)
        {
            TypedArray a = Context.ObtainStyledAttributes(attrs, Resource.Styleable.superrecyclerview);
            try
            {
                mClipToPadding = a.GetBoolean(Resource.Styleable.superrecyclerview_recyclerClipToPadding, false);
                mPadding = (int)a.GetDimension(Resource.Styleable.superrecyclerview_recyclerPadding, -1.0f);
                mPaddingTop = (int)a.GetDimension(Resource.Styleable.superrecyclerview_recyclerPaddingTop, 0.0f);
                mPaddingBottom = (int)a.GetDimension(Resource.Styleable.superrecyclerview_recyclerPaddingBottom, 0.0f);
                mPaddingLeft = (int)a.GetDimension(Resource.Styleable.superrecyclerview_recyclerPaddingLeft, 0.0f);
                mPaddingRight = (int)a.GetDimension(Resource.Styleable.superrecyclerview_recyclerPaddingRight, 0.0f);
                mScrollbarStyle = a.GetInteger(Resource.Styleable.superrecyclerview_scrollbarStyle, -1);
                mScrollbar = a.GetInteger(Resource.Styleable.superrecyclerview_scrollbars, -1);

                mEmptyId = a.GetResourceId(Resource.Styleable.superrecyclerview_layout_empty, 0);
                mProgressId = a.GetResourceId(Resource.Styleable.superrecyclerview_layout_progress, 0);
                mErrorId = a.GetResourceId(Resource.Styleable.superrecyclerview_layout_error, Resource.Layout.common_net_error_view);
            }
            finally
            {
                a.Recycle();
            }
        }

        private void initView()
        {
            if (IsInEditMode)
            {
                return;
            }
            //生成主View
            var v = LayoutInflater.From(Context).Inflate(Resource.Layout.common_recyclerview, this);
            mPtrLayout = (SwipeRefreshLayout)v.FindViewById(Resource.Id.ptr_layout);
            mPtrLayout.Enabled = false;

            mProgressView = (ViewGroup)v.FindViewById(Resource.Id.progress);
            if (mProgressId != 0) LayoutInflater.From(Context).Inflate(mProgressId, mProgressView);
            mEmptyView = (ViewGroup)v.FindViewById(Resource.Id.empty);
            if (mEmptyId != 0) LayoutInflater.From(Context).Inflate(mEmptyId, mEmptyView);
            mErrorView = (ViewGroup)v.FindViewById(Resource.Id.error);
            if (mErrorId != 0) LayoutInflater.From(Context).Inflate(mErrorId, mErrorView);
            initRecyclerView(v);
        }

        public override bool DispatchTouchEvent(MotionEvent ev)
        {
            return mPtrLayout.DispatchTouchEvent(ev);
        }

        /**
         * @param left
         * @param top
         * @param right
         * @param bottom
         */
        public void setRecyclerPadding(int left, int top, int right, int bottom)
        {
            this.mPaddingLeft = left;
            this.mPaddingTop = top;
            this.mPaddingRight = right;
            this.mPaddingBottom = bottom;
            mRecycler.SetPadding(mPaddingLeft, mPaddingTop, mPaddingRight, mPaddingBottom);
        }

        public void setClipToPadding(bool isClip)
        {
            mRecycler.SetClipToPadding(isClip);
        }


        public void setEmptyView(View emptyView)
        {
            mEmptyView.RemoveAllViews();
            mEmptyView.AddView(emptyView);
        }

        public void setProgressView(View progressView)
        {
            mProgressView.RemoveAllViews();
            mProgressView.AddView(progressView);
        }

        public void setErrorView(View errorView)
        {
            mErrorView.RemoveAllViews();
            mErrorView.AddView(errorView);
        }

        public void setEmptyView(int emptyView)
        {
            mEmptyView.RemoveAllViews();
            LayoutInflater.From(Context).Inflate(emptyView, mEmptyView);
        }

        public void setProgressView(int progressView)
        {
            mProgressView.RemoveAllViews();
            LayoutInflater.From(Context).Inflate(progressView, mProgressView);
        }

        public void setErrorView(int errorView)
        {
            mErrorView.RemoveAllViews();
            LayoutInflater.From(Context).Inflate(errorView, mErrorView);
        }

        public void scrollToPosition(int position)
        {
            getRecyclerView().ScrollToPosition(position);
        }

        /**
         * Implement this method to customize the AbsListView
         */
        protected void initRecyclerView(View view)
        {
            mRecycler = (RecyclerView)view.FindViewById(AndroidResource.Id.List);
            tipView = (TextView)view.FindViewById(Resource.Id.tvTip);
            setItemAnimator(null);
            if (mRecycler != null)
            {
                mRecycler.HasFixedSize = true;
                mRecycler.SetClipToPadding(mClipToPadding);
                mInternalOnScrollListener = new CustomOnScrollListener(this);
                mRecycler.AddOnScrollListener(mInternalOnScrollListener);

                if (mPadding != -1.0f)
                {
                    mRecycler.SetPadding(mPadding, mPadding, mPadding, mPadding);
                }
                else
                {
                    mRecycler.SetPadding(mPaddingLeft, mPaddingTop, mPaddingRight, mPaddingBottom);
                }
                if (mScrollbarStyle != -1)
                {
                    mRecycler.ScrollBarStyle = (ScrollbarStyles)mScrollbarStyle;
                }
                switch (mScrollbar)
                {
                    case 0:
                        VerticalScrollBarEnabled = false;
                        break;
                    case 1:
                        HorizontalScrollBarEnabled = false;
                        break;
                    case 2:
                        VerticalScrollBarEnabled = false;
                        HorizontalScrollBarEnabled = false;
                        break;
                }
            }
        }

        public override bool HorizontalFadingEdgeEnabled
        {
            get => mRecycler.HorizontalFadingEdgeEnabled;
            set => mRecycler.HorizontalFadingEdgeEnabled = value;
        }

        /**
         * Set the layout manager to the recycler
         *
         * @param manager
         */
        public void setLayoutManager(RecyclerView.LayoutManager manager)
        {
            mRecycler.SetLayoutManager(manager);
        }

        /**
         * Set the ItemDecoration to the recycler
         *
         * @param color
         * @param height
         * @param paddingLeft
         * @param paddingRight
         */
        public void setItemDecoration(int color, int height, int paddingLeft, int paddingRight)
        {
            DividerDecoration itemDecoration = new DividerDecoration(color, height, paddingLeft, paddingRight);
            itemDecoration.setDrawLastItem(false);
            decorations.Add(itemDecoration);
            mRecycler.AddItemDecoration(itemDecoration);
        }


        public class EasyDataObserver : AdapterDataObserver
        {
            private EasyRecyclerView recyclerView;

            public EasyDataObserver(EasyRecyclerView recyclerView)
            {
                this.recyclerView = recyclerView;
            }

            public override void OnItemRangeChanged(int positionStart, int itemCount)
            {
                base.OnItemRangeChanged(positionStart, itemCount);
                update();
            }

            public override void OnItemRangeInserted(int positionStart, int itemCount)
            {
                base.OnItemRangeInserted(positionStart, itemCount);
                update();
            }

            public override void OnItemRangeRemoved(int positionStart, int itemCount)
            {
                base.OnItemRangeRemoved(positionStart, itemCount);
                update();
            }

            public override void OnItemRangeMoved(int fromPosition, int toPosition, int itemCount)
            {
                base.OnItemRangeMoved(fromPosition, toPosition, itemCount);

                update();
            }

            public override void OnChanged()
            {
                base.OnChanged();
                update();

            }

            //自动更改Container的样式
            private void update()
            {
                log("update");
                int count;
                var adapterObj = recyclerView.getAdapter();
                var adapterType = adapterObj.GetType();
                if (adapterType.IsAssignableFrom(typeof(RecyclerArrayAdapter<>)))
                {
                    count = (int)adapterType.GetMethod("getCount").Invoke(adapterObj, null);
                }
                else
                {
                    count = recyclerView.getAdapter().ItemCount;
                }

                if (count == 0 && !NetworkUtils.isAvailable(recyclerView.Context))
                {
                    recyclerView.showError();
                    return;
                }

                if (count == 0 && adapterType.GetMethod("getHeaderCount") != null && (int)adapterType.GetMethod("getHeaderCount").Invoke(adapterObj, null) == 0)
                {
                    log("no data:" + "show empty");
                    recyclerView.showEmpty();
                }
                else
                {
                    log("has data");
                    recyclerView.showRecycler();
                }
            }
        }

        /**
         * 设置适配器，关闭所有副view。展示recyclerView
         * 适配器有更新，自动关闭所有副view。根据条数判断是否展示EmptyView
         *
         * @param adapter
         */
        public void setAdapter(RecyclerView.Adapter adapter)
        {
            mRecycler.SetAdapter(adapter);
            adapter.RegisterAdapterDataObserver(new EasyDataObserver(this));
            showRecycler();
        }

        /**
         * 设置适配器，关闭所有副view。展示进度条View
         * 适配器有更新，自动关闭所有副view。根据条数判断是否展示EmptyView
         *
         * @param adapter
         */
        public void setAdapterWithProgress(RecyclerView.Adapter adapter)
        {
            mRecycler.SetAdapter(adapter);
            adapter.RegisterAdapterDataObserver(new EasyDataObserver(this));
            var adapterType = adapter.GetType();
            //只有Adapter为空时才显示ProgressView
            
            if (adapterType.IsAssignableFrom(typeof(RecyclerArrayAdapter<>)))
            {
                if ((int)adapterType.GetMethod("getCount").Invoke(adapter, null) == 0)
                {
                    showProgress();
                }
                else
                {
                    showRecycler();
                }
            }
            else
            {
                if (adapter.ItemCount == 0)
                {
                    showProgress();
                }
                else
                {
                    showRecycler();
                }
            }
        }

        /**
         * Remove the adapter from the recycler
         */
        public void clear()
        {
            mRecycler.SetAdapter(null);
        }


        private void hideAll()
        {
            mEmptyView.Visibility = ViewStates.Gone;
            mProgressView.Visibility = ViewStates.Gone;
            mErrorView.Visibility = ViewStates.Gone;
            //        mPtrLayout.SetRefreshing(false);
            mRecycler.Visibility = ViewStates.Invisible;
        }


        public void showError()
        {
            log("showError");
            if (mErrorView.ChildCount > 0)
            {
                hideAll();
                mErrorView.Visibility = ViewStates.Visible;
            }
            else
            {
                showRecycler();
            }
        }

        public void showEmpty()
        {
            log("showEmpty");
            if (mEmptyView.ChildCount > 0)
            {
                hideAll();
                mEmptyView.Visibility = ViewStates.Visible;
            }
            else
            {
                showRecycler();
            }
        }


        public void showProgress()
        {
            log("showProgress");
            if (mProgressView.ChildCount > 0)
            {
                hideAll();
                mProgressView.Visibility = ViewStates.Visible;
            }
            else
            {
                showRecycler();
            }
        }


        public void showRecycler()
        {
            log("showRecycler");
            hideAll();
            mRecycler.Visibility = ViewStates.Visible;
        }

        public void showTipViewAndDelayClose(String tip)
        {
            tipView.Text = tip;
            Animation mShowAction = new TranslateAnimation(Dimension.RelativeToSelf, 0.0f,
                    Dimension.RelativeToSelf, 0.0f, Dimension.RelativeToSelf,
                    -1.0f, Dimension.RelativeToSelf, 0.0f);
            mShowAction.Duration = 500;
            tipView.StartAnimation(mShowAction);
            tipView.Visibility = ViewStates.Visible;

            tipView.PostDelayed(() =>
            {
                Animation mHiddenAction = new TranslateAnimation(Dimension.RelativeToSelf,
                            0.0f, Dimension.RelativeToSelf, 0.0f,
                            Dimension.RelativeToSelf, 0.0f, Dimension.RelativeToSelf,
                            -1.0f);
                mHiddenAction.Duration = 500;
                tipView.StartAnimation(mHiddenAction);
                tipView.Visibility = ViewStates.Gone;
            }, 2200);
        }

        public void showTipView(String tip)
        {
            tipView.Text = tip;
            Animation mShowAction = new TranslateAnimation(Dimension.RelativeToSelf, 0.0f,
                Dimension.RelativeToSelf, 0.0f, Dimension.RelativeToSelf,
                -1.0f, Dimension.RelativeToSelf, 0.0f);
            mShowAction.Duration = 500;
            tipView.StartAnimation(mShowAction);
            tipView.Visibility = ViewStates.Visible;
        }

        public void hideTipView(long delayMillis)
        {
            tipView.PostDelayed(() =>
            {
                Animation mHiddenAction = new TranslateAnimation(Dimension.RelativeToSelf,
                            0.0f, Dimension.RelativeToSelf, 0.0f,
                            Dimension.RelativeToSelf, 0.0f, Dimension.RelativeToSelf,
                            -1.0f);
                mHiddenAction.Duration = 500;
                tipView.StartAnimation(mHiddenAction);
                tipView.Visibility = ViewStates.Gone;
            }, delayMillis);
        }

        public void setTipViewText(String tip)
        {
            if (!isTipViewVisible())
                showTipView(tip);
            else
                tipView.Text = tip;
        }

        public bool isTipViewVisible()
        {
            return tipView.Visibility == ViewStates.Visible;
        }


        /**
         * Set the listener when refresh is triggered and enable the SwipeRefreshLayout
         *
         * @param listener
         */
        public void setRefreshListener(IOnRefreshListener listener)
        {
            mPtrLayout.Enabled = true;
            mPtrLayout.setOnRefreshListener(listener);
            this.mRefreshListener = listener;
        }

        public void setRefreshing(bool isRefreshing)
        {
            mPtrLayout.Post(() =>
            {
                if (isRefreshing)
                { // 避免刷新的loadding和progressview 同时显示
                    mProgressView.Visibility = ViewStates.Gone;
                }
                mPtrLayout.setRefreshing(isRefreshing);
            });
        }

        public void setRefreshing(bool isRefreshing, bool isCallbackListener)
        {
            mPtrLayout.Post(() =>
            {
                mPtrLayout.setRefreshing(isRefreshing);
                if (isRefreshing && isCallbackListener && mRefreshListener != null)
                {
                    mRefreshListener.onRefresh();
                }
            });
        }

        /**
         * Set the colors for the SwipeRefreshLayout states
         *
         * @param colRes
         */
        public void setRefreshingColorResources(int[] colRes)
        {
            mPtrLayout.setColorSchemeResources(colRes);
        }

        /**
         * Set the colors for the SwipeRefreshLayout states
         *
         * @param col
         */
        public void setRefreshingColor(int[] col)
        {
            mPtrLayout.setColorSchemeColors(col);
        }

        /**
         * Set the scroll listener for the recycler
         *
         * @param listener
         */
        public void setOnScrollListener(RecyclerView.OnScrollListener listener)
        {
            mExternalOnScrollListener = listener;
        }

        /**
         * Add the onItemTouchListener for the recycler
         *
         * @param listener
         */
        public void addOnItemTouchListener(RecyclerView.IOnItemTouchListener listener)
        {
            mRecycler.AddOnItemTouchListener(listener);
        }

        /**
         * Remove the onItemTouchListener for the recycler
         *
         * @param listener
         */
        public void removeOnItemTouchListener(RecyclerView.IOnItemTouchListener listener)
        {
            mRecycler.RemoveOnItemTouchListener(listener);
        }

        /**
         * @return the recycler adapter
         */
        public RecyclerView.Adapter getAdapter()
        {
            return mRecycler.GetAdapter();
        }


        public void setOnTouchListener(IOnTouchListener listener)
        {
            mRecycler.SetOnTouchListener(listener);
        }

        public void setItemAnimator(RecyclerView.ItemAnimator animator)
        {
            mRecycler.SetItemAnimator(animator);
        }

        public void addItemDecoration(RecyclerView.ItemDecoration itemDecoration)
        {
            mRecycler.AddItemDecoration(itemDecoration);
        }

        public void addItemDecoration(RecyclerView.ItemDecoration itemDecoration, int index)
        {
            mRecycler.AddItemDecoration(itemDecoration, index);
        }

        public void removeItemDecoration(RecyclerView.ItemDecoration itemDecoration)
        {
            mRecycler.RemoveItemDecoration(itemDecoration);
        }

        public void removeAllItemDecoration()
        {
            foreach (RecyclerView.ItemDecoration decoration in decorations)
            {
                mRecycler.RemoveItemDecoration(decoration);
            }
        }


        /**
         * @return inflated error view or null
         */
        public View getErrorView()
        {
            if (mErrorView.ChildCount > 0) return mErrorView.GetChildAt(0);
            return null;
        }

        /**
         * @return inflated progress view or null
         */
        public View getProgressView()
        {
            if (mProgressView.ChildCount > 0) return mProgressView.GetChildAt(0);
            return null;
        }


        /**
         * @return inflated empty view or null
         */
        public View getEmptyView()
        {
            if (mEmptyView.ChildCount > 0) return mEmptyView.GetChildAt(0);
            return null;
        }

        private static void log(String content)
        {
            if (DEBUG)
            {
                Log.Info(TAG, content);
            }
        }

        private class CustomOnScrollListener : OnScrollListener
        {
            private EasyRecyclerView _easyRecyclerView;

            public CustomOnScrollListener(EasyRecyclerView easyRecyclerView)
            {
                this._easyRecyclerView = easyRecyclerView;
            }

            public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
            {
                base.OnScrolled(recyclerView, dx, dy);
                if (_easyRecyclerView.mExternalOnScrollListener != null)
                    _easyRecyclerView.mExternalOnScrollListener.OnScrolled(recyclerView, dx, dy);
            }

            public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
            {
                base.OnScrollStateChanged(recyclerView, newState);
                if (_easyRecyclerView.mExternalOnScrollListener != null)
                    _easyRecyclerView.mExternalOnScrollListener.OnScrollStateChanged(recyclerView, newState);
            }
        }
    }
}