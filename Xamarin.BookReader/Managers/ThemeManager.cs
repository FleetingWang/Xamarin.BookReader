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
                    view.setBackgroundResource(R.drawable.theme_white_bg);
                    break;
                case YELLOW:
                    view.setBackgroundResource(R.drawable.theme_yellow_bg);
                    break;
                case GREEN:
                    view.setBackgroundResource(R.drawable.theme_green_bg);
                    break;
                case LEATHER:
                    view.setBackgroundResource(R.drawable.theme_leather_bg);
                    break;
                case GRAY:
                    view.setBackgroundResource(R.drawable.theme_gray_bg);
                    break;
                case NIGHT:
                    view.setBackgroundResource(R.drawable.theme_night_bg);
                    break;
                default:
                    break;
            }
        }

        public static Bitmap getThemeDrawable(int theme)
        {
            Bitmap bmp = Bitmap.createBitmap(ScreenUtils.getScreenWidth(), ScreenUtils.getScreenHeight(), Bitmap.Config.ARGB_8888);
            switch (theme)
            {
                case NORMAL:
                    bmp.eraseColor(ContextCompat.getColor(AppUtils.getAppContext(), R.color.read_theme_white));
                    break;
                case YELLOW:
                    bmp.eraseColor(ContextCompat.getColor(AppUtils.getAppContext(), R.color.read_theme_yellow));
                    break;
                case GREEN:
                    bmp.eraseColor(ContextCompat.getColor(AppUtils.getAppContext(), R.color.read_theme_green));
                    break;
                case LEATHER:
                    bmp = BitmapFactory.decodeResource(AppUtils.getAppContext().getResources(), R.drawable.theme_leather_bg);
                    break;
                case GRAY:
                    bmp.eraseColor(ContextCompat.getColor(AppUtils.getAppContext(), R.color.read_theme_gray));
                    break;
                case NIGHT:
                    bmp.eraseColor(ContextCompat.getColor(AppUtils.getAppContext(), R.color.read_theme_night));
                    break;
                default:
                    break;
            }
            return bmp;
        }

        public static List<ReadTheme> getReaderThemeData(int curTheme)
        {
            int[] themes = { NORMAL, YELLOW, GREEN, LEATHER, GRAY, NIGHT };
            List<ReadTheme> list = new ArrayList<>();
            ReadTheme theme;
            for (int i = 0; i < themes.length; i++)
            {
                theme = new ReadTheme();
                theme.theme = themes[i];
                list.add(theme);
            }
            return list;
        }
    }
}