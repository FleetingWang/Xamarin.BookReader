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
using EasyAdapterLibrary.AbsListViews;
using Xamarin.BookReader.Models;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Xamarin.BookReader.Utils;
using Android.Content.Res;
using Android.Graphics;

namespace Xamarin.BookReader.UI.Adapters
{
    public class TocListAdapter : EasyLVAdapter<BookMixAToc.MixToc.Chapters>
    {
        private int currentChapter;
        private String bookId;

        private bool isEpub = false;
        public TocListAdapter(Context context, List<BookMixAToc.MixToc.Chapters> list, String bookId, int currentChapter)
            : base(context, list, Resource.Layout.item_book_read_toc_list)
        {
            this.currentChapter = currentChapter;
            this.bookId = bookId;
        }

        public override void convert(EasyLVHolder holder, int position, BookMixAToc.MixToc.Chapters chapters)
        {
            TextView tvTocItem = holder.getView<TextView>(Resource.Id.tvTocItem);
            tvTocItem.Text = (chapters.title);
            Drawable drawable;
            if (currentChapter == position + 1)
            {
                tvTocItem.SetTextColor(new Color(ContextCompat.GetColor(mContext, Resource.Color.light_red)));
                drawable = ContextCompat.GetDrawable(mContext, Resource.Drawable.ic_toc_item_activated);
            }
            else if (isEpub || FileUtils.getChapterFile(bookId, position + 1).Length() > 10)
            {
                tvTocItem.SetTextColor(new Color(ContextCompat.GetColor(mContext, Resource.Color.light_black)));
                drawable = ContextCompat.GetDrawable(mContext, Resource.Drawable.ic_toc_item_download);
            }
            else
            {
                tvTocItem.SetTextColor(new Color(ContextCompat.GetColor(mContext, Resource.Color.light_black)));
                drawable = ContextCompat.GetDrawable(mContext, Resource.Drawable.ic_toc_item_normal);
            }
            drawable.SetBounds(0, 0, drawable.MinimumWidth, drawable.MinimumHeight);
            tvTocItem.SetCompoundDrawables(drawable, null, null, null);
        }

        public void setCurrentChapter(int chapter)
        {
            currentChapter = chapter;
            NotifyDataSetChanged();
        }

        public void setEpub(bool isEpub)
        {
            this.isEpub = isEpub;
        }
    }
}