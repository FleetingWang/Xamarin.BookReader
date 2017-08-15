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
using Xamarin.BookReader.Models;
using Akavache;
using Xamarin.BookReader.Models.User;
using System.Reactive.Threading.Tasks;

namespace Xamarin.BookReader.Datas
{
    public class BookApi
    {
        public static BookApi Instance { get; set; } = new BookApi();

        private BookApiService service;
        private ChapterReadApiService _chapterReadApiService;
        const string ChapterReadUrl = "http://chapter2.zhuishushenqi.com";
        private ChapterReadApiService ChapterReadApiService
        {
            get => _chapterReadApiService ?? (_chapterReadApiService = RestService.For<ChapterReadApiService>(ChapterReadUrl));
        }

        public BookApi()
        {
            service = RestService.For<BookApiService>(Constant.API_BASE_URL);
        }

        public IObservable<Recommend> getRecommend(string gender)
        {
            return BlobCache.LocalMachine.GetOrFetchObject($"getRecommend_{gender}", () => service.getRecomend(gender));
        }


        public IObservable<HotWord> getHotWord()
        {
            return BlobCache.LocalMachine.GetOrFetchObject("getHotWord", () =>
            service.getHotWord());
        }

        public IObservable<AutoComplete> getAutoComplete(String query)
        {
            return BlobCache.LocalMachine.GetOrFetchObject($"getAutoComplete_{query}", () =>
            service.autoComplete(query));
        }

        public IObservable<SearchDetail> getSearchResult(String query)
        {
            return BlobCache.LocalMachine.GetOrFetchObject($"getSearchResult_{query}", () =>
            service.searchBooks(query));
        }

        public IObservable<BooksByTag> searchBooksByAuthor(String author)
        {
            return BlobCache.LocalMachine.GetOrFetchObject($"searchBooksByAuthor_{author}", () =>
            service.searchBooksByAuthor(author));
        }

        public IObservable<BookDetail> getBookDetail(String bookId)
        {
            return BlobCache.LocalMachine.GetOrFetchObject($"getBookDetail_{bookId}", () =>
            service.getBookDetail(bookId));
        }

        public IObservable<HotReview> getHotReview(String book)
        {
            return BlobCache.LocalMachine.GetOrFetchObject($"getHotReview_{book}", () =>
            service.getHotReview(book));
        }

        public IObservable<RecommendBookList> getRecommendBookList(String bookId, String limit)
        {
            return BlobCache.LocalMachine.GetOrFetchObject($"getRecommendBookList_{bookId}_{limit}", () =>
            service.getRecommendBookList(bookId, limit));
        }

        public IObservable<BooksByTag> getBooksByTag(String tags, String start, String limit)
        {
            return BlobCache.LocalMachine.GetOrFetchObject($"getBooksByTag_{tags}_{start}_{limit}", () =>
            service.getBooksByTag(tags, start, limit));
        }

        public IObservable<BookMixAToc> getBookMixAToc(String bookId, String view)
        {
            return BlobCache.LocalMachine.GetOrFetchObject($"getBookMixAToc_{bookId}_{view}", () =>
            service.getBookMixAToc(bookId, view));
        }

        public /*synchronized*/ IObservable<ChapterRead> getChapterRead(String url)
        {
            return BlobCache.LocalMachine.GetOrFetchObject($"getChapterRead_{url}", () =>
                ChapterReadApiService.getChapterRead(url)
            );
        }

        public /*synchronized*/ IObservable<List<BookSource>> getBookSource(String view, String book)
        {
            return BlobCache.LocalMachine.GetOrFetchObject($"getBookSource_{view}_{book}", () =>
            service.getABookSource(view, book));
        }

        public IObservable<RankingList> getRanking()
        {
            return BlobCache.LocalMachine.GetOrFetchObject($"getRanking", () =>
            service.getRanking());
        }

        public IObservable<Rankings> getRanking(String rankingId)
        {
            return BlobCache.LocalMachine.GetOrFetchObject($"getRanking_{rankingId}", () =>
            service.getRankingById(rankingId));
        }

        public IObservable<BookLists> getBookLists(String duration, String sort, String start, String limit, String tag, String gender)
        {
            return BlobCache.LocalMachine.GetOrFetchObject($"getBookLists_{duration}_{sort}_{start}_{limit}_{tag}_{gender}", () =>
            service.getBookLists(duration, sort, start, limit, tag, gender));
        }

        public IObservable<BookListTags> getBookListTags()
        {
            return BlobCache.LocalMachine.GetOrFetchObject($"getBookListTags", () =>
            service.getBookListTags());
        }

        public IObservable<BookListDetail> getBookListDetail(String bookListId)
        {
            return BlobCache.LocalMachine.GetOrFetchObject($"getBookListDetail_{bookListId}", () =>
            service.getBookListDetail(bookListId));
        }

        public /*synchronized*/ IObservable<CategoryList> getCategoryList()
        {
            return BlobCache.LocalMachine.GetOrFetchObject($"getCategoryList", () =>
            service.getCategoryList());
        }

        public IObservable<CategoryListLv2> getCategoryListLv2()
        {
            return BlobCache.LocalMachine.GetOrFetchObject($"getCategoryListLv2", () =>
            service.getCategoryListLv2());
        }

        public IObservable<BooksByCats> getBooksByCats(String gender, String type, String major, String minor, int start, int limit)
        {
            return BlobCache.LocalMachine.GetOrFetchObject($"getBooksByCats_{gender}_{type}_{major}_{minor}_{start}_{limit}", () =>
            service.getBooksByCats(gender, type, major, minor, start, limit));
        }

        public IObservable<DiscussionList> getBookDisscussionList(String block, String duration, String sort, String type, String start, String limit, String distillate)
        {
            return BlobCache.LocalMachine.GetOrFetchObject($"getBookDisscussionList_{block}_{duration}_{sort}_{type}_{start}_{limit}_{distillate}", () =>
            service.getBookDisscussionList(block, duration, sort, type, start, limit, distillate));
        }

        public IObservable<Disscussion> getBookDisscussionDetail(String disscussionId)
        {
            return BlobCache.LocalMachine.GetOrFetchObject($"getBookDisscussionDetail_{disscussionId}", () =>
            service.getBookDisscussionDetail(disscussionId));
        }

        public IObservable<CommentList> getBestComments(String disscussionId)
        {
            return BlobCache.LocalMachine.GetOrFetchObject($"getBestComments_{disscussionId}", () =>
            service.getBestComments(disscussionId));
        }

        public IObservable<CommentList> getBookDisscussionComments(String disscussionId, String start, String limit)
        {
            return BlobCache.LocalMachine.GetOrFetchObject($"getBookDisscussionComments_{disscussionId}_{start}_{limit}", () =>
            service.getBookDisscussionComments(disscussionId, start, limit));
        }

        public IObservable<BookReviewList> getBookReviewList(String duration, String sort, String type, String start, String limit, String distillate)
        {
            return BlobCache.LocalMachine.GetOrFetchObject($"getBookReviewList_{duration}_{sort}_{type}_{start}_{limit}_{distillate}", () =>
            service.getBookReviewList(duration, sort, type, start, limit, distillate));
        }

        public IObservable<BookReview> getBookReviewDetail(String bookReviewId)
        {
            return BlobCache.LocalMachine.GetOrFetchObject($"getBookReviewDetail_{bookReviewId}", () =>
            service.getBookReviewDetail(bookReviewId));
        }

        public IObservable<CommentList> getBookReviewComments(String bookReviewId, String start, String limit)
        {
            return BlobCache.LocalMachine.GetOrFetchObject($"getBookReviewComments_{bookReviewId}_{start}_{limit}", () =>
            service.getBookReviewComments(bookReviewId, start, limit));
        }

        public IObservable<BookHelpList> getBookHelpList(String duration, String sort, String start, String limit, String distillate)
        {
            return BlobCache.LocalMachine.GetOrFetchObject($"getBookHelpList_{duration}_{sort}_{start}_{limit}_{distillate}", () =>
            service.getBookHelpList(duration, sort, start, limit, distillate));
        }

        public IObservable<BookHelp> getBookHelpDetail(String helpId)
        {
            return BlobCache.LocalMachine.GetOrFetchObject($"getBookHelpDetail_{helpId}", () =>
            service.getBookHelpDetail(helpId));
        }

        public IObservable<Login> login(String platform_uid, String platform_token, String platform_code)
        {
            LoginReq loginReq = new LoginReq();
            loginReq.platform_code = platform_code;
            loginReq.platform_token = platform_token;
            loginReq.platform_uid = platform_uid;

            return service.login(loginReq).ToObservable();
        }

        public IObservable<DiscussionList> getBookDetailDisscussionList(String book, String sort, String type, String start, String limit)
        {
            return BlobCache.LocalMachine.GetOrFetchObject($"getBookDetailDisscussionList_{book}_{sort}_{type}_{start}_{limit}", () =>
            service.getBookDetailDisscussionList(book, sort, type, start, limit));
        }

        public IObservable<HotReview> getBookDetailReviewList(String book, String sort, String start, String limit)
        {
            return BlobCache.LocalMachine.GetOrFetchObject($"getBookDetailReviewList_{book}_{sort}_{start}_{limit}", () =>
            service.getBookDetailReviewList(book, sort, start, limit));
        }

        public IObservable<DiscussionList> getGirlBookDisscussionList(String block, String duration, String sort, String type, String start, String limit, String distillate)
        {
            return BlobCache.LocalMachine.GetOrFetchObject($"getGirlBookDisscussionList_{block}_{duration}_{sort}_{type}_{start}_{limit}_{distillate}", () =>
            service.getBookDisscussionList(block, duration, sort, type, start, limit, distillate));
        }
    }
}