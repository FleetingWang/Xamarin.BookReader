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
using Android.Text;
using Xamarin.BookReader.Utils;
using Xamarin.BookReader.Models;
using System.Threading.Tasks;
using Java.Lang;

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
            if(evnt is DownloadQueue queue)
            {
                if (!TextUtils.IsEmpty(queue.bookId))
                {
                    bool exists = false;
                    // 判断当前书籍缓存任务是否存在
                    for (int i = 0; i < downloadQueues.Count(); i++)
                    {
                        if (downloadQueues[i].bookId.Equals(queue.bookId))
                        {
                            LogUtils.e("addToDownloadQueue:exists");
                            exists = true;
                            break;
                        }
                    }
                    if (exists)
                    {
                        Post(new DownloadMessage(queue.bookId, "当前缓存任务已存在", false));
                        return;
                    }

                    // 添加到下载队列
                    downloadQueues.Add(queue);
                    LogUtils.e("addToDownloadQueue:" + queue.bookId);
                    Post(new DownloadMessage(queue.bookId, "成功加入缓存队列", false));
                }
                // 从队列顺序取出第一条下载
                if (downloadQueues.Count() > 0 && !isBusy)
                {
                    isBusy = true;
                    downloadBook(downloadQueues[0]);
                }
            }
        }

        public /*synchronized*/ void downloadBook(DownloadQueue downloadQueue)
        {
            List<BookMixAToc.MixToc.Chapters> list = downloadQueue.list;
            string bookId = downloadQueue.bookId;
            int start = downloadQueue.start; // 起始章节
            int end = downloadQueue.end; // 结束章节
            int failureCount = 0;

            Task.Factory.StartNew(() =>
            {
                try
                {
                    Thread.Sleep(1000);

                }catch(InterruptedException e) { }
                for (int i = start; i <= end && i <= list.Count(); i++) {
                    if (canceled) {
                        break;
                    }
                    // 网络异常，取消下载
                    if (!NetworkUtils.isAvailable(AppUtils.getAppContext())) {
                        downloadQueue.isCancel = true;
                        Post(new DownloadMessage(bookId, GetString(Resource.String.book_read_download_error), true));
                        failureCount = -1;
                        break;
                    }
                    if (!downloadQueue.isFinish && !downloadQueue.isCancel) {
                        // 章节文件不存在,则下载，否则跳过
                        if (CacheManager.GetChapterFile(bookId, i) == null) {
                            BookMixAToc.MixToc.Chapters chapters = list[i - 1];
                            string url = chapters.link;
                            int ret = download(url, bookId, chapters.title, i, list.Count());
                            if (ret != 1) {
                                failureCount++;
                            }
                        } else {
                            Post(new DownloadProgress(bookId, Java.Lang.String.Format(
                                    GetString(Resource.String.book_read_alreday_download), list[i - 1].title, i, list.Count()),
                                    true));
                        }
                    }
                }
                try {
                    Thread.Sleep(500);
                } catch (InterruptedException e) {
                }
                return failureCount;


            }).ContinueWith(task => {
                downloadQueue.isFinish = true;
                if (failureCount > -1)
                {
                    // 完成通知
                    Post(new DownloadMessage(bookId,
                            Java.Lang.String.Format(GetString(Resource.String.book_read_download_complete), failureCount), true));
                }
                // 下载完成，从队列里移除
                downloadQueues.Remove(downloadQueue);
                // 释放 空闲状态
                isBusy = false;
                if (!canceled)
                {
                    // post一个空事件，通知继续执行下一个任务
                    Post(new DownloadQueue());
                }
                else
                {
                    downloadQueues.Clear();
                }
                canceled = false;
                LogUtils.i(bookId + "缓存完成，失败" + failureCount + "章");
            });

        }

        private int download(string url, string bookId, string title, int chapter, int chapterSize)
        {
            int[] result = { -1 };
            bookApi.getChapterRead(url)
                //.subscribeOn(Schedulers.io())
                //.observeOn(AndroidSchedulers.mainThread())
                .Subscribe(data => {
                    if (data.chapter != null)
                    {
                        Post(new DownloadProgress(bookId, Java.Lang.String.Format(
                                GetString(Resource.String.book_read_download_progress), title, chapter, chapterSize),
                                true));
                        CacheManager.SaveChapterFile(bookId, chapter, data.chapter);
                        result[0] = 1;
                    }
                    else
                    {
                        result[0] = 0;
                    }
                }, e => {
                    result[0] = 0;
                }, () => {
                    result[0] = 1;
                });

            while (result[0] == -1)
            {
                try
                {
                    Thread.Sleep(350);
                }
                catch (InterruptedException e)
                {
                    e.PrintStackTrace();
                }
            }
            return result[0];
        }



        public static void cancel()
        {
            canceled = true;
        }
    }
}