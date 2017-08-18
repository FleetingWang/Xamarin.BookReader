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
using Android.Graphics.Drawables;
using Android.Graphics;

using Android.Content.Res;
using static Android.Graphics.Paint;
using Android.Support.V4.View.Animation;
using Android.Util;
using Android.Views.Animations;
using static Android.Graphics.Drawables.Drawable;

namespace Xamarin.BookReader.Views.RecyclerViews.Swipes
{
    [Register("xamarin.bookreader.views.recyclerviews.swipes.MaterialProgressDrawable")]
    public class MaterialProgressDrawable : Drawable, IAnimatable
    {
        private static IInterpolator LINEAR_INTERPOLATOR = new LinearInterpolator();
        private static IInterpolator MATERIAL_INTERPOLATOR = new FastOutSlowInInterpolator();

        private static float FULL_ROTATION = 1080.0f;
        // Maps to ProgressBar.Large style
        public static int LARGE = 0;
        // Maps to ProgressBar default style
        public static int DEFAULT = 1;

        // Maps to ProgressBar default style
        private static int CIRCLE_DIAMETER = 40;
        private static float CENTER_RADIUS = 8.75f; //should add up to 10 when + stroke_width
        private static float STROKE_WIDTH = 2.5f;

        // Maps to ProgressBar.Large style
        private static int CIRCLE_DIAMETER_LARGE = 56;
        private static float CENTER_RADIUS_LARGE = 12.5f;
        private static float STROKE_WIDTH_LARGE = 3f;

        private int[] COLORS = new int[] {
            Color.Black
        };

        /**
         * The value in the linear interpolator for animating the drawable at which
         * the color transition should start
         */
        private static float COLOR_START_DELAY_OFFSET = 0.75f;
        private static float END_TRIM_START_DELAY_OFFSET = 0.5f;
        private static float START_TRIM_DURATION_OFFSET = 0.5f;

        /** The duration of a single progress spin in milliseconds. */
        private static int ANIMATION_DURATION = 1332;

        /** The number of points in the progress "star". */
        private static float NUM_POINTS = 5f;
        /** The list of animators operating on this drawable. */
        private List<Animation> mAnimators = new List<Animation>();

        /** The indicator ring, used to manage animation state. */
        private Ring mRing;

        /** Canvas rotation in degrees. */
        private float mRotation;

        /** Layout info for the arrowhead in dp */
        private static int ARROW_WIDTH = 10;
        private static int ARROW_HEIGHT = 5;
        private static float ARROW_OFFSET_ANGLE = 5;

        /** Layout info for the arrowhead for the large spinner in dp */
        private static int ARROW_WIDTH_LARGE = 12;
        private static int ARROW_HEIGHT_LARGE = 6;
        private static float MAX_PROGRESS_ARC = .8f;

        private Resources mResources;
        private View mParent;
        private Animation mAnimation;
        private float mRotationCount;
        private double mWidth;
        private double mHeight;
        bool mFinishing;
        private ICallback mCallback;

        public MaterialProgressDrawable(Context context, View parent)
        {
            mParent = parent;
            mResources = context.Resources;
            mCallback = new CustomCallback(this);
            mRing = new Ring(mCallback);
            mRing.setColors(COLORS);

            updateSizes(DEFAULT);
            setupAnimators();
        }

        public MaterialProgressDrawable(IntPtr javaReference, JniHandleOwnership transfer)
            :base(javaReference, transfer)
        {

        }

        private void setSizeParameters(double progressCircleWidth, double progressCircleHeight,
                double centerRadius, double strokeWidth, float arrowWidth, float arrowHeight)
        {
            Ring ring = mRing;
            DisplayMetrics metrics = mResources.DisplayMetrics;
            float screenDensity = metrics.Density;

            mWidth = progressCircleWidth * screenDensity;
            mHeight = progressCircleHeight * screenDensity;
            ring.setStrokeWidth((float)strokeWidth * screenDensity);
            ring.setCenterRadius(centerRadius * screenDensity);
            ring.setColorIndex(0);
            ring.setArrowDimensions(arrowWidth * screenDensity, arrowHeight * screenDensity);
            ring.setInsets((int)mWidth, (int)mHeight);
        }

        /**
         * Set the overall size for the progress spinner. This updates the radius
         * and stroke width of the ring.
         *
         * @param size One of {MaterialProgressDrawable.LARGE} or
         *            {MaterialProgressDrawable.DEFAULT}
         */
        public void updateSizes(int size)
        {
            if (size == LARGE)
            {
                setSizeParameters(CIRCLE_DIAMETER_LARGE, CIRCLE_DIAMETER_LARGE, CENTER_RADIUS_LARGE,
                        STROKE_WIDTH_LARGE, ARROW_WIDTH_LARGE, ARROW_HEIGHT_LARGE);
            }
            else
            {
                setSizeParameters(CIRCLE_DIAMETER, CIRCLE_DIAMETER, CENTER_RADIUS, STROKE_WIDTH,
                        ARROW_WIDTH, ARROW_HEIGHT);
            }
        }

        /**
         * @param show Set to true to display the arrowhead on the progress spinner.
         */
        public void showArrow(bool show)
        {
            mRing.setShowArrow(show);
        }

        /**
         * @param scale Set the scale of the arrowhead for the spinner.
         */
        public void setArrowScale(float scale)
        {
            mRing.setArrowScale(scale);
        }

        /**
         * Set the start and end trim for the progress spinner arc.
         *
         * @param startAngle start angle
         * @param endAngle end angle
         */
        public void setStartEndTrim(float startAngle, float endAngle)
        {
            mRing.setStartTrim(startAngle);
            mRing.setEndTrim(endAngle);
        }

        /**
         * Set the amount of rotation to apply to the progress spinner.
         *
         * @param rotation Rotation is from [0..1]
         */
        public void setProgressRotation(float rotation)
        {
            mRing.setRotation(rotation);
        }

        /**
         * Update the background color of the circle image view.
         */
        public void setBackgroundColor(int color)
        {
            mRing.setBackgroundColor(color);
        }

        /**
         * Set the colors used in the progress animation from color resources.
         * The first color will also be the color of the bar that grows in response
         * to a user swipe gesture.
         *
         * @param colors
         */
        public void setColorSchemeColors(params int[] colors)
        {
            mRing.setColors(colors);
            mRing.setColorIndex(0);
        }

        public override int IntrinsicHeight => (int)mHeight;

        public override int IntrinsicWidth => (int)mWidth;

        public override int Alpha { get => mRing.getAlpha(); set => mRing.setAlpha(value); }

        public override void SetColorFilter(ColorFilter colorFilter)
        {
            mRing.setColorFilter(colorFilter);
        }

        [Java.Lang.SuppressWarnings( Value = new[] { "unused" })]
        void setRotation(float rotation)
        {
            mRotation = rotation;
            InvalidateSelf();
        }

        [Java.Lang.SuppressWarnings( Value = new[] { "unused" })]
        private float getRotation()
        {
            return mRotation;
        }

        private float getMinProgressArc(Ring ring)
        {
            return (float)Java.Lang.Math.ToRadians(
                    ring.getStrokeWidth() / (2 * Math.PI * ring.getCenterRadius()));
        }

        // Adapted from ArgbEvaluator.java
        private int evaluateColorChange(float fraction, int startValue, int endValue)
        {
            int startInt = startValue;
            int startA = (startInt >> 24) & 0xff;
            int startR = (startInt >> 16) & 0xff;
            int startG = (startInt >> 8) & 0xff;
            int startB = startInt & 0xff;

            int endInt = endValue;
            int endA = (endInt >> 24) & 0xff;
            int endR = (endInt >> 16) & 0xff;
            int endG = (endInt >> 8) & 0xff;
            int endB = endInt & 0xff;

            return (startA + (int)(fraction * (endA - startA))) << 24 |
                    (startR + (int)(fraction * (endR - startR))) << 16 |
                    (startG + (int)(fraction * (endG - startG))) << 8 |
                    (startB + (int)(fraction * (endB - startB)));
        }

        /**
         * Update the ring color if this is within the last 25% of the animation.
         * The new ring color will be a translation from the starting ring color to
         * the next color.
         */
        private void updateRingColor(float interpolatedTime, Ring ring)
        {
            if (interpolatedTime > COLOR_START_DELAY_OFFSET)
            {
                // scale the interpolatedTime so that the full
                // transformation from 0 - 1 takes place in the
                // remaining time
                ring.setColor(evaluateColorChange((interpolatedTime - COLOR_START_DELAY_OFFSET)
                        / (1.0f - COLOR_START_DELAY_OFFSET), ring.getStartingColor(),
                        ring.getNextColor()));
            }
        }

        private void applyFinishTranslation(float interpolatedTime, Ring ring)
        {
            // shrink back down and complete a full rotation before
            // starting other circles
            // Rotation goes between [0..1].
            updateRingColor(interpolatedTime, ring);
            float targetRotation = (float)(Math.Floor(ring.getStartingRotation() / MAX_PROGRESS_ARC)
                    + 1f);
            float minProgressArc = getMinProgressArc(ring);
            float startTrim = ring.getStartingStartTrim()
                    + (ring.getStartingEndTrim() - minProgressArc - ring.getStartingStartTrim())
                    * interpolatedTime;
            ring.setStartTrim(startTrim);
            ring.setEndTrim(ring.getStartingEndTrim());
            float rotation = ring.getStartingRotation()
                    + ((targetRotation - ring.getStartingRotation()) * interpolatedTime);
            ring.setRotation(rotation);
        }

        private void setupAnimators()
        {
            Ring ring = mRing;
            Animation animation = new CustomAnimation(this);
            
            animation.RepeatCount = Animation.Infinite;
            animation.RepeatMode = RepeatMode.Restart;
            animation.Interpolator = LINEAR_INTERPOLATOR;
            animation.SetAnimationListener(new CustomAnimationListener(this));
            
            mAnimation = animation;
        }

        public void Start()
        {
            mAnimation.Reset();
            mRing.storeOriginals();
            // Already showing some part of the ring
            if (mRing.getEndTrim() != mRing.getStartTrim())
            {
                mFinishing = true;
                mAnimation.Duration = (ANIMATION_DURATION / 2);
                mParent.StartAnimation(mAnimation);
            }
            else
            {
                mRing.setColorIndex(0);
                mRing.resetOriginals();
                mAnimation.Duration = ANIMATION_DURATION;
                mParent.StartAnimation(mAnimation);
            }
        }

        public void Stop()
        {
            mParent.ClearAnimation();
            setRotation(0);
            mRing.setShowArrow(false);
            mRing.setColorIndex(0);
            mRing.resetOriginals();
        }

        public override void Draw(Canvas c)
        {
            Rect bounds = Bounds;
            int saveCount = c.Save();
            c.Rotate(mRotation, bounds.ExactCenterX(), bounds.ExactCenterY());
            mRing.draw(c, bounds);
            c.RestoreToCount(saveCount);
        }

        public override void SetAlpha(int alpha)
        {
            mRing.setAlpha(alpha);
        }

        private class Ring
        {
            private RectF mTempBounds = new RectF();
            private Paint mPaint = new Paint();
            private Paint mArrowPaint = new Paint();

            private ICallback mCallback;

            private float mStartTrim = 0.0f;
            private float mEndTrim = 0.0f;
            private float mRotation = 0.0f;
            private float mStrokeWidth = 5.0f;
            private float mStrokeInset = 2.5f;

            private int[] mColors;
            // mColorIndex represents the offset into the available mColors that the
            // progress circle should currently display. As the progress circle is
            // animating, the mColorIndex moves by one to the next available color.
            private int mColorIndex;
            private float mStartingStartTrim;
            private float mStartingEndTrim;
            private float mStartingRotation;
            private bool mShowArrow;
            private Path mArrow;
            private float mArrowScale;
            private double mRingCenterRadius;
            private int mArrowWidth;
            private int mArrowHeight;
            private int mAlpha;
            private Paint mCirclePaint = new Paint { Flags = PaintFlags.AntiAlias };
            private int mBackgroundColor;
            private int mCurrentColor;

            public Ring(ICallback callback)
            {
                mCallback = callback;

                mPaint.StrokeCap = Paint.Cap.Square;
                mPaint.AntiAlias = (true);
                mPaint.SetStyle(Style.Stroke);

                mArrowPaint.SetStyle(Style.Fill);
                mArrowPaint.AntiAlias = (true);
            }

            public void setBackgroundColor(int color)
            {
                mBackgroundColor = color;
            }

            /**
             * Set the dimensions of the arrowhead.
             *
             * @param width Width of the hypotenuse of the arrow head
             * @param height Height of the arrow point
             */
            public void setArrowDimensions(float width, float height)
            {
                mArrowWidth = (int)width;
                mArrowHeight = (int)height;
            }

            /**
             * Draw the progress spinner
             */
            public void draw(Canvas c, Rect bounds)
            {
                RectF arcBounds = mTempBounds;
                arcBounds.Set(bounds);
                arcBounds.Inset(mStrokeInset, mStrokeInset);

                float startAngle = (mStartTrim + mRotation) * 360;
                float endAngle = (mEndTrim + mRotation) * 360;
                float sweepAngle = endAngle - startAngle;

                mPaint.Color = new Color(mCurrentColor);
                c.DrawArc(arcBounds, startAngle, sweepAngle, false, mPaint);

                drawTriangle(c, startAngle, sweepAngle, bounds);

                if (mAlpha < 255)
                {
                    mCirclePaint.Color = new Color(mBackgroundColor);
                    mCirclePaint.Alpha = (255 - mAlpha);
                    c.DrawCircle(bounds.ExactCenterX(), bounds.ExactCenterY(), bounds.Width() / 2,
                            mCirclePaint);
                }
            }

            private void drawTriangle(Canvas c, float startAngle, float sweepAngle, Rect bounds)
            {
                if (mShowArrow)
                {
                    if (mArrow == null)
                    {
                        mArrow = new Path();
                        mArrow.SetFillType(Path.FillType.EvenOdd);
                    }
                    else
                    {
                        mArrow.Reset();
                    }

                    // Adjust the position of the triangle so that it is inset as
                    // much as the arc, but also centered on the arc.
                    float inset = (int)mStrokeInset / 2 * mArrowScale;
                    float x = (float)(mRingCenterRadius * Math.Cos(0) + bounds.ExactCenterX());
                    float y = (float)(mRingCenterRadius * Math.Sin(0) + bounds.ExactCenterY());

                    // Update the path each time. This works around an issue in SKIA
                    // where concatenating a rotation matrix to a scale matrix
                    // ignored a starting negative rotation. This appears to have
                    // been fixed as of API 21.
                    mArrow.MoveTo(0, 0);
                    mArrow.LineTo(mArrowWidth * mArrowScale, 0);
                    mArrow.LineTo((mArrowWidth * mArrowScale / 2), (mArrowHeight
                            * mArrowScale));
                    mArrow.Offset(x - inset, y);
                    mArrow.Close();
                    // draw a triangle
                    mArrowPaint.Color = new Color(mCurrentColor);
                    c.Rotate(startAngle + sweepAngle - ARROW_OFFSET_ANGLE, bounds.ExactCenterX(),
                            bounds.ExactCenterY());
                    c.DrawPath(mArrow, mArrowPaint);
                }
            }

            /**
             * Set the colors the progress spinner alternates between.
             *
             * @param colors Array of integers describing the colors. Must be non-<code>null</code>.
             */
            public void setColors(int[] colors)
            {
                mColors = colors;
                // if colors are reset, make sure to reset the color index as well
                setColorIndex(0);
            }

            /**
             * Set the absolute color of the progress spinner. This is should only
             * be used when animating between current and next color when the
             * spinner is rotating.
             *
             * @param color int describing the color.
             */
            public void setColor(int color)
            {
                mCurrentColor = color;
            }

            /**
             * @param index Index into the color array of the color to display in
             *            the progress spinner.
             */
            public void setColorIndex(int index)
            {
                mColorIndex = index;
                mCurrentColor = mColors[mColorIndex];
            }

            /**
             * @return int describing the next color the progress spinner should use when drawing.
             */
            public int getNextColor()
            {
                return mColors[getNextColorIndex()];
            }

            private int getNextColorIndex()
            {
                return (mColorIndex + 1) % (mColors.Length);
            }

            /**
             * Proceed to the next available ring color. This will automatically
             * wrap back to the beginning of colors.
             */
            public void goToNextColor()
            {
                setColorIndex(getNextColorIndex());
            }

            public void setColorFilter(ColorFilter filter)
            {
                mPaint.SetColorFilter(filter);
                invalidateSelf();
            }

            /**
             * @param alpha Set the alpha of the progress spinner and associated arrowhead.
             */
            public void setAlpha(int alpha)
            {
                mAlpha = alpha;
            }

            /**
             * @return Current alpha of the progress spinner and arrowhead.
             */
            public int getAlpha()
            {
                return mAlpha;
            }

            /**
             * @param strokeWidth Set the stroke width of the progress spinner in pixels.
             */
            public void setStrokeWidth(float strokeWidth)
            {
                mStrokeWidth = strokeWidth;
                mPaint.StrokeWidth = strokeWidth;
                invalidateSelf();
            }

            [Java.Lang.SuppressWarnings( Value = new[] { "unused" })]
            public float getStrokeWidth()
            {
                return mStrokeWidth;
            }

            [Java.Lang.SuppressWarnings( Value = new[] { "unused" })]
            public void setStartTrim(float startTrim)
            {
                mStartTrim = startTrim;
                invalidateSelf();
            }

            [Java.Lang.SuppressWarnings( Value = new[] { "unused" })]
            public float getStartTrim()
            {
                return mStartTrim;
            }

            public float getStartingStartTrim()
            {
                return mStartingStartTrim;
            }

            public float getStartingEndTrim()
            {
                return mStartingEndTrim;
            }

            public int getStartingColor()
            {
                return mColors[mColorIndex];
            }

            [Java.Lang.SuppressWarnings( Value = new[] { "unused" })]
            public void setEndTrim(float endTrim)
            {
                mEndTrim = endTrim;
                invalidateSelf();
            }

            [Java.Lang.SuppressWarnings( Value = new[] { "unused" })]
            public float getEndTrim()
            {
                return mEndTrim;
            }

            [Java.Lang.SuppressWarnings( Value = new[] { "unused" })]
            public void setRotation(float rotation)
            {
                mRotation = rotation;
                invalidateSelf();
            }

            [Java.Lang.SuppressWarnings( Value = new[] { "unused" })]
            public float getRotation()
            {
                return mRotation;
            }

            public void setInsets(int width, int height)
            {
                float minEdge = (float)Math.Min(width, height);
                float insets;
                if (mRingCenterRadius <= 0 || minEdge < 0)
                {
                    insets = (float)Math.Ceiling(mStrokeWidth / 2.0f);
                }
                else
                {
                    insets = (float)(minEdge / 2.0f - mRingCenterRadius);
                }
                mStrokeInset = insets;
            }

            //@SuppressWarnings("unused")
            //public float getInsets() {
            //    return mStrokeInset;
            //}

            /**
             * @param centerRadius Inner radius in px of the circle the progress
             *            spinner arc traces.
             */
            public void setCenterRadius(double centerRadius)
            {
                mRingCenterRadius = centerRadius;
            }

            public double getCenterRadius()
            {
                return mRingCenterRadius;
            }

            /**
             * @param show Set to true to show the arrow head on the progress spinner.
             */
            public void setShowArrow(bool show)
            {
                if (mShowArrow != show)
                {
                    mShowArrow = show;
                    invalidateSelf();
                }
            }

            /**
             * @param scale Set the scale of the arrowhead for the spinner.
             */
            public void setArrowScale(float scale)
            {
                if (scale != mArrowScale)
                {
                    mArrowScale = scale;
                    invalidateSelf();
                }
            }

            /**
             * @return The amount the progress spinner is currently rotated, between [0..1].
             */
            public float getStartingRotation()
            {
                return mStartingRotation;
            }

            /**
             * If the start / end trim are offset to begin with, store them so that
             * animation starts from that offset.
             */
            public void storeOriginals()
            {
                mStartingStartTrim = mStartTrim;
                mStartingEndTrim = mEndTrim;
                mStartingRotation = mRotation;
            }

            /**
             * Reset the progress spinner to default rotation, start and end angles.
             */
            public void resetOriginals()
            {
                mStartingStartTrim = 0;
                mStartingEndTrim = 0;
                mStartingRotation = 0;
                setStartTrim(0);
                setEndTrim(0);
                setRotation(0);
            }

            private void invalidateSelf()
            {
                mCallback.InvalidateDrawable(null);
            }
        }

        class CustomCallback: Java.Lang.Object, ICallback
        {
            private MaterialProgressDrawable materialProgressDrawable;

            public CustomCallback(MaterialProgressDrawable materialProgressDrawable)
            {
                this.materialProgressDrawable = materialProgressDrawable;
            }

            public void InvalidateDrawable(Drawable who)
            {
                materialProgressDrawable.InvalidateSelf();
            }
            public void ScheduleDrawable(Drawable who, Java.Lang.IRunnable what, long when)
            {
                materialProgressDrawable.ScheduleSelf(what, when);
            }
            public void UnscheduleDrawable(Drawable who, Java.Lang.IRunnable what)
            {
                materialProgressDrawable.UnscheduleSelf(what);
            }
        }

        class CustomAnimation: Animation
        {
            private MaterialProgressDrawable materialProgressDrawable;

            public CustomAnimation(MaterialProgressDrawable materialProgressDrawable)
            {
                this.materialProgressDrawable = materialProgressDrawable;
            }

            protected override void ApplyTransformation(float interpolatedTime, Transformation t)
            {
                var ring = materialProgressDrawable.mRing;
                if (materialProgressDrawable.mFinishing)
                {
                    materialProgressDrawable.applyFinishTranslation(interpolatedTime, ring);
                }
                else
                {
                    // The minProgressArc is calculated from 0 to create an
                    // angle that matches the stroke width.
                    float minProgressArc = materialProgressDrawable.getMinProgressArc(ring);
                    float startingEndTrim = ring.getStartingEndTrim();
                    float startingTrim = ring.getStartingStartTrim();
                    float startingRotation = ring.getStartingRotation();

                    materialProgressDrawable.updateRingColor(interpolatedTime, ring);

                    // Moving the start trim only occurs in the first 50% of a
                    // single ring animation
                    if (interpolatedTime <= START_TRIM_DURATION_OFFSET)
                    {
                        // scale the interpolatedTime so that the full
                        // transformation from 0 - 1 takes place in the
                        // remaining time
                        float scaledTime = (interpolatedTime)
                                / (1.0f - START_TRIM_DURATION_OFFSET);
                        float startTrim = startingTrim
                                + ((MAX_PROGRESS_ARC - minProgressArc) * MATERIAL_INTERPOLATOR
                                        .GetInterpolation(scaledTime));
                        ring.setStartTrim(startTrim);
                    }

                    // Moving the end trim starts after 50% of a single ring
                    // animation completes
                    if (interpolatedTime > END_TRIM_START_DELAY_OFFSET)
                    {
                        // scale the interpolatedTime so that the full
                        // transformation from 0 - 1 takes place in the
                        // remaining time
                        float minArc = MAX_PROGRESS_ARC - minProgressArc;
                        float scaledTime = (interpolatedTime - START_TRIM_DURATION_OFFSET)
                                / (1.0f - START_TRIM_DURATION_OFFSET);
                        float endTrim = startingEndTrim
                                + (minArc * MATERIAL_INTERPOLATOR.GetInterpolation(scaledTime));
                        ring.setEndTrim(endTrim);
                    }

                    float rotation = startingRotation + (0.25f * interpolatedTime);
                    ring.setRotation(rotation);

                    float groupRotation = ((FULL_ROTATION / NUM_POINTS) * interpolatedTime)
                            + (FULL_ROTATION * (materialProgressDrawable.mRotationCount / NUM_POINTS));
                    materialProgressDrawable.setRotation(groupRotation);
                }
            }
        }

        class CustomAnimationListener : Java.Lang.Object, Animation.IAnimationListener
        {
            private MaterialProgressDrawable materialProgressDrawable;

            public CustomAnimationListener(MaterialProgressDrawable materialProgressDrawable)
            {
                this.materialProgressDrawable = materialProgressDrawable;
            }

            public void OnAnimationStart(Animation animation)
            {
                materialProgressDrawable.mRotationCount = 0;
            }

            public void OnAnimationEnd(Animation animation)
            {
                // do nothing
            }

            public void OnAnimationRepeat(Animation animation)
            {
                Ring ring = materialProgressDrawable.mRing;
                ring.storeOriginals();
                ring.goToNextColor();
                ring.setStartTrim(ring.getEndTrim());
                if (materialProgressDrawable.mFinishing)
                {
                    // finished closing the last ring from the swipe gesture; go
                    // into progress mode
                    materialProgressDrawable.mFinishing = false;
                    animation.Duration = (ANIMATION_DURATION);
                    ring.setShowArrow(false);
                }
                else
                {
                    materialProgressDrawable.mRotationCount = (materialProgressDrawable.mRotationCount + 1) % (NUM_POINTS);
                }
            }

        }

        public override int Opacity => (int)Format.Transparent;

        public bool IsRunning
        {
            get
            {
                List<Animation> animators = mAnimators;
                int N = animators.Count();
                for (int i = 0; i < N; i++)
                {
                    Animation animator = animators[i];
                    if (animator.HasStarted && !animator.HasEnded)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}