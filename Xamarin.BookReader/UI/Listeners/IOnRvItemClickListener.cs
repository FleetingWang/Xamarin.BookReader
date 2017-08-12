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

namespace Xamarin.BookReader.UI.Listeners
{
    public interface IOnRvItemClickListener<T>
    {
        void onItemClick(View view, int position, T data);
    }
}