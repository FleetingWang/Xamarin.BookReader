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
using Refit;
using Xamarin.BookReader.Bases;

namespace Xamarin.BookReader.Datas
{
    public class BookApi
    {
        public static BookApi Instance { get; set; } = new BookApi();

        private BookApiService service;

        public BookApi()
        {
            service = RestService.For<BookApiService>(Constant.API_BASE_URL);
        }


    }
}