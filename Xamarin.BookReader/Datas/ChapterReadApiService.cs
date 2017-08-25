using Android.Runtime;
using Refit;
using System.Threading.Tasks;
using Xamarin.BookReader.Models;

namespace Xamarin.BookReader.Datas
{
    [Preserve]
    public interface ChapterReadApiService
    {
        [Get("/chapter/{url}")]
        Task<ChapterRead> getChapterRead(string url);
    }
}