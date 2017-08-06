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

using Android.Util;

namespace Xamarin.BookReader.Views
{
    /// <summary>
    /// 屏蔽 滑动事件
    /// </summary>
    public class MyScrollview : ScrollView
    {
        private int downX;
        private int downY;
        private int mTouchSlop;

        public MyScrollview(Context context) : base(context)
        {

            mTouchSlop = ViewConfiguration.Get(context).ScaledTouchSlop;
        }

        public MyScrollview(Context context, IAttributeSet attrs) : base(context, attrs)
        {

            mTouchSlop = ViewConfiguration.Get(context).ScaledTouchSlop;
        }

        public MyScrollview(Context context, IAttributeSet attrs, int defStyleAttr)
                : base(context, attrs, defStyleAttr)
        {

            mTouchSlop = ViewConfiguration.Get(context).ScaledTouchSlop;
        }

        public override bool OnInterceptTouchEvent(MotionEvent e)
        {
            MotionEventActions action = e.Action;
            switch (action)
            {
                case MotionEventActions.Down:
                    downX = (int)e.RawX;
                    downY = (int)e.RawY;
                    break;
                case MotionEventActions.Move:
                    int moveY = (int)e.RawY;
                    if (Math.Abs(moveY - downY) > mTouchSlop)
                    {
                        return true;
                    }
                    break;
            }
            return base.OnInterceptTouchEvent(e);
        }
    }
}