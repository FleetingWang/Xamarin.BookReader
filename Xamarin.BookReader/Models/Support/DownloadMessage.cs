using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.BookReader.Models.Support
{
    public class DownloadMessage
    {
        public String bookId;

        public String message;

        public bool isComplete = false;

        public DownloadMessage(String bookId, String message, bool isComplete)
        {
            this.bookId = bookId;
            this.message = message;
            this.isComplete = isComplete;
        }
    }
}
