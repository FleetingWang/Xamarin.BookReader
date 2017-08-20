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

namespace Xamarin.BookReader.UI.Adapters
{
    public class SubjectTagsAdapter : EasyRVAdapter<BookListTags.DataBean>
    {
        private IOnRvItemClickListener<string> listener;
        public SubjectTagsAdapter(Context context, List<BookListTags.DataBean> list)
            : base(context, list, Resource.Layout.item_subject_tags_list)
        {

        }

        protected override void OnBindData(RecyclerView.ViewHolder viewHolder, int position, BookListTags.DataBean item)
        {
            var holder = viewHolder as EasyRVHolder;
            RecyclerView rvTagsItem = holder.getView<RecyclerView>(Resource.Id.rvTagsItem);
            rvTagsItem.HasFixedSize = (true);
            rvTagsItem.SetLayoutManager(new GridLayoutManager(mContext, 4));
            TagsItemAdapter adapter = new TagsItemAdapter(this, mContext, item.tags);
            rvTagsItem.SetAdapter(adapter);

            holder.setText(Resource.Id.tvTagGroupName, item.name);
        }

        public void setItemClickListener(IOnRvItemClickListener<string> listener)
        {
            this.listener = listener;
        }

        private class TagsItemAdapter : EasyRVAdapter<String>
        {
            private SubjectTagsAdapter adapter;
            public TagsItemAdapter(SubjectTagsAdapter adapter, Context context, List<String> list)
                : base(context, list, Resource.Layout.item_subject_tag_list)
            {
                this.adapter = adapter;
            }

            protected override void OnBindData(RecyclerView.ViewHolder viewHolder, int position, string item)
            {
                var holder = viewHolder as EasyRVHolder;
                holder.setText(Resource.Id.tvTagName, item);
                holder.ItemView.Click += (sender, e) =>
                {
                    if (adapter.listener != null)
                    {
                        adapter.listener.onItemClick(sender as View, position, item);
                    }
                };
            }
        }
    }
}