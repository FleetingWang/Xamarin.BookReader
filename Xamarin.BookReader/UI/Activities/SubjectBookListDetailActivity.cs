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
using Xamarin.BookReader.Datas;
using Newtonsoft.Json;
using Com.Bumptech.Glide;
using Com.Bumptech.Glide.Load.Resource.Bitmap;
using Xamarin.BookReader.Views.RecyclerViews.Adapters;
using Xamarin.BookReader.UI.EasyAdapters;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using Xamarin.BookReader.Utils;
using Android.Content.PM;

namespace Xamarin.BookReader.UI.Activities
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class SubjectBookListDetailActivity : BaseRVActivity<BookListDetail.BookListBean.BooksBean>
    {
        private HeaderViewHolder headerViewHolder;

        private List<BookListDetail.BookListBean.BooksBean> mAllBooks = new List<BookListDetail.BookListBean.BooksBean>();

        private int start = 0;
        private int limit = 20;

        public static String INTENT_BEAN = "bookListsBean";

        private BookLists.BookListsBean bookListsBean;

        public static void startActivity(Context context, BookLists.BookListsBean bookListsBean)
        {
            context.StartActivity(new Intent(context, typeof(SubjectBookListDetailActivity))
                .PutExtra(INTENT_BEAN, JsonConvert.SerializeObject(bookListsBean)));
        }

        public override int getLayoutId()
        {
            return Resource.Layout.activity_subject_book_list_detail;
        }

        public override void initToolBar()
        {
            mCommonToolbar.SetTitle(Resource.String.subject_book_list_detail);
            mCommonToolbar.SetNavigationIcon(Resource.Drawable.ab_back);
        }
        public override void bindViews()
        {
        }

        public override void initDatas()
        {
            bookListsBean = JsonConvert.DeserializeObject<BookLists.BookListsBean>(Intent.GetStringExtra(INTENT_BEAN));
        }
        public override void configViews()
        {
            initAdapter(new SubjectBookListDetailBooksAdapter(this), false, true);
            mRecyclerView.removeAllItemDecoration();
            mAdapter.addHeader(new CustomItemView(this));

            getBookListDetail(bookListsBean._id);
        }
        void getBookListDetail(String bookListId)
        {
            BookApi.Instance.getBookListDetail(bookListId)
                .SubscribeOn(DefaultScheduler.Instance)
                .ObserveOn(Application.SynchronizationContext)
                .Subscribe(data => {
                    showBookListDetail(data);
                }, e => {
                    LogUtils.e("SubjectBookListDetailActivity", e.ToString());
                    complete();
                }, () => {
                    LogUtils.i("SubjectBookListDetailActivity", "complete");
                    complete();
                });
        }

        public void showBookListDetail(BookListDetail data)
        {
            headerViewHolder.tvBookListTitle.Text = (data.BookList.Title);
            headerViewHolder.tvBookListDesc.Text = (data.BookList.Desc);
            headerViewHolder.tvBookListAuthor.Text = (data.BookList.Author.Nickname);

            Glide.With(mContext)
                    .Load(Constant.IMG_BASE_URL + data.BookList.Author.Avatar)
                    .Placeholder(Resource.Drawable.avatar_default)
                    .Transform(new GlideCircleTransform(mContext))
                    .Into(headerViewHolder.ivAuthorAvatar);

            List<BookListDetail.BookListBean.BooksBean> list = data.BookList.Books;
            mAllBooks.Clear();
            mAllBooks.AddRange(list);
            mAdapter.clear();
            loadNextPage();
        }

        private void loadNextPage()
        {
            if (start < mAllBooks.Count())
            {
                if (mAllBooks.Count() - start > limit)
                {
                    mAdapter.addAll(mAllBooks.GetRange(start, limit));
                }
                else
                {
                    mAdapter.addAll(mAllBooks.GetRange(start, mAllBooks.Count() - start + 1));
                }
                start += limit;
            }
            else
            {
                mAdapter.addAll(new List<BookListDetail.BookListBean.BooksBean>());
            }
        }

        public void showError()
        {

        }
        public void complete()
        {

        }

        public override void onItemClick(int position)
        {
            BookDetailActivity.startActivity(this, mAdapter.getItem(position).Book._id);
        }

        public override void onRefresh()
        {
            getBookListDetail(bookListsBean._id);
        }

        public override void onLoadMore()
        {
            mRecyclerView.PostDelayed(loadNextPage, 500);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_subject_detail, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.menu_collect)
            {
                CacheManager.AddMyBookList(bookListsBean);
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        class HeaderViewHolder
        {
            public TextView tvBookListTitle;
            public TextView tvBookListDesc;
            public ImageView ivAuthorAvatar;
            public TextView tvBookListAuthor;
            public TextView btnShare;

            public HeaderViewHolder(View view)
            {
                tvBookListTitle = view.FindViewById<TextView>(Resource.Id.tvBookListTitle);
                tvBookListDesc = view.FindViewById<TextView>(Resource.Id.tvBookListDesc);
                ivAuthorAvatar = view.FindViewById<ImageView>(Resource.Id.ivAuthorAvatar);
                tvBookListAuthor = view.FindViewById<TextView>(Resource.Id.tvBookListAuthor);
                btnShare = view.FindViewById<TextView>(Resource.Id.btnShare);
            }
        }

        private class CustomItemView : RecyclerArrayAdapter<BookListDetail.BookListBean.BooksBean>.ItemView
        {
            private SubjectBookListDetailActivity subjectBookListDetailActivity;

            public CustomItemView(SubjectBookListDetailActivity subjectBookListDetailActivity)
            {
                this.subjectBookListDetailActivity = subjectBookListDetailActivity;
            }

            public void onBindView(View headerView)
            {
                subjectBookListDetailActivity.headerViewHolder = new HeaderViewHolder(headerView);
            }

            public View onCreateView(ViewGroup parent)
            {
                View headerView = LayoutInflater.From(subjectBookListDetailActivity).Inflate(Resource.Layout.header_view_book_list_detail, parent, false);
                return headerView;
            }
        }
    }
}