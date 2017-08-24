using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Media;
using Android.Util;
using Java.Util;
using Android.Text;
using Java.Lang;

namespace Xamarin.BookReader.Views
{
    /// <summary>
    /// 打字效果TextView
    /// </summary>
    [Register("xamarin.bookreader.views.TypeTextView")]
    public class TypeTextView : TextView
    {
        private Context mContext = null;
        private MediaPlayer mMediaPlayer = null;
        private string mShowTextString = null;
        private Timer mTypeTimer = null;
        private OnTypeViewListener mOnTypeViewListener = null;
        private static int TYPE_TIME_DELAY = 80;
        private int mTypeTimeDelay = TYPE_TIME_DELAY; // 打字间隔

        public TypeTextView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            initTypeTextView(context);
        }

        public TypeTextView(Context context, IAttributeSet attrs) : base(context, attrs)
        {

            initTypeTextView(context);
        }

        public TypeTextView(Context context) : base(context)
        {

            initTypeTextView(context);
        }

        public void setOnTypeViewListener(OnTypeViewListener onTypeViewListener)
        {
            mOnTypeViewListener = onTypeViewListener;
        }

        public void start(string textString)
        {
            start(textString, TYPE_TIME_DELAY);
        }

        public void start(string textString, int typeTimeDelay)
        {
            if (TextUtils.IsEmpty(textString) || typeTimeDelay < 0)
            {
                return;
            }
            Post(() =>
            {
                mShowTextString = textString;
                mTypeTimeDelay = typeTimeDelay;
                Text = "";
                startTypeTimer();
                if (null != mOnTypeViewListener)
                {
                    mOnTypeViewListener.onTypeStart();
                }
            });
        }

        public void stop()
        {
            stopTypeTimer();
            stopAudio();
        }

        private void initTypeTextView(Context context)
        {
            mContext = context;
        }

        private void startTypeTimer()
        {
            stopTypeTimer();
            mTypeTimer = new Timer();
            mTypeTimer.Schedule(new TypeTimerTask(this), mTypeTimeDelay);
        }

        private void stopTypeTimer()
        {
            if (null != mTypeTimer)
            {
                mTypeTimer.Cancel();
                mTypeTimer = null;
            }
        }

        private void startAudioPlayer()
        {
            stopAudio();
            //TO DO playAudio(R.raw.type_in);
        }

        private void playAudio(int audioResId)
        {
            try
            {
                stopAudio();
                mMediaPlayer = MediaPlayer.Create(mContext, audioResId);
                mMediaPlayer.Start();
            }
            catch (Exception e)
            {
                e.PrintStackTrace();
            }
        }

        private void stopAudio()
        {
            if (mMediaPlayer != null && mMediaPlayer.IsPlaying)
            {
                mMediaPlayer.Stop();
                mMediaPlayer.Release();
                mMediaPlayer = null;
            }
        }

        class TypeTimerTask : TimerTask
        {
            private TypeTextView typeTextView;

            public TypeTimerTask(TypeTextView typeTextView)
            {
                this.typeTextView = typeTextView;
            }

            public override void Run()
            {
                typeTextView.Post(() =>
                {
                    if (typeTextView.Text.Length < typeTextView.mShowTextString.Length)
                    {
                        typeTextView.Text = typeTextView.mShowTextString.Substring(0, typeTextView.Text.Length + 1);
                        typeTextView.startAudioPlayer();
                        typeTextView.startTypeTimer();
                    }
                    else
                    {
                        typeTextView.stopTypeTimer();
                        if (null != typeTextView.mOnTypeViewListener)
                        {
                            typeTextView.mOnTypeViewListener.onTypeOver();
                        }
                    }
                });
            }
        }

        public interface OnTypeViewListener
        {
            void onTypeStart();

            void onTypeOver();
        }
    }
}