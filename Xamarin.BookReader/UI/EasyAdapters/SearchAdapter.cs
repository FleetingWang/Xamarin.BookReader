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
using Xamarin.BookReader.Bases;
using Android.Text;

namespace Xamarin.BookReader.UI.EasyAdapters
{
    /// <summary>
    /// 查询
    /// </summary>
    public class SearchAdapter : RecyclerArrayAdapter<SearchDetail.SearchBooks>
    {


        public SearchAdapter(Context context) : base(context)
        {
        }

        public override BaseViewHolder<SearchDetail.SearchBooks> onCreateViewHolder(ViewGroup parent, int viewType)
        {
            return new CustomViewHolder(this, parent, Resource.Layout.item_search_result_list);
        }

        private class CustomViewHolder : BaseViewHolder<SearchDetail.SearchBooks>
        {
            private SearchAdapter searchAdapter;
            private ViewGroup parent;
            private int item_search_result_list;

            public CustomViewHolder(SearchAdapter searchAdapter, ViewGroup parent, int item_search_result_list)
                : base(parent, item_search_result_list)
            {
                this.searchAdapter = searchAdapter;
                this.parent = parent;
                this.item_search_result_list = item_search_result_list;
            }
            public override void setData(SearchDetail.SearchBooks item)
            {
                holder.setRoundImageUrl(Resource.Id.ivBookCover, Constant.IMG_BASE_URL + item.cover, Resource.Drawable.cover_default)
                        .setText(Resource.Id.tvBookListTitle, item.title)
                        .setText(Resource.Id.tvLatelyFollower, Java.Lang.String.Format(mContext.GetString(Resource.String.search_result_lately_follower), item.latelyFollower))
                        .setText(Resource.Id.tvRetentionRatio, (TextUtils.IsEmpty(item.retentionRatio) ? Java.Lang.String.Format(mContext.GetString(Resource.String.search_result_retention_ratio), "0")
                                : Java.Lang.String.Format(mContext.GetString(Resource.String.search_result_retention_ratio), item.retentionRatio)))
                        .setText(Resource.Id.tvBookListAuthor, Java.Lang.String.Format(mContext.GetString(Resource.String.search_result_author), item.author));
            }
        }
    }

}