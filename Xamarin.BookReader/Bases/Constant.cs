using System;
using System.Collections.Generic;
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

        };
    }
}
