using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide.Load.Engine.Bitmap_recycle;
using Android.Content.Res;

namespace Com.Bumptech.Glide.Load.Resource.Bitmap
{
    public class GlideRoundTransform : BitmapTransformation
    {
        private static float radius = 0f;
        public GlideRoundTransform(Context context)
            :this(context, 4)
        {

        }

        public GlideRoundTransform(Context context, int dp)
            :base(context)
        {
            radius = Resources.System.DisplayMetrics.Density * dp;
        }

        public override string Id => Class.Name + Math.Round(radius);

        protected override Android.Graphics.Bitmap Transform(IBitmapPool pool, Android.Graphics.Bitmap toTransform, int outWidth, int outHeight)
        {
            return roundCrop(pool, toTransform);
        }

        private static Android.Graphics.Bitmap roundCrop(IBitmapPool pool, Android.Graphics.Bitmap source)
        {
            if (source == null) return null;
            Android.Graphics.Bitmap result = pool.Get(source.Width, source.Height, Android.Graphics.Bitmap.Config.Argb8888);
            if (result == null)
            {
                result = Android.Graphics.Bitmap.CreateBitmap(source.Width, source.Height, Android.Graphics.Bitmap.Config.Argb8888);
            }
            Canvas canvas = new Canvas(result);
            Paint paint = new Paint();
            paint.SetShader(new BitmapShader(source, BitmapShader.TileMode.Clamp, BitmapShader.TileMode.Clamp));
            paint.AntiAlias = true;
            RectF rectF = new RectF(0f, 0f, source.Width, source.Height);
            canvas.DrawRoundRect(rectF, radius, radius, paint);
            return result;
        }
    }
}