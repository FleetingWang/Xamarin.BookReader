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
using static Android.Support.V4.Widget.SwipeRefreshLayout;
using Android.Views.Animations;
using static Android.Views.Animations.Animation;
using Android.Util;
using Android.Support.V4.View;
using Java.Lang;
using Android.Content.Res;
using Android.Support.V7.Widget;
using AndroidResource = Android.Resource;
using Android.Support.V4.Content;

namespace Xamarin.BookReader.Views.RecyclerViews.Swipes
{
    public class SwipeRefreshLayout: FrameLayout
    {
        // Maps to ProgressBar.Large style
    public static int LARGE = MaterialProgressDrawable.LARGE;
    // Maps to ProgressBar default style
    public static int DEFAULT = MaterialProgressDrawable.DEFAULT;

        private static string LOG_TAG = "SwipeRefreshLayout";// SwipeRefreshLayout.Class.getSimpleName();

    private static int MAX_ALPHA = 255;
    private static int STARTING_PROGRESS_ALPHA = (int) (.3f * MAX_ALPHA);

    private static int CIRCLE_DIAMETER = 40;
    private static int CIRCLE_DIAMETER_LARGE = 56;

    private static float DECELERATE_INTERPOLATION_FACTOR = 2f;
    private static int INVALID_POINTER = -1;
    private static float DRAG_RATE = .5f;

    // Max amount of circle that can be filled by progress during swipe gesture,
    // where 1.0 is a full circle
    private static float MAX_PROGRESS_ANGLE = .8f;

    private static int SCALE_DOWN_DURATION = 150;

    private static int ALPHA_ANIMATION_DURATION = 300;

    private static int ANIMATE_TO_TRIGGER_DURATION = 200;

    private static int ANIMATE_TO_START_DURATION = 200;

    // Default background for the progress spinner
    private static int CIRCLE_BG_LIGHT = 0xFAFAFA;
    // Default offset in dips from the top of the view to where the progress spinner should stop
    private static int DEFAULT_CIRCLE_TARGET = 64;

    private View mTarget; // the target of the gesture
    private IOnRefreshListener mListener;
    private bool mRefreshing = false;
    private int mTouchSlop;
    private float mTotalDragDistance = -1;
    private int mMediumAnimationDuration;
    private int mCurrentTargetOffsetTop;
    // Whether or not the starting offset has been determined.
    private bool mOriginalOffsetCalculated = false;

    private float mInitialMotionY;
    private float mInitialDownY;
    private bool mIsBeingDragged;
    private int mActivePointerId = INVALID_POINTER;
    // Whether this item is scaled up rather than clipped
    private bool mScale;

    // Target is returning to its start offset because it was cancelled or a
    // refresh was triggered.
    private bool mReturningToStart;
    private DecelerateInterpolator mDecelerateInterpolator;
    private static int[] LAYOUT_ATTRS = new int[] {
        AndroidResource.Attribute.Enabled
    };

    private CircleImageView mCircleView;
    private int mCircleViewIndex = -1;

    protected int mFrom;

    private float mStartingScale;

    protected int mOriginalOffsetTop;

    private MaterialProgressDrawable mProgress;

    private Animation mScaleAnimation;

    private Animation mScaleDownAnimation;

    private Animation mAlphaStartAnimation;

    private Animation mAlphaMaxAnimation;

    private Animation mScaleDownToStartAnimation;

    private float mSpinnerFinalOffset;

    private bool mNotify;

    private int mCircleWidth;

    private int mCircleHeight;

    // Whether the client has set a custom starting position;
    private bool mUsingCustomStart;
    private IAnimationListener mRefreshListener;
    //private IAnimationListener mRefreshListener = new AnimationListener() {
    //    @Override
    //    public void onAnimationStart(Animation animation) {
    //    }

        //    @Override
        //    public void onAnimationRepeat(Animation animation) {
        //    }

        //    @Override
        //    public void onAnimationEnd(Animation animation) {
        //        if (mRefreshing) {
        //            // Make sure the progress view is fully visible
        //            mProgress.setAlpha(MAX_ALPHA);
        //            mProgress.start();
        //            if (mNotify) {
        //                if (mListener != null) {
        //                    mListener.onRefresh();
        //                }
        //            }
        //        } else {
        //            mProgress.stop();
        //            mCircleView.setVisibility(View.ViewStates.Gone);
        //            setColorViewAlpha(MAX_ALPHA);
        //            // Return the circle to its start position
        //            if (mScale) {
        //                setAnimationProgress(0 /* animation complete and view is hidden */);
        //            } else {
        //                setTargetOffsetTopAndBottom(mOriginalOffsetTop - mCurrentTargetOffsetTop,
        //                        true /* requires update */);
        //            }
        //        }
        //        mCurrentTargetOffsetTop = mCircleView.Top;
        //    }
        //};

        private void setColorViewAlpha(int targetAlpha) {
        mCircleView.Background.SetAlpha(targetAlpha);
        mProgress.SetAlpha(targetAlpha);
    }

    /**
     * The refresh indicator starting and resting position is always positioned
     * near the top of the refreshing content. This position is a consistent
     * location, but can be adjusted in either direction based on whether or not
     * there is a toolbar or actionbar present.
     *
     * @param scale Set to true if there is no view at a higher z-order than
     *            where the progress spinner is set to appear.
     * @param start The offset in pixels from the top of this view at which the
     *            progress spinner should appear.
     * @param end The offset in pixels from the top of this view at which the
     *            progress spinner should come to rest after a successful swipe
     *            gesture.
     */
    public void setProgressViewOffset(bool scale, int start, int end) {
        mScale = scale;
            mCircleView.Visibility = ViewStates.Gone;
        mOriginalOffsetTop = mCurrentTargetOffsetTop = start;
        mSpinnerFinalOffset = end;
        mUsingCustomStart = true;
        mCircleView.Invalidate();
    }

    /**
     * The refresh indicator resting position is always positioned near the top
     * of the refreshing content. This position is a consistent location, but
     * can be adjusted in either direction based on whether or not there is a
     * toolbar or actionbar present.
     *
     * @param scale Set to true if there is no view at a higher z-order than
     *            where the progress spinner is set to appear.
     * @param end The offset in pixels from the top of this view at which the
     *            progress spinner should come to rest after a successful swipe
     *            gesture.
     */
    public void setProgressViewEndTarget(bool scale, int end) {
        mSpinnerFinalOffset = end;
        mScale = scale;
        mCircleView.Invalidate();
    }

    /**
     * One of DEFAULT, or LARGE.
     */
    public void setSize(int size) {
        if (size != MaterialProgressDrawable.LARGE && size != MaterialProgressDrawable.DEFAULT) {
            return;
        }
        DisplayMetrics metrics = Resources.DisplayMetrics;
        if (size == MaterialProgressDrawable.LARGE) {
            mCircleHeight = mCircleWidth = (int) (CIRCLE_DIAMETER_LARGE * metrics.Density);
        } else {
            mCircleHeight = mCircleWidth = (int) (CIRCLE_DIAMETER * metrics.Density);
        }
        // force the bounds of the progress circle inside the circle view to
        // update by setting it to null before updating its size and then
        // re-setting it
        mCircleView.SetImageDrawable(null);
        mProgress.updateSizes(size);
        mCircleView.SetImageDrawable(mProgress);
    }

    /**
     * Simple constructor to use when creating a SwipeRefreshLayout from code.
     *
     * @param context
     */
    public SwipeRefreshLayout(Context context)
            : this(context, null)
        {
        
    }

    /**
     * Constructor that is called when inflating SwipeRefreshLayout from XML.
     *
     * @param context
     * @param attrs
     */
    public SwipeRefreshLayout(Context context, IAttributeSet attrs)
            :base(context, attrs)
        {
        

        mTouchSlop = ViewConfiguration.Get(context).ScaledTouchSlop;

        mMediumAnimationDuration = Resources.GetInteger(
                AndroidResource.Integer.ConfigMediumAnimTime);

        SetWillNotDraw(false);
        mDecelerateInterpolator = new DecelerateInterpolator(DECELERATE_INTERPOLATION_FACTOR);

        TypedArray a = context.ObtainStyledAttributes(attrs, LAYOUT_ATTRS);
        Enabled = (a.GetBoolean(0, true));
        a.Recycle();

        DisplayMetrics metrics = Resources.DisplayMetrics;
        mCircleWidth = (int) (CIRCLE_DIAMETER * metrics.Density);
        mCircleHeight = (int) (CIRCLE_DIAMETER * metrics.Density);

        createProgressView();
        ViewCompat.SetChildrenDrawingOrderEnabled(this, true);
        // the absolute offset has to take into account that the circle starts at an offset
        mSpinnerFinalOffset = DEFAULT_CIRCLE_TARGET * metrics.Density;
        mTotalDragDistance = mSpinnerFinalOffset;

        RequestDisallowInterceptTouchEvent(true);
    }

    protected int getChildDrawingOrder(int childCount, int i) {
        if (mCircleViewIndex < 0) {
            return i;
        } else if (i == childCount - 1) {
            // Draw the selected child last
            return mCircleViewIndex;
        } else if (i >= mCircleViewIndex) {
            // Move the children after the selected child earlier one
            return i + 1;
        } else {
            // Keep the children before the selected child the same
            return i;
        }
    }

    private void createProgressView() {
        mCircleView = new CircleImageView(Context, CIRCLE_BG_LIGHT, CIRCLE_DIAMETER/2);
        mProgress = new MaterialProgressDrawable(Context, this);
        mProgress.setBackgroundColor(CIRCLE_BG_LIGHT);
        mCircleView.SetImageDrawable(mProgress);
            mCircleView.Visibility = ViewStates.Gone;
        AddView(mCircleView);
    }

    /**
     * Set the listener to be notified when a refresh is triggered via the swipe
     * gesture.
     */
    public void setOnRefreshListener(IOnRefreshListener listener) {
        mListener = listener;
    }

    /**
     * Pre API 11, alpha is used to make the progress circle appear instead of scale.
     */
    private bool isAlphaUsedForScale() {
            return Build.VERSION.SdkInt < BuildVersionCodes.Honeycomb;// 11;
    }

    /**
     * Notify the widget that refresh state has changed. Do not call this when
     * refresh is triggered by a swipe gesture.
     *
     * @param refreshing Whether or not the view should show refresh progress.
     */
    public void setRefreshing(bool refreshing) {
        if (refreshing && mRefreshing != refreshing) {
            // scale and show
            mRefreshing = refreshing;
            int endTarget = 0;
            if (!mUsingCustomStart) {
                endTarget = (int) (mSpinnerFinalOffset + mOriginalOffsetTop);
            } else {
                endTarget = (int) mSpinnerFinalOffset;
            }
            setTargetOffsetTopAndBottom(endTarget - mCurrentTargetOffsetTop,
                    true /* requires update */);
            mNotify = false;
            startScaleUpAnimation(mRefreshListener);
        } else {
            setRefreshing(refreshing, false /* notify */);
        }
    }

    private void startScaleUpAnimation(IAnimationListener listener) {
        mCircleView.Visibility = ViewStates.Visible;
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb)
            { // 11
              // Pre API 11, alpha is used in place of scale up to show the
              // progress circle appearing.
              // Don't adjust the alpha during appearance otherwise.
                mProgress.SetAlpha(MAX_ALPHA);
        }
        //mScaleAnimation = new Animation() {
        //    @Override
        //    public void applyTransformation(float interpolatedTime, Transformation t) {
        //        setAnimationProgress(interpolatedTime);
        //    }
        //};
        mScaleAnimation.Duration = (mMediumAnimationDuration);
        if (listener != null) {
            mCircleView.setAnimationListener(listener);
        }
        mCircleView.ClearAnimation();
        mCircleView.StartAnimation(mScaleAnimation);
    }

    /**
     * Pre API 11, this does an alpha animation.
     * @param progress
     */
    private void setAnimationProgress(float progress) {
        if (isAlphaUsedForScale()) {
            setColorViewAlpha((int) (progress * MAX_ALPHA));
        } else {
            ViewCompat.SetScaleX(mCircleView, progress);
            ViewCompat.SetScaleY(mCircleView, progress);
        }
    }

    private void setRefreshing(bool refreshing, bool notify) {
        if (mRefreshing != refreshing) {
            mNotify = notify;
            ensureTarget();
            mRefreshing = refreshing;
            if (mRefreshing) {
                animateOffsetToCorrectPosition(mCurrentTargetOffsetTop, mRefreshListener);
            } else {
                startScaleDownAnimation(mRefreshListener);
            }
        }
    }

    private void startScaleDownAnimation(IAnimationListener listener) {
        //mScaleDownAnimation = new Animation() {
        //    @Override
        //    public void applyTransformation(float interpolatedTime, Transformation t) {
        //        setAnimationProgress(1 - interpolatedTime);
        //    }
        //};
        mScaleDownAnimation.Duration = (SCALE_DOWN_DURATION);
        mCircleView.setAnimationListener(listener);
        mCircleView.ClearAnimation();
        mCircleView.StartAnimation(mScaleDownAnimation);
    }

    private void startProgressAlphaStartAnimation() {
        mAlphaStartAnimation = startAlphaAnimation(mProgress.Alpha, STARTING_PROGRESS_ALPHA);
    }

    private void startProgressAlphaMaxAnimation() {
        mAlphaMaxAnimation = startAlphaAnimation(mProgress.Alpha, MAX_ALPHA);
    }

    private Animation startAlphaAnimation(int startingAlpha, int endingAlpha) {
        // Pre API 11, alpha is used in place of scale. Don't also use it to
        // show the trigger point.
        if (mScale && isAlphaUsedForScale()) {
            return null;
        }
            Animation alpha = null;
            //Animation alpha = new Animation() {
            //    @Override
            //    public void applyTransformation(float interpolatedTime, Transformation t) {
            //        mProgress
            //                .setAlpha((int) (startingAlpha+ ((endingAlpha - startingAlpha)
            //                        * interpolatedTime)));
            //    }
            //};
            alpha.Duration = (ALPHA_ANIMATION_DURATION);
        // Clear out the previous animation listeners.
        mCircleView.setAnimationListener(null);
        mCircleView.ClearAnimation();
        mCircleView.StartAnimation(alpha);
        return alpha;
    }

    /**
     * @deprecated Use {@link #setProgressBackgroundColorSchemeResource(int)}
     */
    [Deprecated]
    public void setProgressBackgroundColor(int colorRes) {
        setProgressBackgroundColorSchemeResource(colorRes);
    }

    /**
     * Set the background color of the progress spinner disc.
     *
     * @param colorRes Resource id of the color.
     */
    public void setProgressBackgroundColorSchemeResource(int colorRes) {
        setProgressBackgroundColorSchemeColor(ContextCompat.GetColor(Context, colorRes));
    }

    /**
     * Set the background color of the progress spinner disc.
     *
     * @param color
     */
    public void setProgressBackgroundColorSchemeColor(int color) {
        mCircleView.SetBackgroundColor(color);
        mProgress.setBackgroundColor(color);
    }

    /**
     * @deprecated Use {@link #setColorSchemeResources(int...)}
     */
    [Deprecated]
    public void setColorScheme(int[] colors) {
        setColorSchemeResources(colors);
    }

    /**
     * Set the color resources used in the progress animation from color resources.
     * The first color will also be the color of the bar that grows in response
     * to a user swipe gesture.
     *
     * @param colorResIds
     */
    public void setColorSchemeResources(int[] colorResIds) {
        int[] colorRes = new int[colorResIds.Length];
        for (int i = 0; i < colorResIds.Length; i++) {
            colorRes[i] = ContextCompat.GetColor(Context, colorResIds[i]);
        }
        setColorSchemeColors(colorRes);
    }

    /**
     * Set the colors used in the progress animation. The first
     * color will also be the color of the bar that grows in response to a user
     * swipe gesture.
     *
     * @param colors
     */
    public void setColorSchemeColors(int[] colors) {
        ensureTarget();
        mProgress.setColorSchemeColors(colors);
    }

    /**
     * @return Whether the SwipeRefreshWidget is actively showing refresh
     *         progress.
     */
    public bool isRefreshing() {
        return mRefreshing;
    }

    private void ensureTarget() {
        // Don't bother getting the parent height if the parent hasn't been laid
        // out yet.
        if (mTarget == null) {
            for (int i = 0; i < ChildCount; i++) {
                View child = GetChildAt(i);
                if (child.GetType().IsAssignableFrom(typeof(RecyclerView))) {
                    mTarget = child;
                    break;
                }
            }
        }
    }

    /**
     * Set the distance to trigger a sync in dips
     *
     * @param distance
     */
    public void setDistanceToTriggerSync(int distance) {
        mTotalDragDistance = distance;
    }

    protected override void OnLayout(bool changed, int left, int top, int right, int bottom) {
        base.OnLayout(changed,left,top,right,bottom);
        int width = MeasuredWidth;
        int height = MeasuredHeight;
        if (ChildCount == 0) {
            return;
        }
        if (mTarget == null) {
            ensureTarget();
        }
        if (mTarget == null) {
            return;
        }
        View child = mTarget;
        int childLeft = PaddingLeft;
        int childTop = PaddingTop;
        int childWidth = width - PaddingLeft - PaddingRight;
        int childHeight = height - PaddingTop - PaddingBottom;
        child.Layout(childLeft, childTop, childLeft + childWidth, childTop + childHeight);
        int circleWidth = mCircleView.MeasuredWidth;
        int circleHeight = mCircleView.MeasuredHeight;
        mCircleView.Layout((width / 2 - circleWidth / 2), mCurrentTargetOffsetTop,
                (width / 2 + circleWidth / 2), mCurrentTargetOffsetTop + circleHeight);
    }

    protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec) {
        base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        if (mTarget == null) {
            ensureTarget();
        }
        if (mTarget == null) {
            return;
        }
        mTarget.Measure(MeasureSpec.MakeMeasureSpec(
                MeasuredWidth - PaddingLeft - PaddingRight,
                MeasureSpecMode.Exactly), MeasureSpec.MakeMeasureSpec(
                MeasuredHeight - PaddingTop - PaddingBottom, MeasureSpecMode.Exactly));
        mCircleView.Measure(MeasureSpec.MakeMeasureSpec(mCircleWidth, MeasureSpecMode.Exactly),
                MeasureSpec.MakeMeasureSpec(mCircleHeight, MeasureSpecMode.Exactly));
        if (!mUsingCustomStart && !mOriginalOffsetCalculated) {
            mOriginalOffsetCalculated = true;
            mCurrentTargetOffsetTop = mOriginalOffsetTop = -mCircleView.MeasuredHeight;
        }
        mCircleViewIndex = -1;
        // Get the index of the circleview.
        for (int index = 0; index < ChildCount; index++) {
            if (GetChildAt(index) == mCircleView) {
                mCircleViewIndex = index;
                break;
            }
        }
    }

    /**
     * Get the diameter of the progress circle that is displayed as part of the
     * swipe to refresh layout. This is not valid until a measure pass has
     * completed.
     *
     * @return Diameter in pixels of the progress circle view.
     */
    public int getProgressCircleDiameter() {
        return mCircleView != null ?mCircleView.MeasuredHeight : 0;
    }

    /**
     * @return Whether it is possible for the child view of this layout to
     *         scroll up. Override this if the child view is a custom view.
     */
    public bool canChildScrollUp() {
//        //For make it can work when my recycler view is in Gone.
//        return false;
        if (Build.VERSION.SdkInt < BuildVersionCodes.IceCreamSandwich)
            {// 14
                if (mTarget is AbsListView) {
                AbsListView absListView = (AbsListView) mTarget;
                return absListView.ChildCount > 0
                        && (absListView.FirstVisiblePosition > 0 || absListView.GetChildAt(0)
                                .Top < absListView.PaddingTop);
            } else {
                return ViewCompat.CanScrollVertically(mTarget, -1) || mTarget.ScrollY > 0;
            }
        } else {
            return ViewCompat.CanScrollVertically(mTarget, -1);
        }
    }

    public override bool DispatchTouchEvent(MotionEvent ev) {
        return base.DispatchTouchEvent(ev);
    }

    public override bool OnInterceptTouchEvent(MotionEvent ev) {
        ensureTarget();

        MotionEventActions action = ev.Action;
        if (mReturningToStart && action == MotionEventActions.Down) {
            mReturningToStart = false;
        }
        if (!Enabled || mReturningToStart || canChildScrollUp() || mRefreshing) {
            // Fail fast if we're not in a state where a swipe is possible
            return false;
        }
        switch (action) {
            case MotionEventActions.Down:
                setTargetOffsetTopAndBottom(mOriginalOffsetTop - mCircleView.Top, true);
                mActivePointerId = ev.GetPointerId(0);
                mIsBeingDragged = false;
                float initialDownY = getMotionEventY(ev, mActivePointerId);
                if (initialDownY == -1) {
                    return false;
                }
                mInitialDownY = initialDownY;
                break;

            case MotionEventActions.Move:
                if (mActivePointerId == INVALID_POINTER) {
                    Log.Error(LOG_TAG, "Got ACTION_MOVE event but don't have an active pointer id.");
                    return false;
                }

                float y = getMotionEventY(ev, mActivePointerId);
                if (y == -1) {
                    return false;
                }
                float yDiff = y - mInitialDownY;
                if (yDiff > mTouchSlop && !mIsBeingDragged) {
                    mInitialMotionY = mInitialDownY + mTouchSlop;
                    mIsBeingDragged = true;
                    mProgress.SetAlpha(STARTING_PROGRESS_ALPHA);
                }
                break;

            case MotionEventActions.PointerUp:
                onSecondaryPointerUp(ev);
                break;

            case MotionEventActions.Up:
            case MotionEventActions.Cancel:
                mIsBeingDragged = false;
                mActivePointerId = INVALID_POINTER;
                break;
        }
        return mIsBeingDragged;
    }

    private float getMotionEventY(MotionEvent ev, int activePointerId) {
        int index = ev.FindPointerIndex(activePointerId);
        if (index < 0) {
            return -1;
        }
        return ev.GetY(index);
    }

    public override void RequestDisallowInterceptTouchEvent(bool b) {
        // Nope.
        //Why Nope?
        base.RequestDisallowInterceptTouchEvent(b);
    }

    private bool isAnimationRunning(Animation animation) {
        return animation != null && animation.HasStarted && !animation.HasEnded;
    }

    public override bool OnTouchEvent(MotionEvent ev) {
            MotionEventActions action = ev.Action;

        if (mReturningToStart && action == MotionEventActions.Down) {
            mReturningToStart = false;
        }

        if (!Enabled || mReturningToStart || canChildScrollUp()) {
            // Fail fast if we're not in a state where a swipe is possible
            return false;
        }

        switch (action) {
            case MotionEventActions.Down:
                mActivePointerId = ev.GetPointerId(0);
                mIsBeingDragged = false;
                break;

            case MotionEventActions.Move:
                int pointerIndex = ev.FindPointerIndex(mActivePointerId);
                if (pointerIndex < 0) {
                    Log.Error(LOG_TAG, "Got ACTION_MOVE event but have an invalid active pointer id.");
                    return false;
                }

                float y = ev.GetY(pointerIndex);
                float overscrollTop = (y - mInitialMotionY) * DRAG_RATE;
                if (mIsBeingDragged) {

                    mProgress.showArrow(true);
                    float originalDragPercent = overscrollTop / mTotalDragDistance;
                    if (originalDragPercent < 0) {
                        return false;
                    }
                    float dragPercent = System.Math.Min(1f, System.Math.Abs(originalDragPercent));
                    float adjustedPercent = (float)System.Math.Max(dragPercent - .4, 0) * 5 / 3;
                    float extraOS = System.Math.Abs(overscrollTop) - mTotalDragDistance;
                    float slingshotDist = mUsingCustomStart ? mSpinnerFinalOffset
                            - mOriginalOffsetTop : mSpinnerFinalOffset;
                    float tensionSlingshotPercent = System.Math.Max(0,
                            System.Math.Min(extraOS, slingshotDist * 2) / slingshotDist);
                    float tensionPercent = (float) ((tensionSlingshotPercent / 4) - System.Math.Pow(
                            (tensionSlingshotPercent / 4), 2)) * 2f;
                    float extraMove = (slingshotDist) * tensionPercent * 2;

                    int targetY = mOriginalOffsetTop
                            + (int) ((slingshotDist * dragPercent) + extraMove);
                    // where 1.0f is a full circle
                    if (mCircleView.Visibility != ViewStates.Visible) {
                        mCircleView.Visibility = ViewStates.Visible;
                    }
                    if (!mScale) {
                        ViewCompat.SetScaleX(mCircleView, 1f);
                        ViewCompat.SetScaleY(mCircleView, 1f);
                    }
                    if (overscrollTop < mTotalDragDistance) {
                        if (mScale) {
                            setAnimationProgress(overscrollTop / mTotalDragDistance);
                        }
                        if (mProgress.Alpha > STARTING_PROGRESS_ALPHA
                                && !isAnimationRunning(mAlphaStartAnimation)) {
                            // Animate the alpha
                            startProgressAlphaStartAnimation();
                        }
                        float strokeStart = adjustedPercent * .8f;
                        mProgress.setStartEndTrim(0f, System.Math.Min(MAX_PROGRESS_ANGLE, strokeStart));
                        mProgress.setArrowScale(System.Math.Min(1f, adjustedPercent));
                    } else {
                        if (mProgress.Alpha < MAX_ALPHA
                                && !isAnimationRunning(mAlphaMaxAnimation)) {
                            // Animate the alpha
                            startProgressAlphaMaxAnimation();
                        }
                    }
                    float rotation = (-0.25f + .4f * adjustedPercent + tensionPercent * 2) * .5f;
                    mProgress.setProgressRotation(rotation);
                    setTargetOffsetTopAndBottom(targetY - mCurrentTargetOffsetTop,
                            true /* requires update */);
                }
                break;
            
            //case MotionEventActions.Down: 
            //    int index = ev.ActionIndex;
            //    mActivePointerId = ev.GetPointerId(index);
            //    break;
            

            case MotionEventActions.PointerUp:
                onSecondaryPointerUp(ev);
                break;

            case MotionEventActions.Up:
            case MotionEventActions.Cancel: {
                if (mActivePointerId == INVALID_POINTER) {
                    if (action == MotionEventActions.Up) {
                        Log.Error(LOG_TAG, "Got ACTION_UP event but don't have an active pointer id.");
                    }
                    return false;
                }
                int pointerIndex0 = ev.FindPointerIndex(mActivePointerId);
                float y0 = ev.GetY(pointerIndex0);
                float overscrollTop0 = (y0 - mInitialMotionY) * DRAG_RATE;
                mIsBeingDragged = false;
                if (overscrollTop0 > mTotalDragDistance) {
                    setRefreshing(true, true /* notify */);
                } else {
                    // cancel refresh
                    mRefreshing = false;
                    mProgress.setStartEndTrim(0f, 0f);
                    IAnimationListener listener = null;
                    if (!mScale) {
                        //listener = new AnimationListener() {

                        //    @Override
                        //    public void onAnimationStart(Animation animation) {
                        //    }

                        //    @Override
                        //    public void onAnimationEnd(Animation animation) {
                        //        if (!mScale) {
                        //            startScaleDownAnimation(null);
                        //        }
                        //    }

                        //    @Override
                        //    public void onAnimationRepeat(Animation animation) {
                        //    }

                        //};
                    }
                    animateOffsetToStartPosition(mCurrentTargetOffsetTop, listener);
                    mProgress.showArrow(false);
                }
                mActivePointerId = INVALID_POINTER;
                return false;
            }
        }
        return true;
    }

    private void animateOffsetToCorrectPosition(int from, IAnimationListener listener) {
        mFrom = from;
        mAnimateToCorrectPosition.Reset();
        mAnimateToCorrectPosition.Duration = (ANIMATE_TO_TRIGGER_DURATION);
        mAnimateToCorrectPosition.Interpolator = (mDecelerateInterpolator);
        if (listener != null) {
            mCircleView.setAnimationListener(listener);
        }
        mCircleView.ClearAnimation();
        mCircleView.StartAnimation(mAnimateToCorrectPosition);
    }

    private void animateOffsetToStartPosition(int from, IAnimationListener listener) {
        if (mScale) {
            // Scale the item back down
            startScaleDownReturnToStartAnimation(from, listener);
        } else {
            mFrom = from;
            mAnimateToStartPosition.Reset();
            mAnimateToStartPosition.Duration = (ANIMATE_TO_START_DURATION);
            mAnimateToStartPosition.Interpolator = (mDecelerateInterpolator);
            if (listener != null) {
                mCircleView.setAnimationListener(listener);
            }
            mCircleView.ClearAnimation();
            mCircleView.StartAnimation(mAnimateToStartPosition);
        }
    }
        private Animation mAnimateToCorrectPosition;
    //private Animation mAnimateToCorrectPosition = new Animation() {
    //    @Override
    //    public void applyTransformation(float interpolatedTime, Transformation t) {
    //        int targetTop = 0;
    //        int endTarget = 0;
    //        if (!mUsingCustomStart) {
    //            endTarget = (int) (mSpinnerFinalOffset - Math.abs(mOriginalOffsetTop));
    //        } else {
    //            endTarget = (int) mSpinnerFinalOffset;
    //        }
    //        targetTop = (mFrom + (int) ((endTarget - mFrom) * interpolatedTime));
    //        int offset = targetTop - mCircleView.Top;
    //        setTargetOffsetTopAndBottom(offset, false /* requires update */);
    //        mProgress.setArrowScale(1 - interpolatedTime);
    //    }
    //};

        private void moveToStart(float interpolatedTime) {
        int targetTop = 0;
        targetTop = (mFrom + (int) ((mOriginalOffsetTop - mFrom) * interpolatedTime));
        int offset = targetTop - mCircleView.Top;
        setTargetOffsetTopAndBottom(offset, false /* requires update */);
    }
        private Animation mAnimateToStartPosition;
    //private Animation mAnimateToStartPosition = new Animation() {
    //    @Override
    //    public void applyTransformation(float interpolatedTime, Transformation t) {
    //        moveToStart(interpolatedTime);
    //    }
    //};

        private void startScaleDownReturnToStartAnimation(int from,
            IAnimationListener listener) {
        mFrom = from;
        if (isAlphaUsedForScale()) {
            mStartingScale = mProgress.Alpha;
        } else {
            mStartingScale = ViewCompat.GetScaleX(mCircleView);
        }
        //mScaleDownToStartAnimation = new Animation() {
        //    @Override
        //    public void applyTransformation(float interpolatedTime, Transformation t) {
        //        float targetScale = (mStartingScale + (-mStartingScale  * interpolatedTime));
        //        setAnimationProgress(targetScale);
        //        moveToStart(interpolatedTime);
        //    }
        //};
        mScaleDownToStartAnimation.Duration = (SCALE_DOWN_DURATION);
        if (listener != null) {
            mCircleView.setAnimationListener(listener);
        }
        mCircleView.ClearAnimation();
        mCircleView.StartAnimation(mScaleDownToStartAnimation);
    }

    private void setTargetOffsetTopAndBottom(int offset, bool requiresUpdate) {
        mCircleView.BringToFront();
        mCircleView.OffsetTopAndBottom(offset);
        mCurrentTargetOffsetTop = mCircleView.Top;
        if (requiresUpdate && Build.VERSION.SdkInt < BuildVersionCodes.Honeycomb) {
            Invalidate();
        }
    }

    private void onSecondaryPointerUp(MotionEvent ev) {
        int pointerIndex = ev.ActionIndex;
        int pointerId = ev.GetPointerId(pointerIndex);
        if (pointerId == mActivePointerId) {
            // This was our active pointer going up. Choose a new
            // active pointer and adjust accordingly.
            int newPointerIndex = pointerIndex == 0 ? 1 : 0;
            mActivePointerId = ev.GetPointerId(newPointerIndex);
        }
    }
    }
}