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
using Xamarin.BookReader.UI.EasyAdapters;

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
            return Resource.Layout.activity_common_recyclerview;
        }
        public override void bindViews()
        {
        }

        public override void initToolBar()
        {
            mCommonToolbar.SetTitle(Resource.String.subject_book_list_my_book_list);
            mCommonToolbar.SetNavigationIcon(Resource.Drawable.ab_back);
        }
        public override void initDatas()
        {
        }
        public override void configViews()
        {
            initAdapter(new SubjectBookListAdapter(this), true, false);
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
        private void showLongClickDialog(int position)
        {
            new AlertDialog.Builder(this)
                    .SetTitle(mAdapter.getItem(position).title)
                    .SetItems(Resources.GetStringArray(Resource.Array.my_book_list_item_long_click_choice),
                        (sender, e) =>
                        {
                            var dialog = sender as AlertDialog;
                            var which = e.Which;
                            switch (which)
                            {
                                case 0:
                                //删除
                                CacheManager.RemoveMyBookList(mAdapter.getItem(position)._id);
                                    mAdapter.remove(position);
                                    break;
                                default:
                                    break;
                            }
                            dialog?.Dismiss();
                        })
                    .SetNegativeButton(string.Empty, (sender, e) => { })
                    .Create().Show();
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