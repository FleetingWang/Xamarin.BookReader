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
using DSoft.Messaging;
using Xamarin.BookReader.Models.Support;
using Com.Bumptech.Glide;
using Com.Bumptech.Glide.Load.Resource.Bitmap;
using Xamarin.BookReader.Utils;
using Android.Text;
using Xamarin.BookReader.Managers;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Xamarin.BookReader.UI.Adapters;

namespace Xamarin.BookReader.UI.Activities
{
    public class BookDetailActivity : BaseActivity, IOnRvItemClickListener<Object>
    {
        ImageView mIvBookCover;
        TextView mTvBookTitle;
        TextView mTvAuthor;
        TextView mTvCatgory;
        TextView mTvWordCount;
        TextView mTvLatelyUpdate;
        DrawableCenterButton mBtnRead;
        DrawableCenterButton mBtnJoinCollection;
        TextView mTvLatelyFollower;
        TextView mTvRetentionRatio;
        TextView mTvSerializeWordCount;
        TagGroup mTagGroup;
        TextView mTvlongIntro;
        TextView mTvMoreReview;
        RecyclerView mRvHotReview;
        RelativeLayout mRlCommunity;
        TextView mTvCommunity;
        TextView mTvPostCount;
        TextView mTvRecommendBookList;

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

        private bool collapseLongIntro = true;
        private Recommend.RecommendBooks recommendBooks;
        private bool isJoinedCollections = false;

        public override int getLayoutId()
        {
            return Resource.Layout.activity_book_detail;
        }

        public override void bindViews()
        {
            mIvBookCover = FindViewById<ImageView>(Resource.Id.ivBookCover);
            mTvBookTitle = FindViewById<TextView>(Resource.Id.tvBookListTitle);
            mTvAuthor = FindViewById<TextView>(Resource.Id.tvBookListAuthor);
            mTvCatgory = FindViewById<TextView>(Resource.Id.tvCatgory);
            mTvWordCount = FindViewById<TextView>(Resource.Id.tvWordCount);
            mTvLatelyUpdate = FindViewById<TextView>(Resource.Id.tvLatelyUpdate);
            mBtnRead = FindViewById<DrawableCenterButton>(Resource.Id.btnRead);
            mBtnJoinCollection = FindViewById<DrawableCenterButton>(Resource.Id.btnJoinCollection);
            mTvLatelyFollower = FindViewById<TextView>(Resource.Id.tvLatelyFollower);
            mTvRetentionRatio = FindViewById<TextView>(Resource.Id.tvRetentionRatio);
            mTvSerializeWordCount = FindViewById<TextView>(Resource.Id.tvSerializeWordCount);
            mTagGroup = FindViewById<TagGroup>(Resource.Id.tag_group);
            mTvlongIntro = FindViewById<TextView>(Resource.Id.tvlongIntro);
            mTvMoreReview = FindViewById<TextView>(Resource.Id.tvMoreReview);
            mRvHotReview = FindViewById<RecyclerView>(Resource.Id.rvHotReview);
            mRlCommunity = FindViewById<RelativeLayout>(Resource.Id.rlCommunity);
            mTvCommunity = FindViewById<TextView>(Resource.Id.tvCommunity);
            mTvPostCount = FindViewById<TextView>(Resource.Id.tvHelpfulYes);
            mTvRecommendBookList = FindViewById<TextView>(Resource.Id.tvRecommendBookList);
            mRvRecommendBoookList = FindViewById<RecyclerView>(Resource.Id.rvRecommendBoookList);

            mBtnRead.Click += (sender, e) => onClickRead();
            mBtnJoinCollection.Click += (sender, e) => onClickJoinCollection();
            mTvAuthor.Click += (sender, e) => searchByAuthor();
            mTvlongIntro.Click += (sender, e) => collapseLongIntroHandler();
            mTvMoreReview.Click += (sender, e) => onClickMoreReview();
            mRlCommunity.Click += (sender, e) => onClickCommunity();
        }

        public override void initToolBar()
        {
            mCommonToolbar.SetNavigationIcon(Resource.Drawable.ab_back);
            mCommonToolbar.SetTitle(Resource.String.book_detail);
        }

        public override void initDatas()
        {
            bookId = Intent.GetStringExtra(INTENT_BOOK_ID);
            MessageBus.Default.Register<RefreshCollectionIconEvent>(RefreshCollectionIcon);
        }

        public override void configViews()
        {
            mRvHotReview.HasFixedSize = (true);
            mRvHotReview.SetLayoutManager(new LinearLayoutManager(this));
            mHotReviewAdapter = new HotReviewAdapter(mContext, mHotReviewList, this);
            mRvHotReview.SetAdapter(mHotReviewAdapter);

            mRvRecommendBoookList.HasFixedSize = (true);
            mRvRecommendBoookList.SetLayoutManager(new LinearLayoutManager(this));
            mRecommendBookListAdapter = new RecommendBookListAdapter(mContext, mRecommendBookList, this);
            mRvRecommendBoookList.SetAdapter(mRecommendBookListAdapter);

            mTagGroup.setOnTagClickListener(new CustomOnTagClickListener(this));

            //TODO: mPresenter.getBookDetail(bookId);
            //TODO: mPresenter.getHotReview(bookId);
            //TODO: mPresenter.getRecommendBookList(bookId, "3");
        }

        public void showBookDetail(BookDetail data)
        {
            Glide.With(mContext)
                .Load(Constant.IMG_BASE_URL + data.cover)
                .Placeholder(Resource.Drawable.cover_default)
                .Transform(new GlideRoundTransform(mContext))
                .Into(mIvBookCover);

            mTvBookTitle.Text = (data.title);
            mTvAuthor.Text = (String.Format(GetString(Resource.String.book_detail_author), data.author));
            mTvCatgory.Text = (String.Format(GetString(Resource.String.book_detail_category), data.cat));
            mTvWordCount.Text = (FormatUtils.formatWordCount(data.wordCount));
            mTvLatelyUpdate.Text = (FormatUtils.getDescriptionTimeFromDateString(data.updated));
            mTvLatelyFollower.Text = data.latelyFollower.ToString();
            mTvRetentionRatio.Text = (TextUtils.IsEmpty(data.retentionRatio) ?
                    "-" : String.Format(GetString(Resource.String.book_detail_retention_ratio),
                    data.retentionRatio));
            mTvSerializeWordCount.Text = (data.serializeWordCount < 0 ? "-" :
                    data.serializeWordCount.ToString());

            tagList.Clear();
            tagList.AddRange(data.tags);
            times = 0;
            showHotWord();

            mTvlongIntro.Text = (data.longIntro);
            mTvCommunity.Text = (String.Format(GetString(Resource.String.book_detail_community), data.title));
            mTvPostCount.Text = (String.Format(GetString(Resource.String.book_detail_post_count), data.postCount));

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
        private void refreshCollectionIcon()
        {
            if (CollectionsManager.getInstance().isCollected(recommendBooks._id))
            {
                initCollection(false);
            }
            else
            {
                initCollection(true);
            }
        }

        public void RefreshCollectionIcon(object sender, MessageBusEvent evnt)
        {
            if (evnt is RefreshCollectionIconEvent)
            {
                refreshCollectionIcon();
            }
        }

        /// <summary>
        /// 每次显示8个
        /// </summary>
        private void showHotWord()
        {
            int start, end;
            if (times < tagList.Count() && times + 8 <= tagList.Count())
            {
                start = times;
                end = times + 8;
            }
            else if (times < tagList.Count() - 1 && times + 8 > tagList.Count())
            {
                start = times;
                end = tagList.Count() - 1;
            }
            else
            {
                start = 0;
                end = tagList.Count() > 8 ? 8 : tagList.Count();
            }
            times = end;
            if (end - start > 0)
            {
                List<String> batch = tagList.GetRange(start, end - start + 1);
                List<TagColor> colors = TagColor.getRandomColors(batch.Count());
                mTagGroup.setTags(colors, batch.ToArray());
            }
        }

        public void showHotReview(List<HotReview.Reviews> list)
        {
            mHotReviewList.Clear();
            mHotReviewList.AddRange(list);
            mHotReviewAdapter.NotifyDataSetChanged();
        }

        public void showRecommendBookList(List<RecommendBookList.RecommendBook> list)
        {
            if (list.Any())
            {
                mTvRecommendBookList.Visibility = ViewStates.Visible;
                mRecommendBookList.Clear();
                mRecommendBookList.AddRange(list);
                mRecommendBookListAdapter.NotifyDataSetChanged();
            }
        }

        public void onItemClick(View view, int position, object data)
        {
            if (data is HotReview.Reviews)
            {
                BookDiscussionDetailActivity.startActivity(this, ((HotReview.Reviews)data)._id);
            }
            else if (data is RecommendBookList.RecommendBook)
            {
                RecommendBookList.RecommendBook recommendBook = (RecommendBookList.RecommendBook)data;

                BookLists bookLists = new BookLists();
                BookLists.BookListsBean bookListsBean = new BookLists.BookListsBean();
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

        public void onClickJoinCollection()
        {
            if (!isJoinedCollections)
            {
                if (recommendBooks != null)
                {
                    CollectionsManager.getInstance().add(recommendBooks);
                    ToastUtils.showToast(String.Format(GetString(
                            Resource.String.book_detail_has_joined_the_book_shelf), recommendBooks.title));
                    initCollection(false);
                }
            }
            else
            {
                CollectionsManager.getInstance().remove(recommendBooks._id);
                ToastUtils.showToast(String.Format(GetString(
                        Resource.String.book_detail_has_remove_the_book_shelf), recommendBooks.title));
                initCollection(true);
            }
        }

        private void initCollection(bool coll)
        {
            if (coll)
            {
                mBtnJoinCollection.SetText(Resource.String.book_detail_join_collection);
                Drawable drawable = ContextCompat.GetDrawable(this, Resource.Drawable.book_detail_info_add_img);
                drawable.SetBounds(0, 0, drawable.MinimumWidth, drawable.MinimumHeight);
                mBtnJoinCollection.SetBackgroundDrawable(ContextCompat.GetDrawable(this, Resource.Drawable.shape_common_btn_solid_normal));
                mBtnJoinCollection.SetCompoundDrawables(drawable, null, null, null);
                mBtnJoinCollection.PostInvalidate();
                isJoinedCollections = false;
            }
            else
            {
                mBtnJoinCollection.SetText(Resource.String.book_detail_remove_collection);
                Drawable drawable = ContextCompat.GetDrawable(this, Resource.Drawable.book_detail_info_del_img);
                drawable.SetBounds(0, 0, drawable.MinimumWidth, drawable.MinimumHeight);
                mBtnJoinCollection.SetBackgroundDrawable(ContextCompat.GetDrawable(this, Resource.Drawable.btn_join_collection_pressed));
                mBtnJoinCollection.SetCompoundDrawables(drawable, null, null, null);
                mBtnJoinCollection.PostInvalidate();
                isJoinedCollections = true;
            }
        }

        public void onClickRead()
        {
            if (recommendBooks == null) return;
            // TODO: ReadActivity.startActivity(this, recommendBooks);
        }

        public void searchByAuthor()
        {
            String author = mTvAuthor.Text.ToString().Replace(" ", "");
            SearchByAuthorActivity.startActivity(this, author);
        }

        public void collapseLongIntroHandler()
        {
            if (collapseLongIntro)
            {
                mTvlongIntro.SetMaxLines(20);
                collapseLongIntro = false;
            }
            else
            {
                mTvlongIntro.SetMaxLines(4);
                collapseLongIntro = true;
            }
        }

        public void onClickMoreReview()
        {
            BookDetailCommunityActivity.startActivity(this, bookId, mTvBookTitle.Text.ToString(), 1);
        }

        public void onClickCommunity()
        {
            BookDetailCommunityActivity.startActivity(this, bookId, mTvBookTitle.Text.ToString(), 0);
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
            MessageBus.Default.DeRegister<RefreshCollectionIconEvent>(RefreshCollectionIcon);
        }

        class CustomOnTagClickListener : Java.Lang.Object, TagGroup.OnTagClickListener
        {
            private BookDetailActivity bookDetailActivity;

            public CustomOnTagClickListener(BookDetailActivity bookDetailActivity)
            {
                this.bookDetailActivity = bookDetailActivity;
            }

            public void onTagClick(string tag)
            {
                bookDetailActivity.StartActivity(new Intent(bookDetailActivity, typeof(BooksByTagActivity))
                        .PutExtra("tag", tag));
            }
        }
    }
}