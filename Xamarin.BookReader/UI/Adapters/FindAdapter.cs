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
using Xamarin.BookReader.Models.Support;
using Xamarin.BookReader.UI.Listeners;

namespace Xamarin.BookReader.UI.Adapters
{
    public class FindAdapter: EasyRVAdapter<FindBean>
    {
        private IOnRvItemClickListener<FindBean> itemClickListener;
        public FindAdapter(Context context, List<FindBean> list, IOnRvItemClickListener<FindBean> listener)
            : base(context, list, Resource.Layout.item_find)
        {
            this.itemClickListener = listener;
        }

        protected override void OnBindData(RecyclerView.ViewHolder viewHolder, int position, FindBean item)
        {
            var holder = viewHolder as EasyRVHolder;
            holder.setText(Resource.Id.tvTitle, item.Title);
            holder.setImageResource(Resource.Id.ivIcon, item.IconResId);
            holder.setOnItemViewClickListener(new CustomOnClickListener(() => {
                itemClickListener.onItemClick(holder.getItemView(), position, item);
            }));
        }

        class CustomOnClickListener : Java.Lang.Object, View.IOnClickListener
        {
            Action _action;
            public CustomOnClickListener(Action action)
            {
                _action = action;
            }
            public void OnClick(View v)
            {
                _action?.Invoke();
            }
        }
    }
}