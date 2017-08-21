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
using Xamarin.BookReader.Utils;

namespace Xamarin.BookReader.UI.EasyAdapters
{
    /// <summary>
    /// 帖子 评论、回复
    /// </summary>
    public class CommentListAdapter : RecyclerArrayAdapter<CommentList.CommentsBean>
    {

        public CommentListAdapter(Context context) : base(context)
        {
        }

        public override BaseViewHolder<CommentList.CommentsBean> onCreateViewHolder(ViewGroup parent, int viewType)
        {
            return new CustomViewHolder(this, parent, Resource.Layout.item_comment_list);
        }

        private class CustomViewHolder : BaseViewHolder<CommentList.CommentsBean>
        {
            private CommentListAdapter commentListAdapter;
            private ViewGroup parent;
            private int item_comment_list;

            public CustomViewHolder(CommentListAdapter commentListAdapter, ViewGroup parent, int item_comment_list)
                : base(parent, item_comment_list)
            {
                this.commentListAdapter = commentListAdapter;
                this.parent = parent;
                this.item_comment_list = item_comment_list;
            }

            public override void setData(CommentList.CommentsBean item)
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
                        .setText(Resource.Id.tvContent, item.content)
                        .setText(Resource.Id.tvBookType, String.Format(mContext.GetString(Resource.String.book_detail_user_lv), item.author.lv))
                        .setText(Resource.Id.tvFloor, String.Format(mContext.GetString(Resource.String.comment_floor), item.floor))
                        .setText(Resource.Id.tvTime, FormatUtils.getDescriptionTimeFromDateString(item.created));

                if (item.replyTo == null)
                {
                    holder.setVisible(Resource.Id.tvReplyNickName, false);
                    holder.setVisible(Resource.Id.tvReplyFloor, false);
                }
                else
                {
                    holder.setText(Resource.Id.tvReplyNickName, String.Format(mContext.GetString(Resource.String.comment_reply_nickname), item.replyTo.author.nickname))
                            .setText(Resource.Id.tvReplyFloor, String.Format(mContext.GetString(Resource.String.comment_reply_floor), item.replyTo.floor));
                    holder.setVisible(Resource.Id.tvReplyNickName, true);
                    holder.setVisible(Resource.Id.tvReplyFloor, true);
                }
            }
        }
    }
}