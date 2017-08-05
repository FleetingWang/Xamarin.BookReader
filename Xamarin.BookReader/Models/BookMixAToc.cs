using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.BookReader.Models
{
    public class BookMixAToc
    {
     /**
     * _id:577e528e2160421a02d7380d
     * name:优质书源
     * link:http://vip.zhuishushenqi.com/toc/577e528e2160421a02d7380d
     */
    public MixToc mixToc;
    public class MixToc {
        public String _id;
        public String book;
        public String chaptersUpdated;
        /**
         * title : 第一章 死在万花丛中
         * link : http://vip.zhuishushenqi.com/chapter/577e5290260289ff64a29213?cv=1467896464908
         * id : 577e5290260289ff64a29213
         * currency : 15
         * unreadble : false
         * isVip : false
         */

        public List<Chapters> chapters;

        public class Chapters {
            public String title;
            public String link;
            public String id;
            public int currency;
            public bool unreadble;
            public bool isVip;

            public Chapters() {
            }

            public Chapters(String title, String link) {
                this.title = title;
                this.link = link;
            }
        }
    }
    }
}
