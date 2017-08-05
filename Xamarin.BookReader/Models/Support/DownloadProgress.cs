using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.BookReader.Models.Support
{
    public class DownloadProgress
    {
        public String bookId;

        public String message;

        public bool isAlreadyDownload = false;

        public DownloadProgress(String bookId, String message, bool isAlreadyDownload)
        {
            this.bookId = bookId;
            this.message = message;
            this.isAlreadyDownload = isAlreadyDownload;
        }
    }
}
