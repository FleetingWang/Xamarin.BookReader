using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using Android.Graphics;
using Android.Animation;
using Java.Lang;
using Android.Content.Res;
using Android.Support.V4.View.Animation;
using Android.Views.Animations;
using static Android.Animation.Animator;

namespace Xamarin.BookReader.Views.Loading
{
    public class LoadingView : View
    {
        //the size in wrap_content model
        private const int CIRCLE_DIAMETER = 56;

        private const float CENTER_RADIUS = 15f;
        private const float STROKE_WIDTH = 3.5f;

        private const float MAX_PROGRESS_ARC = 300f;
        private const float MIN_PROGRESS_ARC = 20f;

        private const long ANIMATOR_DURATION = 1332;

        private Rect bounds;
        private Ring mRing;

        private Animator animator = null;
        private AnimatorSet animatorSet = null;
        private bool mIsAnimatorCancel = false;

        private IInterpolator interpolator = null;
        //the ring's RectF
        private RectF mTempBounds = new RectF();
        //绘制半圆的paint
        private Paint mPaint;
        private const int DEFAULT_COLOR = 0x3B99DF;
        private bool mAnimationStarted = false;
        //the ring style
        const int RING_STYLE_SQUARE = 0;
        const int RING_STYLE_ROUND = 1;

        //the animator style
        const int PROGRESS_STYLE_MATERIAL = 0;
        const int PROGRESS_STYLE_LINEAR = 1;

        private float mRotation = 0f;

        public LoadingView(Context context)
            : this(context, null)
        {

        }
        public LoadingView(Context context, IAttributeSet attrs)
            : this(context, null, 0)
        {

        }

        public LoadingView(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {
            mRing = new Ring();
            bounds = new Rect();
            mPaint = new Paint();
            mPaint = new Paint(PaintFlags.AntiAlias);
            mPaint.SetStyle(Paint.Style.Stroke);
            mPaint.StrokeWidth = mRing.strokeWidth;

            animatorListener = new CustomAnimatorListener(this);

            if (attrs != null)
            {
                TypedArray a = context.ObtainStyledAttributes(attrs, Resource.Styleable.LoadingView, 0, 0);
                setColor(a.GetInt(Resource.Styleable.LoadingView_loadding_color, DEFAULT_COLOR));
                setRingStyle(a.GetInt(Resource.Styleable.LoadingView_ring_style, RING_STYLE_SQUARE));
                setProgressStyle(a.GetInt(Resource.Styleable.LoadingView_progress_style, PROGRESS_STYLE_MATERIAL));
                setStrokeWidth(a.GetDimension(Resource.Styleable.LoadingView_ring_width, dp2px(STROKE_WIDTH)));
                setCenterRadius(a.GetDimension(Resource.Styleable.LoadingView_ring_radius, dp2px(CENTER_RADIUS)));
                a.Recycle();
            }
        }


        /**
         * set the ring strokeWidth
         *
         * @param stroke
         */
        public void setStrokeWidth(float stroke)
        {
            mRing.strokeWidth = stroke;
            mPaint.StrokeWidth = stroke;
        }

        public void setCenterRadius(float radius)
        {
            mRing.ringCenterRadius = radius;
        }

        public void setRingStyle(int style)
        {
            switch (style)
            {
                case RING_STYLE_SQUARE:
                    mPaint.StrokeCap = Paint.Cap.Square;
                    break;
                case RING_STYLE_ROUND:
                    mPaint.StrokeCap = Paint.Cap.Round;
                    break;
            }
        }

        /**
         * set the animator's interpolator
         *
         * @param style
         */
        public void setProgressStyle(int style)
        {
            switch (style)
            {
                case PROGRESS_STYLE_MATERIAL:
                    interpolator = new FastOutSlowInInterpolator();
                    break;
                case PROGRESS_STYLE_LINEAR:
                    interpolator = new LinearInterpolator();
                    break;
            }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            var widthSpecMode = MeasureSpec.GetMode(widthMeasureSpec);
            int widthSpecSize = MeasureSpec.GetSize(widthMeasureSpec);
            var heightSpecMode = MeasureSpec.GetMode(heightMeasureSpec);
            int heightSpecSize = MeasureSpec.GetSize(heightMeasureSpec);
            int width = (int)dp2px(CIRCLE_DIAMETER);
            int height = (int)dp2px(CIRCLE_DIAMETER);
            if (widthSpecMode == MeasureSpecMode.AtMost && heightSpecMode == MeasureSpecMode.AtMost)
            {
                SetMeasuredDimension(width, height);
            }
            else if (widthSpecMode == MeasureSpecMode.AtMost)
            {
                SetMeasuredDimension(width, heightSpecSize);
            }
            else if (heightSpecMode == MeasureSpecMode.AtMost)
            {
                SetMeasuredDimension(widthSpecSize, height);
            }
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            Ring ring = mRing;
            ring.setInsets(w, h);
            bounds.Set(0, 0, w, h);
        }

        private void buildAnimator()
        {
            var valueAnimator = ValueAnimator.OfFloat(0f, 1f);
            valueAnimator.SetDuration(ANIMATOR_DURATION);
            valueAnimator.RepeatCount = -1;
            valueAnimator.SetInterpolator(new LinearInterpolator());
            valueAnimator.Update += (sender, e) => {
                mRotation = (float)valueAnimator.AnimatedValue;
                Invalidate();
            };
            animator = valueAnimator;
            animatorSet = buildFlexibleAnimation();
            animatorSet.AddListener(animatorListener);
        }

        protected override void OnDraw(Canvas canvas)
        {
            if (!mIsAnimatorCancel)
            {
                Rect bounds = getBounds();
                int saveCount = canvas.Save();
                canvas.Rotate(mRotation * 360, bounds.ExactCenterX(), bounds.ExactCenterY());
                drawRing(canvas, bounds);
                canvas.RestoreToCount(saveCount);
            }
            else
            {
                canvas.Restore();
            }
        }

        /**
         * draw the ring
         *
         * @param canvas to draw the Ring
         * @param bounds the ring's rect
         */
        private void drawRing(Canvas canvas, Rect bounds)
        {
            RectF arcBounds = mTempBounds;
            Ring ring = mRing;
            arcBounds.Set(bounds);
            arcBounds.Inset(ring.strokeInset, ring.strokeInset);
            canvas.DrawArc(arcBounds, ring.start, ring.sweep, false, mPaint);
        }

        public void start()
        {
            if (mAnimationStarted)
            {
                return;
            }

            if (animator == null || animatorSet == null)
            {
                mRing.reset();
                buildAnimator();
            }

            animator.Start();
            animatorSet.Start();
            mAnimationStarted = true;
            mIsAnimatorCancel = false;
        }


        public void stop()
        {
            mIsAnimatorCancel = true;
            if (animator != null)
            {
                animator.End();
                animator.Cancel();
            }
            if (animatorSet != null)
            {

                animatorSet.End();
                animatorSet.Cancel();
            }
            animator = null;
            animatorSet = null;

            mAnimationStarted = false;
            mRing.reset();
            mRotation = 0;
            Invalidate();
        }

        public Rect getBounds()
        {
            return bounds;
        }

        public void setBounds(Rect bounds)
        {
            this.bounds = bounds;
        }

        /**
         * build FlexibleAnimation to control the progress
         *
         * @return Animatorset for control the progress
         */
        private AnimatorSet buildFlexibleAnimation()
        {
            Ring ring = mRing;
            AnimatorSet set = new AnimatorSet();
            ValueAnimator increment = ValueAnimator.OfFloat(0, MAX_PROGRESS_ARC - MIN_PROGRESS_ARC);
            increment.SetDuration(ANIMATOR_DURATION / 2);
            increment.SetInterpolator(new LinearInterpolator());
            increment.Update += (sender, e) => {
                float sweeping = ring.sweeping;
                float value = (float)e.Animation.AnimatedValue;
                ring.sweep = sweeping + value;
                Invalidate();
            };
            increment.AddListener(animatorListener);
            ValueAnimator reduce = ValueAnimator.OfFloat(0, MAX_PROGRESS_ARC - MIN_PROGRESS_ARC);
            reduce.SetDuration(ANIMATOR_DURATION / 2);
            reduce.SetInterpolator(interpolator);
            reduce.Update += (sender, e) => {
                float sweeping = ring.sweeping;
                float starting = ring.starting;
                float value = (float)e.Animation.AnimatedValue;
                ring.sweep = sweeping - value;
                ring.start = starting + value;
            };
            set.Play(reduce).After(increment);
            return set;
        }

        public void setColor(int color)
        {
            mRing.color = color;
            mPaint.Color = new Color(color);
        }

        public int getColor()
        {
            return mRing.color;
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            start();
        }

        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();
            stop();
        }

        protected override void OnVisibilityChanged(View changedView, ViewStates visibility)
        {
            base.OnVisibilityChanged(changedView, visibility);
            if (visibility == ViewStates.Visible)
            {
                start();
            }
            else
            {
                stop();
            }
        }

        /**
         * turn dp to px
         *
         * @param dp value
         * @return result px value
         */
        private float dp2px(float dp)
        {
            return TypedValue.ApplyDimension(ComplexUnitType.Dip, dp, Resources.DisplayMetrics);
        }


        protected override IParcelable OnSaveInstanceState()
        {
            IParcelable parcelable = base.OnSaveInstanceState();
            SavedState state = new SavedState(parcelable);
            state.ring = mRing;
            return state;
        }

        protected override void OnRestoreInstanceState(IParcelable state)
        {
            SavedState savedState = (SavedState)state;
            base.OnRestoreInstanceState(state);
            mRing = savedState.ring;
        }

        protected class Ring : Java.Lang.Object, IParcelable
        {
            public float strokeInset = 0f;
            public float strokeWidth = 0f;
            public float ringCenterRadius = 0f;
            public float start = 0f;
            public float end = 0f;
            public float sweep = 0f;
            public float sweeping = MIN_PROGRESS_ARC;

            public float starting = 0f;
            public float ending = 0f;
            public int color;


            public void restore()
            {
                starting = start;
                sweeping = sweep;
                ending = end;
            }

            public void reset()
            {
                end = 0f;
                start = 0f;
                sweeping = MIN_PROGRESS_ARC;
                sweep = 0f;
                starting = 0f;
            }

            public void setInsets(int width, int height)
            {
                float minEdge = Math.Min(width, height);
                float insets;
                if (ringCenterRadius <= 0 || minEdge < 0)
                {
                    insets = (float)Math.Ceil(strokeWidth / 2.0f);
                }
                else
                {
                    insets = (minEdge / 2.0f - ringCenterRadius);
                }

                strokeInset = insets;
            }


            public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
            {
                dest.WriteFloat(this.strokeInset);
                dest.WriteFloat(this.strokeWidth);
                dest.WriteFloat(this.ringCenterRadius);
                dest.WriteFloat(this.start);
                dest.WriteFloat(this.end);
                dest.WriteFloat(this.sweep);
                dest.WriteFloat(this.sweeping);
                dest.WriteFloat(this.starting);
                dest.WriteFloat(this.ending);
                dest.WriteInt(this.color);
            }

            public int DescribeContents()
            {
                return 0;
            }

            public Ring()
            {
            }

            protected Ring(Parcel parcel)
            {
                this.strokeInset = parcel.ReadFloat();
                this.strokeWidth = parcel.ReadFloat();
                this.ringCenterRadius = parcel.ReadFloat();
                this.start = parcel.ReadFloat();
                this.end = parcel.ReadFloat();
                this.sweep = parcel.ReadFloat();
                this.sweeping = parcel.ReadFloat();
                this.starting = parcel.ReadFloat();
                this.ending = parcel.ReadFloat();
                this.color = parcel.ReadInt();
            }

            public static IParcelableCreator CREATOR = new CustomParcelableCreator();

            private class CustomParcelableCreator : Java.Lang.Object, IParcelableCreator
            {
                public Java.Lang.Object CreateFromParcel(Parcel source)
                {
                    return new Ring(source);
                }

                public Java.Lang.Object[] NewArray(int size)
                {
                    return new Ring[size];
                }
            }
        }

        protected class SavedState : BaseSavedState
        {
            public Ring ring;


            public SavedState(IParcelable superState)
                : base(superState)
            {
            }

            private SavedState(Parcel parcel) :
                    base(parcel)
            {
                ring = (Ring)parcel.ReadParcelable(Class.FromType(typeof(Ring)).ClassLoader);
            }

            public override void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
            {
                base.WriteToParcel(dest, flags);
                dest.WriteParcelable(this.ring, flags);
            }


            public static IParcelableCreator CREATOR = new CustomParcelableCreator();

            private class CustomParcelableCreator : Java.Lang.Object, IParcelableCreator
            {
                public Java.Lang.Object CreateFromParcel(Parcel source)
                {
                    return new SavedState(source);
                }

                public Java.Lang.Object[] NewArray(int size)
                {
                    return new SavedState[size];
                }
            }
        }

        IAnimatorListener animatorListener;

        private class CustomAnimatorListener : Java.Lang.Object, IAnimatorListener
        {
            private LoadingView _loadingView;

            public CustomAnimatorListener(LoadingView loadingView)
            {
                _loadingView = loadingView;
            }

            public void OnAnimationCancel(Animator animation)
            {
            }

            public void OnAnimationEnd(Animator animation)
            {
                if (_loadingView.mIsAnimatorCancel) return;
                if (animation is ValueAnimator) {
                    _loadingView.mRing.sweeping = _loadingView.mRing.sweep;
                } else if (animation is AnimatorSet) {
                    _loadingView.mRing.restore();
                    _loadingView.animatorSet.Start();
                }
            }

            public void OnAnimationRepeat(Animator animation)
            {
            }

            public void OnAnimationStart(Animator animation)
            {
            }
        }

    }
}