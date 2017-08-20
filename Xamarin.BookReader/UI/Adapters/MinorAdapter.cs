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
using Xamarin.BookReader.Utils;

namespace Xamarin.BookReader.UI.Adapters
{
    public class MinorAdapter : EasyLVAdapter<String>
    {
        private int current = 0;
        public MinorAdapter(Context context, List<String> list)
            : base(context, list, Resource.Layout.item_minor_list)
        {

        }

        public override void convert(EasyLVHolder holder, int position, string s)
        {
            holder.setText(Resource.Id.tvMinorItem, s);

            if (current == position)
            {
                holder.setVisible(Resource.Id.ivMinorChecked, true);
            }
            else
            {
                holder.setVisible(Resource.Id.ivMinorChecked, false);
            }

            if (position != 0)
            { // 子项右移
                TextView textView = holder.getView<TextView>(Resource.Id.tvMinorItem);
                RelativeLayout.LayoutParams layoutParams = (RelativeLayout.LayoutParams)textView.LayoutParameters;
                layoutParams.LeftMargin = ScreenUtils.dpToPxInt(25);
                textView.LayoutParameters = layoutParams;
            }
        }

        public void setChecked(int position)
        {
            current = position;
            NotifyDataSetChanged();
        }
    }
}