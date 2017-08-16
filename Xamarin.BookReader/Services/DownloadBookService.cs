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
using Xamarin.BookReader.Datas;
using Xamarin.BookReader.Models.Support;
using DSoft.Messaging;

namespace Xamarin.BookReader.Services
{
    [Service]
    public class DownloadBookService : Service
    {
        public static List<DownloadQueue> downloadQueues = new List<DownloadQueue>();

        public BookApi bookApi;
        //protected CompositeSubscription mCompositeSubscription;

        public bool isBusy = false; // 当前是否有下载任务在进行

        public static bool canceled = false;

        public override void OnCreate()
        {
            base.OnCreate();
            MessageBus.Default.Register<DownloadQueue>(addToDownloadQueue);
            // TODO: 日志
            bookApi = BookApi.Instance;
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            MessageBus.Default.DeRegister<DownloadQueue>(addToDownloadQueue);
        }

        public static void Post(DownloadQueue downloadQueue)
        {
            MessageBus.Default.Post(downloadQueue);
        }

        public void Post(DownloadProgress progress)
        {
            MessageBus.Default.Post(progress);
        }

        private void Post(DownloadMessage message)
        {
            MessageBus.Default.Post(message);
        }

        private void addToDownloadQueue(object sender, MessageBusEvent evnt)
        {
            // TODO: addToDownloadQueue
        }


        public static void cancel()
        {
            canceled = true;
        }
    }
}