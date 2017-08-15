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
using Xamarin.BookReader.Models;
using System.Threading.Tasks;
using Refit;

namespace Xamarin.BookReader.Datas
{
    public interface ChapterReadApiService
    {
        [Get("/chapter/{url}")]
        Task<ChapterRead> getChapterRead(string url);
    }
}