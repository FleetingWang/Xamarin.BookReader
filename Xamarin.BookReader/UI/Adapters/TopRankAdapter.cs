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
using Xamarin.BookReader.UI.Listeners;
using Xamarin.BookReader.Models;
using Java.Lang;
using Android.Text;
using Xamarin.BookReader.Bases;
using Com.Bumptech.Glide;
using Com.Bumptech.Glide.Load.Resource.Bitmap;

namespace Xamarin.BookReader.UI.Adapters
{
    public class TopRankAdapter: BaseExpandableListAdapter
    {
        private Context mContext;
        private LayoutInflater inflater;

        private List<RankingList.MaleBean> groupArray;
        private List<List<RankingList.MaleBean>> childArray;

        private IOnRvItemClickListener<RankingList.MaleBean> listener;

        public TopRankAdapter(Context context, List<RankingList.MaleBean> groupArray, List<List<RankingList.MaleBean>> childArray)
        {
            this.childArray = childArray;
            this.groupArray = groupArray;
            mContext = context;
            inflater = LayoutInflater.From(context);
        }

        public override int GroupCount => groupArray.Count();

        public override bool HasStableIds => false;

        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            return childArray[groupPosition][childPosition];
        }

        public override long GetChildId(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override int GetChildrenCount(int groupPosition)
        {
            return childArray[groupPosition].Count();
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            View child = inflater.Inflate(Resource.Layout.item_top_rank_child, null);

            TextView tvName = child.FindViewById<TextView>(Resource.Id.tvRankChildName);
            tvName.Text = (childArray[groupPosition][childPosition].title);
            child.Click += (sender, e) => {
                listener.onItemClick(child, childPosition, childArray[groupPosition][childPosition]);
            };
            return child;
        }

        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            return groupArray[groupPosition];
        }

        public override long GetGroupId(int groupPosition)
        {
            return groupPosition;
        }

        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            View group = inflater.Inflate(Resource.Layout.item_top_rank_group, null);

            ImageView ivCover = group.FindViewById<ImageView>(Resource.Id.ivRankCover);
            if (!TextUtils.IsEmpty(groupArray[groupPosition].cover)) {
                Glide.With(mContext)
                    .Load(Constant.IMG_BASE_URL + groupArray[groupPosition].cover)
                    .Placeholder(Resource.Drawable.avatar_default)
                    .Transform(new GlideCircleTransform(mContext))
                    .Into(ivCover);
                group.Click += (sender, e) => {
                    if (listener != null)
                    {
                        listener.onItemClick(group, groupPosition, groupArray[groupPosition]);
                    }
                };
            } else {
                ivCover.SetImageResource(Resource.Drawable.ic_rank_collapse);
            }

            TextView tvName = group.FindViewById<TextView>(Resource.Id.tvRankGroupName);
            tvName.Text = (groupArray[groupPosition].title);

            ImageView ivArrow = group.FindViewById<ImageView>(Resource.Id.ivRankArrow);
            if (childArray[groupPosition].Count() > 0) {
                if (isExpanded) {
                    ivArrow.SetImageResource(Resource.Drawable.rank_arrow_up);
                } else {
                    ivArrow.SetImageResource(Resource.Drawable.rank_arrow_down);
                }
            } else {
                ivArrow.Visibility = ViewStates.Gone;
            }
            return group;
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }
        public void setItemClickListener(IOnRvItemClickListener<RankingList.MaleBean> listener)
        {
            this.listener = listener;
        }
    }
}