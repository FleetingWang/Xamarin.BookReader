using DSoft.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.BookReader.Models.Support
{
    /// <summary>
    /// 下载队列实体
    /// </summary>
    public class DownloadQueue : MessageBusEvent
    {
        public String bookId;

        public List<BookMixAToc.MixToc.Chapters> list;

        public int start;

        public int end;

        /**
         * 是否已经开始下载
         */
        public bool isStartDownload = false;

        /**
         * 是否中断下载
         */
        public bool isCancel = false;

        /**
         * 是否下载完成
         */
        public bool isFinish = false;

        public override string EventId => "DownloadQueue";

        public DownloadQueue(String bookId, List<BookMixAToc.MixToc.Chapters> list, int start, int end)
        {
            this.bookId = bookId;
            this.list = list;
            this.start = start;
            this.end = end;
        }

        /**
         * 空事件。表示通知继续执行下一条任务
         */
        public DownloadQueue()
        {
        }
    }
}
