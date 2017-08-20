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
    public class AutoCompleteAdapter: EasyLVAdapter<String>
    {
        public AutoCompleteAdapter(Context context, List<String> list)
            : base(context, list, Resource.Layout.item_auto_complete_list)
        {
            
        }

        public override void convert(EasyLVHolder holder, int position, String s)
        {
            holder.setText(Resource.Id.tvAutoCompleteItem, s);
        }
    }
}