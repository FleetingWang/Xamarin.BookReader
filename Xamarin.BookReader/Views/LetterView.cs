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
using Android.Text;
using Android.Util;
using static Android.Graphics.Paint;

namespace Xamarin.BookReader.Views
{
    public class LetterView : TextView
    {
        // 颜色画板集
        private static int[] colors = {
            0x1abc9c, 0x16a085, 0xf1c40f, 0xf39c12, 0x2ecc71,
            0x27ae60, 0xe67e22, 0xd35400, 0x3498db, 0x2980b9,
            0xe74c3c, 0xc0392b, 0x9b59b6, 0x8e44ad, 0xbdc3c7,
            0x34495e, 0x2c3e50, 0x95a5a6, 0x7f8c8d, 0xec87bf,
            0xd870ad, 0xf69785, 0x9ba37e, 0xb49255, 0xb49255
    };


        private Paint mPaintBackground;
        private Paint mPaintText;
        private Rect mRect;

        private String text;

        private int charHash;

        public LetterView(Context context) : this(context, null)
        {

        }

        public LetterView(Context context, IAttributeSet attrs) : this(context, attrs, 0)
        {

        }

        public LetterView(Context context, IAttributeSet attrs, int defStyleAttr)
                : base(context, attrs, defStyleAttr)
        {
            init();

            setText(Text);
        }

        private void init()
        {
            mPaintText = new Paint()
            {
                Flags = PaintFlags.AntiAlias,
                Color = Color.White
            };
            mPaintBackground = new Paint()
            {
                Flags = PaintFlags.AntiAlias,
            };

            mRect = new Rect();
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, widthMeasureSpec); // 宽高相同
        }

        protected override void OnDraw(Canvas canvas)
        {
            if (null != text)
            {
                // 画圆
                canvas.DrawCircle(Width / 2, Width / 2, Width / 2, mPaintBackground);
                // 写字
                mPaintText.TextSize = (Width / 2);
                mPaintText.StrokeWidth = (3);
                mPaintText.GetTextBounds(text, 0, 1, mRect);
                // 垂直居中
                Paint.FontMetricsInt fontMetrics = mPaintText.GetFontMetricsInt();
                int baseline = (MeasuredHeight - fontMetrics.Bottom - fontMetrics.Top) / 2;
                // 左右居中
                mPaintText.TextAlign = Align.Center;
                canvas.DrawText(text, Width / 2, baseline, mPaintText);
            }
        }

        public void setText(String content)
        {
            if (TextUtils.IsEmpty(content))
            {
                content = " ";
            }
            this.text = content.ToCharArray()[0].ToString();
            this.text = text.ToUpper();
            charHash = this.text.GetHashCode();
            int color = colors[charHash % colors.Length];
            mPaintBackground.Color = new Color(color);
            Invalidate();
        }
    }
}