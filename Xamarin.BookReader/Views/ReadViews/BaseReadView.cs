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
using Xamarin.BookReader.Models;
using Xamarin.BookReader.Utils;
using Xamarin.BookReader.Managers;
using Java.Lang;
using Settings = Xamarin.BookReader.Helpers.Settings;

namespace Xamarin.BookReader.Views.ReadViews
{
    public abstract class BaseReadView:View
    {
        protected int mScreenWidth;
    protected int mScreenHeight;

    protected PointF mTouch = new PointF();
    protected float actiondownX, actiondownY;
    protected float touch_down = 0; // 当前触摸点与按下时的点的差值

    protected Bitmap mCurPageBitmap, mNextPageBitmap;
    protected Canvas mCurrentPageCanvas, mNextPageCanvas;
    protected PageFactory pagefactory = null;

    protected IOnReadStateChangeListener listener;
    protected string bookId;
    public bool isPrepared = false;

    public Scroller mScroller;

    public BaseReadView(Context context, string bookId, List<BookMixAToc.MixToc.Chapters> chaptersList,
                        IOnReadStateChangeListener listener):base(context) {
        
        this.listener = listener;
        this.bookId = bookId;

        mScreenWidth = ScreenUtils.getScreenWidth();
        mScreenHeight = ScreenUtils.getScreenHeight();

        mCurPageBitmap = Bitmap.CreateBitmap(mScreenWidth, mScreenHeight, Bitmap.Config.Argb8888);
        mNextPageBitmap = Bitmap.CreateBitmap(mScreenWidth, mScreenHeight, Bitmap.Config.Argb8888);
        mCurrentPageCanvas = new Canvas(mCurPageBitmap);
        mNextPageCanvas = new Canvas(mNextPageBitmap);

        mScroller = new Scroller(Context);

        pagefactory = new PageFactory(Context, bookId, chaptersList);
        pagefactory.setOnReadStateChangeListener(listener);
    }

    public /*synchronized*/ void init(int theme) {
        if (!isPrepared) {
            try {
                pagefactory.setBgBitmap(ThemeManager.getThemeDrawable(theme));
                    // 自动跳转到上次阅读位置
                    int[] pos = Settings.GetReadProgress(bookId);
                    int ret = pagefactory.openBook(pos[0], new int[]{pos[1], pos[2]});
                LogUtils.i("上次阅读位置：chapter=" + pos[0] + " startPos=" + pos[1] + " endPos=" + pos[2]);
                if (ret == 0) {
                    listener.onLoadChapterFailure(pos[0]);
                    return;
                }
                pagefactory.onDraw(mCurrentPageCanvas);
                PostInvalidate();
            } catch (Java.Lang.Exception e) {
            }
            isPrepared = true;
        }
    }

    private int dx, dy;
    private long et = 0;
    private bool cancel = false;
    private bool center = false;

    public override bool OnTouchEvent(MotionEvent e) {
        switch (e.Action) {
            case MotionEventActions.Down:
                et = JavaSystem.CurrentTimeMillis();
                dx = (int) e.GetX();
                dy = (int) e.GetY();
                mTouch.X = dx;
                mTouch.Y = dy;
                actiondownX = dx;
                actiondownY = dy;
                touch_down = 0;
                pagefactory.onDraw(mCurrentPageCanvas);
                if (actiondownX >= mScreenWidth / 3 && actiondownX <= mScreenWidth * 2 / 3
                        && actiondownY >= mScreenHeight / 3 && actiondownY <= mScreenHeight * 2 / 3) {
                    center = true;
                } else {
                    center = false;
                    calcCornerXY(actiondownX, actiondownY);
                    if (actiondownX < mScreenWidth / 2) {// 从左翻
                        BookStatus status = pagefactory.prePage();
                        if (status == BookStatus.NO_PRE_PAGE) {
                            ToastUtils.showSingleToast("没有上一页啦");
                            return false;
                        } else if (status == BookStatus.LOAD_SUCCESS) {
                            abortAnimation();
                            pagefactory.onDraw(mNextPageCanvas);
                        } else {
                            return false;
                        }
                    } else if (actiondownX >= mScreenWidth / 2) {// 从右翻
                        BookStatus status = pagefactory.nextPage();
                        if (status == BookStatus.NO_NEXT_PAGE) {
                            ToastUtils.showSingleToast("没有下一页啦");
                            return false;
                        } else if (status == BookStatus.LOAD_SUCCESS) {
                            abortAnimation();
                            pagefactory.onDraw(mNextPageCanvas);
                        } else {
                            return false;
                        }
                    }
                    listener.onFlip();
                    setBitmaps(mCurPageBitmap, mNextPageBitmap);
                }
                break;
            case MotionEventActions.Move:
                if (center)
                    break;
                int mx = (int) e.GetX();
                int my = (int) e.GetY();
                cancel = (actiondownX < mScreenWidth / 2 && mx < mTouch.X) || (actiondownX > mScreenWidth / 2 && mx > mTouch.X);
                mTouch.X = mx;
                mTouch.Y = my;
                touch_down = mTouch.X - actiondownX;
                this.PostInvalidate();
                break;
            case MotionEventActions.Up:
            case MotionEventActions.Cancel:

                long t = JavaSystem.CurrentTimeMillis();
                int ux = (int) e.GetX();
                int uy = (int) e.GetY();

                if (center) { // ACTION_DOWN的位置在中间，则不响应滑动事件
                    resetTouchPoint();
                    if (System.Math.Abs(ux - actiondownX) < 5 && System.Math.Abs(uy - actiondownY) < 5) {
                        listener.onCenterClick();
                        return false;
                    }
                    break;
                }

                if ((System.Math.Abs(ux - dx) < 10) && (System.Math.Abs(uy - dy) < 10)) {
                    if ((t - et < 1000)) { // 单击
                        startAnimation();
                    } else { // 长按
                        pagefactory.cancelPage();
                        restoreAnimation();
                    }
                    PostInvalidate();
                    return true;
                }
                if (cancel) {
                    pagefactory.cancelPage();
                    restoreAnimation();
                    PostInvalidate();
                } else {
                    startAnimation();
                    PostInvalidate();
                }
                cancel = false;
                center = false;
                break;
            default:
                break;
        }
        return true;
    }

    protected override void OnDraw(Canvas canvas) {
        calcPoints();
        drawCurrentPageArea(canvas);
        drawNextPageAreaAndShadow(canvas);
        drawCurrentPageShadow(canvas);
        drawCurrentBackArea(canvas);
    }

    protected abstract void drawNextPageAreaAndShadow(Canvas canvas);

    protected abstract void drawCurrentPageShadow(Canvas canvas);

    protected abstract void drawCurrentBackArea(Canvas canvas);

    protected abstract void drawCurrentPageArea(Canvas canvas);

    protected abstract void calcPoints();

    protected abstract void calcCornerXY(float x, float y);

    /**
     * 开启翻页
     */
    protected abstract void startAnimation();

    /**
     * 停止翻页动画（滑到一半调用停止的话  翻页效果会卡住 可调用#{restoreAnimation} 还原效果）
     */
    protected abstract void abortAnimation();

    /**
     * 还原翻页
     */
    protected abstract void restoreAnimation();

    protected abstract void setBitmaps(Bitmap mCurPageBitmap, Bitmap mNextPageBitmap);

    public abstract void setTheme(int theme);

    /**
     * 复位触摸点位
     */
    protected void resetTouchPoint() {
        mTouch.X = 0.1f;
        mTouch.Y = 0.1f;
        touch_down = 0;
        calcCornerXY(mTouch.X, mTouch.Y);
    }

    public virtual void jumpToChapter(int chapter) {
        resetTouchPoint();
        pagefactory.openBook(chapter, new int[]{0, 0});
        pagefactory.onDraw(mCurrentPageCanvas);
        pagefactory.onDraw(mNextPageCanvas);
        PostInvalidate();
    }

    public void nextPage() {
        BookStatus status = pagefactory.nextPage();
        if (status == BookStatus.NO_NEXT_PAGE) {
            ToastUtils.showSingleToast("没有下一页啦");
            return;
        } else if (status == BookStatus.LOAD_SUCCESS) {
            if (isPrepared) {
                pagefactory.onDraw(mCurrentPageCanvas);
                pagefactory.onDraw(mNextPageCanvas);
                PostInvalidate();
            }
        } else {
            return;
        }

    }

    public void prePage() {
        BookStatus status = pagefactory.prePage();
        if (status == BookStatus.NO_PRE_PAGE) {
            ToastUtils.showSingleToast("没有上一页啦");
            return;
        } else if (status == BookStatus.LOAD_SUCCESS) {
            if (isPrepared) {
                pagefactory.onDraw(mCurrentPageCanvas);
                pagefactory.onDraw(mNextPageCanvas);
                PostInvalidate();
            }
        } else {
            return;
        }
    }

    public /*synchronized*/ void setFontSize(int fontSizePx) {
        resetTouchPoint();
        pagefactory.setTextFont(fontSizePx);
        if (isPrepared) {
            pagefactory.onDraw(mCurrentPageCanvas);
            pagefactory.onDraw(mNextPageCanvas);
            Settings.FontSize = fontSizePx;
            PostInvalidate();
        }
    }

    public /*synchronized*/ void setTextColor(int textColor, int titleColor) {
        resetTouchPoint();
        pagefactory.setTextColor(textColor, titleColor);
        if (isPrepared) {
            pagefactory.onDraw(mCurrentPageCanvas);
            pagefactory.onDraw(mNextPageCanvas);
            PostInvalidate();
        }
    }

    public void setBattery(int battery) {
        pagefactory.setBattery(battery);
        if (isPrepared) {
            pagefactory.onDraw(mCurrentPageCanvas);
            PostInvalidate();
        }
    }

    public void setTime(string time) {
        pagefactory.setTime(time);
    }

    public void setPosition(int[] pos) {
        int ret = pagefactory.openBook(pos[0], new int[]{pos[1], pos[2]});
        if (ret == 0) {
            listener.onLoadChapterFailure(pos[0]);
            return;
        }
        pagefactory.onDraw(mCurrentPageCanvas);
        PostInvalidate();
    }

    public int[] getReadPos() {
        return pagefactory.getPosition();
    }

    public string getHeadLine() {
        return pagefactory.getHeadLineStr().Replace("@", "");
    }

    protected override void OnDetachedFromWindow() {
        base.OnDetachedFromWindow();
        if (pagefactory != null) {
            pagefactory.recycle();
        }

        if (mCurPageBitmap != null && !mCurPageBitmap.IsRecycled) {
            mCurPageBitmap.Recycle();
            mCurPageBitmap = null;
            LogUtils.d("mCurPageBitmap recycle");
        }

        if (mNextPageBitmap != null && !mNextPageBitmap.IsRecycled) {
            mNextPageBitmap.Recycle();
            mNextPageBitmap = null;
            LogUtils.d("mNextPageBitmap recycle");
        }
    }
    }
}