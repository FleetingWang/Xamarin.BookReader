using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.BookReader.Models
{
    /// <summary>
    /// 推荐书单
    /// </summary>
    public class RecommendBookList
    {
        /**
         * id : 5617c5f3e8a2065627e4cb85
         * title : 此单在手，书荒不再有！
         * author : 选择
         * desc : 应有尽有！注：随时有可能添加新书！
         * bookCount : 498
         * cover : /agent/http://image.cmfu.com/books/3582111/3582111.jpg
         * collectorCount : 3925
         */

        public List<RecommendBook> booklists;

        public class RecommendBook
        {
            public String id;
            public String title;
            public String author;
            public String desc;
            public int bookCount;
            public String cover;
            public int collectorCount;
        }
    }
}
