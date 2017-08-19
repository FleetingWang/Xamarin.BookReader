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
using Xamarin.BookReader.UI.Listeners;
using Xamarin.BookReader.Views;
using Android.Support.V7.Widget;
using Xamarin.BookReader.Views.RecyclerViews.Adapters;
using Com.Bumptech.Glide;
using Com.Bumptech.Glide.Load.Resource.Bitmap;
using Xamarin.BookReader.Utils;

namespace Xamarin.BookReader.UI.Activities
{
    /// <summary>
    /// 综合讨论区详情
    /// </summary>
    public class BookDiscussionDetailActivity : BaseRVActivity<CommentList.CommentsBean>,
        IOnRvItemClickListener<CommentList.CommentsBean>
    {
        private static String INTENT_ID = "id";

        public static void startActivity(Context context, String id)
        {
            context.StartActivity(new Intent(context, typeof(BookDiscussionDetailActivity))
                .PutExtra(INTENT_ID, id));
        }

        private String id;
        private List<CommentList.CommentsBean> mBestCommentList = new List<CommentList.CommentsBean>();
        private BestCommentListAdapter mBestCommentListAdapter;
        private HeaderViewHolder headerViewHolder;

        public void showError()
        {

        }

        public void complete()
        {

        }




        public override int getLayoutId()
        {
            return Resource.Layout.activity_community_book_discussion_detail;
        }
        public override void bindViews()
        {

        }

        public override void initToolBar()
        {
            mCommonToolbar.Title = ("详情");
            mCommonToolbar.SetNavigationIcon(Resource.Drawable.ab_back);
        }
        public override void initDatas()
        {
            id = Intent.GetStringExtra(INTENT_ID);

            //TODO: mPresenter.attachView(this);
            //TODO: mPresenter.getBookDisscussionDetail(id);
            //TODO: mPresenter.getBestComments(id);
            //TODO: mPresenter.getBookDisscussionComments(id, start, limit);
        }
        public override void configViews()
        {
            initAdapter(CommentListAdapter, false, true);
            mAdapter.addHeader(new CustomItemView(this));
        }

        public void showBookDisscussionDetail(Disscussion disscussion)
        {
            Glide.With(mContext)
                    .Load(Constant.IMG_BASE_URL + disscussion.post.author.avatar)
                    .Placeholder(Resource.Drawable.avatar_default)
                    .Transform(new GlideCircleTransform(mContext))
                    .Into(headerViewHolder.ivAvatar);

            headerViewHolder.tvNickName.Text = (disscussion.post.author.nickname);
            headerViewHolder.tvTime.Text = (FormatUtils.getDescriptionTimeFromDateString(disscussion.post.created));
            headerViewHolder.tvTitle.Text = (disscussion.post.title);
            headerViewHolder.tvContent.Text = (disscussion.post.content);
            headerViewHolder.tvCommentCount.Text = (String.Format(mContext.GetString(Resource.String.comment_comment_count), disscussion.post.commentCount));
        }

        public void showBestComments(CommentList list)
        {
            if (!list.comments.Any())
            {
                gone(headerViewHolder.tvBestComments, headerViewHolder.rvBestComments);
            }
            else
            {
                mBestCommentList.AddRange(list.comments);
                headerViewHolder.rvBestComments.HasFixedSize = (true);
                headerViewHolder.rvBestComments.SetLayoutManager(new LinearLayoutManager(this));
                headerViewHolder.rvBestComments.AddItemDecoration(new SupportDividerItemDecoration(mContext, LinearLayoutManager.VERTICAL, true));
                mBestCommentListAdapter = new BestCommentListAdapter(mContext, mBestCommentList);
                mBestCommentListAdapter.setOnItemClickListener(this);
                headerViewHolder.rvBestComments.SetAdapter(mBestCommentListAdapter);
                visible(headerViewHolder.tvBestComments, headerViewHolder.rvBestComments);
            }
        }

        public void showBookDisscussionComments(CommentList list)
        {
            mAdapter.addAll(list.comments);
            start = start + list.comments.Count();
        }

        public override void onLoadMore()
        {
            base.onLoadMore();
            // TODO: mPresenter.getBookDisscussionComments(id, start, limit);
        }

        public void onItemClick(View view, int position, CommentList.CommentsBean data)
        {
        }

        public override void onItemClick(int position)
        {
            CommentList.CommentsBean data = mAdapter.getItem(position);
        }

        class HeaderViewHolder
        {
            public ImageView ivAvatar;
            public TextView tvNickName;
            public TextView tvTime;
            public TextView tvTitle;
            public BookContentTextView tvContent;
            public TextView tvBestComments;
            public RecyclerView rvBestComments;
            public TextView tvCommentCount;

            public HeaderViewHolder(View view)
            {
                ivAvatar = view.FindViewById<ImageView>(Resource.Id.ivBookCover);
                tvNickName = view.FindViewById<TextView>(Resource.Id.tvBookTitle);
                tvTime = view.FindViewById<TextView>(Resource.Id.tvTime);
                tvTitle = view.FindViewById<TextView>(Resource.Id.tvTitle);
                tvContent = view.FindViewById<BookContentTextView>(Resource.Id.tvContent);
                tvBestComments = view.FindViewById<TextView>(Resource.Id.tvBestComments);
                rvBestComments = view.FindViewById<RecyclerView>(Resource.Id.rvBestComments);
                tvCommentCount = view.FindViewById<TextView>(Resource.Id.tvCommentCount);
            }
        }

        private class CustomItemView : RecyclerArrayAdapter<CommentList.CommentsBean>.ItemView
        {
            private BookDiscussionDetailActivity bookDiscussionDetailActivity;

            public CustomItemView(BookDiscussionDetailActivity bookDiscussionDetailActivity)
            {
                this.bookDiscussionDetailActivity = bookDiscussionDetailActivity;
            }

            public void onBindView(View headerView)
            {
                bookDiscussionDetailActivity.headerViewHolder = new HeaderViewHolder(headerView);
            }

            public View onCreateView(ViewGroup parent)
            {
                View headerView = LayoutInflater.From(bookDiscussionDetailActivity).Inflate(Resource.Layout.header_view_book_discussion_detail, parent, false);
                return headerView;
            }
        }
    }
}