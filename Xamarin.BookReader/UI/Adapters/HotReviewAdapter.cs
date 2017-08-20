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
using Xamarin.BookReader.Bases;
using Xamarin.BookReader.Views;

namespace Xamarin.BookReader.UI.Adapters
{
    public class HotReviewAdapter : EasyRVAdapter<HotReview.Reviews>
    {
        private IOnRvItemClickListener<Object> itemClickListener;

        public HotReviewAdapter(Context context, List<HotReview.Reviews> list, IOnRvItemClickListener<Object> listener)
            : base(context, list, Resource.Layout.item_book_detai_hot_review_list)
        {
            this.itemClickListener = listener;
        }

        protected override void OnBindData(RecyclerView.ViewHolder viewHolder, int position, HotReview.Reviews item)
        {
            var holder = viewHolder as EasyRVHolder;
            holder.setCircleImageUrl(Resource.Id.ivBookCover, Constant.IMG_BASE_URL + item.author.avatar, Resource.Drawable.avatar_default)
                .setText(Resource.Id.tvBookTitle, item.author.nickname)
                .setText(Resource.Id.tvBookType, String.Format(mContext.GetString(Resource.String
                        .book_detail_user_lv), item.author.lv))
                .setText(Resource.Id.tvTitle, item.title)
                .setText(Resource.Id.tvContent, item.content)
                .setText(Resource.Id.tvHelpfulYes, item.helpful.yes.ToString());
            XLHRatingBar ratingBar = holder.getView<XLHRatingBar>(Resource.Id.rating);
            ratingBar.setCountSelected(item.rating);
            holder.Click += (sender, e) =>
            {
                itemClickListener.onItemClick(holder.getItemView(), position, item);
            };
        }
    }
}