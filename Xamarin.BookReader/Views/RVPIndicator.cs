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
using Android.Content.Res;
using Android.Graphics;
using Android.Support.V4.View;
using Android.Graphics.Drawables;
using Android.Util;
using static Android.Graphics.Paint;

namespace Xamarin.BookReader.Views
{
    public class RVPIndicator : LinearLayout
    {
        // 指示器风格-图标
        private const int STYLE_BITMAP = 0;
        // 指示器风格-下划线
        private const int STYLE_LINE = 1;
        // 指示器风格-方形背景
        private const int STYLE_SQUARE = 2;
        // 指示器风格-三角形
        private const int STYLE_TRIANGLE = 3;

        /*
         * 系统默认:Tab数量
         */
        private static int D_TAB_COUNT = 3;

        /*
         * 系统默认:文字正常时颜色
         */
        private static int D_TEXT_COLOR_NORMAL = Color.ParseColor("#000000");

        /*
         * 系统默认:文字选中时颜色
         */
        private static int D_TEXT_COLOR_HIGHLIGHT = Color.ParseColor("#FF0000");

        /*
         * 系统默认:指示器颜色
         */
        private static int D_INDICATOR_COLOR = Color.ParseColor("#f29b76");

        /**
         * tab上的内容
         */
        private List<String> mTabTitles;

        /**
         * 可见tab数量
         */
        private int mTabVisibleCount = D_TAB_COUNT;

        /**
         * 与之绑定的ViewPager
         */
        public ViewPager mViewPager;

        /**
         * 文字大小
         */
        private int mTextSize = 16;

        /**
         * 文字正常时的颜色
         */
        private int mTextColorNormal = D_TEXT_COLOR_NORMAL;

        /**
         * 文字选中时的颜色
         */
        private int mTextColorHighlight = D_TEXT_COLOR_HIGHLIGHT;

        /**
         * 指示器正常时的颜色
         */
        private int mIndicatorColor = D_INDICATOR_COLOR;

        /**
         * 画笔
         */
        private Paint mPaint;

        /**
         * 矩形
         */
        private Rect mRectF;

        /**
         * bitmap
         */
        private Bitmap mBitmap;

        /**
         * 指示器高
         */
        private int mIndicatorHeight = 5;

        /**
         * 指示器宽
         */
        private int mIndicatorWidth;

        /**
         * 三角形的宽度为单个Tab的1/6
         */
        private static float RADIO_TRIANGEL = 1.0f / 6;

        /**
         * 手指滑动时的偏移量
         */
        private float mTranslationX;

        /**
         * 指示器风格
         */
        private int mIndicatorStyle = STYLE_LINE;

        /**
         * 曲线path
         */
        private Path mPath;

        /**
         * viewPage初始下标
         */
        private int mPosition = 0;

        public RVPIndicator(Context context) : this(context, null)
        {

        }

        public RVPIndicator(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            mIndicatorWidth = Width / mTabVisibleCount;
            // 获得自定义属性
            TypedArray a = context.ObtainStyledAttributes(attrs,
                    Resource.Styleable.RVPIndicator);

            mTabVisibleCount = a.GetInt(Resource.Styleable.RVPIndicator_item_count,
                    D_TAB_COUNT);
            mTextColorNormal = a
                    .GetColor(Resource.Styleable.RVPIndicator_text_color_normal,
                            D_TEXT_COLOR_NORMAL);
            mTextColorHighlight = a.GetColor(
                    Resource.Styleable.RVPIndicator_text_color_hightlight,
                    D_TEXT_COLOR_HIGHLIGHT);
            mTextSize = a.GetDimensionPixelSize(Resource.Styleable.RVPIndicator_text_size,
                    16);
            mIndicatorColor = a.GetColor(Resource.Styleable.RVPIndicator_indicator_color,
                    D_INDICATOR_COLOR);
            mIndicatorStyle = a.GetInt(Resource.Styleable.RVPIndicator_indicator_style,
                    STYLE_LINE);

            Drawable drawable = a
                    .GetDrawable(Resource.Styleable.RVPIndicator_indicator_src);

            if (drawable != null)
            {
                if (drawable is BitmapDrawable)
                {
                    mBitmap = ((BitmapDrawable)drawable).Bitmap;
                }
                else if (drawable is NinePatchDrawable)
                {
                    // .9图处理
                    Bitmap bitmap = Bitmap.CreateBitmap(
                            drawable.IntrinsicWidth,
                            drawable.IntrinsicHeight, Bitmap.Config.Argb8888);
                    Canvas canvas = new Canvas(bitmap);
                    drawable.SetBounds(0, 0, canvas.Width, canvas.Height);
                    drawable.Draw(canvas);
                    mBitmap = bitmap;

                }
            }
            else
            {
                mBitmap = BitmapFactory.DecodeResource(Resources,
                        Resource.Drawable.heart_love);
            }

            a.Recycle();

            /**
             * 设置画笔
             */
            mPaint = new Paint();
            mPaint.AntiAlias = (true);
            mPaint.Color = new Color(mIndicatorColor);
            mPaint.SetStyle(Style.Fill);
        }

        /**
         * 初始化指示器
         */
        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);

            switch (mIndicatorStyle)
            {

                case STYLE_LINE:

                    /*
                     * 下划线指示器:宽与item相等,高是item的1/10
                     */
                    mIndicatorWidth = w / mTabVisibleCount;
                    mIndicatorHeight = h / 10;
                    mTranslationX = 0;
                    mRectF = new Rect(0, 0, mIndicatorWidth, mIndicatorHeight);

                    break;
                case STYLE_SQUARE:
                case STYLE_BITMAP:

                    /*
                     * 方形/图标指示器:宽,高与item相等
                     */
                    mIndicatorWidth = w / mTabVisibleCount;
                    mIndicatorHeight = h;
                    mTranslationX = 0;
                    mRectF = new Rect(0, 0, mIndicatorWidth, mIndicatorHeight);

                    break;
                case STYLE_TRIANGLE:

                    /*
                     * 三角形指示器:宽与item(1/4)相等,高是item的1/4
                     */
                    //mIndicatorWidth = w / mTabVisibleCount / 4;
                    // mIndicatorHeight = h / 4;
                    mIndicatorWidth = (int)(w / mTabVisibleCount * RADIO_TRIANGEL);// 1/6 of  width  ;
                    mIndicatorHeight = (int)(mIndicatorWidth / 2 / Math.Sqrt(2));
                    mTranslationX = 0;

                    break;
            }
            // 初始化tabItem
            initTabItem();

            // 高亮
            setHighLightTextView(mPosition);
        }

        /**
         * 绘制指示器(子view)
         */
        protected override void DispatchDraw(Canvas canvas)
        {
            // 保存画布
            canvas.Save();

            switch (mIndicatorStyle)
            {

                case STYLE_BITMAP:

                    canvas.Translate(mTranslationX, 0);
                    canvas.DrawBitmap(mBitmap, null, mRectF, mPaint);

                    break;
                case STYLE_LINE:

                    canvas.Translate(mTranslationX, Height - mIndicatorHeight);
                    canvas.DrawRect(mRectF, mPaint);

                    break;
                case STYLE_SQUARE:

                    canvas.Translate(mTranslationX, 0);
                    canvas.DrawRect(mRectF, mPaint);

                    break;
                case STYLE_TRIANGLE:

                    canvas.Translate(mTranslationX, 0);
                    // 笔锋圆滑度
                    // mPaint.setPathEffect(new CornerPathEffect(10));
                    mPath = new Path();
                    int midOfTab = Width / mTabVisibleCount / 2;
                    mPath.MoveTo(midOfTab, Height - mIndicatorHeight);
                    mPath.LineTo(midOfTab - mIndicatorWidth / 2, Height);
                    mPath.LineTo(midOfTab + mIndicatorWidth / 2, Height);
                    mPath.Close();
                    canvas.DrawPath(mPath, mPaint);

                    break;
            }

            // 恢复画布
            canvas.Restore();
            base.DispatchDraw(canvas);
        }

        /**
         * 设置tab上的内容
         *
         * @param datas
         */
        public void setTabItemTitles(List<String> datas)
        {
            this.mTabTitles = datas;
        }

        /**
         * 设置关联viewPager
         *
         * @param viewPager
         * @param pos
         */
        //TODO：@SuppressWarnings("deprecation")
        [Obsolete]
        public void setViewPager(ViewPager viewPager, int pos)
        {
            this.mViewPager = viewPager;

            mViewPager.SetOnPageChangeListener(new CustomOnPageChangeListener(this));

            // 设置当前页
            mViewPager.CurrentItem = pos;
            mPosition = pos;

        }

        /**
         *
         * 初始化tabItem
         *
         * @author Ruffian
         */
        private void initTabItem()
        {

            if (mTabTitles != null && mTabTitles.Count() > 0)
            {
                if (this.ChildCount != 0)
                {
                    this.RemoveAllViews();
                }

                foreach (String title in mTabTitles)
                {
                    AddView(createTextView(title));
                }

                // 设置点击事件
                setItemClickEvent();
            }

        }

        /**
         * 设置点击事件
         */
        private void setItemClickEvent()
        {
            int cCount = ChildCount;
            for (int i = 0; i < cCount; i++)
            {
                int j = i;
                View view = GetChildAt(i);
                view.Click += (sender, e) =>
                {
                    mViewPager.CurrentItem = j;
                };
            }
        }

        /**
         * 设置文本高亮
         *
         * @param position
         */
        private void setHighLightTextView(int position)
        {

            for (int i = 0; i < ChildCount; i++)
            {
                View view = GetChildAt(i);
                if (view is TextView)
                {
                    if (i == position)
                    {
                        ((TextView)view).SetTextColor(ColorStateList.ValueOf(new Color(mTextColorHighlight)));
                    }
                    else
                    {
                        ((TextView)view).SetTextColor(ColorStateList.ValueOf(new Color(mTextColorNormal)));
                    }
                }
            }
        }

        /**
         * 滚动
         *
         * @param position
         * @param offset
         */
        private void onScoll(int position, float offset)
        {

            // 不断改变偏移量，invalidate
            mTranslationX = Width / mTabVisibleCount * (position + offset);

            int tabWidth = Width / mTabVisibleCount;

            // 容器滚动，当移动到倒数第二个的时候，开始滚动
            if (offset > 0 && position >= (mTabVisibleCount - 2)
                    && ChildCount > mTabVisibleCount
                    && position < (ChildCount - 2))
            {
                if (mTabVisibleCount != 1)
                {

                    int xValue = (position - (mTabVisibleCount - 2)) * tabWidth
                            + (int)(tabWidth * offset);
                    this.ScrollTo(xValue, 0);

                }
                else
                {
                    // 为count为1时 的特殊处理
                    this.ScrollTo(position * tabWidth + (int)(tabWidth * offset),
                            0);
                }
            }

            Invalidate();
        }

        /**
         * 创建标题view
         *
         * @param text
         * @return
         */
        private TextView createTextView(String text)
        {
            TextView tv = new TextView(Context);
            LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams(
                    LayoutParams.MatchParent, LayoutParams.MatchParent);
            lp.Width = Width / mTabVisibleCount;
            tv.Gravity = (GravityFlags.Center);
            tv.SetTextColor(ColorStateList.ValueOf(new Color(mTextColorNormal)));
            tv.Text = text;
            tv.SetTextSize(ComplexUnitType.Sp, mTextSize);
            tv.LayoutParameters = lp;
            return tv;
        }

        /**
         * 对外的ViewPager的回调接口
         *
         * @author Ruffian
         *
         */
        public interface PageChangeListener
        {
            void onPageScrolled(int position, float positionOffset,
                                int positionOffsetPixels);

            void onPageSelected(int position);

            void onPageScrollStateChanged(int state);
        }

        // 对外的ViewPager的回调接口
        private PageChangeListener onPageChangeListener;

        // 对外的ViewPager的回调接口的设置
        public void setOnPageChangeListener(PageChangeListener pageChangeListener)
        {
            this.onPageChangeListener = pageChangeListener;
        }

        class CustomOnPageChangeListener : Java.Lang.Object, ViewPager.IOnPageChangeListener
        {
            private RVPIndicator rVPIndicator;

            public CustomOnPageChangeListener(RVPIndicator rVPIndicator)
            {
                this.rVPIndicator = rVPIndicator;
            }

            public void OnPageSelected(int position)
            {
                // 设置文本高亮
                rVPIndicator.setHighLightTextView(position);

                // 回调
                if (rVPIndicator.onPageChangeListener != null)
                {
                    rVPIndicator.onPageChangeListener.onPageSelected(position);
                }
            }

            public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
            {
                // scoll
                rVPIndicator.onScoll(position, positionOffset);

                // 回调
                if (rVPIndicator.onPageChangeListener != null)
                {
                    rVPIndicator.onPageChangeListener.onPageScrolled(position,
                            positionOffset, positionOffsetPixels);
                }
            }

            public void OnPageScrollStateChanged(int state)
            {
                // 回调
                if (rVPIndicator.onPageChangeListener != null)
                {
                    rVPIndicator.onPageChangeListener.onPageScrollStateChanged(state);
                }
            }

        }
    }
}