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
using Xamarin.BookReader.Views.RecyclerViews.Adapters;
using Xamarin.BookReader.Datas;

namespace Xamarin.BookReader.UI.Activities
{
    /// <summary>
    /// 我的书单
    /// </summary>
    public class MyBookListActivity : BaseRVActivity<BookLists.BookListsBean>,
        RecyclerArrayAdapter<BookLists.BookListsBean>.OnItemLongClickListener
    {

        public override int getLayoutId()
        {
            return R.layout.activity_common_recyclerview;
        }
        public override void bindViews()
        {
        }

        public override void initToolBar()
        {
            mCommonToolbar.setTitle(R.string.subject_book_list_my_book_list);
            mCommonToolbar.setNavigationIcon(R.drawable.ab_back);
        }
        public override void initDatas()
        {
        }
        public override void configViews()
        {
            initAdapter(SubjectBookListAdapter, true, false);
            mAdapter.setOnItemLongClickListener(this);
            onRefresh();
        }

        public override void onItemClick(int position)
        {
            SubjectBookListDetailActivity.startActivity(this, mAdapter.getItem(position));
        }

        public bool onItemLongClick(int position)
        {
            showLongClickDialog(position);
            return false;
        }
        /// <summary>
        /// 显示长按对话框
        /// </summary>
        /// <param name="position"></param>
        private void showLongClickDialog(int position) {
        new AlertDialog.Builder(this)
                .setTitle(mAdapter.getItem(position).title)
                //.setItems(getResources().getStringArray(R.array.my_book_list_item_long_click_choice),
                //        new DialogInterface.OnClickListener() {
                //            @Override
                //            public void onClick(DialogInterface dialog, int which) {
                //                switch (which) {
                //                    case 0:
                //                        //删除
                //                        CacheManager.getInstance().removeCollection(mAdapter.getItem(position)._id);
                //                        mAdapter.remove(position);
                //                        break;
                //                    default:
                //                        break;
                //                }
                //                dialog.dismiss();
                //            }
                //        })
                .setNegativeButton(null, null)
                .create().show();
        }

        public override void onRefresh()
        {
            base.onRefresh();
            List<BookLists.BookListsBean> data = CacheManager.MyBookList;
            mAdapter.clear();
            mAdapter.addAll(data);
            mRecyclerView.setRefreshing(false);
        }
    }
}