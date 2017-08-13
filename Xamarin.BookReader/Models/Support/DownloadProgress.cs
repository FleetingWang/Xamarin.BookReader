using DSoft.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.BookReader.Models.Support
{
    public class DownloadProgress : MessageBusEvent
    {
        public String bookId;

        public String message;

        public bool isAlreadyDownload = false;

        public DownloadProgress() { }

        public DownloadProgress(String bookId, String message, bool isAlreadyDownload)
        {
            this.bookId = bookId;
            this.message = message;
            this.isAlreadyDownload = isAlreadyDownload;
        }

        public override string EventId => "DownloadProgress";
    }
}
