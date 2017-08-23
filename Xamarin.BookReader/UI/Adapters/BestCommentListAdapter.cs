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

namespace Xamarin.BookReader.UI.Adapters
{
    public class BestCommentListAdapter : EasyRVAdapter<CommentList.CommentsBean>
    {
        private IOnRvItemClickListener<CommentList.CommentsBean> listener;
        public BestCommentListAdapter(Context context, List<CommentList.CommentsBean> list)
            :base(context, list, Resource.Layout.item_comment_best_list)
        {

        }
        protected override void OnBindData(RecyclerView.ViewHolder viewHolder, int position, CommentList.CommentsBean item)
        {
            var holder = viewHolder as EasyRVHolder;
            holder.setCircleImageUrl(Resource.Id.ivBookCover, Constant.IMG_BASE_URL + item.author.avatar, Resource.Drawable.avatar_default)
                .setText(Resource.Id.tvBookTitle, item.author.nickname)
                .setText(Resource.Id.tvContent, item.content)
                .setText(Resource.Id.tvBookType, Java.Lang.String.Format(mContext.GetString(Resource.String.book_detail_user_lv), item.author.lv))
                .setText(Resource.Id.tvFloor, Java.Lang.String.Format(mContext.GetString(Resource.String.comment_floor), item.floor))
                .setText(Resource.Id.tvLikeCount, Java.Lang.String.Format(mContext.GetString(Resource.String.comment_like_count), item.likeCount));
            holder.Click += (sender, e) => {
                if (listener != null)
                    listener.onItemClick(viewHolder.ItemView, position, item);
            };
        }

        public void setOnItemClickListener(IOnRvItemClickListener<CommentList.CommentsBean> listener)
        {
            this.listener = listener;
        }
    }
}