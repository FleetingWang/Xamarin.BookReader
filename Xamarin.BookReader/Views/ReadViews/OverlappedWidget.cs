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

namespace Xamarin.BookReader.Views.ReadViews
{
    public class OverlappedWidget: BaseReadView
    {
        private Path mPath0;

    GradientDrawable mBackShadowDrawableLR;
    GradientDrawable mBackShadowDrawableRL;

    public OverlappedWidget(Context context, string bookId,
                            List<BookMixAToc.mixToc.Chapters> chaptersList,
                            OnReadStateChangeListener listener) {
        super(context, bookId, chaptersList, listener);

        mTouch.x = 0.01f;
        mTouch.y = 0.01f;

        mPath0 = new Path();

        int[] mBackShadowColors = new int[]{0xaa666666, 0x666666};
        mBackShadowDrawableRL = new GradientDrawable(GradientDrawable.Orientation.RIGHT_LEFT, mBackShadowColors);
        mBackShadowDrawableRL.setGradientType(GradientDrawable.LINEAR_GRADIENT);

        mBackShadowDrawableLR = new GradientDrawable(GradientDrawable.Orientation.LEFT_RIGHT, mBackShadowColors);
        mBackShadowDrawableLR.setGradientType(GradientDrawable.LINEAR_GRADIENT);
    }

    @Override
    protected void drawCurrentPageArea(Canvas canvas) {
        mPath0.reset();

        canvas.save();
        if (actiondownX > mScreenWidth >> 1) {
            mPath0.moveTo(mScreenWidth + touch_down, 0);
            mPath0.lineTo(mScreenWidth + touch_down, mScreenHeight);
            mPath0.lineTo(mScreenWidth, mScreenHeight);
            mPath0.lineTo(mScreenWidth, 0);
            mPath0.lineTo(mScreenWidth + touch_down, 0);
            mPath0.close();
            canvas.clipPath(mPath0, Region.Op.XOR);
            canvas.drawBitmap(mCurPageBitmap, touch_down, 0, null);
        } else {
            mPath0.moveTo(touch_down, 0);
            mPath0.lineTo(touch_down, mScreenHeight);
            mPath0.lineTo(mScreenWidth, mScreenHeight);
            mPath0.lineTo(mScreenWidth, 0);
            mPath0.lineTo(touch_down, 0);
            mPath0.close();
            canvas.clipPath(mPath0);
            canvas.drawBitmap(mCurPageBitmap, touch_down, 0, null);
        }
        try {
            canvas.restore();
        } catch (Exception e) {

        }
    }

    @Override
    protected void drawCurrentPageShadow(Canvas canvas) {
        canvas.save();
        GradientDrawable shadow;
        if (actiondownX > mScreenWidth >> 1) {
            shadow = mBackShadowDrawableLR;
            shadow.setBounds((int) (mScreenWidth + touch_down - 5), 0, (int) (mScreenWidth + touch_down + 5), mScreenHeight);

        } else {
            shadow = mBackShadowDrawableRL;
            shadow.setBounds((int) (touch_down - 5), 0, (int) (touch_down + 5), mScreenHeight);
        }
        shadow.draw(canvas);
        try {
            canvas.restore();
        } catch (Exception e) {

        }
    }

    @Override
    protected void drawCurrentBackArea(Canvas canvas) {
        // none
    }

    @Override
    protected void drawNextPageAreaAndShadow(Canvas canvas) {
        canvas.save();
        if (actiondownX > mScreenWidth >> 1) {
            canvas.clipPath(mPath0);
            canvas.drawBitmap(mNextPageBitmap, 0, 0, null);
        } else {
            canvas.clipPath(mPath0, Region.Op.XOR);
            canvas.drawBitmap(mNextPageBitmap, 0, 0, null);
        }
        try {
            canvas.restore();
        } catch (Exception e) {

        }
    }

    @Override
    protected void calcPoints() {

    }

    @Override
    protected void calcCornerXY(float x, float y) {

    }

    @Override
    public void computeScroll() {
        super.computeScroll();
        if (mScroller.computeScrollOffset()) {
            float x = mScroller.getCurrX();
            float y = mScroller.getCurrY();
            if (actiondownX > mScreenWidth >> 1) {
                touch_down = -(mScreenWidth - x);
            } else {
                touch_down = x;
            }
            mTouch.y = y;
            //touch_down = mTouch.x - actiondownX;
            postInvalidate();
        }
    }

    @Override
    protected void startAnimation() {
        int dx;
        if (actiondownX > mScreenWidth / 2) {
            dx = (int) -(mScreenWidth + touch_down);
            mScroller.startScroll((int) (mScreenWidth + touch_down), (int) mTouch.y, dx, 0, 700);
        } else {
            dx = (int) (mScreenWidth - touch_down);
            mScroller.startScroll((int) touch_down, (int) mTouch.y, dx, 0, 700);
        }
    }

    @Override
    protected void abortAnimation() {
        if (!mScroller.isFinished()) {
            mScroller.abortAnimation();
        }
    }

    @Override
    protected void restoreAnimation() {
        int dx;
        if (actiondownX > mScreenWidth / 2) {
            dx = (int) (mScreenWidth - mTouch.x);
        } else {
            dx = (int) (-mTouch.x);
        }
        mScroller.startScroll((int) mTouch.x, (int) mTouch.y, dx, 0, 300);
    }

    public void setBitmaps(Bitmap bm1, Bitmap bm2) {
        mCurPageBitmap = bm1;
        mNextPageBitmap = bm2;
    }

    @Override
    public synchronized void setTheme(int theme) {
        resetTouchPoint();
        Bitmap bg = ThemeManager.getThemeDrawable(theme);
        if (bg != null) {
            pagefactory.setBgBitmap(bg);
            if (isPrepared) {
                pagefactory.onDraw(mCurrentPageCanvas);
                pagefactory.onDraw(mNextPageCanvas);
                postInvalidate();
            }
        }
        if (theme < 5) {
            SettingManager.getInstance().saveReadTheme(theme);
        }
    }
    }
}