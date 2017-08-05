using Android.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Xamarin.BookReader.Utils;

namespace Xamarin.BookReader.Bases
{
    public class Constant
    {
        public const String IMG_BASE_URL = "http://statics.zhuishushenqi.com";

        public const String API_BASE_URL = "http://api.zhuishushenqi.com";

        public static String PATH_DATA = FileUtils.createRootPath(AppUtils.getAppContext()) + "/cache";

        public static String PATH_COLLECT = FileUtils.createRootPath(AppUtils.getAppContext()) + "/collect";

        public static String PATH_TXT = PATH_DATA + "/book/";

        public static String PATH_EPUB = PATH_DATA + "/epub";

        public static String PATH_CHM = PATH_DATA + "/chm";

        public static String BASE_PATH = AppUtils.getAppContext().CacheDir.Path;

        public const String ISNIGHT = "isNight";

        public const String ISBYUPDATESORT = "isByUpdateSort";
        public const String FLIP_STYLE = "flipStyle";

        public const String SUFFIX_TXT = ".txt";
        public const String SUFFIX_PDF = ".pdf";
        public const String SUFFIX_EPUB = ".epub";
        public const String SUFFIX_ZIP = ".zip";
        public const String SUFFIX_CHM = ".chm";

        public static int[] tagColors = new int[] {
            Color.ParseColor("#90C5F0"),
            Color.ParseColor("#91CED5"),
            Color.ParseColor("#F88F55"),
            Color.ParseColor("#C0AFD0"),
            Color.ParseColor("#E78F8F"),
            Color.ParseColor("#67CCB7"),
            Color.ParseColor("#F6BC7E")
        };

        public enum Gender
        {
            [Description("male")]
            Male,
            [Description("female")]
            Female
        }

        public enum CateType
        {
            [Description("hot")]
            Hot,
            [Description("new")]
            New,
            [Description("reputation")]
            Reputation,
            [Description("over")]
            Over
        }

        public enum Distillate
        {
            [Description("")]
            All,
            [Description("true")]
            Distillate
        }

        public enum SortType
        {
            [Description("updated")]
            Default,
            [Description("created")]
            Created,
            [Description("helpful")]
            Helpful,
            [Description("comment_count")]
            CommentCount
        }

        public static List<SortType> sortTypeList = Enum.GetValues(typeof(SortType))
                                                    .Cast<SortType>()
                                                    //.Select(s => s.GetType()
                                                    //    .GetCustomAttribute<DescriptionAttribute>()
                                                    //    .Description
                                                    //)
                                                    .ToList();

        public enum BookType
        {
            [Description("all")]
            ALL,
            [Description("xhqh")]
            XHQH,
            [Description("wxxx")]
            WXXX,
            [Description("dsyn")]
            DSYN,
            [Description("lsjs")]
            LSJS,
            [Description("yxjj")]
            YXJJ,
            [Description("khly")]
            KHLY,
            [Description("cyjk")]
            CYJK,
            [Description("hmzc")]
            HMZC,
            [Description("xdyq")]
            XDYQ,
            [Description("gdyq")]
            GDYQ,
            [Description("hxyq")]
            HXYQ,
            [Description("dmtr")]
            DMTR
        }

        public static List<BookType> bookTypeList = Enum.GetValues(typeof(BookType))
                                                    .Cast<BookType>()
                                                    .ToList();

        public static Dictionary<string, string> bookType = new Dictionary<string, string> {
            { "qt", "其他" },
            { BookType.XHQH.ToString().ToLower(), "玄幻奇幻" },
            { BookType.WXXX.ToString().ToLower(), "武侠仙侠" },
            { BookType.DSYN.ToString().ToLower(), "都市异能" },
            { BookType.LSJS.ToString().ToLower(), "历史军事" },
            { BookType.YXJJ.ToString().ToLower(), "游戏竞技" },
            { BookType.KHLY.ToString().ToLower(), "科幻灵异" },
            { BookType.CYJK.ToString().ToLower(), "穿越架空" },
            { BookType.HMZC.ToString().ToLower(), "豪门总裁" },
            { BookType.XDYQ.ToString().ToLower(), "现代言情" },
            { BookType.GDYQ.ToString().ToLower(), "古代言情" },
            { BookType.HXYQ.ToString().ToLower(), "幻想言情" },
            { BookType.DMTR.ToString().ToLower(), "耽美同人" }
        };

    }
}
