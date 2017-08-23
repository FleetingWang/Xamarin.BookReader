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
using Xamarin.BookReader.Helpers;
using Xamarin.BookReader.Bases;
using Android.Text;
using Xamarin.BookReader.Utils;

namespace Xamarin.BookReader.UI.EasyAdapters
{
    public class BookReviewAdapter : RecyclerArrayAdapter<BookReviewList.ReviewsBean>
    {


        public BookReviewAdapter(Context context) : base(context)
        {
        }

        public override BaseViewHolder<BookReviewList.ReviewsBean> onCreateViewHolder(ViewGroup parent, int viewType)
        {
            return new CustomViewHolder(this, parent, Resource.Layout.item_community_book_review_list);
        }

        private class CustomViewHolder : BaseViewHolder<BookReviewList.ReviewsBean>
        {
            private BookReviewAdapter bookReviewAdapter;
            private ViewGroup parent;
            private int item_community_book_review_list;

            public CustomViewHolder(BookReviewAdapter bookReviewAdapter, ViewGroup parent, int item_community_book_review_list)
                :base(parent, item_community_book_review_list)
            {
                this.bookReviewAdapter = bookReviewAdapter;
                this.parent = parent;
                this.item_community_book_review_list = item_community_book_review_list;
            }

            public override void setData(BookReviewList.ReviewsBean item)
            {
                if (!Settings.IsNoneCover)
                {
                    holder.setRoundImageUrl(Resource.Id.ivBookCover, Constant.IMG_BASE_URL + item.book.cover,
                            Resource.Drawable.cover_default);
                }
                else
                {
                    holder.setImageResource(Resource.Id.ivBookCover, Resource.Drawable.cover_default);
                }

                holder.setText(Resource.Id.tvBookTitle, item.book.title)
                        .setText(Resource.Id.tvBookType, Java.Lang.String.Format(mContext.GetString(Resource.String.book_review_book_type), Constant.bookType[item.book.type]))
                        .setText(Resource.Id.tvTitle, item.title)
                        .setText(Resource.Id.tvHelpfulYes, Java.Lang.String.Format(mContext.GetString(Resource.String.book_review_helpful_yes), item.helpful.yes));

                if (TextUtils.Equals(item.state, "hot"))
                {
                    holder.setVisible(Resource.Id.tvHot, true);
                    holder.setVisible(Resource.Id.tvTime, false);
                    holder.setVisible(Resource.Id.tvDistillate, false);
                }
                else if (TextUtils.Equals(item.state, "distillate"))
                {
                    holder.setVisible(Resource.Id.tvDistillate, true);
                    holder.setVisible(Resource.Id.tvHot, false);
                    holder.setVisible(Resource.Id.tvTime, false);
                }
                else
                {
                    holder.setVisible(Resource.Id.tvTime, true);
                    holder.setVisible(Resource.Id.tvHot, false);
                    holder.setVisible(Resource.Id.tvDistillate, false);
                    holder.setText(Resource.Id.tvTime, FormatUtils.getDescriptionTimeFromDateString(item.created));
                }
            }
        }
    }
}