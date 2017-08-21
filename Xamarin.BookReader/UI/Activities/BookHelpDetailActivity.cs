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
using System.Reactive.Concurrency;
using Xamarin.BookReader.Datas;
using System.Reactive.Linq;

namespace Xamarin.BookReader.UI.Activities
{
    /// <summary>
    /// 书荒互助区详情
    /// </summary>
    public class BookHelpDetailActivity : BaseRVActivity<CommentList.CommentsBean>,
        IOnRvItemClickListener<CommentList.CommentsBean>
    {
        private static String INTENT_ID = "id";

        public static void startActivity(Context context, String id)
        {
            context.StartActivity(new Intent(context, typeof(BookHelpDetailActivity))
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
            mCommonToolbar.Title = ("书荒互助区详情");
            mCommonToolbar.SetNavigationIcon(Resource.Drawable.ab_back);
        }
        public override void initDatas()
        {
            id = Intent.GetStringExtra(INTENT_ID);

            getBookHelpDetail(id);
            getBestComments(id);
            getBookHelpComments(id, start, limit);
        }
        public override void configViews()
        {
            initAdapter(new CommentListAdapter(this), false, true);
            mAdapter.addHeader(new CustomItemView(this));
        }
        void getBookHelpDetail(String id)
        {
            BookApi.Instance.getBookHelpDetail(id)
                .SubscribeOn(DefaultScheduler.Instance)
                .ObserveOn(Application.SynchronizationContext)
                .Subscribe(data => {
                    showBookHelpDetail(data);
                }, e => {
                    LogUtils.e("BookHelpDetailActivity", e.ToString());
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
                    LogUtils.e("BookHelpDetailActivity", e.ToString());
                }, () => {

                });
        }
        void getBookHelpComments(String disscussionId, int start, int limit)
        {
            BookApi.Instance.getBookReviewComments(disscussionId, start.ToString(), limit.ToString())
                .SubscribeOn(DefaultScheduler.Instance)
                .ObserveOn(Application.SynchronizationContext)
                .Subscribe(data => {
                    showBookHelpComments(data);
                }, e => {
                    LogUtils.e("BookDiscussionDetailActivity", e.ToString());
                }, () => {

                });
        }

        public void showBookHelpDetail(BookHelp data)
        {
            Glide.With(mContext).Load(Constant.IMG_BASE_URL + data.help.author.avatar)
                    .Placeholder(Resource.Drawable.avatar_default)
                    .Transform(new GlideCircleTransform(mContext))
                    .Into(headerViewHolder.ivAvatar);

            headerViewHolder.tvNickName.Text = (data.help.author.nickname);
            headerViewHolder.tvTime.Text = (FormatUtils.getDescriptionTimeFromDateString(data.help.created));
            headerViewHolder.tvTitle.Text = (data.help.title);
            headerViewHolder.tvContent.Text = (data.help.content);
            headerViewHolder.tvCommentCount.Text = (String.Format(mContext.GetString(Resource.String.comment_comment_count), data.help.commentCount));
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

        public void showBookHelpComments(CommentList list)
        {
            mAdapter.addAll(list.comments);
            start = start + list.comments.Count();
        }

        public override void onLoadMore()
        {
            base.onLoadMore();
            getBookHelpComments(id, start, limit);
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
            private BookHelpDetailActivity bookHelpDetailActivity;

            public CustomItemView(BookHelpDetailActivity bookHelpDetailActivity)
            {
                this.bookHelpDetailActivity = bookHelpDetailActivity;
            }

            public void onBindView(View headerView)
            {
                bookHelpDetailActivity.headerViewHolder = new HeaderViewHolder(headerView);
            }

            public View onCreateView(ViewGroup parent)
            {
                View headerView = LayoutInflater.From(bookHelpDetailActivity).Inflate(Resource.Layout.header_view_book_discussion_detail, parent, false);
                return headerView;
            }
        }
    }
}