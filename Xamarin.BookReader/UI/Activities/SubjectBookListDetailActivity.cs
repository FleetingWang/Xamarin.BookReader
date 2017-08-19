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
        //    @Bind(R.id.tvBookListTitle)
        //    TextView tvBookListTitle;
        //    @Bind(R.id.tvBookListDesc)
        //    TextView tvBookListDesc;
        //    @Bind(R.id.ivAuthorAvatar)
        //    ImageView ivAuthorAvatar;
        //    @Bind(R.id.tvBookListAuthor)
        //    TextView tvBookListAuthor;
        //    @Bind(R.id.btnShare)
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
            return R.layout.activity_subject_book_list_detail;
        }
        
        public override void initToolBar()
        {
            mCommonToolbar.setTitle(R.string.subject_book_list_detail);
            mCommonToolbar.setNavigationIcon(R.drawable.ab_back);
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
            //        View headerView = LayoutInflater.from(mContext).inflate(R.layout.header_view_book_list_detail, parent, false);
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
            headerViewHolder.tvBookListTitle.setText(data.getBookList().getTitle());
            headerViewHolder.tvBookListDesc.setText(data.getBookList().getDesc());
            headerViewHolder.tvBookListAuthor.setText(data.getBookList().getAuthor().getNickname());

            Glide.with(mContext)
                    .load(Constant.IMG_BASE_URL + data.getBookList().getAuthor().getAvatar())
                    .placeholder(R.drawable.avatar_default)
                    .transform(new GlideCircleTransform(mContext))
                    .into(headerViewHolder.ivAuthorAvatar);

            List<BookListDetail.BookListBean.BooksBean> list = data.getBookList().getBooks();
            mAllBooks.clear();
            mAllBooks.addAll(list);
            mAdapter.clear();
            loadNextPage();
        }

        private void loadNextPage()
        {
            if (start < mAllBooks.size())
            {
                if (mAllBooks.size() - start > limit)
                {
                    mAdapter.addAll(mAllBooks.subList(start, start + limit));
                }
                else
                {
                    mAdapter.addAll(mAllBooks.subList(start, mAllBooks.size()));
                }
                start += limit;
            }
            else
            {
                mAdapter.addAll(new ArrayList<BookListDetail.BookListBean.BooksBean>());
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
            getMenuInflater().inflate(R.menu.menu_subject_detail, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.getItemId() == R.id.menu_collect)
            {
                CacheManager.AddMyBookList(bookListsBean);
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}