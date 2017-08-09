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
using Xamarin.BookReader.Utils;
using Android.Support.V4.Content;
using Xamarin.BookReader.Models.Support;

namespace Xamarin.BookReader.Managers
{
    public class ThemeManager
    {
        public const int NORMAL = 0;
        public const int YELLOW = 1;
        public const int GREEN = 2;
        public const int LEATHER = 3;
        public const int GRAY = 4;
        public const int NIGHT = 5;

        public static void setReaderTheme(int theme, View view)
        {
            switch (theme)
            {
                case NORMAL:
                    view.SetBackgroundResource(Resource.Drawable.theme_white_bg);
                    break;
                case YELLOW:
                    view.SetBackgroundResource(Resource.Drawable.theme_yellow_bg);
                    break;
                case GREEN:
                    view.SetBackgroundResource(Resource.Drawable.theme_green_bg);
                    break;
                case LEATHER:
                    view.SetBackgroundResource(Resource.Drawable.theme_leather_bg);
                    break;
                case GRAY:
                    view.SetBackgroundResource(Resource.Drawable.theme_gray_bg);
                    break;
                case NIGHT:
                    view.SetBackgroundResource(Resource.Drawable.theme_night_bg);
                    break;
                default:
                    break;
            }
        }

        public static Bitmap getThemeDrawable(int theme)
        {
            Bitmap bmp = Bitmap.CreateBitmap(ScreenUtils.getScreenWidth(), ScreenUtils.getScreenHeight(), Bitmap.Config.Argb8888);
            switch (theme)
            {
                case NORMAL:
                    bmp.EraseColor(ContextCompat.GetColor(AppUtils.getAppContext(), Resource.Color.read_theme_white));
                    break;
                case YELLOW:
                    bmp.EraseColor(ContextCompat.GetColor(AppUtils.getAppContext(), Resource.Color.read_theme_yellow));
                    break;
                case GREEN:
                    bmp.EraseColor(ContextCompat.GetColor(AppUtils.getAppContext(), Resource.Color.read_theme_green));
                    break;
                case LEATHER:
                    bmp = BitmapFactory.DecodeResource(AppUtils.getAppContext().Resources, Resource.Drawable.theme_leather_bg);
                    break;
                case GRAY:
                    bmp.EraseColor(ContextCompat.GetColor(AppUtils.getAppContext(), Resource.Color.read_theme_gray));
                    break;
                case NIGHT:
                    bmp.EraseColor(ContextCompat.GetColor(AppUtils.getAppContext(), Resource.Color.read_theme_night));
                    break;
                default:
                    break;
            }
            return bmp;
        }

        public static List<ReadTheme> getReaderThemeData(int curTheme)
        {
            int[] themes = { NORMAL, YELLOW, GREEN, LEATHER, GRAY, NIGHT };
            List<ReadTheme> list = new List<ReadTheme>();
            ReadTheme theme;
            for (int i = 0; i < themes.Length; i++)
            {
                theme = new ReadTheme();
                theme.theme = themes[i];
                list.Add(theme);
            }
            return list;
        }
    }
}