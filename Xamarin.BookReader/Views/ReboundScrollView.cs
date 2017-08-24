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
using Android.Util;
using Android.Views.Animations;

namespace Xamarin.BookReader.Views
{
    /// <summary>
    /// 弹性ScrollView 上下拉超出后，手指离开后弹回的“阻尼”效果
    /// </summary>
    [Register("xamarin.bookreader.views.ReboundScrollView")]
    public class ReboundScrollView : ScrollView
    {
        // 移动因子, 是一个百分比, 比如手指移动了100px, 那么View就只移动50px 目的是达到一个延迟的效果
        private static float MOVE_FACTOR = 0.5f;

        // 松开手指后, 界面回到正常位置需要的动画时间
        private static int ANIM_TIME = 300;

        // ScrollView的子View， 也是ScrollView的唯一一个子View
        private View contentView;

        // 手指按下时的Y值, 用于在移动时计算移动距离
        // 如果按下时不能上拉和下拉， 会在手指移动时更新为当前手指的Y值
        private float startY;

        // 用于记录正常的布局位置
        private Rect originalRect = new Rect();

        // 手指按下时记录是否可以继续下拉
        private bool canPullDown = false;

        // 手指按下时记录是否可以继续上拉
        private bool canPullUp = false;

        // 在手指滑动的过程中记录是否移动了布局
        private bool isMoved = false;

        public ReboundScrollView(Context context) : base(context)
        {

        }

        public ReboundScrollView(Context context, IAttributeSet attrs) : base(context, attrs)
        {

        }

        protected override void OnFinishInflate()
        {
            if (ChildCount > 0)
            {
                contentView = GetChildAt(0);
            }
            base.OnFinishInflate();
        }

        /**
         * 在该方法中获取ScrollView中的唯一子控件的位置信息 这个位置信息在整个控件的生命周期中保持不变
         */
        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            if (contentView == null)
                return;

            // ScrollView中的唯一子控件的位置信息, 这个位置信息在整个控件的生命周期中保持不变
            originalRect.Set(contentView.Left, contentView.Top, contentView.Right, contentView.Bottom);
        }

        /**
         * 在触摸事件中, 处理上拉和下拉的逻辑
         */
        public override bool DispatchTouchEvent(MotionEvent ev)
        {

            if (contentView == null)
            {
                return base.DispatchTouchEvent(ev);
            }

            MotionEventActions action = ev.Action;

            switch (action)
            {
                case MotionEventActions.Down:

                    // 判断是否可以上拉和下拉
                    canPullDown = isCanPullDown();
                    canPullUp = isCanPullUp();

                    // 记录按下时的Y值
                    startY = ev.GetY();
                    break;

                case MotionEventActions.Up:

                    if (!isMoved)
                        break; // 如果没有移动布局， 则跳过执行

                    // 开启动画
                    TranslateAnimation anim = new TranslateAnimation(0, 0, contentView.Top, originalRect.Top);
                    anim.Duration = (ANIM_TIME);

                    contentView.StartAnimation(anim);

                    // 设置回到正常的布局位置
                    contentView.Layout(originalRect.Left, originalRect.Top, originalRect.Right, originalRect.Bottom);

                    // 将标志位设回false
                    canPullDown = false;
                    canPullUp = false;
                    isMoved = false;

                    break;
                case MotionEventActions.Move:

                    // 在移动的过程中， 既没有滚动到可以上拉的程度， 也没有滚动到可以下拉的程度
                    if (!canPullDown && !canPullUp)
                    {
                        startY = ev.GetY();
                        canPullDown = isCanPullDown();
                        canPullUp = isCanPullUp();

                        break;
                    }

                    // 计算手指移动的距离
                    float nowY = ev.GetY();
                    int deltaY = (int)(nowY - startY);

                    // 是否应该移动布局
                    bool shouldMove = (canPullDown && deltaY > 0) // 可以下拉， 并且手指向下移动
                            || (canPullUp && deltaY < 0) // 可以上拉， 并且手指向上移动
                            || (canPullUp && canPullDown); // 既可以上拉也可以下拉（这种情况出现在ScrollView包裹的控件比ScrollView还小）

                    if (shouldMove)
                    {

                        // 计算偏移量
                        int offset = (int)(deltaY * MOVE_FACTOR);

                        // 随着手指的移动而移动布局
                        contentView.Layout(originalRect.Left, originalRect.Top + offset, originalRect.Right, originalRect.Bottom + offset);

                        isMoved = true; // 记录移动了布局
                    }

                    break;
                default:
                    break;
            }

            return base.DispatchTouchEvent(ev);
        }

        /**
         * 判断是否滚动到顶部
         */
        private bool isCanPullDown()
        {
            return ScrollY == 0 || contentView.Height < Height + ScrollY;
        }

        /**
         * 判断是否滚动到底部
         */
        private bool isCanPullUp()
        {
            return contentView.Height <= Height + ScrollY;
        }
    }
}