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

namespace Xamarin.BookReader.Utils
{
    public class StringUtils
    {
        public static string creatAcacheKey(params Object[] param)
        {
            string key = "";
            for (Object o : param)
            {
                key += "-" + o;
            }
            return key.replaceFirst("-", "");
        }

        /**
         * 格式化小说内容。
         * <p/>
         * <li>小说的开头，缩进2格。在开始位置，加入2格空格。
         * <li>所有的段落，缩进2格。所有的\n,替换为2格空格。
         *
         * @param str
         * @return
         */
        public static string formatContent(string str)
        {
            str = str.replaceAll("[ ]*", "");//替换来自服务器上的，特殊空格
            str = str.replaceAll("[ ]*", "");//
            str = str.replace("\n\n", "\n");
            str = str.replace("\n", "\n" + getTwoSpaces());
            str = getTwoSpaces() + str;
            //        str = convertToSBC(str);
            return str;
        }

        /**
         * Return a string that only has two spaces.
         *
         * @return
         */
        public static string getTwoSpaces()
        {
            return "\u3000\u3000";
        }
    }
}