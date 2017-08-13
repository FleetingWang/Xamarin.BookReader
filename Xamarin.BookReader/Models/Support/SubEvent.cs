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
using DSoft.Messaging;

namespace Xamarin.BookReader.Models.Support
{
    public class SubEvent: MessageBusEvent
    {
        public String minor;

        public String type;
        public SubEvent() { }
        public SubEvent(String minor, String type)
        {
            this.minor = minor;
            this.type = type;
        }

        public override string EventId => "SubEvent";
    }
}