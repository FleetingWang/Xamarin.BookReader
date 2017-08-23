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
using Android.Text;

namespace Xamarin.BookReader.UI.EasyAdapters
{
    /// <summary>
    /// 二级排行榜 / 二级分类
    /// </summary>
    public class SubCategoryAdapter : RecyclerArrayAdapter<BooksByCats.BooksBean>
    {

        public SubCategoryAdapter(Context context) : base(context)
        {
        }

        public override BaseViewHolder<BooksByCats.BooksBean> onCreateViewHolder(ViewGroup parent, int viewType)
        {
            return new CustomViewHolder(this, parent, Resource.Layout.item_sub_category_list);
        }

        private class CustomViewHolder : BaseViewHolder<BooksByCats.BooksBean>
        {
            private SubCategoryAdapter subCategoryAdapter;
            private ViewGroup parent;
            private int item_sub_category_list;

            public CustomViewHolder(SubCategoryAdapter subCategoryAdapter, ViewGroup parent, int item_sub_category_list)
                : base(parent, item_sub_category_list)
            {
                this.subCategoryAdapter = subCategoryAdapter;
                this.parent = parent;
                this.item_sub_category_list = item_sub_category_list;
            }

            public override void setData(BooksByCats.BooksBean item)
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
                        .setText(Resource.Id.tvSubCateAuthor, (item.author == null ? "未知" : item.author) + " | " + (item.majorCate == null ? "未知" : item.majorCate))
                        .setText(Resource.Id.tvSubCateShort, item.shortIntro)
                        .setText(Resource.Id.tvSubCateMsg, Java.Lang.String.Format(mContext.Resources.GetString(Resource.String.category_book_msg),
                                item.latelyFollower,
                                TextUtils.IsEmpty(item.retentionRatio) ? "0" : item.retentionRatio));
            }
        }
    }
}