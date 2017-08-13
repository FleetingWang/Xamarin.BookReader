using DSoft.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.BookReader.Models.Support
{
    public class RefreshCollectionListEvent : MessageBusEvent
    {
        public override string EventId => "RefreshCollectionListEvent";
    }
}
