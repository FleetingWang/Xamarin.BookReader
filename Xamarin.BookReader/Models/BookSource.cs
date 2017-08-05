using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.BookReader.Models
{
    public class BookSource
    {
        /**
     * _id : 55219c4ea9240fb868282e65
     * lastChapter : 学神被封了，书也应该不可能放出来了。
     * link : http://leduwo.com/book/56/56121/index.html
     * source : tianyibook
     * name : 乐读窝
     * isCharge : false
     * chaptersCount : 515
     * updated : 2016-08-09T18:42:34.880Z
     * starting : false
     * host : leduwo.com
     */

        public String _id;
        public String lastChapter;
        public String link;
        public String source;
        public String name;
        public bool isCharge;
        public int chaptersCount;
        public String updated;
        public bool starting;
        public String host;
    }
}
