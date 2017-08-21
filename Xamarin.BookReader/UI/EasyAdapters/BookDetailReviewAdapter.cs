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
using Xamarin.BookReader.Views.RecyclerViews.Adapters;
using Xamarin.BookReader.Models;
using Xamarin.BookReader.Bases;
using Xamarin.BookReader.Views;
using Xamarin.BookReader.Utils;
using Xamarin.BookReader.Helpers;

namespace Xamarin.BookReader.UI.EasyAdapters
{
    public class BookDetailReviewAdapter : RecyclerArrayAdapter<HotReview.Reviews>
    {


        public BookDetailReviewAdapter(Context context) : base(context)
        {
        }

        public override BaseViewHolder<HotReview.Reviews> onCreateViewHolder(ViewGroup parent, int viewType)
        {
            return new CustomViewHolder(this, parent, Resource.Layout.item_book_detai_hot_review_list);
        }

        private class CustomViewHolder : BaseViewHolder<HotReview.Reviews>
        {
            private BookDetailReviewAdapter bookDetailReviewAdapter;
            private ViewGroup parent;
            private int item_book_detai_hot_review_list;

            public CustomViewHolder(BookDetailReviewAdapter bookDetailReviewAdapter, ViewGroup parent, int item_book_detai_hot_review_list)
                : base(parent, item_book_detai_hot_review_list)
            {
                this.bookDetailReviewAdapter = bookDetailReviewAdapter;
                this.parent = parent;
                this.item_book_detai_hot_review_list = item_book_detai_hot_review_list;
            }

            public override void setData(HotReview.Reviews item)
            {
                if (!Settings.IsNoneCover)
                {
                    holder.setCircleImageUrl(Resource.Id.ivBookCover, Constant.IMG_BASE_URL + item.author.avatar,
                            Resource.Drawable.avatar_default);
                }
                else
                {
                    holder.setImageResource(Resource.Id.ivBookCover, Resource.Drawable.avatar_default);
                }

                holder.setText(Resource.Id.tvBookTitle, item.author.nickname)
                        .setText(Resource.Id.tvBookType, String.Format(mContext.GetString(Resource.String
                                .book_detail_user_lv), item.author.lv))
                        .setText(Resource.Id.tvTime, FormatUtils.getDescriptionTimeFromDateString(item.created))
                        .setText(Resource.Id.tvTitle, item.title)
                        .setText(Resource.Id.tvContent, item.content)
                        .setText(Resource.Id.tvHelpfulYes, item.helpful.yes.ToString());
                holder.setVisible(Resource.Id.tvTime, true);
                XLHRatingBar ratingBar = holder.getView<XLHRatingBar>(Resource.Id.rating);
                ratingBar.setCountSelected(item.rating);
            }
        }
    }
}