using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using EasyAdapterLibrary.RecyclerViews;
using Xamarin.BookReader.Models;
using Xamarin.BookReader.UI.Listeners;

namespace Xamarin.BookReader.UI.Adapters
{
    public class TopCategoryListAdapter: EasyRVAdapter<CategoryList.MaleBean>
    {
        private IOnRvItemClickListener<CategoryList.MaleBean> itemClickListener;
        public TopCategoryListAdapter(Context context, List<CategoryList.MaleBean> list, IOnRvItemClickListener<CategoryList.MaleBean> listener)
            : base(context, list, Resource.Layout.item_top_category_list)
        {
            this.itemClickListener = listener;
        }

        protected override void OnBindData(RecyclerView.ViewHolder viewHolder, int position, CategoryList.MaleBean item)
        {
            var holder = viewHolder as EasyRVHolder;
            holder.setText(Resource.Id.tvName, item.name)
                   .setText(Resource.Id.tvBookCount, Java.Lang.String.Format(mContext.GetString(Resource.String
                        .category_book_count), item.bookCount));
            holder.Click += (sender, e) => {
                itemClickListener.onItemClick(holder.getItemView(), position, item);
            };
        }
    }
}