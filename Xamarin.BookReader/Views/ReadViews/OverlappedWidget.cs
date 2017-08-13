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
using Xamarin.BookReader.Managers;
using Android.Graphics.Drawables;
using Xamarin.BookReader.Models;
using Settings = Xamarin.BookReader.Helpers.Settings;

namespace Xamarin.BookReader.Views.ReadViews
{
    [Register("xamarin.bookreader.views.readviews.OverlappedWidget")]
    public class OverlappedWidget : BaseReadView
    {
        private Path mPath0;

        GradientDrawable mBackShadowDrawableLR;
        GradientDrawable mBackShadowDrawableRL;

        public OverlappedWidget(Context context, string bookId,
                                List<BookMixAToc.MixToc.Chapters> chaptersList,
                                IOnReadStateChangeListener listener) : base(context, bookId, chaptersList, listener)
        {


            mTouch.X = 0.01f;
            mTouch.Y = 0.01f;

            mPath0 = new Path();
            var uintValue = 0xaa666666;
            int[] mBackShadowColors = new int[] { (int)uintValue, 0x666666 };
            mBackShadowDrawableRL = new GradientDrawable(GradientDrawable.Orientation.RightLeft, mBackShadowColors);
            mBackShadowDrawableRL.SetGradientType(GradientType.LinearGradient);

            mBackShadowDrawableLR = new GradientDrawable(GradientDrawable.Orientation.LeftRight, mBackShadowColors);
            mBackShadowDrawableLR.SetGradientType(GradientType.LinearGradient);
        }

        protected override void drawCurrentPageArea(Canvas canvas)
        {
            mPath0.Reset();

            canvas.Save();
            if (actiondownX > mScreenWidth >> 1)
            {
                mPath0.MoveTo(mScreenWidth + touch_down, 0);
                mPath0.LineTo(mScreenWidth + touch_down, mScreenHeight);
                mPath0.LineTo(mScreenWidth, mScreenHeight);
                mPath0.LineTo(mScreenWidth, 0);
                mPath0.LineTo(mScreenWidth + touch_down, 0);
                mPath0.Close();
                canvas.ClipPath(mPath0, Region.Op.Xor);
                canvas.DrawBitmap(mCurPageBitmap, touch_down, 0, null);
            }
            else
            {
                mPath0.MoveTo(touch_down, 0);
                mPath0.LineTo(touch_down, mScreenHeight);
                mPath0.LineTo(mScreenWidth, mScreenHeight);
                mPath0.LineTo(mScreenWidth, 0);
                mPath0.LineTo(touch_down, 0);
                mPath0.Close();
                canvas.ClipPath(mPath0);
                canvas.DrawBitmap(mCurPageBitmap, touch_down, 0, null);
            }
            try
            {
                canvas.Restore();
            }
            catch (Exception e)
            {

            }
        }

        protected override void drawCurrentPageShadow(Canvas canvas)
        {
            canvas.Save();
            GradientDrawable shadow;
            if (actiondownX > mScreenWidth >> 1)
            {
                shadow = mBackShadowDrawableLR;
                shadow.SetBounds((int)(mScreenWidth + touch_down - 5), 0, (int)(mScreenWidth + touch_down + 5), mScreenHeight);

            }
            else
            {
                shadow = mBackShadowDrawableRL;
                shadow.SetBounds((int)(touch_down - 5), 0, (int)(touch_down + 5), mScreenHeight);
            }
            shadow.Draw(canvas);
            try
            {
                canvas.Restore();
            }
            catch (Exception e)
            {

            }
        }

        protected override void drawCurrentBackArea(Canvas canvas)
        {
            // none
        }

        protected override void drawNextPageAreaAndShadow(Canvas canvas)
        {
            canvas.Save();
            if (actiondownX > mScreenWidth >> 1)
            {
                canvas.ClipPath(mPath0);
                canvas.DrawBitmap(mNextPageBitmap, 0, 0, null);
            }
            else
            {
                canvas.ClipPath(mPath0, Region.Op.Xor);
                canvas.DrawBitmap(mNextPageBitmap, 0, 0, null);
            }
            try
            {
                canvas.Restore();
            }
            catch (Exception e)
            {

            }
        }

        protected override void calcPoints()
        {

        }

        protected override void calcCornerXY(float x, float y)
        {

        }

        public override void ComputeScroll()
        {
            base.ComputeScroll();
            if (mScroller.ComputeScrollOffset())
            {
                float x = mScroller.CurrX;
                float y = mScroller.CurrY;
                if (actiondownX > mScreenWidth >> 1)
                {
                    touch_down = -(mScreenWidth - x);
                }
                else
                {
                    touch_down = x;
                }
                mTouch.Y = y;
                //touch_down = mTouch.X - actiondownX;
                PostInvalidate();
            }
        }

        protected override void startAnimation()
        {
            int dx;
            if (actiondownX > mScreenWidth / 2)
            {
                dx = (int)-(mScreenWidth + touch_down);
                mScroller.StartScroll((int)(mScreenWidth + touch_down), (int)mTouch.Y, dx, 0, 700);
            }
            else
            {
                dx = (int)(mScreenWidth - touch_down);
                mScroller.StartScroll((int)touch_down, (int)mTouch.Y, dx, 0, 700);
            }
        }

        protected override void abortAnimation()
        {
            if (!mScroller.IsFinished)
            {
                mScroller.AbortAnimation();
            }
        }

        protected override void restoreAnimation()
        {
            int dx;
            if (actiondownX > mScreenWidth / 2)
            {
                dx = (int)(mScreenWidth - mTouch.X);
            }
            else
            {
                dx = (int)(-mTouch.X);
            }
            mScroller.StartScroll((int)mTouch.X, (int)mTouch.Y, dx, 0, 300);
        }

        protected override void setBitmaps(Bitmap bm1, Bitmap bm2)
        {
            mCurPageBitmap = bm1;
            mNextPageBitmap = bm2;
        }

        public /*synchronized*/ override void setTheme(int theme)
        {
            resetTouchPoint();
            Bitmap bg = ThemeManager.getThemeDrawable(theme);
            if (bg != null)
            {
                pagefactory.setBgBitmap(bg);
                if (isPrepared)
                {
                    pagefactory.onDraw(mCurrentPageCanvas);
                    pagefactory.onDraw(mNextPageCanvas);
                    PostInvalidate();
                }
            }
            if (theme < 5)
            {
                Settings.ReadTheme = theme;
            }
        }
    }
}