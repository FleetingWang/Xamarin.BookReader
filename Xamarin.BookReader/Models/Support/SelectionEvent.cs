using DSoft.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.BookReader.Bases;

namespace Xamarin.BookReader.Models.Support
{
    public class SelectionEvent: MessageBusEvent
    {
        private Constant.Distillate distillate;
        private Constant.BookType type;
        private Constant.SortType sort;

        public SelectionEvent() { }
        public SelectionEvent(Constant.Distillate distillate, Constant.BookType type, Constant.SortType sort)
        {
            this.distillate = distillate;
            this.type = type;
            this.sort = sort;
        }

        public override string EventId => "SelectionEvent";
    }
}
