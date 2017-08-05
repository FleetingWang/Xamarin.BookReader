using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.BookReader.Models
{
    public class CategoryList
    {
        /**
     * male : [{"name":"玄幻","bookCount":188244},{"name":"奇幻","bookCount":24183}]
     * ok : true
     */

        public List<MaleBean> male;
        /**
         * name : 古代言情
         * bookCount : 125103
         */

        public List<MaleBean> female;

        public class MaleBean
        {
            public String name;
            public int bookCount;
        }
    }
}
