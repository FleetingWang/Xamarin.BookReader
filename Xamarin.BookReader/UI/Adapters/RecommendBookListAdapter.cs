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
using Xamarin.BookReader.Helpers;
using Xamarin.BookReader.Bases;

namespace Xamarin.BookReader.UI.Adapters
{
    public class RecommendBookListAdapter: EasyRVAdapter<RecommendBookList.RecommendBook>
    {
        private IOnRvItemClickListener<Object> itemClickListener;
        public RecommendBookListAdapter(Context context, List<RecommendBookList.RecommendBook> list,
                                    IOnRvItemClickListener<Object> listener)
            : base(context, list, Resource.Layout.item_book_detail_recommend_book_list)
        {
            this.itemClickListener = listener;
        }

        protected override void OnBindData(RecyclerView.ViewHolder viewHolder, int position, RecommendBookList.RecommendBook item)
        {
            EasyRVHolder holder = viewHolder as EasyRVHolder;
            if (!Settings.IsNoneCover)
            {
                holder.setRoundImageUrl(Resource.Id.ivBookListCover, Constant.IMG_BASE_URL + item.cover, Resource.Drawable.cover_default);
            }

            holder.setText(Resource.Id.tvBookListTitle, item.title)
                    .setText(Resource.Id.tvBookAuthor, item.author)
                    .setText(Resource.Id.tvBookListTitle, item.title)
                    .setText(Resource.Id.tvBookListDesc, item.desc)
                    .setText(Resource.Id.tvBookCount, Java.Lang.String.Format(mContext.GetString(Resource.String
                            .book_detail_recommend_book_list_book_count), item.bookCount))
                    .setText(Resource.Id.tvCollectorCount, Java.Lang.String.Format(mContext.GetString(Resource.String
                            .book_detail_recommend_book_list_collector_count), item.collectorCount));
            // TODO: NoDoubleClickListener
            holder.Click += (sender, e) => {
                itemClickListener.onItemClick(holder.getItemView(), position, item);
            };
        }
    }
}