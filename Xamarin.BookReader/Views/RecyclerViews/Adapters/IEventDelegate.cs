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

namespace Xamarin.BookReader.Views.RecyclerViews.Adapters
{
    public interface IEventDelegate
    {
        void addData(int length);
        void clear();

        void stopLoadMore();
        void pauseLoadMore();
        void resumeLoadMore();

        void setMore(View view, IOnLoadMoreListener listener);
        void setNoMore(View view);
        void setErrorMore(View view);
    }
}