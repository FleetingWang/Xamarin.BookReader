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
using Xamarin.BookReader.Bases;
using Xamarin.BookReader.Models;
using Org.Greenrobot.Eventbus;
using Xamarin.BookReader.Views.RecyclerViews;
using Android.Support.V7.Widget;
using Android.Support.V4.Content;
using Xamarin.BookReader.UI.EasyAdapters;
using Xamarin.BookReader.Managers;
using Xamarin.BookReader.Services;
using Xamarin.BookReader.Models.Support;
using Android.Support.V4.App;

namespace Xamarin.BookReader.UI.Fragments
{
    public class RecommendFragment : BaseRVFragment<Recommend.RecommendBooks>,
        RecyclerArrayAdapter<Recommend.RecommendBooks>.OnItemLongClickListener

    {
        LinearLayout llBatchManagement;
        TextView tvSelectAll;
        TextView tvDelete;

        public override int LayoutResId => Resource.Layout.fragment_recommend;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle state)
        {
            var view = base.OnCreateView(inflater, container, state);
            llBatchManagement = view.FindViewById<LinearLayout>(Resource.Id.llBatchManagement);
            tvSelectAll = view.FindViewById<TextView>(Resource.Id.tvSelectAll);
            tvDelete = view.FindViewById<TextView>(Resource.Id.tvDelete);

            tvSelectAll.Click += TvSelectAll_Click;
            tvDelete.Click += TvDelete_Click;

            return view;
        }

        private bool isSelectAll = false;
        private List<BookMixAToc.MixToc.Chapters> chaptersList = new List<BookMixAToc.MixToc.Chapters>();


        public override void InitDatas()
        {
            EventBus.Default.Register(this);
        }

        public override void ConfigViews()
        {
            initAdapter(new RecommendAdapter(Activity), true, false);
            mAdapter.setOnItemLongClickListener(this);
            mAdapter.addFooter(new CustomItemView(Activity));
            mRecyclerView.getEmptyView().FindViewById(Resource.Id.btnToAdd).Click += (sender, e) =>
            {
                (Activity as MainActivity).setCurrentItem(2);
            };
            onRefresh();
        }

        public void showRecommendList(List<Recommend.RecommendBooks> list)
        {
            mAdapter.clear();
            mAdapter.addAll(list);
            // 推荐列表默认加入收藏
            foreach (var bean in list)
            {
                //TODO 此处可优化：批量加入收藏->加入前需先判断是否收藏过
                CollectionsManager.getInstance().add(bean);
            }
        }

        public void showBookToc(String bookId, List<BookMixAToc.MixToc.Chapters> list)
        {
            chaptersList.Clear();
            chaptersList.AddRange(list);
            //DownloadBookService.post(new DownloadQueue(bookId, list, 1, list.Count()));
            DismissDialog();
        }

        //TODO: @Subscribe(threadMode = ThreadMode.MAIN)
        public void downloadMessage(DownloadMessage msg)
        {
            mRecyclerView.setTipViewText(msg.message);
            if (msg.isComplete)
            {
                mRecyclerView.hideTipView(2200);
            }
        }

        //TODO: @Subscribe(threadMode = ThreadMode.MAIN)
        public void showDownProgress(DownloadProgress progress)
        {
            mRecyclerView.setTipViewText(progress.message);
        }

        public override void onItemClick(int position)
        {
            if (IsViewVisible(llBatchManagement)) //批量管理时，屏蔽点击事件
                return;
            // TODO: ReadActivity.StartActivity(Activity, mAdapter.getItem(position), mAdapter.getItem(position).isFromSD);
        }

        public bool onItemLongClick(int position)
        {
            //批量管理时，屏蔽长按事件
            if (IsViewVisible(llBatchManagement)) return false;
            showLongClickDialog(position);
            return false;
        }

        /// <summary>
        /// 显示长按对话框
        /// </summary>
        /// <param name="position"></param>
        private void showLongClickDialog(int position)
        {
            // TODO: 显示长按对话框
        }

        /// <summary>
        /// 显示删除本地缓存对话框
        /// </summary>
        /// <param name="removeList"></param>
        private void showDeleteCacheDialog(List<Recommend.RecommendBooks> removeList)
        {
            // TODO: 显示删除本地缓存对话框
        }

        /// <summary>
        /// 隐藏批量管理布局并刷新页面
        /// </summary>
        public void goneBatchManagementAndRefreshUI()
        {
            if (mAdapter == null) return;
            Gone(llBatchManagement);
            foreach (Recommend.RecommendBooks bean in
                mAdapter.getAllData())
            {
                bean.showCheckBox = false;
            }
            mAdapter.NotifyDataSetChanged();
        }

        /// <summary>
        /// 显示批量管理布局
        /// </summary>
        private void showBatchManagementLayout()
        {
            Visible(llBatchManagement);
            foreach (Recommend.RecommendBooks bean in
                mAdapter.getAllData())
            {
                bean.showCheckBox = true;
            }
            mAdapter.NotifyDataSetChanged();
        }

        private void TvSelectAll_Click(object sender, EventArgs e)
        {
            isSelectAll = !isSelectAll;
            tvSelectAll.Text = (isSelectAll ? Activity.GetString(Resource.String.cancel_selected_all) : Activity.GetString(Resource.String.selected_all));
            foreach (var bean in mAdapter.getAllData())
            {
                bean.isSeleted = isSelectAll;
            }
            mAdapter.NotifyDataSetChanged();
        }

        private void TvDelete_Click(object sender, EventArgs e)
        {
            List<Recommend.RecommendBooks> removeList = new List<Recommend.RecommendBooks>();
            foreach (var bean in mAdapter.getAllData())
            {
                if (bean.isSeleted) removeList.Add(bean);
            }
            if (removeList.Any())
            {
                showDeleteCacheDialog(removeList);
            }
            else
            {
                mRecyclerView.showTipViewAndDelayClose(Activity.GetString(Resource.String.has_not_selected_delete_book));
            }
        }

        public override void onRefresh()
        {
            base.onRefresh();
            Gone(llBatchManagement);
            List<Recommend.RecommendBooks> data = CollectionsManager.getInstance().getCollectionListBySort();
            mAdapter.clear();
            mAdapter.addAll(data);
            //不加下面这句代码会导致，添加本地书籍的时候，部分书籍添加后直接崩溃
            //报错：Scrapped or attached views may not be recycled. isScrap:false isAttached:true
            mAdapter.NotifyDataSetChanged();
            mRecyclerView.setRefreshing(false);
        }

        // TODO: @Subscribe(threadMode = ThreadMode.MAIN)
        public void RefreshCollectionList(RefreshCollectionListEvent e)
        {
            mRecyclerView.setRefreshing(true);
            onRefresh();
        }

        // TODO: @Subscribe(threadMode = ThreadMode.MAIN)
        public void UserSexChooseFinished(UserSexChooseFinishedEvent e)
        {
            //首次进入APP，选择性别后，获取推荐列表
            // TODO: mPresenter.getRecommendList();
        }

        public void showError()
        {
            loaddingError();
            DismissDialog();
        }

        public void complete()
        {
            mRecyclerView.setRefreshing(false);
        }

        public override bool UserVisibleHint
        {
            get => base.UserVisibleHint;
            set
            {
                base.UserVisibleHint = value;
                if (!UserVisibleHint)
                {
                    goneBatchManagementAndRefreshUI();
                }
            }
        }

        public override void OnResume()
        {
            base.OnResume();
            //这样监听返回键有个缺点就是没有拦截Activity的返回监听，如果有更优方案可以改掉
            View.FocusableInTouchMode = true;
            View.RequestFocus();
            View.KeyPress += (sender, e) =>
            {
                if (e.KeyCode == Keycode.Back && e.Event.Action == KeyEventActions.Up)
                {
                    if (IsViewVisible(llBatchManagement))
                    {
                        goneBatchManagementAndRefreshUI();
                        e.Handled = true;
                    }
                }
                e.Handled = false;
            };
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            EventBus.Default.Unregister(this);
        }

        private bool isForeground()
        {
            ActivityManager am = (ActivityManager)Activity.GetSystemService(Context.ActivityService);
            var list = am.GetRunningTasks(1);
            if (list != null && list.Count() > 0)
            {
                ComponentName cpn = list[0].TopActivity;
                if (Java.Lang.Class.FromType(typeof(MainActivity)).Name.Contains(cpn.ClassName))
                {
                    return true;
                }
            }

            return false;
        }

        class CustomItemView : Java.Lang.Object, RecyclerArrayAdapter<Recommend.RecommendBooks>.ItemView
        {
            private FragmentActivity activity;

            public CustomItemView(FragmentActivity activity)
            {
                this.activity = activity;
            }

            public View onCreateView(ViewGroup parent)
            {
                View headerView = LayoutInflater.From(activity).Inflate(Resource.Layout.foot_view_shelf, parent, false);
                return headerView;
            }

            public void onBindView(View headerView)
            {
                headerView.Click += (sender, e) => {
                    (activity as MainActivity).setCurrentItem(2);
                };
            }
        }
    }
}