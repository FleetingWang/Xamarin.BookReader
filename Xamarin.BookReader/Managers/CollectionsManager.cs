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
using Xamarin.BookReader.Models;
using Xamarin.BookReader.Utils;
using Xamarin.BookReader.Bases;
using Java.Util;
using Android.Text;
using Java.IO;
using Java.Lang;
using Settings = Xamarin.BookReader.Helpers.Settings;
using Akavache;
using System.Reactive.Linq;

namespace Xamarin.BookReader.Managers
{
    /// <summary>
    /// 收藏列表管理
    /// </summary>
    public class CollectionsManager
    {
        private volatile static CollectionsManager singleton;

        private CollectionsManager()
        {

        }

        private static readonly object lockObj = new object();
        public static CollectionsManager getInstance()
        {
            if (singleton == null)
            {
                lock (lockObj)
                {
                    if (singleton == null)
                    {
                        singleton = new CollectionsManager();
                    }
                }
            }
            return singleton;
        }

        /**
         * 获取收藏列表
         *
         * @return
         */
        public List<Recommend.RecommendBooks> getCollectionList()
        {
            List<Recommend.RecommendBooks> list;
            try
            {
                list = BlobCache.LocalMachine.GetObject<List<Recommend.RecommendBooks>>("RecommendBooks").Wait();
            }
            catch (KeyNotFoundException ex)
            {
                list = new List<Recommend.RecommendBooks>();
            }
            return list;
        }

        public async void putCollectionList(List<Recommend.RecommendBooks> list)
        {
            await BlobCache.LocalMachine.InsertObject("RecommendBooks", list);
        }

        /**
         * 按排序方式获取收藏列表
         *
         * @return
         */
        public List<Recommend.RecommendBooks> getCollectionListBySort()
        {
            List<Recommend.RecommendBooks> list = getCollectionList();
            if (list == null)
            {
                return null;
            }
            else
            {
                if (SharedPreferencesUtil.getInstance().getBoolean(Constant.ISBYUPDATESORT, true))
                {
                    list.Sort(new LatelyUpdateTimeComparator());
                }
                else
                {
                    list.Sort(new RecentReadingTimeComparator());
                }
                return list;
            }
        }

        /**
         * 移除单个收藏
         *
         * @param bookId
         */
        public void remove(string bookId)
        {
            List<Recommend.RecommendBooks> list = getCollectionList();
            if (list == null)
            {
                return;
            }
            foreach (Recommend.RecommendBooks bean in list)
            {
                if (TextUtils.Equals(bean._id, bookId))
                {
                    list.Remove(bean);
                    putCollectionList(list);
                    break;
                }
            }
            EventManager.refreshCollectionList();
        }

        /**
         * 是否已收藏
         *
         * @param bookId
         * @return
         */
        public bool isCollected(string bookId)
        {
            List<Recommend.RecommendBooks> list = getCollectionList();
            if (list == null || list.Count() == 0)
            {
                return false;
            }
            foreach (Recommend.RecommendBooks bean in list)
            {
                if (bean._id.Equals(bookId))
                {
                    return true;
                }
            }
            return false;
        }

        /**
         * 是否已置顶
         *
         * @param bookId
         * @return
         */
        public bool isTop(string bookId)
        {
            List<Recommend.RecommendBooks> list = getCollectionList();
            if (list == null || list.Count() == 0)
            {
                return false;
            }
            foreach (Recommend.RecommendBooks bean in list)
            {
                if (bean._id.Equals(bookId))
                {
                    if (bean.isTop)
                        return true;
                }
            }
            return false;
        }

        /**
         * 移除多个收藏
         *
         * @param removeList
         */
        public void removeSome(List<Recommend.RecommendBooks> removeList, bool removeCache)
        {
            List<Recommend.RecommendBooks> list = getCollectionList();
            if (list == null)
            {
                return;
            }
            if (removeCache)
            {
                foreach (Recommend.RecommendBooks book in removeList)
                {
                    try
                    {
                        // 移除章节文件
                        FileUtils.deleteFileOrDirectory(FileUtils.getBookDir(book._id));
                        // 移除目录缓存
                        // TODO: CacheManager.getInstance().removeTocList(AppUtils.getAppContext(), book._id);
                        // 移除阅读进度
                        Settings.RemoveReadProgress(book._id);
                    }
                    catch (IOException e)
                    {
                        LogUtils.e(e.ToString());
                    }
                }
            }
            foreach (var item in removeList)
            {
                list.Remove(item);
            }
            putCollectionList(list);
        }

        /**
         * 加入收藏
         *
         * @param bean
         */
        public bool add(Recommend.RecommendBooks bean)
        {
            if (isCollected(bean._id))
            {
                return false;
            }
            List<Recommend.RecommendBooks> list = getCollectionList();
            if (list == null)
            {
                list = new List<Recommend.RecommendBooks>();
            }
            list.Add(bean);
            putCollectionList(list);
            EventManager.refreshCollectionList();
            return true;
        }

        /**
         * 置顶收藏、取消置顶
         *
         * @param bookId
         */
        public void top(string bookId, bool isTop)
        {
            List<Recommend.RecommendBooks> list = getCollectionList();
            if (list == null)
            {
                return;
            }
            foreach (Recommend.RecommendBooks bean in list)
            {
                if (TextUtils.Equals(bean._id, bookId))
                {
                    bean.isTop = isTop;
                    list.Remove(bean);
                    list.Insert(0, bean);
                    putCollectionList(list);
                    break;
                }
            }
            EventManager.refreshCollectionList();
        }

        /**
         * 设置最新章节和更新时间
         *
         * @param bookId
         */
        public /*synchronized*/ void setLastChapterAndLatelyUpdate(string bookId, string lastChapter, string latelyUpdate)
        {
            List<Recommend.RecommendBooks> list = getCollectionList();
            if (list == null)
            {
                return;
            }
            foreach (Recommend.RecommendBooks bean in list)
            {
                if (TextUtils.Equals(bean._id, bookId))
                {
                    bean.lastChapter = lastChapter;
                    bean.updated = latelyUpdate;
                    list.Remove(bean);
                    list.Add(bean);
                    putCollectionList(list);
                    break;
                }
            }
        }

        /**
         * 设置最近阅读时间
         *
         * @param bookId
         */
        public void setRecentReadingTime(string bookId)
        {
            List<Recommend.RecommendBooks> list = getCollectionList();
            if (list == null)
            {
                return;
            }
            foreach (Recommend.RecommendBooks bean in list)
            {
                if (TextUtils.Equals(bean._id, bookId))
                {
                    bean.recentReadingTime = FormatUtils.getCurrentTimeString(FormatUtils.FORMAT_DATE_TIME);
                    list.Remove(bean);
                    list.Add(bean);
                    putCollectionList(list);
                    break;
                }
            }
        }

        public void clear()
        {
            try
            {
                FileUtils.deleteFileOrDirectory(new File(Constant.PATH_COLLECT));
            }
            catch (Java.Lang.Exception e)
            {
                e.PrintStackTrace();
            }
        }


        /**
         * 自定义比较器：按最近阅读时间来排序，置顶优先，降序
         */
        class RecentReadingTimeComparator : IComparer<Recommend.RecommendBooks>
        {

            public int Compare(Recommend.RecommendBooks p1, Recommend.RecommendBooks p2)
            {
                if (p1.isTop && p2.isTop || !p1.isTop && !p2.isTop)
                {
                    return p2.recentReadingTime.CompareTo(p1.recentReadingTime);
                }
                else
                {
                    return p1.isTop ? -1 : 1;
                }
            }
        }

        /**
         * 自定义比较器：按更新时间来排序，置顶优先，降序
         */
        class LatelyUpdateTimeComparator : IComparer<Recommend.RecommendBooks>
        {
            public int Compare(Recommend.RecommendBooks p1, Recommend.RecommendBooks p2)
            {
                if (p1.isTop && p2.isTop || !p1.isTop && !p2.isTop)
                {
                    return p2.updated.CompareTo(p1.updated);
                }
                else
                {
                    return p1.isTop ? -1 : 1;
                }
            }
        }
    }
}