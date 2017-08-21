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
using Uri = Android.Net.Uri;
using Xamarin.BookReader.UI.Activities;

namespace Xamarin.BookReader.UI.Fragments
{
    [Register("xamarin.bookreader.ui.fragments.FindFragment")]
    public class FindFragment : BaseFragment, IOnRvItemClickListener<FindBean>
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
            mList.Add(new FindBean("排行榜", Resource.Drawable.home_find_rank));
            mList.Add(new FindBean("主题书单", Resource.Drawable.home_find_topic));
            mList.Add(new FindBean("分类", Resource.Drawable.home_find_category));
            mList.Add(new FindBean("官方QQ群", Resource.Drawable.home_find_listen));
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
                    TopRankActivity.startActivity(Activity);
                    break;
                case 1:
                    SubjectBookListActivity.startActivity(Activity);
                    break;
                case 2:
                    StartActivity(new Intent(Activity, typeof(TopCategoryListActivity)));
                    break;
                case 3:
                    StartActivity(new Intent(Intent.ActionView, Uri.Parse("https://jq.qq.com/?_wv=1027&k=46qbql8")));
                    break;
                default:
                    break;
            }
        }
    }
}