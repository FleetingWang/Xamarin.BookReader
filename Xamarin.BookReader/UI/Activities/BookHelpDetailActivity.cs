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
    /// 书荒互助区详情
    /// </summary>
    public class BookHelpDetailActivity : BaseRVActivity<CommentList.CommentsBean>,
        IOnRvItemClickListener<CommentList.CommentsBean>
    {
         private static String INTENT_ID = "id";

    public static void startActivity(Context context, String id) {
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
            return R.layout.activity_community_book_discussion_detail;
        }
        public override void bindViews()
        {
            throw new NotImplementedException();
        }

        public override void initToolBar()
        {
            mCommonToolbar.setTitle("书荒互助区详情");
            mCommonToolbar.setNavigationIcon(R.drawable.ab_back);
        }
        public override void initDatas()
        {
            id = getIntent().getStringExtra(INTENT_ID);

            mPresenter.attachView(this);
            mPresenter.getBookHelpDetail(id);
            mPresenter.getBestComments(id);
            mPresenter.getBookHelpComments(id, start, limit);
        }
        public override void configViews()
        {
            initAdapter(CommentListAdapter, false, true);

            //mAdapter.addHeader(new RecyclerArrayAdapter.ItemView() {
            //    @Override
            //    public View onCreateView(ViewGroup parent) {
            //        View headerView = LayoutInflater.from(BookHelpDetailActivity.this).inflate(R.layout.header_view_book_discussion_detail, parent, false);
            //        return headerView;
            //    }

            //    @Override
            //    public void onBindView(View headerView) {
            //        headerViewHolder = new HeaderViewHolder(headerView);
            //    }
            //});
        }

    public void showBookHelpDetail(BookHelp data)
    {
        Glide.with(mContext).load(Constant.IMG_BASE_URL + data.help.author.avatar)
                .placeholder(R.drawable.avatar_default)
                .transform(new GlideCircleTransform(mContext))
                .into(headerViewHolder.ivAvatar);

        headerViewHolder.tvNickName.setText(data.help.author.nickname);
        headerViewHolder.tvTime.setText(FormatUtils.getDescriptionTimeFromDateString(data.help.created));
        headerViewHolder.tvTitle.setText(data.help.title);
        headerViewHolder.tvContent.setText(data.help.content);
        headerViewHolder.tvCommentCount.setText(String.format(mContext.getString(R.string.comment_comment_count), data.help.commentCount));
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

    public void showBookHelpComments(CommentList list)
    {
        mAdapter.addAll(list.comments);
        start = start + list.comments.size();
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