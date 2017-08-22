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
using Xamarin.BookReader.Bases;
using Xamarin.BookReader.Views.RecyclerViews.Adapters;
using Xamarin.BookReader.Models;
using Xamarin.BookReader.UI.EasyAdapters;
using Xamarin.BookReader.Views.RecyclerViews;
using Android.Support.V7.Widget;
using Android.Support.V4.Content;
using Android.Provider;
using Xamarin.BookReader.Utils;
using Android.Database;
using Uri = Android.Net.Uri;
using Java.IO;
using Xamarin.BookReader.Managers;
using Android.Content.PM;

namespace Xamarin.BookReader.UI.Activities
{
    /// <summary>
    /// 扫描本地书籍
    /// </summary>
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class ScanLocalBookActivity : BaseActivity, RecyclerArrayAdapter<Recommend.RecommendBooks>.OnItemClickListener
    {
        public static void startActivity(Context context)
        {
            context.StartActivity(new Intent(context, typeof(ScanLocalBookActivity)));
        }

        EasyRecyclerView mRecyclerView;

        private RecommendAdapter mAdapter;
        public override int getLayoutId()
        {
            return Resource.Layout.activity_scan_local_book;
        }
        public override void bindViews()
        {
            mRecyclerView = FindViewById<EasyRecyclerView>(Resource.Id.recyclerview);
        }

        public override void initToolBar()
        {
            mCommonToolbar.Title = ("扫描本地书籍");
            mCommonToolbar.SetNavigationIcon(Resource.Drawable.ab_back);
        }
        public override void initDatas()
        {
        }
        public override void configViews()
        {
            mRecyclerView.setLayoutManager(new LinearLayoutManager(this));
            mRecyclerView.setItemDecoration(ContextCompat.GetColor(this, Resource.Color.common_divider_narrow), 1, 0, 0);

            mAdapter = new RecommendAdapter(this);
            mAdapter.setOnItemClickListener(this);
            mRecyclerView.setAdapterWithProgress(mAdapter);

            queryFiles();
        }

        private void queryFiles()
        {
            String[] projection = new String[]{
                MediaStore.Files.FileColumns.Id,
                MediaStore.Files.FileColumns.Data,
                MediaStore.Files.FileColumns.Size
            };

            // cache
            String bookpath = FileUtils.createRootPath(AppUtils.getAppContext());

            // 查询后缀名为txt与pdf，并且不位于项目缓存中的文档
            ICursor cursor = ContentResolver.Query(
                    Uri.Parse("content://media/external/file"),
                    projection,
                    MediaStore.Files.FileColumns.Data + " not like ? and ("
                            + MediaStore.Files.FileColumns.Data + " like ? or "
                            + MediaStore.Files.FileColumns.Data + " like ? or "
                            + MediaStore.Files.FileColumns.Data + " like ? or "
                            + MediaStore.Files.FileColumns.Data + " like ? )",
                    new String[]{"%" + bookpath + "%",
                        "%" + Constant.SUFFIX_TXT,
                        "%" + Constant.SUFFIX_PDF,
                        "%" + Constant.SUFFIX_EPUB,
                        "%" + Constant.SUFFIX_CHM}, null);

            if (cursor != null && cursor.MoveToFirst())
            {
                int idindex = cursor.GetColumnIndex(MediaStore.Files.FileColumns.Id);
                int dataindex = cursor.GetColumnIndex(MediaStore.Files.FileColumns.Data);
                int sizeindex = cursor.GetColumnIndex(MediaStore.Files.FileColumns.Size);
                List<Recommend.RecommendBooks> list = new List<Recommend.RecommendBooks>();


                do
                {
                    String path = cursor.GetString(dataindex);

                    int dot = path.LastIndexOf("/");
                    String name = path.Substring(dot + 1);
                    if (name.LastIndexOf(".") > 0)
                        name = name.Substring(0, name.LastIndexOf("."));

                    Recommend.RecommendBooks books = new Recommend.RecommendBooks();
                    books._id = name;
                    books.path = path;
                    books.title = name;
                    books.isFromSD = true;
                    books.lastChapter = FileUtils.formatFileSizeToString(cursor.GetLong(sizeindex));

                    list.Add(books);
                } while (cursor.MoveToNext());

                cursor.Close();

                mAdapter.addAll(list);
            }
            else
            {
                mAdapter.clear();
            }
        }

        public void onItemClick(int position)
        {
            Recommend.RecommendBooks books = mAdapter.getItem(position);

            if (books.path.EndsWith(Constant.SUFFIX_TXT))
            {
                // TXT
                new AlertDialog.Builder(this)
                    .SetTitle("提示")
                    .SetMessage(String.Format(GetString(
                            Resource.String.book_detail_is_joined_the_book_shelf), books.title))
                    .SetPositiveButton("确定", (sender, e) =>
                    {
                        // 拷贝到缓存目录
                        FileUtils.fileChannelCopy(new File(books.path),
                                new File(FileUtils.getChapterPath(books._id, 1)));
                        // 加入书架
                        if (CollectionsManager.getInstance().add(books))
                        {
                            mRecyclerView.showTipViewAndDelayClose(String.Format(GetString(
                                    Resource.String.book_detail_has_joined_the_book_shelf), books.title));
                            // 通知
                            EventManager.refreshCollectionList();
                        }
                        else
                        {
                            mRecyclerView.showTipViewAndDelayClose("书籍已存在");
                        }
                        var dialog = sender as AlertDialog;
                        dialog?.Dismiss();
                    })
                    .SetNegativeButton("取消", (sender, e) =>
                    {
                        var dialog = sender as AlertDialog;
                        dialog?.Dismiss();
                    })
                    .Show();
            }
            else if (books.path.EndsWith(Constant.SUFFIX_PDF))
            {
                // PDF
                // TODO: ReadPDFActivity.start(this, books.path);
            }
            else if (books.path.EndsWith(Constant.SUFFIX_EPUB))
            {
                // EPub
                // TODO: ReadEPubActivity.start(this, books.path);
            }
            else if (books.path.EndsWith(Constant.SUFFIX_CHM))
            {
                // CHM
                // TODO: ReadCHMActivity.start(this, books.path);
            }
        }
    }
}