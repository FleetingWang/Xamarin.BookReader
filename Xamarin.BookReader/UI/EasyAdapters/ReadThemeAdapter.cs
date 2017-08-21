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
using EasyAdapterLibrary.AbsListViews;
using Xamarin.BookReader.Models.Support;
using Xamarin.BookReader.Managers;
using Xamarin.BookReader.Utils;

namespace Xamarin.BookReader.UI.EasyAdapters
{
    public class ReadThemeAdapter : EasyLVAdapter<ReadTheme>
    {

        private int selected = 0;

        public ReadThemeAdapter(Context context, List<ReadTheme> list, int selected)
            : base(context, list, Resource.Layout.item_read_theme)
        {
            this.selected = selected;
        }

        public override void convert(EasyLVHolder holder, int position, ReadTheme readTheme)
        {
            if (readTheme != null)
            {
                ThemeManager.setReaderTheme(readTheme.theme, holder.getView<View>(Resource.Id.ivThemeBg));
                if (selected == position)
                {
                    holder.setVisible(Resource.Id.ivSelected, true);
                }
                else
                {
                    holder.setVisible(Resource.Id.ivSelected, false);
                }
            }
        }

        public void select(int position)
        {
            selected = position;
            LogUtils.i("curtheme=" + selected);
            NotifyDataSetChanged();
        }
    }
}