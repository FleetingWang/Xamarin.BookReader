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
using Com.Bumptech.Glide.Load.Resource.Bitmap;

namespace Com.Bumptech.Glide.Load.Resource.Bitmap
{
    public class GlideCircleTransform : BitmapTransformation
    {
        public GlideCircleTransform(Context context)
            :base(context)
        {

        }

        public override string Id => throw new NotImplementedException();

        protected override Android.Graphics.Bitmap Transform(IBitmapPool pool, Android.Graphics.Bitmap toTransform, int outWidth, int outHeight)
        {
            return circleCrop(pool, toTransform);
        }

        private static Android.Graphics.Bitmap circleCrop(IBitmapPool pool, Android.Graphics.Bitmap source)
        {
            if (source == null)
                return null;
            int size = Math.Min(source.Width, source.Height);
            int x = (source.Width - size) / 2;
            int y = (source.Height - size) / 2;
            // TODO this could be acquired from the pool too
            Android.Graphics.Bitmap squared = Android.Graphics.Bitmap.CreateBitmap(source, x, y, size, size);
            Android.Graphics.Bitmap result = pool.Get(size, size, Android.Graphics.Bitmap.Config.Argb8888);
            if (result == null)
            {
                result = Android.Graphics.Bitmap.CreateBitmap(size, size, Android.Graphics.Bitmap.Config.Argb8888);
            }
            Canvas canvas = new Canvas(result);
            Paint paint = new Paint();
            paint.SetShader(new BitmapShader(squared, BitmapShader.TileMode.Clamp,
                    BitmapShader.TileMode.Clamp));
            paint.AntiAlias = true;
            float r = size / 2f;
            canvas.DrawCircle(r, r, r, paint);
            return result;
        }
    }
}