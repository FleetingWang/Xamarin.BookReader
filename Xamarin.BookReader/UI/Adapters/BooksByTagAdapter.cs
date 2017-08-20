using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using EasyAdapterLibrary.RecyclerViews;
using Xamarin.BookReader.Models;
using Xamarin.BookReader.UI.Listeners;
using Java.Lang;
using Android.Text;
using Xamarin.BookReader.Bases;

namespace Xamarin.BookReader.UI.Adapters
{
    public class BooksByTagAdapter : EasyRVAdapter<BooksByTag.TagBook>
    {
        private IOnRvItemClickListener<BooksByTag.TagBook> itemClickListener;

        public BooksByTagAdapter(Context context, List<BooksByTag.TagBook> list,
                             IOnRvItemClickListener<BooksByTag.TagBook> listener)
            : base(context, list, Resource.Layout.item_tag_book_list)
        {

            this.itemClickListener = listener;
        }

        protected override void OnBindData(RecyclerView.ViewHolder viewHolder, int position, BooksByTag.TagBook item)
        {
            StringBuffer sbTags = new StringBuffer();
            foreach (string tag in item.tags)
            {
                if (!TextUtils.IsEmpty(tag))
                {
                    sbTags.Append(tag);
                    sbTags.Append(" | ");
                }
            }
            var holder = viewHolder as EasyRVHolder;
            holder.setRoundImageUrl(Resource.Id.ivBookCover, Constant.IMG_BASE_URL + item.cover, Resource.Drawable.cover_default)
                    .setText(Resource.Id.tvBookListTitle, item.title)
                    .setText(Resource.Id.tvShortIntro, item.shortIntro)
                    .setText(Resource.Id.tvTags, (item.tags.Count() == 0 ? "" : sbTags.Substring(0, sbTags
                            .LastIndexOf(" | "))));
            holder.Click += (sender, e) => {
                itemClickListener.onItemClick(holder.getItemView(), position, item);
            };
        }
    }
}