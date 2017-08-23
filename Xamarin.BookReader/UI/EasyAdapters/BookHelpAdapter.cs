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
    public class BookHelpAdapter : RecyclerArrayAdapter<BookHelpList.HelpsBean>
    {


        public BookHelpAdapter(Context context) : base(context)
        {
        }

        public override BaseViewHolder<BookHelpList.HelpsBean> onCreateViewHolder(ViewGroup parent, int viewType)
        {
            return new CustomViewHolder(this, parent, Resource.Layout.item_community_book_help_list);
        }

        private class CustomViewHolder : BaseViewHolder<BookHelpList.HelpsBean>
        {
            private BookHelpAdapter bookHelpAdapter;
            private ViewGroup parent;
            private int item_community_book_help_list;

            public CustomViewHolder(BookHelpAdapter bookHelpAdapter, ViewGroup parent, int item_community_book_help_list)
                : base(parent, item_community_book_help_list)
            {
                this.bookHelpAdapter = bookHelpAdapter;
                this.parent = parent;
                this.item_community_book_help_list = item_community_book_help_list;
            }
            public override void setData(BookHelpList.HelpsBean item)
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

                holder.setText(Resource.Id.tvBookType, Java.Lang.String.Format(mContext.GetString(Resource.String
                        .book_detail_user_lv), item.author.lv))
                        .setText(Resource.Id.tvTitle, item.title)
                        .setText(Resource.Id.tvHelpfulYes, item.commentCount + "");

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