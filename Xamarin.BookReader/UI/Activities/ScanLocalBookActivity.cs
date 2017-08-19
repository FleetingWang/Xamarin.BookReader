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

namespace Xamarin.BookReader.UI.Activities
{
    /// <summary>
    /// 扫描本地书籍
    /// </summary>
    public class ScanLocalBookActivity : BaseActivity, RecyclerArrayAdapter<Recommend.RecommendBooks>.OnItemClickListener
    {
        public static void startActivity(Context context)
        {
            context.StartActivity(new Intent(context, typeof(ScanLocalBookActivity)));
        }

        //@Bind(R.id.recyclerview)
        EasyRecyclerView mRecyclerView;

        private RecommendAdapter mAdapter;
        public override int getLayoutId()
        {
            return R.layout.activity_scan_local_book;
        }
        public override void bindViews()
        {
            throw new NotImplementedException();
        }

        public override void initToolBar()
        {
            mCommonToolbar.setTitle("扫描本地书籍");
            mCommonToolbar.setNavigationIcon(R.drawable.ab_back);
        }
        public override void initDatas()
        {
        }
        public override void configViews()
        {
            mRecyclerView.setLayoutManager(new LinearLayoutManager(this));
            mRecyclerView.setItemDecoration(ContextCompat.getColor(this, R.color.common_divider_narrow), 1, 0, 0);

            mAdapter = new RecommendAdapter(this);
            mAdapter.setOnItemClickListener(this);
            mRecyclerView.setAdapterWithProgress(mAdapter);

            queryFiles();
        }

        private void queryFiles()
        {
            String[] projection = new String[]{MediaStore.Files.FileColumns._ID,
                MediaStore.Files.FileColumns.DATA,
                MediaStore.Files.FileColumns.SIZE
        };

            // cache
            String bookpath = FileUtils.createRootPath(AppUtils.getAppContext());

            // 查询后缀名为txt与pdf，并且不位于项目缓存中的文档
            Cursor cursor = getContentResolver().query(
                    Uri.parse("content://media/external/file"),
                    projection,
                    MediaStore.Files.FileColumns.DATA + " not like ? and ("
                            + MediaStore.Files.FileColumns.DATA + " like ? or "
                            + MediaStore.Files.FileColumns.DATA + " like ? or "
                            + MediaStore.Files.FileColumns.DATA + " like ? or "
                            + MediaStore.Files.FileColumns.DATA + " like ? )",
                    new String[]{"%" + bookpath + "%",
                        "%" + Constant.SUFFIX_TXT,
                        "%" + Constant.SUFFIX_PDF,
                        "%" + Constant.SUFFIX_EPUB,
                        "%" + Constant.SUFFIX_CHM}, null);

            if (cursor != null && cursor.moveToFirst())
            {
                int idindex = cursor.getColumnIndex(MediaStore.Files.FileColumns._ID);
                int dataindex = cursor.getColumnIndex(MediaStore.Files.FileColumns.DATA);
                int sizeindex = cursor.getColumnIndex(MediaStore.Files.FileColumns.SIZE);
                List<Recommend.RecommendBooks> list = new ArrayList<>();


                do
                {
                    String path = cursor.getString(dataindex);

                    int dot = path.lastIndexOf("/");
                    String name = path.substring(dot + 1);
                    if (name.lastIndexOf(".") > 0)
                        name = name.substring(0, name.lastIndexOf("."));

                    Recommend.RecommendBooks books = new Recommend.RecommendBooks();
                    books._id = name;
                    books.path = path;
                    books.title = name;
                    books.isFromSD = true;
                    books.lastChapter = FileUtils.formatFileSizeToString(cursor.getLong(sizeindex));

                    list.add(books);
                } while (cursor.moveToNext());

                cursor.close();

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

            if (books.path.endsWith(Constant.SUFFIX_TXT)) {
                // TXT
                new AlertDialog.Builder(this)
                        .setTitle("提示")
                        .setMessage(String.format(getString(
                                R.string.book_detail_is_joined_the_book_shelf), books.title))
                //        .setPositiveButton("确定", new DialogInterface.OnClickListener() {
                //            @Override
                //            public void onClick(DialogInterface dialog, int which) {
                //                // 拷贝到缓存目录
                //                FileUtils.fileChannelCopy(new File(books.path),
                //                        new File(FileUtils.getChapterPath(books._id, 1)));
                //                // 加入书架
                //                if (CollectionsManager.getInstance().add(books)) {
                //                    mRecyclerView.showTipViewAndDelayClose(String.format(getString(
                //                            R.string.book_detail_has_joined_the_book_shelf), books.title));
                //                    // 通知
                //                    EventManager.refreshCollectionList();
                //                } else {
                //                    mRecyclerView.showTipViewAndDelayClose("书籍已存在");
                //                }
                //                dialog.dismiss();
                //            }
                //        }).setNegativeButton("取消", new DialogInterface.OnClickListener() {
                //    @Override
                //    public void onClick(DialogInterface dialog, int which) {
                //        dialog.dismiss();
                //    }
                //})
            .show();
            } else if (books.path.endsWith(Constant.SUFFIX_PDF)) {
                // PDF
                ReadPDFActivity.start(this, books.path);
            } else if (books.path.endsWith(Constant.SUFFIX_EPUB)) {
                // EPub
                ReadEPubActivity.start(this, books.path);
            } else if (books.path.endsWith(Constant.SUFFIX_CHM)) {
                // CHM
                ReadCHMActivity.start(this, books.path);
            }
        }
    }
}