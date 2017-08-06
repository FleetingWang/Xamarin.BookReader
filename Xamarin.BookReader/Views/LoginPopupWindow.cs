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
using Android.Views.Animations;

namespace Xamarin.BookReader.Views
{
    public class LoginPopupWindow : PopupWindow, View.IOnTouchListener
    {
        private View mContentView;
        private Activity mActivity;

        private ImageView qq;
        private ImageView weibo;
        private ImageView wechat;

        LoginTypeListener listener;

        public LoginPopupWindow(Activity activity)
        {
            mActivity = activity;
            Width = (ViewGroup.LayoutParams.MatchParent);
            Height = (ViewGroup.LayoutParams.WrapContent);

            mContentView = LayoutInflater.From(activity).Inflate(Resource.Layout.layout_login_popup_window, null);
            ContentView = mContentView;

            qq = (ImageView)mContentView.FindViewById(Resource.Id.ivQQ);
            weibo = (ImageView)mContentView.FindViewById(Resource.Id.ivWeibo);
            wechat = (ImageView)mContentView.FindViewById(Resource.Id.ivWechat);

            qq.SetOnTouchListener(this);
            weibo.SetOnTouchListener(this);
            wechat.SetOnTouchListener(this);

            Focusable = (true);
            OutsideTouchable = (true);
            SetBackgroundDrawable(new ColorDrawable(Color.ParseColor("#00000000")));

            AnimationStyle = Resource.Style.LoginPopup;

            DismissEvent += (sender, e) => { lighton(); };
        }

        private void scale(View v, bool isDown)
        {
            if (v.Id == qq.Id || v.Id == weibo.Id || v.Id == wechat.Id)
            {
                if (isDown)
                {
                    Animation testAnim = AnimationUtils.LoadAnimation(mActivity, Resource.Animation.scale_down);
                    v.StartAnimation(testAnim);
                }
                else
                {
                    Animation testAnim = AnimationUtils.LoadAnimation(mActivity, Resource.Animation.scale_up);
                    v.StartAnimation(testAnim);
                }
            }
            if (!isDown && listener != null)
            {
                switch (v.Id)
                {
                    case Resource.Id.ivQQ:
                        listener.onLogin(qq, "QQ");
                        break;
                }

                qq.PostDelayed(() => Dismiss(), 500);

            }
        }

        private void lighton()
        {
            WindowManagerLayoutParams lp = mActivity.Window.Attributes;
            lp.Alpha = 1.0f;
            mActivity.Window.Attributes = lp;
        }

        private void lightoff()
        {
            WindowManagerLayoutParams lp = mActivity.Window.Attributes;
            lp.Alpha = 0.3f;
            mActivity.Window.Attributes = lp;
        }

        public override void ShowAsDropDown(View anchor, int xoff, int yoff)
        {
            lightoff();
            base.ShowAsDropDown(anchor, xoff, yoff);
        }

        public override void ShowAtLocation(View parent, GravityFlags gravity, int x, int y)
        {
            lightoff();
            base.ShowAtLocation(parent, gravity, x, y);
        }


        public bool OnTouch(View v, MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    scale(v, true);
                    break;
                case MotionEventActions.Up:
                    scale(v, false);
                    break;
            }
            return false;
        }

        public interface LoginTypeListener
        {

            void onLogin(ImageView view, String type);
        }

        public void setLoginTypeListener(LoginTypeListener listener)
        {
            this.listener = listener;
        }
    }
}