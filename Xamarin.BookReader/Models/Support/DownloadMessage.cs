using DSoft.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.BookReader.Models.Support
{
    public class DownloadMessage : MessageBusEvent
    {
        public String bookId;

        public String message;

        public bool isComplete = false;

        public DownloadMessage() { }

        public DownloadMessage(String bookId, String message, bool isComplete)
        {
            this.bookId = bookId;
            this.message = message;
            this.isComplete = isComplete;
        }

        public override string EventId => "DownloadMessage";
    }
}
