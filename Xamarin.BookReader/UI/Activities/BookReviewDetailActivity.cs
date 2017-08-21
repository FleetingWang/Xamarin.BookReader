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
using Xamarin.BookReader.UI.Adapters;
using Xamarin.BookReader.UI.EasyAdapters;
using Xamarin.BookReader.Datas;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Xamarin.BookReader.UI.Activities
{
    /// <summary>
    /// 书评区详情
    /// </summary>
    public class BookReviewDetailActivity : BaseRVActivity<CommentList.CommentsBean>,
        IOnRvItemClickListener<CommentList.CommentsBean>
    {
        private static String INTENT_ID = "id";

        public static void startActivity(Context context, String id)
        {
            context.StartActivity(new Intent(context, typeof(BookReviewDetailActivity))
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
            throw new NotImplementedException();
        }

        public override void initToolBar()
        {
            mCommonToolbar.Title = ("书评详情");
            mCommonToolbar.SetNavigationIcon(Resource.Drawable.ab_back);
        }
        public override void initDatas()
        {
            id = Intent.GetStringExtra(INTENT_ID);

            getBookReviewDetail(id);
            getBestComments(id);
            getBookReviewComments(id, start, limit);
        }
        public override void configViews()
        {
            initAdapter(new CommentListAdapter(this), false, true);
            mAdapter.addHeader(new CustomItemView(this));
        }
        void getBookReviewDetail(String id)
        {
            BookApi.Instance.getBookReviewDetail(id)
                .SubscribeOn(DefaultScheduler.Instance)
                .ObserveOn(Application.SynchronizationContext)
                .Subscribe(data => {
                    showBookReviewDetail(data);
                }, e => {
                    LogUtils.e("BookReviewDetailActivity", e.ToString());
                }, () => {

                });
        }
        void getBestComments(String disscussionId)
        {
            BookApi.Instance.getBestComments(disscussionId)
                .SubscribeOn(DefaultScheduler.Instance)
                .ObserveOn(Application.SynchronizationContext)
                .Subscribe(data => {
                    showBestComments(data);
                }, e => {
                    LogUtils.e("BookReviewDetailActivity", e.ToString());
                }, () => {

                });
        }
        void getBookReviewComments(String bookReviewId, int start, int limit)
        {
            BookApi.Instance.getBookReviewComments(bookReviewId, start.ToString(), limit.ToString())
                .SubscribeOn(DefaultScheduler.Instance)
                .ObserveOn(Application.SynchronizationContext)
                .Subscribe(data => {
                    showBookReviewComments(data);
                }, e => {
                    LogUtils.e("BookReviewDetailActivity", e.ToString());
                }, () => {

                });
        }


        public void showBookReviewDetail(BookReview data)
        {
            Glide.With(mContext)
                    .Load(Constant.IMG_BASE_URL + data.review.author.avatar)
                    .Placeholder(Resource.Drawable.avatar_default)
                    .Transform(new GlideCircleTransform(mContext))
                    .Into(headerViewHolder.ivAuthorAvatar);

            headerViewHolder.tvBookAuthor.Text = (data.review.author.nickname);
            headerViewHolder.tvTime.Text = (FormatUtils.getDescriptionTimeFromDateString(data.review.created));
            headerViewHolder.tvTitle.Text = (data.review.title);
            headerViewHolder.tvContent.Text = (data.review.content);

            Glide.With(mContext)
                    .Load(Constant.IMG_BASE_URL + data.review.book.cover)
                    .Placeholder(Resource.Drawable.cover_default)
                    .Transform(new GlideRoundTransform(mContext))
                    .Into(headerViewHolder.ivBookCover);
            headerViewHolder.tvBookTitle.Text = (data.review.book.title);

            headerViewHolder.tvHelpfullYesCount.Text = data.review.helpful.yes.ToString();
            headerViewHolder.tvHelpfullNoCount.Text = data.review.helpful.no.ToString();

            headerViewHolder.tvCommentCount.Text = (String.Format(mContext.GetString(Resource.String.comment_comment_count), data.review.commentCount));
            headerViewHolder.rlBookInfo.Click += (sender, e) =>
            {
                BookDetailActivity.startActivity(this, data.review.book._id);
            };

            headerViewHolder.ratingBar.setCountSelected(data.review.rating);
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
                headerViewHolder.rvBestComments.AddItemDecoration(new SupportDividerItemDecoration(mContext, LinearLayoutManager.Vertical, true));
                mBestCommentListAdapter = new BestCommentListAdapter(mContext, mBestCommentList);
                mBestCommentListAdapter.setOnItemClickListener(this);
                headerViewHolder.rvBestComments.SetAdapter(mBestCommentListAdapter);
                visible(headerViewHolder.tvBestComments, headerViewHolder.rvBestComments);
            }
        }

        public void showBookReviewComments(CommentList list)
        {
            mAdapter.addAll(list.comments);
            start = start + list.comments.Count();
        }

        public override void onLoadMore()
        {
            base.onLoadMore();
            getBookReviewComments(id, start, limit);
        }

        public override void onItemClick(int position)
        {
            CommentList.CommentsBean data = mAdapter.getItem(position);
        }

        public void onItemClick(View view, int position, CommentList.CommentsBean data)
        {
        }

        class HeaderViewHolder
        {
            public ImageView ivAuthorAvatar;
            public TextView tvBookAuthor;
            public TextView tvTime;
            public TextView tvTitle;
            public BookContentTextView tvContent;
            public RelativeLayout rlBookInfo;
            public ImageView ivBookCover;
            public TextView tvBookTitle;
            public TextView tvHelpfullYesCount;
            public TextView tvHelpfullNoCount;
            public TextView tvBestComments;
            public RecyclerView rvBestComments;
            public TextView tvCommentCount;
            public XLHRatingBar ratingBar;


            public HeaderViewHolder(View view)
            {
                ivAuthorAvatar = view.FindViewById<ImageView>(Resource.Id.ivAuthorAvatar);
                tvBookAuthor = view.FindViewById<TextView>(Resource.Id.tvBookAuthor);
                tvTime = view.FindViewById<TextView>(Resource.Id.tvTime);
                tvTitle = view.FindViewById<TextView>(Resource.Id.tvTitle);
                tvContent = view.FindViewById<BookContentTextView>(Resource.Id.tvContent);
                rlBookInfo = view.FindViewById<RelativeLayout>(Resource.Id.rlBookInfo);
                ivBookCover = view.FindViewById<ImageView>(Resource.Id.ivBookCover);
                tvBookTitle = view.FindViewById<TextView>(Resource.Id.tvBookTitle);
                tvHelpfullYesCount = view.FindViewById<TextView>(Resource.Id.tvHelpfullYesCount);
                tvHelpfullNoCount = view.FindViewById<TextView>(Resource.Id.tvHelpfullNoCount);
                tvBestComments = view.FindViewById<TextView>(Resource.Id.tvBestComments);
                rvBestComments = view.FindViewById<RecyclerView>(Resource.Id.rvBestComments);
                tvCommentCount = view.FindViewById<TextView>(Resource.Id.tvCommentCount);
                ratingBar = view.FindViewById<XLHRatingBar>(Resource.Id.rating);
                ivAuthorAvatar = view.FindViewById<ImageView>(Resource.Id.ivAuthorAvatar);
            }
        }
        private class CustomItemView : RecyclerArrayAdapter<CommentList.CommentsBean>.ItemView
        {
            private BookReviewDetailActivity bookReviewDetailActivity;

            public CustomItemView(BookReviewDetailActivity bookReviewDetailActivity)
            {
                this.bookReviewDetailActivity = bookReviewDetailActivity;
            }

            public void onBindView(View headerView)
            {
                bookReviewDetailActivity.headerViewHolder = new HeaderViewHolder(headerView);
            }

            public View onCreateView(ViewGroup parent)
            {
                View headerView = LayoutInflater.From(bookReviewDetailActivity).Inflate(Resource.Layout.header_view_book_review_detail, parent, false);
                return headerView;
            }
        }
    }
}