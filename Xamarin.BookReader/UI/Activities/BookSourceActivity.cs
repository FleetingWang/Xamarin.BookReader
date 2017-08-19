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
using Xamarin.BookReader.Models;

namespace Xamarin.BookReader.UI.Activities
{
    public class BookSourceActivity : BaseRVActivity<BookSource>
    {
        public static String INTENT_BOOK_ID = "bookId";

        public static void startActivityForResult(Activity activity, String bookId, int reqId) {
            activity.StartActivityForResult(new Intent(activity, typeof(BookSourceActivity))
                    .PutExtra(INTENT_BOOK_ID, bookId), reqId);
        }
        private String bookId = "";


        public override int getLayoutId()
        {
            return R.layout.activity_common_recyclerview;
        }
        public override void bindViews()
        {
        }

        public override void initToolBar()
        {
            bookId = getIntent().getStringExtra(INTENT_BOOK_ID);
            mCommonToolbar.setTitle("选择来源");
            mCommonToolbar.setNavigationIcon(R.drawable.ab_back);
        }
        public override void initDatas()
        {
            initAdapter(BookSourceAdapter, false, false);
        }
        public override void configViews()
        {
            mPresenter.attachView(this);
        mPresenter.getBookSource("summary", bookId);

        new AlertDialog.Builder(this)
                .setMessage("换源功能暂未实现，后续更新...")
                //.setPositiveButton("OK", new DialogInterface.OnClickListener() {
                //    @Override
                //    public void onClick(DialogInterface dialog, int which) {
                //        dialog.dismiss();
                //    }
                //})
            .create().show();
        }

        public override void onItemClick(int position)
        {
            BookSource data = mAdapter.getItem(position);
            Intent intent = new Intent();
            intent.putExtra("source", data);
            setResult(RESULT_OK, intent);
            finish();
        }

        public void showBookSource(List<BookSource> list)
        {
            mAdapter.clear();
            mAdapter.addAll(list);
        }

        public void showError()
        {
            loaddingError();
        }

        public void complete()
        {
            mRecyclerView.setRefreshing(false);
        }


    }
}