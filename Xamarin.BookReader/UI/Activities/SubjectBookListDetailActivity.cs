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

namespace Xamarin.BookReader.UI.Activities
{
    public class SubjectBookListDetailActivity : BaseRVActivity<BookListDetail.BookListBean.BooksBean>
    {
        private HeaderViewHolder headerViewHolder;
        //class HeaderViewHolder
        //{
        //    @Bind(Resource.Id.tvBookListTitle)
        //    TextView tvBookListTitle;
        //    @Bind(Resource.Id.tvBookListDesc)
        //    TextView tvBookListDesc;
        //    @Bind(Resource.Id.ivAuthorAvatar)
        //    ImageView ivAuthorAvatar;
        //    @Bind(Resource.Id.tvBookListAuthor)
        //    TextView tvBookListAuthor;
        //    @Bind(Resource.Id.btnShare)
        //    TextView btnShare;

        //    public HeaderViewHolder(View view)
        //    {
        //        ButterKnife.bind(this, view);
        //    }
        //}
        private List<BookListDetail.BookListBean.BooksBean> mAllBooks = new ArrayList<>();

        private int start = 0;
        private int limit = 20;

        public static String INTENT_BEAN = "bookListsBean";

        private BookLists.BookListsBean bookListsBean;

        public static void startActivity(Context context, BookLists.BookListsBean bookListsBean)
        {
            context.StartActivity(new Intent(context, typeof(SubjectBookListDetailActivity))
                .PutExtra(INTENT_BEAN, bookListsBean));
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
            throw new NotImplementedException();
        }

        public override void initDatas()
        {
            bookListsBean = (BookLists.BookListsBean)getIntent().getSerializableExtra(INTENT_BEAN);
        }
        public override void configViews()
        {
            initAdapter(SubjectBookListDetailBooksAdapter, false, true);
            mRecyclerView.removeAllItemDecoration();
            //mAdapter.addHeader(new RecyclerArrayAdapter.ItemView() {
            //    @Override
            //    public View onCreateView(ViewGroup parent) {
            //        View headerView = LayoutInflater.From(mContext).Inflate(Resource.Layout.header_view_book_list_detail, parent, false);
            //        return headerView;
            //    }

            //    @Override
            //    public void onBindView(View headerView) {
            //        headerViewHolder = new HeaderViewHolder(headerView);
            //    }
            //});

            mPresenter.attachView(this);
            mPresenter.getBookListDetail(bookListsBean._id);
        }

        public void showBookListDetail(BookListDetail data)
        {
            headerViewHolder.tvBookListTitle.Text = (data.getBookList().getTitle());
            headerViewHolder.tvBookListDesc.Text = (data.getBookList().getDesc());
            headerViewHolder.tvBookListAuthor.Text = (data.getBookList().getAuthor().getNickname());

            Glide.With(mContext)
                    .Load(Constant.IMG_BASE_URL + data.getBookList().getAuthor().getAvatar())
                    .Placeholder(Resource.Drawable.avatar_default)
                    .Transform(new GlideCircleTransform(mContext))
                    .Into(headerViewHolder.ivAuthorAvatar);

            List<BookListDetail.BookListBean.BooksBean> list = data.getBookList().getBooks();
            mAllBooks.Clear();
            mAllBooks.AddRange(list);
            mAdapter.Clear();
            loadNextPage();
        }

        private void loadNextPage()
        {
            if (start < mAllBooks.Count())
            {
                if (mAllBooks.Count() - start > limit)
                {
                    mAdapter.AddRange(mAllBooks.subList(start, start + limit));
                }
                else
                {
                    mAdapter.AddRange(mAllBooks.subList(start, mAllBooks.Count()));
                }
                start += limit;
            }
            else
            {
                mAdapter.AddRange(new ArrayList<BookListDetail.BookListBean.BooksBean>());
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
            BookDetailActivity.startActivity(this, mAdapter.getItem(position).getBook().get_id());
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
    }
}