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
//using Org.Greenrobot.Eventbus;
using Xamarin.BookReader.Views.RecyclerViews;
using Android.Support.V7.Widget;
using Android.Support.V4.Content;
using Xamarin.BookReader.UI.EasyAdapters;
using Xamarin.BookReader.Managers;
using Xamarin.BookReader.Services;
using Xamarin.BookReader.Models.Support;
using Android.Support.V4.App;
using DSoft.Messaging;
using Xamarin.BookReader.Datas;
using Settings = Xamarin.BookReader.Helpers.Settings;
using System.Threading.Tasks;
using Xamarin.BookReader.Utils;
using Xamarin.BookReader.UI.Activities;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Xamarin.BookReader.Extensions;

namespace Xamarin.BookReader.UI.Fragments
{
    [Register("xamarin.bookreader.ui.fragments.RecommendFragment")]
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
            MessageBus.Default.Register<DownloadMessage>(DownloadMessageEventHandler);
            MessageBus.Default.Register<DownloadProgress>(DownloadProgressEventHandler);
            MessageBus.Default.Register<RefreshCollectionListEvent>(RefreshCollectionListEventHandler);
            MessageBus.Default.Register<UserSexChooseFinishedEvent>(UserSexChooseFinishedEventHandler);
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
                //TO DO 此处可优化：批量加入收藏->加入前需先判断是否收藏过
                CollectionsManager.getInstance().add(bean);
            }
        }

        public void showBookToc(String bookId, List<BookMixAToc.MixToc.Chapters> list)
        {
            chaptersList.Clear();
            chaptersList.AddRange(list);
            DownloadBookService.Post(new DownloadQueue(bookId, list, 1, list.Count()));
            DismissDialog();
        }

        public void DownloadMessageEventHandler(object sender, MessageBusEvent evnt)
        {
            if (evnt is DownloadMessage msg)
            {
                Activity.RunOnUiThread(() => {
                    mRecyclerView.setTipViewText(msg.message);
                    if (msg.isComplete)
                    {
                        mRecyclerView.hideTipView(2200);
                    }
                });
            }
        }

        public void DownloadProgressEventHandler(object sender, MessageBusEvent evnt)
        {
            if (evnt is DownloadMessage progress)
            {
                Activity.RunOnUiThread(() => {
                    mRecyclerView.setTipViewText(progress.message);
                });
            }
        }

        public override void onItemClick(int position)
        {
            if (IsViewVisible(llBatchManagement)) //批量管理时，屏蔽点击事件
                return;
            //TODO: ReadActivity.StartActivity(Activity, mAdapter.getItem(position), mAdapter.getItem(position).isFromSD);
            ToastUtils.showSingleToast("等待开发中");
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
            var isTop = CollectionsManager.getInstance().isTop(mAdapter.getItem(position)._id);
            string[] items;
            EventHandler<DialogClickEventArgs> handler;

            if (mAdapter.getItem(position).isFromSD)
            {
                items = Resources.GetStringArray(Resource.Array.recommend_item_long_click_choice_local);
                handler = (sender, e) => {
                    var dialog = (AlertDialog)sender;
                    var which = e.Which;
                    switch (which)
                    {
                        case 0:
                            //置顶、取消置顶
                            CollectionsManager.getInstance().top(mAdapter.getItem(position)._id, !isTop);
                            break;
                        case 1:
                            //删除
                            List<Recommend.RecommendBooks> removeList = new List<Recommend.RecommendBooks>();
                            removeList.Add(mAdapter.getItem(position));
                            showDeleteCacheDialog(removeList);
                            break;
                        case 2:
                            //批量管理
                            showBatchManagementLayout();
                            break;
                        default:
                            break;
                    }
                    dialog.Dismiss();
                };
            }
            else
            {
                items = Resources.GetStringArray(Resource.Array.recommend_item_long_click_choice);
                handler = (sender, e) => {
                    var dialog = (AlertDialog)sender;
                    var which = e.Which;
                    switch (which)
                    {
                        case 0:
                            //置顶、取消置顶
                            CollectionsManager.getInstance().top(mAdapter.getItem(position)._id, !isTop);
                            break;
                        case 1:
                            // 书籍详情
                            BookDetailActivity.startActivity(Activity,
                                    mAdapter.getItem(position)._id);
                            break;
                        case 2:
                            //移入养肥区
                            mRecyclerView.showTipViewAndDelayClose("正在拼命开发中...");
                            break;
                        case 3:
                            //缓存全本
                            if (mAdapter.getItem(position).isFromSD)
                            {
                                mRecyclerView.showTipViewAndDelayClose("本地文件不支持该选项哦");
                            }
                            else
                            {
                                ShowDialog();
                                getTocList(mAdapter.getItem(position)._id);
                            }
                            break;
                        case 4:
                            //删除
                            List<Recommend.RecommendBooks> removeList = new List<Recommend.RecommendBooks>();
                            removeList.Add(mAdapter.getItem(position));
                            showDeleteCacheDialog(removeList);
                            break;
                        case 5:
                            //批量管理
                            showBatchManagementLayout();
                            break;
                        default:
                            break;
                    }
                    dialog.Dismiss();
                };
            }
            if (isTop) items[0] = GetString(Resource.String.cancle_top);
            new AlertDialog.Builder(Activity)
                .SetTitle(mAdapter.getItem(position).title)
                .SetItems(items, handler)
                //.SetNegativeButton(text:string.Empty, handler:(sender, e) => {
                //    // 不干任何事情
                //})
                .Create().Show();
        }

        private void getTocList(string bookId)
        {
            BookApi.Instance.getBookMixAToc(bookId, "chapters")
                .SubscribeOn(DefaultScheduler.Instance)
                .ObserveOn(Application.SynchronizationContext)
                .Subscribe(data => {
                    List<BookMixAToc.MixToc.Chapters> list = data.mixToc.chapters;
                    if (list != null && list.Any())
                    {
                        Activity.RunOnUiThread(() => {
                            showBookToc(bookId, list);
                        });
                    }
                }, e => {
                    LogUtils.e("onError: " + e);
                    showError();
                }, () => {

                });
        }

        /// <summary>
        /// 显示删除本地缓存对话框
        /// </summary>
        /// <param name="removeList"></param>
        private void showDeleteCacheDialog(List<Recommend.RecommendBooks> removeList)
        {
            bool[] selected = { true };
            new AlertDialog.Builder(Activity)
                .SetTitle(Activity.GetString(Resource.String.remove_selected_book))
                .SetMultiChoiceItems(new String[] { Activity.GetString(Resource.String.delete_local_cache) }, selected,
                    (sender, e) => {
                        selected[0] = e.IsChecked;
                    })
                .SetPositiveButton(Activity.GetString(Resource.String.confirm), (sender, e) => {
                    var dialog = sender as AlertDialog;
                    dialog?.Dismiss();
                    // DoInBackground
                    ShowDialog();
                    Task.Factory
                    .StartNew(() =>
                        CollectionsManager.getInstance().removeSome(removeList, selected[0])
                    )
                    .ContinueWith(task =>
                        Activity.RunOnUiThread(() => {
                                mRecyclerView.showTipViewAndDelayClose("成功移除书籍");
                                foreach (Recommend.RecommendBooks bean in removeList)
                                {
                                    mAdapter.remove(bean);
                                }
                                if (IsViewVisible(llBatchManagement))
                                {
                                    //批量管理完成后，隐藏批量管理布局并刷新页面
                                    goneBatchManagementAndRefreshUI();
                                }
                                HideDialog();
                            }
                        ));
                })
                .SetNegativeButton(Activity.GetString(Resource.String.cancel), (sender, e) => { })
                .Create().Show();
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

        public void RefreshCollectionListEventHandler(object sender, MessageBusEvent evnt)
        {
            Activity.RunOnUiThread(() => {
                mRecyclerView.setRefreshing(true);
                onRefresh();
            });
        }

        public void UserSexChooseFinishedEventHandler(object sender, MessageBusEvent evnt)
        {
            //首次进入APP，选择性别后，获取推荐列表
            BookApi.Instance.getRecommend(Settings.UserChooseSex.GetEnumDescription())
            .Subscribe(recommend => {
                if (recommend != null)
                {
                    Activity.RunOnUiThread(() => {
                        List<Recommend.RecommendBooks> list = recommend.books;
                        if (list != null && list.Any())
                        {
                            showRecommendList(list);
                        }
                    });
                }
            }, e => showError(), complete);
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
            MessageBus.Default.DeRegister<DownloadMessage>(DownloadMessageEventHandler);
            MessageBus.Default.DeRegister<DownloadProgress>(DownloadProgressEventHandler);
            MessageBus.Default.DeRegister<RefreshCollectionListEvent>(RefreshCollectionListEventHandler);
            MessageBus.Default.DeRegister<UserSexChooseFinishedEvent>(UserSexChooseFinishedEventHandler);
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