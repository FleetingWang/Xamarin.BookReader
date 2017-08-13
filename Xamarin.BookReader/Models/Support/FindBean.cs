using DSoft.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.BookReader.Models.Support
{
    public class FindBean : MessageBusEvent
    {
        public String Title { get; set; }
        public int IconResId { get; set; }

        public override string EventId => "FindBean";
        public FindBean() { }
        public FindBean(String title, int iconResId)
        {
            Title = title;
            IconResId = iconResId;
        }
    }
}
