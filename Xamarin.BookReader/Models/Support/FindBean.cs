using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.BookReader.Models.Support
{
    public class FindBean
    {
        public String Title { get; set; }
        public int IconResId { get; set; }

        public FindBean(String title, int iconResId)
        {
            Title = title;
            IconResId = iconResId;
        }
    }
}
