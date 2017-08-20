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

namespace Xamarin.BookReader.UI.Adapters
{
    public class SearchHistoryAdapter : EasyLVAdapter<String>
    {

        public SearchHistoryAdapter(Context context, List<String> list)
                : base(context, list, Resource.Layout.item_search_history)
        {

        }

        public override void convert(EasyLVHolder holder, int position, String s)
        {
            holder.setText(Resource.Id.tvTitle, s);
        }
    }
}