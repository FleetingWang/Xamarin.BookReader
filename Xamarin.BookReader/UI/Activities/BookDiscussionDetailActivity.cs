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
    private List<CommentList.CommentsBean> mBestCommentList = new ArrayList<>();
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
            mCommonToolbar.setTitle("详情");
            mCommonToolbar.setNavigationIcon(R.drawable.ab_back);
        }
        public override void initDatas()
        {
            id = getIntent().getStringExtra(INTENT_ID);

            mPresenter.attachView(this);
            mPresenter.getBookDisscussionDetail(id);
            mPresenter.getBestComments(id);
            mPresenter.getBookDisscussionComments(id, start, limit);
        }
        public override void configViews()
        {
            initAdapter(CommentListAdapter, false, true);

            //mAdapter.addHeader(new RecyclerArrayAdapter.ItemView() {
            //    @Override
            //    public View onCreateView(ViewGroup parent) {
            //        View headerView = LayoutInflater.from(BookDiscussionDetailActivity.this).inflate(R.layout.header_view_book_discussion_detail, parent, false);
            //        return headerView;
            //    }

            //    @Override
            //    public void onBindView(View headerView) {
            //        headerViewHolder = new HeaderViewHolder(headerView);
            //    }
            //});
        }

    public void showBookDisscussionDetail(Disscussion disscussion)
    {
        Glide.with(mContext)
                .load(Constant.IMG_BASE_URL + disscussion.post.author.avatar)
                .placeholder(R.drawable.avatar_default)
                .transform(new GlideCircleTransform(mContext))
                .into(headerViewHolder.ivAvatar);

        headerViewHolder.tvNickName.setText(disscussion.post.author.nickname);
        headerViewHolder.tvTime.setText(FormatUtils.getDescriptionTimeFromDateString(disscussion.post.created));
        headerViewHolder.tvTitle.setText(disscussion.post.title);
        headerViewHolder.tvContent.setText(disscussion.post.content);
        headerViewHolder.tvCommentCount.setText(String.format(mContext.getString(R.string.comment_comment_count), disscussion.post.commentCount));
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

        public void showBookDisscussionComments(CommentList list)
        {
            mAdapter.addAll(list.comments);
            start = start + list.comments.size();
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

        //class HeaderViewHolder {
        //    @Bind(R.id.ivBookCover)
        //    ImageView ivAvatar;
        //    @Bind(R.id.tvBookTitle)
        //    TextView tvNickName;
        //    @Bind(R.id.tvTime)
        //    TextView tvTime;
        //    @Bind(R.id.tvTitle)
        //    TextView tvTitle;
        //    @Bind(R.id.tvContent)
        //    BookContentTextView tvContent;
        //    @Bind(R.id.tvBestComments)
        //    TextView tvBestComments;
        //    @Bind(R.id.rvBestComments)
        //    RecyclerView rvBestComments;
        //    @Bind(R.id.tvCommentCount)
        //    TextView tvCommentCount;

        //    public HeaderViewHolder(View view) {
        //        ButterKnife.bind(this, view);   //view绑定
        //    }
        //}
    }
}