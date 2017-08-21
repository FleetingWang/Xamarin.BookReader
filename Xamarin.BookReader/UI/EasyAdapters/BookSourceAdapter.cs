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
using Xamarin.BookReader.Views;

namespace Xamarin.BookReader.UI.EasyAdapters
{
    /// <summary>
    /// 查询
    /// </summary>
    public class BookSourceAdapter : RecyclerArrayAdapter<BookSource>
    {


        public BookSourceAdapter(Context context) : base(context)
        {
        }

        public override BaseViewHolder<BookSource> onCreateViewHolder(ViewGroup parent, int viewType)
        {
            return new CustomViewHolder(this, parent, Resource.Layout.item_book_source);
        }

        private class CustomViewHolder : BaseViewHolder<BookSource>
        {
            private BookSourceAdapter bookSourceAdapter;
            private ViewGroup parent;
            private int item_book_source;

            public CustomViewHolder(BookSourceAdapter bookSourceAdapter, ViewGroup parent, int item_book_source)
                : base(parent, item_book_source)
            {
                this.bookSourceAdapter = bookSourceAdapter;
                this.parent = parent;
                this.item_book_source = item_book_source;
            }
            public override void setData(BookSource item)
            {
                holder.setText(Resource.Id.tv_source_title, item.host)
                        .setText(Resource.Id.tv_source_content, item.lastChapter);

                LetterView letterView = holder.getView<LetterView>(Resource.Id.letter_view);
                letterView.setText(item.host);
            }
        }
    }
}