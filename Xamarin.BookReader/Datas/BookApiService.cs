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
using System.Threading.Tasks;
using Xamarin.BookReader.Models;
using Refit;
using Xamarin.BookReader.Models.User;

namespace Xamarin.BookReader.Datas
{
    [Preserve]
    public interface BookApiService
    {
        [Get("/book/recommend")]
        Task<Recommend> getRecomend(string gender);

        /// <summary>
        /// 获取正版源(若有) 与 盗版源
        /// </summary>
        /// <param name="view"></param>
        /// <param name="book"></param>
        /// <returns></returns>
        [Get("/atoc")]
        Task<List<BookSource>> getABookSource(string view, string book);

        [Get("/btoc")]
        Task<List<BookSource>> getBBookSource(string view, string book);

        [Get("/mix-atoc/{bookId}")]
        Task<BookMixAToc> getBookMixAToc(string bookId, string view);

        [Get("/mix-toc/{bookId}")]
        Task<BookRead> getBookRead(string bookId);

        [Get("/btoc/{bookId}")]
        Task<BookMixAToc> getBookBToc(string bookId, string view);
        
        [Get("/post/post-count-by-book")]
        Task<PostCount> postCountByBook(string bookId);

        [Get("/post/total-count")]
        Task<PostCount> postTotalCount(string bookId);

        [Get("/book/hot-word")]
        Task<HotWord> getHotWord();

        /**
         * 关键字自动补全
         *
         * @param query
         * @return
         */
        [Get("/book/auto-complete")]
        Task<AutoComplete> autoComplete(string query);

        /**
         * 书籍查询
         *
         * @param query
         * @return
         */
        [Get("/book/fuzzy-search")]
        Task<SearchDetail> searchBooks(string query);

        /**
         * 通过作者查询书名
         *
         * @param author
         * @return
         */
        [Get("/book/accurate-search")]
        Task<BooksByTag> searchBooksByAuthor(string author);

        /**
         * 热门评论
         *
         * @param book
         * @return
         */
        [Get("/post/review/best-by-book")]
        Task<HotReview> getHotReview(string book);

        [Get("/book-list/{bookId}/recommend")]
        Task<RecommendBookList> getRecommendBookList(string bookId, string limit);

        [Get("/book/{bookId}")]
        Task<BookDetail> getBookDetail(string bookId);

        [Get("/book/by-tags")]
        Task<BooksByTag> getBooksByTag(string tags, string start, string limit);

        /**
         * 获取所有排行榜
         *
         * @return
         */
        [Get("/ranking/gender")]
        Task<RankingList> getRanking();

        /**
         * 获取单一排行榜
         * 周榜：rankingId->_id
         * 月榜：rankingId->monthRank
         * 总榜：rankingId->totalRank
         *
         * @return
         */
        [Get("/ranking/{rankingId}")]
        Task<Rankings> getRankingById(string rankingId);

        /**
         * 获取主题书单列表
         * 本周最热：duration=last-seven-days&sort=collectorCount
         * 最新发布：duration=all&sort=created
         * 最多收藏：duration=all&sort=collectorCount
         *
         * @param tag    都市、古代、架空、重生、玄幻、网游
         * @param gender male、female
         * @param limit  20
         * @return
         */
        [Get("/book-list")]
        Task<BookLists> getBookLists(string duration, string sort, string start, string limit, string tag, string gender);

        /**
         * 获取主题书单标签列表
         *
         * @return
         */
        [Get("/book-list/tagType")]
        Task<BookListTags> getBookListTags();

        /**
         * 获取书单详情
         *
         * @return
         */
        [Get("/book-list/{bookListId}")]
        Task<BookListDetail> getBookListDetail(string bookListId);

        /**
         * 获取分类
         *
         * @return
         */
        [Get("/cats/lv2/statistics")]
        Task<CategoryList> getCategoryList();

        /**
         * 获取二级分类
         *
         * @return
         */
        [Get("/cats/lv2")]
        Task<CategoryListLv2> getCategoryListLv2();

        /**
         * 按分类获取书籍列表
         *
         * @param gender male、female
         * @param type   hot(热门)、new(新书)、reputation(好评)、over(完结)
         * @param major  玄幻
         * @param minor  东方玄幻、异界大陆、异界争霸、远古神话
         * @param limit  50
         * @return
         */
        [Get("/book/by-categories")]
        Task<BooksByCats> getBooksByCats(string gender, string type, string major, string minor, int start, int limit);


        /**
         * 获取综合讨论区帖子列表
         * 全部、默认排序  http://api.zhuishushenqi.com/post/by-block?block=ramble&duration=all&sort=updated&type=all&start=0&limit=20&distillate=
         * 精品、默认排序  http://api.zhuishushenqi.com/post/by-block?block=ramble&duration=all&sort=updated&type=all&start=0&limit=20&distillate=true
         *
         * @param block      ramble:综合讨论区
         *                   original：原创区
         * @param duration   all
         * @param sort       updated(默认排序)
         *                   created(最新发布)
         *                   comment-count(最多评论)
         * @param type       all
         * @param start      0
         * @param limit      20
         * @param distillate true(精品)
         * @return
         */
        [Get("/post/by-block")]
        Task<DiscussionList> getBookDisscussionList(string block, string duration, string sort, string type, string start, string limit, string distillate);

        /**
         * 获取综合讨论区帖子详情
         *
         * @param disscussionId->_id
         * @return
         */
        [Get("/post/{disscussionId}")]
        Task<Disscussion> getBookDisscussionDetail(string disscussionId);

        /**
         * 获取神评论列表(综合讨论区、书评区、书荒区皆为同一接口)
         *
         * @param disscussionId->_id
         * @return
         */
        [Get("/post/{disscussionId}/comment/best")]
        Task<CommentList> getBestComments(string disscussionId);

        /**
         * 获取综合讨论区帖子详情内的评论列表
         *
         * @param disscussionId->_id
         * @param start              0
         * @param limit              30
         * @return
         */
        [Get("/post/{disscussionId}/comment")]
        Task<CommentList> getBookDisscussionComments(string disscussionId, string start, string limit);

        /**
         * 获取书评区帖子列表
         * 全部、全部类型、默认排序  http://api.zhuishushenqi.com/post/review?duration=all&sort=updated&type=all&start=0&limit=20&distillate=
         * 精品、玄幻奇幻、默认排序  http://api.zhuishushenqi.com/post/review?duration=all&sort=updated&type=xhqh&start=0&limit=20&distillate=true
         *
         * @param duration   all
         * @param sort       updated(默认排序)
         *                   created(最新发布)
         *                   helpful(最有用的)
         *                   comment-count(最多评论)
         * @param type       all(全部类型)、xhqh(玄幻奇幻)、dsyn(都市异能)...
         * @param start      0
         * @param limit      20
         * @param distillate true(精品) 、空字符（全部）
         * @return
         */
        [Get("/post/review")]
        Task<BookReviewList> getBookReviewList(string duration, string sort, string type, string start, string limit, string distillate);

        /**
         * 获取书评区帖子详情
         *
         * @param bookReviewId->_id
         * @return
         */
        [Get("/post/review/{bookReviewId}")]
        Task<BookReview> getBookReviewDetail(string bookReviewId);

        /**
         * 获取书评区、书荒区帖子详情内的评论列表
         *
         * @param bookReviewId->_id
         * @param start             0
         * @param limit             30
         * @return
         */
        [Get("/post/review/{bookReviewId}/comment")]
        Task<CommentList> getBookReviewComments(string bookReviewId, string start, string limit);

        /**
         * 获取书荒区帖子列表
         * 全部、默认排序  http://api.zhuishushenqi.com/post/help?duration=all&sort=updated&start=0&limit=20&distillate=
         * 精品、默认排序  http://api.zhuishushenqi.com/post/help?duration=all&sort=updated&start=0&limit=20&distillate=true
         *
         * @param duration   all
         * @param sort       updated(默认排序)
         *                   created(最新发布)
         *                   comment-count(最多评论)
         * @param start      0
         * @param limit      20
         * @param distillate true(精品) 、空字符（全部）
         * @return
         */
        [Get("/post/help")]
        Task<BookHelpList> getBookHelpList(string duration, string sort, string start, string limit, string distillate);

        /**
         * 获取书荒区帖子详情
         *
         * @param helpId->_id
         * @return
         */
        [Get("/post/help/{helpId}")]
        Task<BookHelp> getBookHelpDetail(string helpId);

        /**
         * 第三方登陆
         *
         * @param platform_uid
         * @param platform_token
         * @param platform_code  “QQ”
         * @return
         */
        [Post("/user/login")]
        Task<Login> login([Body]LoginReq loginReq);

        [Get("/user/followings/{userid}")]
        Task<Following> getFollowings(string userId);

        /**
         * 获取书籍详情讨论列表
         *
         * @param book  bookId
         * @param sort  updated(默认排序)
         *              created(最新发布)
         *              comment-count(最多评论)
         * @param type  normal
         *              vote
         * @param start 0
         * @param limit 20
         * @return
         */
        [Get("/post/by-book")]
        Task<DiscussionList> getBookDetailDisscussionList(string book, string sort, string type, string start, string limit);

        /**
         * 获取书籍详情书评列表
         *
         * @param book  bookId
         * @param sort  updated(默认排序)
         *              created(最新发布)
         *              helpful(最有用的)
         *              comment-count(最多评论)
         * @param start 0
         * @param limit 20
         * @return
         */
        [Get("/post/review/by-book")]
        Task<HotReview> getBookDetailReviewList(string book, string sort, string start, string limit);

        [Get("/post/original")]
        Task<DiscussionList> getBookOriginalList(string block, string duration, string sort, string type, string start, string limit, string distillate);
    }
}