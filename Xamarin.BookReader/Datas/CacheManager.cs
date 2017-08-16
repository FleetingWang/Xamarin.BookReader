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
using Akavache;
using System.Reactive.Linq;
using Xamarin.BookReader.Models;
using Android.Text;
using Xamarin.BookReader.Utils;
using Java.IO;
using Xamarin.BookReader.Bases;
using Settings = Xamarin.BookReader.Helpers.Settings;
using Xamarin.BookReader.Managers;

namespace Xamarin.BookReader.Datas
{
    public static class CacheManager
    {
        public static CacheManager Instance { get; set; } = new CacheManager();

        private static string searchHistoryKey = "search_history";
        public static List<string> SearchHistory
        {
            get {
                try
                {
                    return BlobCache.LocalMachine.GetObject<List<string>>(searchHistoryKey).Wait();
                }
                catch (KeyNotFoundException ex)
                {
                    return new List<string>();
                }
            }
            set => BlobCache.LocalMachine.InsertObject(searchHistoryKey, value);
        }

        private static string myBookListKey = "my_book_lists";
        /// <summary>
        /// 获取我收藏的书单列表
        /// </summary>
        public static List<BookLists.BookListsBean> MyBookList
        {
            get
            {
                try
                {
                    return BlobCache.LocalMachine.GetObject<List<BookLists.BookListsBean>>(myBookListKey).Wait();
                }
                catch (KeyNotFoundException ex)
                {
                    return new List<BookLists.BookListsBean>();
                }
            }
        }

        private static void SaveMyBookList(List<BookLists.BookListsBean> list)
        {
            BlobCache.LocalMachine.InsertObject(myBookListKey, list).Wait();
        }

        public static void RemoveMyBookList(string bookListId)
        {
            foreach(var item in MyBookList)
            {
                if(item != null && TextUtils.Equals(item._id, bookListId))
                {
                    MyBookList.Remove(item);
                    SaveMyBookList(MyBookList);
                    break;
                }
            }
        }

        public static void AddMyBookList(BookLists.BookListsBean bean)
        {
            foreach(var item in MyBookList)
            {
                if(item != null && TextUtils.Equals(item._id, bean._id))
                {
                    ToastUtils.showToast("已经收藏过啦");
                    return;
                }
            }
            MyBookList.Add(bean);
            SaveMyBookList(MyBookList);
            ToastUtils.showToast("收藏成功");
        }

        private static string getTocListKey(string bookId) => "bookToc_" + bookId;
        public static List<BookMixAToc.MixToc.Chapters> GetTocList(string bookId)
        {
            try
            {
                var bookMixAToc = BlobCache.LocalMachine.GetObject<BookMixAToc>(getTocListKey(bookId)).Wait();
                return bookMixAToc.mixToc.chapters;
            }
            catch (KeyNotFoundException ex)
            {
            }
            return null;
        }

        public static void SaveTocList(string bookId, BookMixAToc data)
        {
            BlobCache.LocalMachine.InsertObject(getTocListKey(bookId), data);
        }

        public static void RemoveTocList(string bookId)
        {
            BlobCache.LocalMachine.InvalidateObject<BookMixAToc>(getTocListKey(bookId));
        }

        public static File GetChapterFile(String bookId, int chapter)
        {
            File file = FileUtils.getChapterFile(bookId, chapter);
            if (file != null && file.Length() > 50)
                return file;
            return null;
        }

        public static void SaveChapterFile(String bookId, int chapter, ChapterRead.Chapter data)
        {
            File file = FileUtils.getChapterFile(bookId, chapter);
            FileUtils.writeFile(file.AbsolutePath, StringUtils.formatContent(data.body), false);
        }

        /**
         * 获取缓存大小
         *
         * @return
         */
        public static /*synchronized*/ string GetCacheSize()
        {
            long cacheSize = 0;

            try
            {
                String cacheDir = Constant.BASE_PATH;
                cacheSize += FileUtils.getFolderSize(cacheDir);
                if (FileUtils.isSdCardAvailable())
                {
                    String extCacheDir = AppUtils.getAppContext().ExternalCacheDir.Path;
                    cacheSize += FileUtils.getFolderSize(extCacheDir);
                }
            }
            catch (Exception e)
            {
                LogUtils.e(e.ToString());
            }

            return FileUtils.formatFileSizeToString(cacheSize);
        }

        /**
         * 清除缓存
         *
         * @param clearReadPos 是否删除阅读记录
         */
        public static /*synchronized*/ void clearCache(bool clearReadPos, bool clearCollect)
        {
            try
            {
                // 删除内存缓存
                String cacheDir = AppUtils.getAppContext().CacheDir.Path;
                FileUtils.deleteFileOrDirectory(new File(cacheDir));
                if (FileUtils.isSdCardAvailable())
                {
                    // 删除SD书籍缓存
                    FileUtils.deleteFileOrDirectory(new File(Constant.PATH_DATA));
                }
                // 删除阅读记录（SharePreference）
                if (clearReadPos)
                {
                    //防止再次弹出性别选择框，sp要重写入保存的性别
                    var chooseSex = Settings.UserChooseSex;
                    SharedPreferencesUtil.getInstance().removeAll();
                    Settings.UserChooseSex = chooseSex;
                }
                // 清空书架
                if (clearCollect)
                {
                    CollectionsManager.getInstance().clear();
                }
                // 清除其他缓存
                BlobCache.LocalMachine.InvalidateAll();
                BlobCache.LocalMachine.Flush();
            }
            catch (Exception e)
            {
                LogUtils.e(e.ToString());
            }
        }
    }
}