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
using Android.Support.V7.Widget;
using Xamarin.BookReader.Helpers;
using Xamarin.BookReader.Bases;

namespace Xamarin.BookReader.UI.EasyAdapters
{
    /// <summary>
    /// 主题书单
    /// </summary>
    public class SubjectBookListAdapter : RecyclerArrayAdapter<BookLists.BookListsBean>
    {
        public SubjectBookListAdapter(Context context) : base(context)
        {
        }

        public override BaseViewHolder<BookLists.BookListsBean> onCreateViewHolder(ViewGroup parent, int viewType)
        {
            return new CustomViewHolder(this, parent, Resource.Layout.item_sub_category_list);
        }

        private class CustomViewHolder : BaseViewHolder<BookLists.BookListsBean>
        {
            private SubjectBookListAdapter subjectBookListAdapter;
            private ViewGroup parent;
            private int item_sub_category_list;

            public CustomViewHolder(SubjectBookListAdapter subjectBookListAdapter, ViewGroup parent, int item_sub_category_list)
                : base(parent, item_sub_category_list)
            {
                this.subjectBookListAdapter = subjectBookListAdapter;
                this.parent = parent;
                this.item_sub_category_list = item_sub_category_list;
            }

            public override void setData(BookLists.BookListsBean item)
            {
                base.setData(item);
                if (!Settings.IsNoneCover)
                {
                    holder.setRoundImageUrl(Resource.Id.ivSubCateCover, Constant.IMG_BASE_URL + item.cover,
                            Resource.Drawable.cover_default);
                }
                else
                {
                    holder.setImageResource(Resource.Id.ivSubCateCover, Resource.Drawable.cover_default);
                }

                holder.setText(Resource.Id.tvSubCateTitle, item.title)
                        .setText(Resource.Id.tvSubCateAuthor, item.author)
                        .setText(Resource.Id.tvSubCateShort, item.desc)
                        .setText(Resource.Id.tvSubCateMsg, String.Format(mContext.Resources.GetString(Resource.String.subject_book_msg), item.bookCount, item.collectorCount));
            }
        }
    }
}