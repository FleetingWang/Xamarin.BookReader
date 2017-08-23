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
using Xamarin.BookReader.Views.RecyclerViews.Adapters;
using Xamarin.BookReader.Models;
using Xamarin.BookReader.Helpers;
using Xamarin.BookReader.Bases;

namespace Xamarin.BookReader.UI.EasyAdapters
{
    public class SubjectBookListDetailBooksAdapter : RecyclerArrayAdapter<BookListDetail.BookListBean.BooksBean>
    {
        public SubjectBookListDetailBooksAdapter(Context context)
            : base(context)
        {
        }

        public override BaseViewHolder<BookListDetail.BookListBean.BooksBean> onCreateViewHolder(ViewGroup parent, int viewType)
        {
            return new CustomBaseViewHolder(this, parent, Resource.Layout.item_subject_book_list_detail);
        }

        private class CustomBaseViewHolder : BaseViewHolder<BookListDetail.BookListBean.BooksBean>
        {
            private SubjectBookListDetailBooksAdapter subjectBookListDetailBooksAdapter;
            private ViewGroup parent;
            private int item_subject_book_list_detail;

            public CustomBaseViewHolder(SubjectBookListDetailBooksAdapter subjectBookListDetailBooksAdapter, ViewGroup parent, int item_subject_book_list_detail)
                :base(parent, item_subject_book_list_detail)
            {
                this.subjectBookListDetailBooksAdapter = subjectBookListDetailBooksAdapter;
                this.parent = parent;
                this.item_subject_book_list_detail = item_subject_book_list_detail;
            }

            public override void setData(BookListDetail.BookListBean.BooksBean item)
            {
                if (!Settings.IsNoneCover)
                {
                    holder.setRoundImageUrl(Resource.Id.ivBookCover, Constant.IMG_BASE_URL + item.Book.Cover,
                            Resource.Drawable.cover_default);
                }
                else
                {
                    holder.setImageResource(Resource.Id.ivBookCover, Resource.Drawable.cover_default);
                }

                holder.setText(Resource.Id.tvBookListTitle, item.Book.Title)
                        .setText(Resource.Id.tvBookAuthor, item.Book.Author)
                        .setText(Resource.Id.tvBookLatelyFollower, Java.Lang.String.Format(mContext.Resources.GetString(Resource.String.subject_book_list_detail_book_lately_follower),
                                item.Book.LatelyFollower))
                        .setText(Resource.Id.tvBookWordCount, Java.Lang.String.Format(mContext.Resources.GetString(Resource.String.subject_book_list_detail_book_word_count),
                                item.Book.WordCount / 10000))
                        .setText(Resource.Id.tvBookDetail, item.Book.LongIntro);
            }
        }
    }
}