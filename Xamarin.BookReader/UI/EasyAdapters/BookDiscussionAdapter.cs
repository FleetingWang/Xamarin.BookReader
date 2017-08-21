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
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Xamarin.BookReader.Utils;
using Android.Text;
using Xamarin.BookReader.Helpers;

namespace Xamarin.BookReader.UI.EasyAdapters
{
    public class BookDiscussionAdapter : RecyclerArrayAdapter<DiscussionList.PostsBean>
    {


        public BookDiscussionAdapter(Context context) : base(context)
        {
        }

        public override BaseViewHolder<DiscussionList.PostsBean> onCreateViewHolder(ViewGroup parent, int viewType)
        {
            return new CustomViewHolder(this, parent, Resource.Layout.item_community_book_discussion_list);
        }

        private class CustomViewHolder : BaseViewHolder<DiscussionList.PostsBean>
        {
            private BookDiscussionAdapter bookDiscussionAdapter;
            private ViewGroup parent;
            private int item_community_book_discussion_list;

            public CustomViewHolder(BookDiscussionAdapter bookDiscussionAdapter, ViewGroup parent, int item_community_book_discussion_list)
                : base(parent, item_community_book_discussion_list)
            {
                this.bookDiscussionAdapter = bookDiscussionAdapter;
                this.parent = parent;
                this.item_community_book_discussion_list = item_community_book_discussion_list;
            }
            public override void setData(DiscussionList.PostsBean item)
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
                        .setText(Resource.Id.tvBookType, String.Format(mContext.GetString(Resource.String.book_detail_user_lv), item.author.lv))
                        .setText(Resource.Id.tvTitle, item.title)
                        .setText(Resource.Id.tvHelpfulYes, item.commentCount + "")
                        .setText(Resource.Id.tvLikeCount, item.likeCount + "");

                try
                {
                    TextView textView = holder.getView<TextView>(Resource.Id.tvHelpfulYes);
                    if (item.type.Equals("vote"))
                    {
                        Drawable drawable = ContextCompat.GetDrawable(mContext, Resource.Drawable.ic_notif_vote);
                        drawable.SetBounds(0, 0, ScreenUtils.dpToPxInt(15), ScreenUtils.dpToPxInt(15));
                        textView.SetCompoundDrawables(drawable, null, null, null);
                    }
                    else
                    {
                        Drawable drawable = ContextCompat.GetDrawable(mContext, Resource.Drawable.ic_notif_post);
                        drawable.SetBounds(0, 0, ScreenUtils.dpToPxInt(15), ScreenUtils.dpToPxInt(15));
                        textView.SetCompoundDrawables(drawable, null, null, null);
                    }

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
                catch (Exception e)
                {
                    LogUtils.e(e.ToString());
                }
            }
        }
    }
}