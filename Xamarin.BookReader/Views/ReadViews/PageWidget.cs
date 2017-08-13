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
    [Register("xamarin.bookreader.views.readviews.PageWidget")]
    public class PageWidget : BaseReadView
    {
        private int mCornerX = 1; // 拖拽点对应的页脚
        private int mCornerY = 1;
        private Path mPath0;
        private Path mPath1;

        PointF mBezierStart1 = new PointF(); // 贝塞尔曲线起始点
        PointF mBezierControl1 = new PointF(); // 贝塞尔曲线控制点
        PointF mBeziervertex1 = new PointF(); // 贝塞尔曲线顶点
        PointF mBezierEnd1 = new PointF(); // 贝塞尔曲线结束点

        PointF mBezierStart2 = new PointF(); // 另一条贝塞尔曲线
        PointF mBezierControl2 = new PointF();
        PointF mBeziervertex2 = new PointF();
        PointF mBezierEnd2 = new PointF();

        float mMiddleX;
        float mMiddleY;
        float mDegrees;
        float mTouchToCornerDis;
        ColorMatrixColorFilter mColorMatrixFilter;
        Matrix mMatrix;
        float[] mMatrixArray = { 0, 0, 0, 0, 0, 0, 0, 0, 1.0f };

        bool mIsRTandLB; // 是否属于右上左下
        private float mMaxLength;
        int[] mBackShadowColors;// 背面颜色组
        int[] mFrontShadowColors;// 前面颜色组

        GradientDrawable mBackShadowDrawableLR; // 有阴影的GradientDrawable
        GradientDrawable mBackShadowDrawableRL;
        GradientDrawable mFolderShadowDrawableLR;
        GradientDrawable mFolderShadowDrawableRL;

        GradientDrawable mFrontShadowDrawableHBT;
        GradientDrawable mFrontShadowDrawableHTB;
        GradientDrawable mFrontShadowDrawableVLR;
        GradientDrawable mFrontShadowDrawableVRL;

        Paint mPaint;

        public PageWidget(Context context, string bookId,
                          List<BookMixAToc.MixToc.Chapters> chaptersList,
                          IOnReadStateChangeListener listener) : base(context, bookId, chaptersList, listener)
        {

            mPath0 = new Path();
            mPath1 = new Path();
            mMaxLength = (float)Java.Lang.Math.Hypot(mScreenWidth, mScreenHeight);
            mPaint = new Paint();
            mPaint.SetStyle(Paint.Style.Fill);

            createDrawable();

            ColorMatrix cm = new ColorMatrix();//设置颜色数组
            float[] array = { 0.55f, 0, 0, 0, 80.0f, 0, 0.55f, 0, 0, 80.0f, 0, 0, 0.55f, 0, 80.0f, 0, 0, 0, 0.2f, 0 };
            cm.Set(array);
            mColorMatrixFilter = new ColorMatrixColorFilter(cm);
            mMatrix = new Matrix();

            mTouch.X = 0.01f; // 不让x,y为0,否则在点计算时会有问题
            mTouch.Y = 0.01f;
        }

        /**
         * 计算拖拽点对应的拖拽脚
         *
         * @param x 触摸点x坐标
         * @param y 触摸点y坐标
         */
        protected override void calcCornerXY(float x, float y)
        {
            if (x <= mScreenWidth / 2)
                mCornerX = 0;
            else
                mCornerX = mScreenWidth;
            if (y <= mScreenHeight / 2)
                mCornerY = 0;
            else
                mCornerY = mScreenHeight;
            mIsRTandLB = (mCornerX == 0 && mCornerY == mScreenHeight)
                    || (mCornerX == mScreenWidth && mCornerY == 0);
        }

        /**
         * 求解直线P1P2和直线P3P4的交点坐标
         */
        public PointF getCross(PointF P1, PointF P2, PointF P3, PointF P4)
        {
            PointF CrossP = new PointF();
            float a1 = (P2.Y - P1.Y) / (P2.X - P1.X);
            float b1 = ((P1.X * P2.Y) - (P2.X * P1.Y)) / (P1.X - P2.X);
            float a2 = (P4.Y - P3.Y) / (P4.X - P3.X);
            float b2 = ((P3.X * P4.Y) - (P4.X * P3.Y)) / (P3.X - P4.X);
            CrossP.X = (b2 - b1) / (a1 - a2);
            CrossP.Y = a1 * CrossP.X + b1;
            return CrossP;
        }

        protected override void calcPoints()
        {
            mMiddleX = (mTouch.X + mCornerX) / 2;
            mMiddleY = (mTouch.Y + mCornerY) / 2;
            mBezierControl1.X = mMiddleX - (mCornerY - mMiddleY) * (mCornerY - mMiddleY) / (mCornerX - mMiddleX);
            mBezierControl1.Y = mCornerY;
            mBezierControl2.X = mCornerX;
            //mBezierControl2.Y = mMiddleY - (mCornerX - mMiddleX) * (mCornerX - mMiddleX) / (mCornerY - mMiddleY);

            float f4 = mCornerY - mMiddleY;
            if (f4 == 0)
            {
                mBezierControl2.Y = mMiddleY - (mCornerX - mMiddleX) * (mCornerX - mMiddleX) / 0.1f;
            }
            else
            {
                mBezierControl2.Y = mMiddleY - (mCornerX - mMiddleX) * (mCornerX - mMiddleX) / (mCornerY - mMiddleY);
            }

            mBezierStart1.X = mBezierControl1.X - (mCornerX - mBezierControl1.X) / 2;
            mBezierStart1.Y = mCornerY;

            // 当mBezierStart1.X < 0或者mBezierStart1.X > 480时
            // 如果继续翻页，会出现BUG故在此限制
            if (mTouch.X > 0 && mTouch.X < mScreenWidth)
            {
                if (mBezierStart1.X < 0 || mBezierStart1.X > mScreenWidth)
                {
                    if (mBezierStart1.X < 0)
                        mBezierStart1.X = mScreenWidth - mBezierStart1.X;

                    float f1 = System.Math.Abs(mCornerX - mTouch.X);
                    float f2 = mScreenWidth * f1 / mBezierStart1.X;
                    mTouch.X = System.Math.Abs(mCornerX - f2);

                    float f3 = System.Math.Abs(mCornerX - mTouch.X)
                            * System.Math.Abs(mCornerY - mTouch.Y) / f1;
                    mTouch.Y = System.Math.Abs(mCornerY - f3);

                    mMiddleX = (mTouch.X + mCornerX) / 2;
                    mMiddleY = (mTouch.Y + mCornerY) / 2;

                    mBezierControl1.X = mMiddleX - (mCornerY - mMiddleY) * (mCornerY - mMiddleY) / (mCornerX - mMiddleX);
                    mBezierControl1.Y = mCornerY;

                    mBezierControl2.X = mCornerX;
                    //mBezierControl2.Y = mMiddleY - (mCornerX - mMiddleX) * (mCornerX - mMiddleX) / (mCornerY - mMiddleY);

                    float f5 = mCornerY - mMiddleY;
                    if (f5 == 0)
                    {
                        mBezierControl2.Y = mMiddleY - (mCornerX - mMiddleX) * (mCornerX - mMiddleX) / 0.1f;
                    }
                    else
                    {
                        mBezierControl2.Y = mMiddleY - (mCornerX - mMiddleX) * (mCornerX - mMiddleX) / (mCornerY - mMiddleY);
                    }
                    mBezierStart1.X = mBezierControl1.X - (mCornerX - mBezierControl1.X) / 2;
                }
            }
            mBezierStart2.X = mCornerX;
            mBezierStart2.Y = mBezierControl2.Y - (mCornerY - mBezierControl2.Y) / 2;

            mTouchToCornerDis = (float)Java.Lang.Math.Hypot((mTouch.X - mCornerX), (mTouch.Y - mCornerY));

            mBezierEnd1 = getCross(mTouch, mBezierControl1, mBezierStart1, mBezierStart2);
            mBezierEnd2 = getCross(mTouch, mBezierControl2, mBezierStart1, mBezierStart2);

            /*
             * mBeziervertex1.X 推导
             * ((mBezierStart1.x+mBezierEnd1.X)/2+mBezierControl1.X)/2 化简等价于
             * (mBezierStart1.x+ 2*mBezierControl1.x+mBezierEnd1.X) / 4
             */
            mBeziervertex1.X = (mBezierStart1.X + 2 * mBezierControl1.X + mBezierEnd1.X) / 4;
            mBeziervertex1.Y = (2 * mBezierControl1.Y + mBezierStart1.Y + mBezierEnd1.Y) / 4;
            mBeziervertex2.X = (mBezierStart2.X + 2 * mBezierControl2.X + mBezierEnd2.X) / 4;
            mBeziervertex2.Y = (2 * mBezierControl2.Y + mBezierStart2.Y + mBezierEnd2.Y) / 4;
        }

        protected override void drawCurrentPageArea(Canvas canvas)
        {
            mPath0.Reset();
            mPath0.MoveTo(mBezierStart1.X, mBezierStart1.Y);
            mPath0.QuadTo(mBezierControl1.X, mBezierControl1.Y, mBezierEnd1.X, mBezierEnd1.Y);
            mPath0.LineTo(mTouch.X, mTouch.Y);
            mPath0.LineTo(mBezierEnd2.X, mBezierEnd2.Y);
            mPath0.QuadTo(mBezierControl2.X, mBezierControl2.Y, mBezierStart2.X, mBezierStart2.Y);
            mPath0.LineTo(mCornerX, mCornerY);
            mPath0.Close();

            canvas.Save();
            canvas.ClipPath(mPath0, Region.Op.Xor);
            canvas.DrawBitmap(mCurPageBitmap, 0, 0, null);
            try
            {
                canvas.Restore();
            }
            catch (Exception e)
            {

            }
        }

        protected override void drawNextPageAreaAndShadow(Canvas canvas)
        {
            mPath1.Reset();
            mPath1.MoveTo(mBezierStart1.X, mBezierStart1.Y);
            mPath1.LineTo(mBeziervertex1.X, mBeziervertex1.Y);
            mPath1.LineTo(mBeziervertex2.X, mBeziervertex2.Y);
            mPath1.LineTo(mBezierStart2.X, mBezierStart2.Y);
            mPath1.LineTo(mCornerX, mCornerY);
            mPath1.Close();

            mDegrees = (float)Java.Lang.Math.ToDegrees(Java.Lang.Math.Atan2(mBezierControl1.X - mCornerX, mBezierControl2.Y - mCornerY));
            int leftx;
            int rightx;
            GradientDrawable mBackShadowDrawable;
            if (mIsRTandLB)
            {  //左下及右上
                leftx = (int)(mBezierStart1.X);
                rightx = (int)(mBezierStart1.X + mTouchToCornerDis / 4);
                mBackShadowDrawable = mBackShadowDrawableLR;
            }
            else
            {
                leftx = (int)(mBezierStart1.X - mTouchToCornerDis / 4);
                rightx = (int)mBezierStart1.X;
                mBackShadowDrawable = mBackShadowDrawableRL;
            }
            canvas.Save();
            try
            {
                canvas.ClipPath(mPath0);
                canvas.ClipPath(mPath1, Region.Op.Intersect);
            }
            catch (Exception e)
            {
            }


            canvas.DrawBitmap(mNextPageBitmap, 0, 0, null);
            canvas.Rotate(mDegrees, mBezierStart1.X, mBezierStart1.Y);
            mBackShadowDrawable.SetBounds(leftx, (int)mBezierStart1.Y,
                    rightx, (int)(mMaxLength + mBezierStart1.Y));//左上及右下角的xy坐标值,构成一个矩形
            mBackShadowDrawable.Draw(canvas);
            canvas.Restore();
        }

        protected override void setBitmaps(Bitmap bm1, Bitmap bm2)
        {
            mCurPageBitmap = bm1;
            mNextPageBitmap = bm2;
        }

        /**
         * 创建阴影的GradientDrawable
         */
        private void createDrawable()
        {
            var uintValue = 0xb0333333;
            int[] color = { 0x333333, (int)uintValue };
            mFolderShadowDrawableRL = new GradientDrawable(GradientDrawable.Orientation.RightLeft, color);
            mFolderShadowDrawableRL.SetGradientType(GradientType.LinearGradient);

            mFolderShadowDrawableLR = new GradientDrawable(GradientDrawable.Orientation.LeftRight, color);
            mFolderShadowDrawableLR.SetGradientType(GradientType.LinearGradient);
            var uintValue2 = 0xff111111;
            mBackShadowColors = new int[] { (int)uintValue2, 0x111111 };
            mBackShadowDrawableRL = new GradientDrawable(GradientDrawable.Orientation.RightLeft, mBackShadowColors);
            mBackShadowDrawableRL.SetGradientType(GradientType.LinearGradient);

            mBackShadowDrawableLR = new GradientDrawable(GradientDrawable.Orientation.LeftRight, mBackShadowColors);
            mBackShadowDrawableLR.SetGradientType(GradientType.LinearGradient);
            var uintValue3 = 0x80111111;
            mFrontShadowColors = new int[] { (int)uintValue3, 0x111111 };
            mFrontShadowDrawableVLR = new GradientDrawable(GradientDrawable.Orientation.LeftRight, mFrontShadowColors);
            mFrontShadowDrawableVLR.SetGradientType(GradientType.LinearGradient);
            mFrontShadowDrawableVRL = new GradientDrawable(GradientDrawable.Orientation.RightLeft, mFrontShadowColors);
            mFrontShadowDrawableVRL.SetGradientType(GradientType.LinearGradient);

            mFrontShadowDrawableHTB = new GradientDrawable(GradientDrawable.Orientation.TopBottom, mFrontShadowColors);
            mFrontShadowDrawableHTB.SetGradientType(GradientType.LinearGradient);

            mFrontShadowDrawableHBT = new GradientDrawable(GradientDrawable.Orientation.BottomTop, mFrontShadowColors);
            mFrontShadowDrawableHBT.SetGradientType(GradientType.LinearGradient);
        }

        /**
         * 绘制翻起页的阴影
         *
         * @param canvas
         */
        protected override void drawCurrentPageShadow(Canvas canvas)
        {
            double degree;
            if (mIsRTandLB)
            {
                degree = Math.PI / 4 - Java.Lang.Math.Atan2(mBezierControl1.Y - mTouch.Y, mTouch.X - mBezierControl1.X);
            }
            else
            {
                degree = Math.PI / 4 - Java.Lang.Math.Atan2(mTouch.Y - mBezierControl1.Y, mTouch.X - mBezierControl1.X);
            }
            // 翻起页阴影顶点与touch点的距离
            double d1 = (float)25 * 1.414 * Math.Cos(degree);
            double d2 = (float)25 * 1.414 * Math.Sin(degree);
            float x = (float)(mTouch.X + d1);
            float y;
            if (mIsRTandLB)
            {
                y = (float)(mTouch.Y + d2);
            }
            else
            {
                y = (float)(mTouch.Y - d2);
            }
            mPath1.Reset();
            mPath1.MoveTo(x, y);
            mPath1.LineTo(mTouch.X, mTouch.Y);
            mPath1.LineTo(mBezierControl1.X, mBezierControl1.Y);
            mPath1.LineTo(mBezierStart1.X, mBezierStart1.Y);
            mPath1.Close();
            float rotateDegrees;
            canvas.Save();
            try
            {
                canvas.ClipPath(mPath0, Region.Op.Xor);
                canvas.ClipPath(mPath1, Region.Op.Intersect);
            }
            catch (Exception e)
            {
            }

            int leftx;
            int rightx;
            GradientDrawable mCurrentPageShadow;
            if (mIsRTandLB)
            {
                leftx = (int)(mBezierControl1.X);
                rightx = (int)mBezierControl1.X + 25;
                mCurrentPageShadow = mFrontShadowDrawableVLR;
            }
            else
            {
                leftx = (int)(mBezierControl1.X - 25);
                rightx = (int)mBezierControl1.X + 1;
                mCurrentPageShadow = mFrontShadowDrawableVRL;
            }

            rotateDegrees = (float)Java.Lang.Math.ToDegrees(Java.Lang.Math.Atan2(mTouch.X - mBezierControl1.X,
                    mBezierControl1.Y - mTouch.Y));
            canvas.Rotate(rotateDegrees, mBezierControl1.X, mBezierControl1.Y);
            mCurrentPageShadow.SetBounds(leftx, (int)(mBezierControl1.Y - mMaxLength),
                    rightx, (int)(mBezierControl1.Y));
            mCurrentPageShadow.Draw(canvas);
            canvas.Restore();

            mPath1.Reset();
            mPath1.MoveTo(x, y);
            mPath1.LineTo(mTouch.X, mTouch.Y);
            mPath1.LineTo(mBezierControl2.X, mBezierControl2.Y);
            mPath1.LineTo(mBezierStart2.X, mBezierStart2.Y);
            mPath1.Close();
            canvas.Save();
            try
            {
                canvas.ClipPath(mPath0, Region.Op.Xor);
                canvas.ClipPath(mPath1, Region.Op.Intersect);
            }
            catch (Exception e)
            {
            }

            if (mIsRTandLB)
            {
                leftx = (int)(mBezierControl2.Y);
                rightx = (int)(mBezierControl2.Y + 25);
                mCurrentPageShadow = mFrontShadowDrawableHTB;
            }
            else
            {
                leftx = (int)(mBezierControl2.Y - 25);
                rightx = (int)(mBezierControl2.Y + 1);
                mCurrentPageShadow = mFrontShadowDrawableHBT;
            }
            rotateDegrees = (float)Java.Lang.Math.ToDegrees(Java.Lang.Math.Atan2(mBezierControl2.Y - mTouch.Y, mBezierControl2.X - mTouch.X));
            canvas.Rotate(rotateDegrees, mBezierControl2.X, mBezierControl2.Y);
            float temp;
            if (mBezierControl2.Y < 0)
                temp = mBezierControl2.Y - mScreenHeight;
            else
                temp = mBezierControl2.Y;

            int hmg = (int)Java.Lang.Math.Hypot(mBezierControl2.X, temp);
            if (hmg > mMaxLength)
            {
                mCurrentPageShadow.SetBounds((int)(mBezierControl2.X - 25) - hmg, leftx,
                        (int)(mBezierControl2.X + mMaxLength) - hmg, rightx);
            }
            else
            {
                mCurrentPageShadow.SetBounds((int)(mBezierControl2.X - mMaxLength), leftx,
                        (int)(mBezierControl2.X), rightx);
            }
            mCurrentPageShadow.Draw(canvas);
            canvas.Restore();
        }

        protected override void drawCurrentBackArea(Canvas canvas)
        {
            int i = (int)(mBezierStart1.X + mBezierControl1.X) / 2;
            float f1 = System.Math.Abs(i - mBezierControl1.X);
            int i1 = (int)(mBezierStart2.Y + mBezierControl2.Y) / 2;
            float f2 = System.Math.Abs(i1 - mBezierControl2.Y);
            float f3 = Math.Min(f1, f2);
            mPath1.Reset();
            mPath1.MoveTo(mBeziervertex2.X, mBeziervertex2.Y);
            mPath1.LineTo(mBeziervertex1.X, mBeziervertex1.Y);
            mPath1.LineTo(mBezierEnd1.X, mBezierEnd1.Y);
            mPath1.LineTo(mTouch.X, mTouch.Y);
            mPath1.LineTo(mBezierEnd2.X, mBezierEnd2.Y);
            mPath1.Close();
            GradientDrawable mFolderShadowDrawable;
            int left;
            int right;
            if (mIsRTandLB)
            {
                left = (int)(mBezierStart1.X - 1);
                right = (int)(mBezierStart1.X + f3 + 1);
                mFolderShadowDrawable = mFolderShadowDrawableLR;
            }
            else
            {
                left = (int)(mBezierStart1.X - f3 - 1);
                right = (int)(mBezierStart1.X + 1);
                mFolderShadowDrawable = mFolderShadowDrawableRL;
            }
            canvas.Save();
            try
            {
                canvas.ClipPath(mPath0);
                canvas.ClipPath(mPath1, Region.Op.Intersect);
            }
            catch (Exception e)
            {
            }

            mPaint.SetColorFilter(mColorMatrixFilter);

            float dis = (float)Java.Lang.Math.Hypot(mCornerX - mBezierControl1.X,
                    mBezierControl2.Y - mCornerY);
            float f8 = (mCornerX - mBezierControl1.X) / dis;
            float f9 = (mBezierControl2.Y - mCornerY) / dis;
            mMatrixArray[0] = 1 - 2 * f9 * f9;
            mMatrixArray[1] = 2 * f8 * f9;
            mMatrixArray[3] = mMatrixArray[1];
            mMatrixArray[4] = 1 - 2 * f8 * f8;
            mMatrix.Reset();
            mMatrix.SetValues(mMatrixArray);
            mMatrix.PreTranslate(-mBezierControl1.X, -mBezierControl1.Y);
            mMatrix.PostTranslate(mBezierControl1.X, mBezierControl1.Y);
            canvas.DrawBitmap(mCurPageBitmap, mMatrix, mPaint);
            // canvas.DrawBitmap(bitmap, mMatrix, null);
            mPaint.SetColorFilter(null);
            canvas.Rotate(mDegrees, mBezierStart1.X, mBezierStart1.Y);
            mFolderShadowDrawable.SetBounds(left, (int)mBezierStart1.Y, right,
                    (int)(mBezierStart1.Y + mMaxLength));
            mFolderShadowDrawable.Draw(canvas);
            canvas.Restore();
        }

        public override void ComputeScroll()
        {
            base.ComputeScroll();
            if (mScroller.ComputeScrollOffset())
            {
                float x = mScroller.CurrX;
                float y = mScroller.CurrY;
                mTouch.X = x;
                mTouch.Y = y;
                PostInvalidate();
            }
        }

        protected override void startAnimation()
        {
            int dx, dy;
            if (mCornerX > 0)
            {
                dx = -(int)(mScreenWidth + mTouch.X);
            }
            else
            {
                dx = (int)(mScreenWidth - mTouch.X + mScreenWidth);
            }
            if (mCornerY > 0)
            {
                dy = (int)(mScreenHeight - mTouch.Y);
            }
            else
            {
                dy = (int)(1 - mTouch.Y); // 防止mTouch.y最终变为0
            }
            mScroller.StartScroll((int)mTouch.X, (int)mTouch.Y, dx, dy, 700);
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
            int dx, dy;
            if (mCornerX > 0)
            {
                dx = (int)(mScreenWidth - mTouch.X);
            }
            else
            {
                dx = (int)(-mTouch.X);
            }
            if (mCornerY > 0)
            {
                dy = (int)(mScreenHeight - mTouch.Y);
            }
            else
            {
                dy = (int)(1 - mTouch.Y);
            }
            mScroller.StartScroll((int)mTouch.X, (int)mTouch.Y, dx, dy, 300);
        }

        public /*synchronized*/ override void setTheme(int theme)
        {
            resetTouchPoint();
            calcCornerXY(mTouch.X, mTouch.Y);
            Bitmap bg = ThemeManager.getThemeDrawable(theme);
            if (bg != null)
            {
                pagefactory.setBgBitmap(bg);
                pagefactory.convertBetteryBitmap();
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

        public override void jumpToChapter(int chapter)
        {
            calcCornerXY(mTouch.X, mTouch.Y);
            base.jumpToChapter(chapter);
        }
    }
}