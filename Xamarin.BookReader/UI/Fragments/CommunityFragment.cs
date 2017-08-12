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
using Xamarin.BookReader.Bases;
using Xamarin.BookReader.Models.Support;
using Xamarin.BookReader.UI.Listeners;
using Android.Support.V7.Widget;
using Xamarin.BookReader.Views;
using Xamarin.BookReader.UI.Adapters;

namespace Xamarin.BookReader.UI.Fragments
{
    public class CommunityFragment : BaseFragment, IOnRvItemClickListener<FindBean>
    {
        RecyclerView mRecyclerView;

        private FindAdapter mAdapter;
        private List<FindBean> mList = new List<FindBean>();

        public override int LayoutResId => Resource.Layout.fragment_find;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle state)
        {
            var view = base.OnCreateView(inflater, container, state);
            mRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerview);
            return view;
        }

        public override void InitDatas()
        {
            mList.Clear();
            mList.Add(new FindBean("综合讨论区", Resource.Drawable.discuss_section));
            mList.Add(new FindBean("书评区", Resource.Drawable.comment_section));
            mList.Add(new FindBean("书荒互助区", Resource.Drawable.helper_section));
            mList.Add(new FindBean("女生区", Resource.Drawable.girl_section));
            mList.Add(new FindBean("原创区", Resource.Drawable.yuanchuang));
        }

        public override void ConfigViews()
        {
            mRecyclerView.HasFixedSize = true;
            mRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity));
            mRecyclerView.AddItemDecoration(new SupportDividerItemDecoration(Activity, LinearLayoutManager.Vertical, true));

            mAdapter = new FindAdapter(Activity, mList, this);
            mRecyclerView.SetAdapter(mAdapter);
        }


        public void onItemClick(View view, int position, FindBean data)
        {
            switch (position)
            {
                case 0:
                    // TODO:BookDiscussionActivity.startActivity(Activity, true);
                    break;
                case 1:
                    // TODO:BookReviewActivity.startActivity(Activity);
                    break;
                case 2:
                    // TODO:BookHelpActivity.startActivity(Activity);
                    break;
                case 3:
                    // TODO:GirlBookDiscussionActivity.startActivity(Activity);
                    break;
                case 4:
                    // TODO:BookDiscussionActivity.startActivity(Activity, false);
                    break;
                default:
                    break;
            }
        }
    }
}