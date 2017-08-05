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

namespace Xamarin.BookReader.Services
{
    [Service]
    public class DownloadBookService : Service
    {
        //public static List<DownloadQueue> downloadQueues = new ArrayList<>();

        //public BookApi bookApi;
        //protected CompositeSubscription mCompositeSubscription;

        public bool isBusy = false; // 当前是否有下载任务在进行

        public static bool canceled = false;

        // TODO: DownloadBookService
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public static void cancel()
        {
            canceled = true;
        }
    }
}