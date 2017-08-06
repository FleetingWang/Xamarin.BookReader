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
using Xamarin.BookReader.Bases;

namespace Xamarin.BookReader.Views
{
    public class TagColor
    {
        public int borderColor = Color.ParseColor("#49C120");
        public int backgroundColor = Color.ParseColor("#49C120");
        public int textColor = Color.White;

        public static List<TagColor> getRandomColors(int size)
        {
            List<TagColor> list = new List<TagColor>();
            for (int i = 0; i < size; i++)
            {
                TagColor color = new TagColor();
                color.borderColor = color.backgroundColor = Constant.tagColors[i % Constant.tagColors.Length];
                list.Add(color);
            }
            return list;
        }
    }
}