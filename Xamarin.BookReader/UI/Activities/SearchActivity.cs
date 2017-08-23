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
using Xamarin.BookReader.Models;
using Xamarin.BookReader.Views;
using Xamarin.BookReader.Datas;
using Android.Support.V4.View;
using Android.Text;
using Xamarin.BookReader.UI.Adapters;
using Xamarin.BookReader.UI.EasyAdapters;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Xamarin.BookReader.Utils;
using Android.Content.PM;
using SearchView = Android.Support.V7.Widget.SearchView;

namespace Xamarin.BookReader.UI.Activities
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class SearchActivity : BaseRVActivity<SearchDetail.SearchBooks>
    {
        public static String INTENT_QUERY = "query";

        public static void startActivity(Context context, String query)
        {
            context.StartActivity(new Intent(context, typeof(SearchActivity))
                .PutExtra(INTENT_QUERY, query));
        }


        TextView mTvChangeWords;
        TagGroup mTagGroup;
        LinearLayout mRootLayout;
        RelativeLayout mLayoutHotWord;
        RelativeLayout rlHistory;
        TextView tvClear;
        ListView lvSearchHistory;

        private List<String> tagList = new List<String>();
        private int times = 0;

        private AutoCompleteAdapter mAutoAdapter;
        private List<String> mAutoList = new List<String>();

        private SearchHistoryAdapter mHisAdapter;
        private List<String> mHisList = new List<String>();
        private String key;
        private IMenuItem searchMenuItem;
        private SearchView searchView;

        private ListPopupWindow mListPopupWindow;

        int hotIndex = 0;


        public override int getLayoutId()
        {
            return Resource.Layout.activity_search;
        }

        public override void initToolBar()
        {
            mCommonToolbar.Title = ("");
            mCommonToolbar.SetNavigationIcon(Resource.Drawable.ab_back);
        }

        public override void bindViews()
        {
            base.bindViews();
            mTvChangeWords = FindViewById<TextView>(Resource.Id.tvChangeWords);
            mTagGroup = FindViewById<TagGroup>(Resource.Id.tag_group);
            mRootLayout = FindViewById<LinearLayout>(Resource.Id.rootLayout);
            mLayoutHotWord = FindViewById<RelativeLayout>(Resource.Id.layoutHotWord);
            rlHistory = FindViewById<RelativeLayout>(Resource.Id.rlHistory);
            tvClear = FindViewById<TextView>(Resource.Id.tvClear);
            lvSearchHistory = FindViewById<ListView>(Resource.Id.lvSearchHistory);

            tvClear.Click += (sender, e) => clearSearchHistory();
        }

        public override void initDatas()
        {
            key = Intent.GetStringExtra(INTENT_QUERY);

            mHisAdapter = new SearchHistoryAdapter(this, mHisList);
            lvSearchHistory.SetAdapter(mHisAdapter);
            lvSearchHistory.ItemClick += (sender, e) =>
            {
                search(mHisList[e.Position]);
            };
            initSearchHistory();
        }

        public override void configViews()
        {
            initAdapter(new SearchAdapter(this), false, false);

            initAutoList();

            mTagGroup.setOnTagClickListener(new CustomOnTagClickListener(this));
            mTvChangeWords.Click += (sender, e) =>
            {
                showHotWord();
            };

            getHotWordList();
        }

        void getHotWordList()
        {
            BookApi.Instance.getHotWord()
                .SubscribeOn(DefaultScheduler.Instance)
                .ObserveOn(Application.SynchronizationContext)
                .Subscribe(data => {
                    List<String> list = data.hotWords;
                    if (list != null && list.Any())
                    {
                        showHotWordList(list);
                    }
                }, e => {
                    LogUtils.e("SearchActivity", e.ToString());
                }, () => {

                });
        }
        void getAutoCompleteList(String query)
        {
            BookApi.Instance.getAutoComplete(query)
                .SubscribeOn(DefaultScheduler.Instance)
                .ObserveOn(Application.SynchronizationContext)
                .Subscribe(data => {
                    LogUtils.i("getAutoCompleteList" + data.keywords.ToString());
                    List<String> list = data.keywords;
                    if (list != null && list.Any())
                    {
                        showAutoCompleteList(list);
                    }
                }, e => {
                    LogUtils.e("SearchActivity", e.ToString());
                }, () => {

                });
        }
        void getSearchResultList(String query)
        {
            BookApi.Instance.getSearchResult(query)
                .SubscribeOn(DefaultScheduler.Instance)
                .ObserveOn(Application.SynchronizationContext)
                .Subscribe(data => {
                    List<SearchDetail.SearchBooks> list = data.books;
                    if (list != null && list.Any())
                    {
                        showSearchResultList(list);
                    }
                }, e => {
                    LogUtils.e("SearchActivity", e.ToString());
                }, () => {

                });
        }

        private void initAutoList()
        {
            mAutoAdapter = new AutoCompleteAdapter(this, mAutoList);
            mListPopupWindow = new ListPopupWindow(this);
            mListPopupWindow.SetAdapter(mAutoAdapter);
            mListPopupWindow.Width = (ViewGroup.LayoutParams.MatchParent);
            mListPopupWindow.Height = (ViewGroup.LayoutParams.WrapContent);
            mListPopupWindow.AnchorView = (mCommonToolbar);
            mListPopupWindow.ItemClick += (sender, e) =>
            {
                mListPopupWindow.Dismiss();
                TextView tv = e.View.FindViewById<TextView>(Resource.Id.tvAutoCompleteItem);
                String str = tv.Text.ToString();
                search(str);
            };
        }

        public /*synchronized*/ void showHotWordList(List<String> list)
        {
            visible(mTvChangeWords);
            tagList.Clear();
            tagList.AddRange(list);
            times = 0;
            showHotWord();
        }
        /// <summary>
        /// 每次显示8个热搜词
        /// </summary>
        private /*synchronized*/ void showHotWord()
        {
            int tagSize = 8;
            String[] tags = new String[tagSize];
            for (int j = 0; j < tagSize && j < tagList.Count(); hotIndex++, j++)
            {
                tags[j] = tagList[hotIndex % tagList.Count()];
            }
            List<TagColor> colors = TagColor.getRandomColors(tagSize);
            mTagGroup.setTags(colors, tags);
        }

        public void showAutoCompleteList(List<String> list)
        {
            mAutoList.Clear();
            mAutoList.AddRange(list);

            if (!mListPopupWindow.IsShowing)
            {
                mListPopupWindow.InputMethodMode = ListPopupWindowInputMethodMode.Needed;
                mListPopupWindow.SoftInputMode = SoftInput.AdjustResize;
                mListPopupWindow.Show();
            }
            mAutoAdapter.NotifyDataSetChanged();
        }

        public void showSearchResultList(List<SearchDetail.SearchBooks> list)
        {
            mAdapter.clear();
            mAdapter.addAll(list);
            mAdapter.NotifyDataSetChanged();
            initSearchResult();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_search, menu);

            searchMenuItem = menu.FindItem(Resource.Id.action_search);//在菜单中找到对应控件的item
            searchView = (SearchView)MenuItemCompat.GetActionView(searchMenuItem);
            searchView.QueryTextSubmit += (sender, e) =>
            {
                var query = e.Query;
                key = query;
                getSearchResultList(query);
                saveSearchHistory(query);
                e.Handled = false;
            };
            searchView.QueryTextChange += (sender, e) =>
            {
                var newText = e.NewText;
                if (TextUtils.IsEmpty(newText))
                {
                    if (mListPopupWindow.IsShowing)
                        mListPopupWindow.Dismiss();
                    initTagGroup();
                }
                else
                {
                    getAutoCompleteList(newText);
                }
                e.Handled = false;
            };
            search(key); // 外部调用搜索，则打开页面立即进行搜索
            MenuItemCompat.SetOnActionExpandListener(searchMenuItem, new CustomOnActionExpandListener(this));

            return true;
        }


        /**
         * 保存搜索记录.不重复，最多保存20条
         *
         * @param query
         */
        private void saveSearchHistory(String query)
        {
            List<String> list = CacheManager.SearchHistory;
            if (list == null)
            {
                list = new List<String>();
                list.Add(query);
            }
            else
            {
                list.RemoveAll(s => TextUtils.Equals(query, s));
                list.Insert(0, query);
            }
            // 前20
            CacheManager.SearchHistory = list.Take(20).ToList();
            initSearchHistory();
        }

        private void initSearchHistory()
        {
            List<String> list = CacheManager.SearchHistory;
            mHisAdapter.clear();
            if (list != null && list.Count() > 0)
            {
                tvClear.Enabled = (true);
                mHisAdapter.addAll(list);
            }
            else
            {
                tvClear.Enabled = (false);
            }
            mHisAdapter.NotifyDataSetChanged();
        }

        /**
         * 展开SearchView进行查询
         *
         * @param key
         */
        private void search(String key)
        {
            MenuItemCompat.ExpandActionView(searchMenuItem);
            if (!TextUtils.IsEmpty(key))
            {
                searchView.SetQuery(key, true);
                saveSearchHistory(key);
            }
        }

        private void initSearchResult()
        {
            gone(mTagGroup, mLayoutHotWord, rlHistory);
            visible(mRecyclerView);
            if (mListPopupWindow.IsShowing)
                mListPopupWindow.Dismiss();
        }

        private void initTagGroup()
        {
            visible(mTagGroup, mLayoutHotWord, rlHistory);
            gone(mRecyclerView);
            if (mListPopupWindow.IsShowing)
                mListPopupWindow.Dismiss();
        }

        public override void onItemClick(int position)
        {
            SearchDetail.SearchBooks data = mAdapter.getItem(position);
            BookDetailActivity.startActivity(this, data._id);
        }

        public void clearSearchHistory()
        {
            CacheManager.SearchHistory = (null);
            initSearchHistory();
        }

        public void showError()
        {
            loaddingError();
        }
        public void complete()
        {
            mRecyclerView.setRefreshing(false);
        }

        private class CustomOnTagClickListener : TagGroup.OnTagClickListener
        {
            private SearchActivity searchActivity;

            public CustomOnTagClickListener(SearchActivity searchActivity)
            {
                this.searchActivity = searchActivity;
            }

            public void onTagClick(string tag)
            {
                searchActivity.search(tag);
            }
        }

        private class CustomOnActionExpandListener : Java.Lang.Object, MenuItemCompat.IOnActionExpandListener
        {
            private SearchActivity searchActivity;

            public CustomOnActionExpandListener(SearchActivity searchActivity)
            {
                this.searchActivity = searchActivity;
            }

            public bool OnMenuItemActionCollapse(IMenuItem item)
            {
                searchActivity.initTagGroup();
                return true;
            }

            public bool OnMenuItemActionExpand(IMenuItem item)
            {
                return true;
            }
        }
    }
}