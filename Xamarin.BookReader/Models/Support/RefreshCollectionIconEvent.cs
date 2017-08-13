using DSoft.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.BookReader.Models.Support
{
    public class RefreshCollectionIconEvent : MessageBusEvent
    {
        public override string EventId => "RefreshCollectionIconEvent";
    }
}
