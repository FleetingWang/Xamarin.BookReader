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
using Xamarin.BookReader.UI.Listeners;
using Xamarin.BookReader.Models;
using Xamarin.BookReader.Views;
using Android.Support.V7.Widget;

namespace Xamarin.BookReader.UI.Activities
{
    public class BookDetailActivity: BaseActivity, IOnRvItemClickListener<Object>
    {


    //@Bind(R.id.ivBookCover)
    ImageView mIvBookCover;
    //@Bind(R.id.tvBookListTitle)
    TextView mTvBookTitle;
    //@Bind(R.id.tvBookListAuthor)
    TextView mTvAuthor;
    //@Bind(R.id.tvCatgory)
    TextView mTvCatgory;
    //@Bind(R.id.tvWordCount)
    TextView mTvWordCount;
    //@Bind(R.id.tvLatelyUpdate)
    TextView mTvLatelyUpdate;
    //@Bind(R.id.btnRead)
    DrawableCenterButton mBtnRead;
    //@Bind(R.id.btnJoinCollection)
    DrawableCenterButton mBtnJoinCollection;
    //@Bind(R.id.tvLatelyFollower)
    TextView mTvLatelyFollower;
    //@Bind(R.id.tvRetentionRatio)
    TextView mTvRetentionRatio;
    //@Bind(R.id.tvSerializeWordCount)
    TextView mTvSerializeWordCount;
    //@Bind(R.id.tag_group)
    TagGroup mTagGroup;
    //@Bind(R.id.tvlongIntro)
    TextView mTvlongIntro;
    //@Bind(R.id.tvMoreReview)
    TextView mTvMoreReview;
    //@Bind(R.id.rvHotReview)
    RecyclerView mRvHotReview;
    //@Bind(R.id.rlCommunity)
    RelativeLayout mRlCommunity;
    //@Bind(R.id.tvCommunity)
    TextView mTvCommunity;
    //@Bind(R.id.tvHelpfulYes)
    TextView mTvPostCount;
    //@Bind(R.id.tvRecommendBookList)
    TextView mTvRecommendBookList;

    //@Bind(R.id.rvRecommendBoookList)
    RecyclerView mRvRecommendBoookList;

        public static String INTENT_BOOK_ID = "bookId";
        public static void startActivity(Context context, String bookId)
        {
            context.StartActivity(new Intent(context, typeof(BookDetailActivity))
                .PutExtra(INTENT_BOOK_ID, bookId));
        }

        private List<String> tagList = new List<String>();
        private int times = 0;

        private HotReviewAdapter mHotReviewAdapter;
        private List<HotReview.Reviews> mHotReviewList = new List<HotReview.Reviews>();
        private RecommendBookListAdapter mRecommendBookListAdapter;
        private List<RecommendBookList.RecommendBook> mRecommendBookList = new List<RecommendBookList.RecommendBook>();
        private String bookId;

        private boolean collapseLongIntro = true;
        private Recommend.RecommendBooks recommendBooks;
        private boolean isJoinedCollections = false;

        public override int getLayoutId()
        {
            return R.layout.activity_book_detail;
        }

        public override void bindViews()
        {
            throw new NotImplementedException();
        }

        public override void initToolBar()
        {
            mCommonToolbar.setNavigationIcon(R.drawable.ab_back);
            mCommonToolbar.setTitle(R.string.book_detail);
        }

        public override void initDatas()
        {
            bookId = getIntent().getStringExtra(INTENT_BOOK_ID);
            EventBus.getDefault().register(this);
        }

        public override void configViews()
        {
            mRvHotReview.setHasFixedSize(true);
            mRvHotReview.setLayoutManager(new LinearLayoutManager(this));
            mHotReviewAdapter = new HotReviewAdapter(mContext, mHotReviewList, this);
            mRvHotReview.setAdapter(mHotReviewAdapter);

            mRvRecommendBoookList.setHasFixedSize(true);
            mRvRecommendBoookList.setLayoutManager(new LinearLayoutManager(this));
            mRecommendBookListAdapter = new RecommendBookListAdapter(mContext, mRecommendBookList, this);
            mRvRecommendBoookList.setAdapter(mRecommendBookListAdapter);

            //mTagGroup.setOnTagClickListener(new TagGroup.OnTagClickListener() {
            //@Override
            //public void onTagClick(String tag) {
            //    startActivity(new Intent(BookDetailActivity.this, BooksByTagActivity.class)
            //            .putExtra("tag", tag));
            //    }
            //});

            //mPresenter.attachView(this);
            //mPresenter.getBookDetail(bookId);
            //mPresenter.getHotReview(bookId);
            //mPresenter.getRecommendBookList(bookId, "3");
        }

        public void showBookDetail(BookDetail data)
        {
            Glide.with(mContext)
                .load(Constant.IMG_BASE_URL + data.cover)
                .placeholder(R.drawable.cover_default)
                .transform(new GlideRoundTransform(mContext))
                .into(mIvBookCover);

            mTvBookTitle.setText(data.title);
            mTvAuthor.setText(String.format(getString(R.string.book_detail_author), data.author));
            mTvCatgory.setText(String.format(getString(R.string.book_detail_category), data.cat));
            mTvWordCount.setText(FormatUtils.formatWordCount(data.wordCount));
            mTvLatelyUpdate.setText(FormatUtils.getDescriptionTimeFromDateString(data.updated));
            mTvLatelyFollower.setText(String.valueOf(data.latelyFollower));
            mTvRetentionRatio.setText(TextUtils.isEmpty(data.retentionRatio) ?
                    "-" : String.format(getString(R.string.book_detail_retention_ratio),
                    data.retentionRatio));
            mTvSerializeWordCount.setText(data.serializeWordCount < 0 ? "-" :
                    String.valueOf(data.serializeWordCount));

            tagList.clear();
            tagList.addAll(data.tags);
            times = 0;
            showHotWord();

            mTvlongIntro.setText(data.longIntro);
            mTvCommunity.setText(String.format(getString(R.string.book_detail_community), data.title));
            mTvPostCount.setText(String.format(getString(R.string.book_detail_post_count), data.postCount));

            recommendBooks = new Recommend.RecommendBooks();
            recommendBooks.title = data.title;
            recommendBooks._id = data._id;
            recommendBooks.cover = data.cover;
            recommendBooks.lastChapter = data.lastChapter;
            recommendBooks.updated = data.updated;

            refreshCollectionIcon();
        }

        /// <summary>
        /// 刷新收藏图标
        /// </summary>
        private void refreshCollectionIcon() {
            if (CollectionsManager.getInstance().isCollected(recommendBooks._id)) {
                initCollection(false);
            } else {
                initCollection(true);
            }
        }

        //@Subscribe(threadMode = ThreadMode.MAIN)
        //public void RefreshCollectionIcon(RefreshCollectionIconEvent event) {
        //    refreshCollectionIcon();
        //}

        /// <summary>
        /// 每次显示8个
        /// </summary>
        private void showHotWord()
        {
            int start, end;
            if (times < tagList.size() && times + 8 <= tagList.size())
            {
                start = times;
                end = times + 8;
            }
            else if (times < tagList.size() - 1 && times + 8 > tagList.size())
            {
                start = times;
                end = tagList.size() - 1;
            }
            else
            {
                start = 0;
                end = tagList.size() > 8 ? 8 : tagList.size();
            }
            times = end;
            if (end - start > 0)
            {
                List<String> batch = tagList.subList(start, end);
                List<TagColor> colors = TagColor.getRandomColors(batch.size());
                mTagGroup.setTags(colors, (String[])batch.toArray(new String[batch.size()]));
            }
        }

        public void showHotReview(List<HotReview.Reviews> list)
        {
            mHotReviewList.clear();
            mHotReviewList.addAll(list);
            mHotReviewAdapter.notifyDataSetChanged();
        }

        public void showRecommendBookList(List<RecommendBookList.RecommendBook> list)
        {
            if (!list.isEmpty())
            {
                mTvRecommendBookList.setVisibility(View.VISIBLE);
                mRecommendBookList.clear();
                mRecommendBookList.addAll(list);
                mRecommendBookListAdapter.notifyDataSetChanged();
            }
        }

        public void onItemClick(View view, int position, object data)
        {
            if (data instanceof HotReview.Reviews) {
                BookDiscussionDetailActivity.startActivity(this, ((HotReview.Reviews)data)._id);
            } else if (data instanceof RecommendBookList.RecommendBook) {
                RecommendBookList.RecommendBook recommendBook = (RecommendBookList.RecommendBook)data;

                BookLists bookLists = new BookLists();
                BookLists.BookListsBean bookListsBean = bookLists.new BookListsBean();
                bookListsBean._id = recommendBook.id;
                bookListsBean.author = recommendBook.author;
                bookListsBean.bookCount = recommendBook.bookCount;
                bookListsBean.collectorCount = recommendBook.collectorCount;
                bookListsBean.cover = recommendBook.cover;
                bookListsBean.desc = recommendBook.desc;
                bookListsBean.title = recommendBook.title;

                SubjectBookListDetailActivity.startActivity(this, bookListsBean);
            }
        }

        //TODO: @OnClick(R.id.btnJoinCollection)
        public void onClickJoinCollection()
        {
            if (!isJoinedCollections)
            {
                if (recommendBooks != null)
                {
                    CollectionsManager.getInstance().add(recommendBooks);
                    ToastUtils.showToast(String.format(getString(
                            R.string.book_detail_has_joined_the_book_shelf), recommendBooks.title));
                    initCollection(false);
                }
            }
            else
            {
                CollectionsManager.getInstance().remove(recommendBooks._id);
                ToastUtils.showToast(String.format(getString(
                        R.string.book_detail_has_remove_the_book_shelf), recommendBooks.title));
                initCollection(true);
            }
        }

        private void initCollection(boolean coll)
        {
            if (coll)
            {
                mBtnJoinCollection.setText(R.string.book_detail_join_collection);
                Drawable drawable = ContextCompat.getDrawable(this, R.drawable.book_detail_info_add_img);
                drawable.setBounds(0, 0, drawable.getMinimumWidth(), drawable.getMinimumHeight());
                mBtnJoinCollection.setBackgroundDrawable(ContextCompat.getDrawable(this, R.drawable.shape_common_btn_solid_normal));
                mBtnJoinCollection.setCompoundDrawables(drawable, null, null, null);
                mBtnJoinCollection.postInvalidate();
                isJoinedCollections = false;
            }
            else
            {
                mBtnJoinCollection.setText(R.string.book_detail_remove_collection);
                Drawable drawable = ContextCompat.getDrawable(this, R.drawable.book_detail_info_del_img);
                drawable.setBounds(0, 0, drawable.getMinimumWidth(), drawable.getMinimumHeight());
                mBtnJoinCollection.setBackgroundDrawable(ContextCompat.getDrawable(this, R.drawable.btn_join_collection_pressed));
                mBtnJoinCollection.setCompoundDrawables(drawable, null, null, null);
                mBtnJoinCollection.postInvalidate();
                isJoinedCollections = true;
            }
        }



        //TODO: @OnClick(R.id.btnRead)
        public void onClickRead()
        {
            if (recommendBooks == null) return;
            ReadActivity.startActivity(this, recommendBooks);
        }

        //TODO: @OnClick(R.id.tvBookListAuthor)
        public void searchByAuthor()
        {
            String author = mTvAuthor.getText().toString().replaceAll(" ", "");
            SearchByAuthorActivity.startActivity(this, author);
        }

        //TODO: @OnClick(R.id.tvlongIntro)
        public void collapseLongIntro()
        {
            if (collapseLongIntro)
            {
                mTvlongIntro.setMaxLines(20);
                collapseLongIntro = false;
            }
            else
            {
                mTvlongIntro.setMaxLines(4);
                collapseLongIntro = true;
            }
        }

        //TODO: @OnClick(R.id.tvMoreReview)
        public void onClickMoreReview()
        {
            BookDetailCommunityActivity.startActivity(this, bookId, mTvBookTitle.getText().toString(), 1);
        }

        //TODO: @OnClick(R.id.rlCommunity)
        public void onClickCommunity()
        {
            BookDetailCommunityActivity.startActivity(this, bookId, mTvBookTitle.getText().toString(), 0);
        }

        public void showError()
        {

        }

        public void complete()
        {

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            EventBus.getDefault().unregister(this);
        }
    }
}