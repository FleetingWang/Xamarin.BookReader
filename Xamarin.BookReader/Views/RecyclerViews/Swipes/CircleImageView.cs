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
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Support.V4.View;
using Android.Views.Animations;
using Android.Support.V4.Content;

namespace Xamarin.BookReader.Views.RecyclerViews.Swipes
{
    public class CircleImageView : ImageView
    {
        private static int KEY_SHADOW_COLOR = 0x1E000000;
        private static int FILL_SHADOW_COLOR = 0x3D000000;
        // PX
        private static float X_OFFSET = 0f;
        private static float Y_OFFSET = 1.75f;
        private static float SHADOW_RADIUS = 3.5f;
        private static int SHADOW_ELEVATION = 4;

        private Animation.IAnimationListener mListener;
        private int mShadowRadius;

        public CircleImageView(Context context, int color, float radius) : base(context)
        {

            float density = Context.Resources.DisplayMetrics.Density;
            int diameter = (int)(radius * density * 2);
            int shadowYOffset = (int)(density * Y_OFFSET);
            int shadowXOffset = (int)(density * X_OFFSET);

            mShadowRadius = (int)(density * SHADOW_RADIUS);

            ShapeDrawable circle;
            if (elevationSupported())
            {
                circle = new ShapeDrawable(new OvalShape());
                ViewCompat.SetElevation(this, SHADOW_ELEVATION * density);
            }
            else
            {
                OvalShape oval = new OvalShadow(this, mShadowRadius, diameter);
                circle = new ShapeDrawable(oval);
                ViewCompat.SetLayerType(this, ViewCompat.LayerTypeSoftware, circle.Paint);
                circle.Paint.SetShadowLayer(mShadowRadius, shadowXOffset, shadowYOffset,
                        new Color(KEY_SHADOW_COLOR));
                int padding = mShadowRadius;
                // set padding so the inner image sits correctly within the shadow.
                SetPadding(padding, padding, padding, padding);
            }
            circle.Paint.Color = new Color(color);
            SetBackgroundDrawable(circle);
        }

        private bool elevationSupported()
        {
            return Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop;// 21;
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            if (!elevationSupported())
            {
                SetMeasuredDimension(MeasuredWidth + mShadowRadius * 2, MeasuredHeight
                        + mShadowRadius * 2);
            }
        }

        public void setAnimationListener(Animation.IAnimationListener listener)
        {
            mListener = listener;
        }

        protected override void OnAnimationStart()
        {
            base.OnAnimationStart();
            if (mListener != null)
            {
                mListener.OnAnimationStart(Animation);
            }
        }

        protected override void OnAnimationEnd()
        {
            base.OnAnimationEnd();
            if (mListener != null)
            {
                mListener.OnAnimationEnd(Animation);
            }
        }

        /**
         * Update the background color of the circle image view.
         *
         * @param colorRes Id of a color resource.
         */
        public void setBackgroundColorRes(int colorRes)
        {
            SetBackgroundColor(ContextCompat.GetColor(Context, colorRes));
        }

        // TODO: override
        public void SetBackgroundColor(int color)
        {
            if (Background is ShapeDrawable)
            {
                ((ShapeDrawable)Background).Paint.Color = new Color(color);
            }
        }

        private class OvalShadow : OvalShape
        {

            private CircleImageView _circleImageView;

            private RadialGradient mRadialGradient;
            private Paint mShadowPaint;
            private int mCircleDiameter;

            public OvalShadow(CircleImageView circleImageView, int shadowRadius, int circleDiameter) : base()
            {
                _circleImageView = circleImageView;
                mShadowPaint = new Paint();
                _circleImageView.mShadowRadius = shadowRadius;
                mCircleDiameter = circleDiameter;
                mRadialGradient = new RadialGradient(mCircleDiameter / 2, mCircleDiameter / 2,
                         _circleImageView.mShadowRadius, new int[] {
                            FILL_SHADOW_COLOR, Color.Transparent.ToArgb()
                        }, null, Shader.TileMode.Clamp);
                mShadowPaint.SetShader(mRadialGradient);
            }

            public override void Draw(Canvas canvas, Paint paint)
            {
                int viewWidth = _circleImageView.Width;
                int viewHeight = _circleImageView.Height;
                canvas.DrawCircle(viewWidth / 2, viewHeight / 2, (mCircleDiameter / 2 + _circleImageView.mShadowRadius),
                        mShadowPaint);
                canvas.DrawCircle(viewWidth / 2, viewHeight / 2, (mCircleDiameter / 2), paint);
            }
        }
    }
}