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

namespace Xamarin.BookReader.UI.Activities
{
    /// <summary>
    /// 书评区详情
    /// </summary>
    public class BookReviewDetailActivity : BaseRVActivity<CommentList.CommentsBean>,
        IOnRvItemClickListener<CommentList.CommentsBean>
    {
        private static String INTENT_ID = "id";

        public static void startActivity(Context context, String id) {
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
            return R.layout.activity_community_book_discussion_detail;
        }
        public override void bindViews()
        {
            throw new NotImplementedException();
        }

        public override void initToolBar()
        {
            mCommonToolbar.setTitle("书评详情");
            mCommonToolbar.setNavigationIcon(R.drawable.ab_back);
        }
        public override void initDatas()
        {
            id = getIntent().getStringExtra(INTENT_ID);

            mPresenter.attachView(this);
            mPresenter.getBookReviewDetail(id);
            mPresenter.getBestComments(id);
            mPresenter.getBookReviewComments(id, start, limit);
        }
        public override void configViews()
        {
            initAdapter(CommentListAdapter, false, true);

            //mAdapter.addHeader(new RecyclerArrayAdapter.ItemView() {
            //    @Override
            //    public View onCreateView(ViewGroup parent) {
            //        View headerView =  LayoutInflater.from(BookReviewDetailActivity.this).inflate(R.layout.header_view_book_review_detail, parent, false);
            //        return headerView;
            //    }

            //    @Override
            //    public void onBindView(View headerView) {
            //        headerViewHolder = new HeaderViewHolder(headerView);
            //    }
            //});
        }

    public void showBookReviewDetail(final BookReview data) {
        Glide.with(mContext)
                .load(Constant.IMG_BASE_URL + data.review.author.avatar)
                .placeholder(R.drawable.avatar_default)
                .transform(new GlideCircleTransform(mContext))
                .into(headerViewHolder.ivAuthorAvatar);

        headerViewHolder.tvBookAuthor.setText(data.review.author.nickname);
        headerViewHolder.tvTime.setText(FormatUtils.getDescriptionTimeFromDateString(data.review.created));
        headerViewHolder.tvTitle.setText(data.review.title);
        headerViewHolder.tvContent.setText(data.review.content);

        Glide.with(mContext)
                .load(Constant.IMG_BASE_URL + data.review.book.cover)
                .placeholder(R.drawable.cover_default)
                .transform(new GlideRoundTransform(mContext))
                .into(headerViewHolder.ivBookCover);
        headerViewHolder.tvBookTitle.setText(data.review.book.title);

        headerViewHolder.tvHelpfullYesCount.setText(String.valueOf(data.review.helpful.yes));
        headerViewHolder.tvHelpfullNoCount.setText(String.valueOf(data.review.helpful.no));

        headerViewHolder.tvCommentCount.setText(String.format(mContext.getString(R.string.comment_comment_count), data.review.commentCount));

        //headerViewHolder.rlBookInfo.setOnClickListener(new View.OnClickListener() {
        //    @Override
        //    public void onClick(View v) {
        //        BookDetailActivity.startActivity(BookReviewDetailActivity.this, data.review.book._id);
        //    }
        //});

        headerViewHolder.ratingBar.setCountSelected(data.review.rating);
    }

    public void showBestComments(CommentList list)
    {
        if (list.comments.isEmpty())
        {
            gone(headerViewHolder.tvBestComments, headerViewHolder.rvBestComments);
        }
        else
        {
            mBestCommentList.addAll(list.comments);
            headerViewHolder.rvBestComments.setHasFixedSize(true);
            headerViewHolder.rvBestComments.setLayoutManager(new LinearLayoutManager(this));
            headerViewHolder.rvBestComments.addItemDecoration(new SupportDividerItemDecoration(mContext, LinearLayoutManager.VERTICAL, true));
            mBestCommentListAdapter = new BestCommentListAdapter(mContext, mBestCommentList);
            mBestCommentListAdapter.setOnItemClickListener(this);
            headerViewHolder.rvBestComments.setAdapter(mBestCommentListAdapter);
            visible(headerViewHolder.tvBestComments, headerViewHolder.rvBestComments);
        }
    }

    public void showBookReviewComments(CommentList list)
    {
        mAdapter.addAll(list.comments);
        start = start + list.comments.size();
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

        //class HeaderViewHolder
        //{
        //    @Bind(R.id.ivAuthorAvatar)
        //    ImageView ivAuthorAvatar;
        //    @Bind(R.id.tvBookAuthor)
        //    TextView tvBookAuthor;
        //    @Bind(R.id.tvTime)
        //    TextView tvTime;
        //    @Bind(R.id.tvTitle)
        //    TextView tvTitle;
        //    @Bind(R.id.tvContent)
        //    BookContentTextView tvContent;
        //    @Bind(R.id.rlBookInfo)
        //    RelativeLayout rlBookInfo;
        //    @Bind(R.id.ivBookCover)
        //    ImageView ivBookCover;
        //    @Bind(R.id.tvBookTitle)
        //    TextView tvBookTitle;
        //    @Bind(R.id.tvHelpfullYesCount)
        //    TextView tvHelpfullYesCount;
        //    @Bind(R.id.tvHelpfullNoCount)
        //    TextView tvHelpfullNoCount;
        //    @Bind(R.id.tvBestComments)
        //    TextView tvBestComments;
        //    @Bind(R.id.rvBestComments)
        //    RecyclerView rvBestComments;
        //    @Bind(R.id.tvCommentCount)
        //    TextView tvCommentCount;
        //    @Bind(R.id.rating)
        //    XLHRatingBar ratingBar;

        //        public HeaderViewHolder(View view)
        //        {
        //            ButterKnife.bind(this, view);   //view绑定
        //        }
        //    }
    }
}