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

namespace Xamarin.BookReader.UI.Activities
{
    public class SearchActivity: BaseRVActivity<SearchDetail.SearchBooks>
    {
        public static String INTENT_QUERY = "query";

        public static void startActivity(Context context, String query)
        {
            context.StartActivity(new Intent(context, typeof(SearchActivity))
                .PutExtra(INTENT_QUERY, query));
        }


        //@Bind(R.id.tvChangeWords)
        TextView mTvChangeWords;
        //@Bind(R.id.tag_group)
        TagGroup mTagGroup;
        //@Bind(R.id.rootLayout)
        LinearLayout mRootLayout;
        //@Bind(R.id.layoutHotWord)
        RelativeLayout mLayoutHotWord;
        //@Bind(R.id.rlHistory)
        RelativeLayout rlHistory;
        //@Bind(R.id.tvClear)
        TextView tvClear;
        //@Bind(R.id.lvSearchHistory)
        ListView lvSearchHistory;

        private List<String> tagList = new ArrayList<>();
        private int times = 0;

        private AutoCompleteAdapter mAutoAdapter;
        private List<String> mAutoList = new ArrayList<>();

        private SearchHistoryAdapter mHisAdapter;
        private List<String> mHisList = new ArrayList<>();
        private String key;
        private MenuItem searchMenuItem;
        private SearchView searchView;

        private ListPopupWindow mListPopupWindow;

        int hotIndex = 0;


        public override int getLayoutId()
        {
            return R.layout.activity_search;
        }

        public override void initToolBar()
        {
            mCommonToolbar.setTitle("");
            mCommonToolbar.setNavigationIcon(R.drawable.ab_back);
        }

        public override void bindViews()
        {
            throw new NotImplementedException();
        }

        public override void initDatas()
        {
            key = getIntent().getStringExtra(INTENT_QUERY);

            mHisAdapter = new SearchHistoryAdapter(this, mHisList);
            lvSearchHistory.setAdapter(mHisAdapter);
            //lvSearchHistory.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            //    @Override
            //    public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
            //        search(mHisList.get(position));
            //    }
            //});
            initSearchHistory();
        }

        public override void configViews()
        {
            initAdapter(SearchAdapter, false, false);

            initAutoList();

            //mTagGroup.setOnTagClickListener(new TagGroup.OnTagClickListener() {
            //    @Override
            //    public void onTagClick(String tag) {
            //        search(tag);
            //    }
            //});

            //mTvChangeWords.setOnClickListener(new View.OnClickListener() {
            //    @Override
            //    public void onClick(View v) {
            //        showHotWord();
            //    }
            //});

            mPresenter.attachView(this);
            mPresenter.getHotWordList();
        }

        private void initAutoList() {
            mAutoAdapter = new AutoCompleteAdapter(this, mAutoList);
            mListPopupWindow = new ListPopupWindow(this);
            mListPopupWindow.setAdapter(mAutoAdapter);
            mListPopupWindow.setWidth(ViewGroup.LayoutParams.MATCH_PARENT);
            mListPopupWindow.setHeight(ViewGroup.LayoutParams.WRAP_CONTENT);
            mListPopupWindow.setAnchorView(mCommonToolbar);
            //mListPopupWindow.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            //    @Override
            //    public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
            //        mListPopupWindow.dismiss();
            //        TextView tv = (TextView) view.findViewById(R.id.tvAutoCompleteItem);
            //        String str = tv.getText().toString();
            //        search(str);
            //    }
            //});
        }

        public /*synchronized*/ void showHotWordList(List<String> list)
        {
            visible(mTvChangeWords);
            tagList.clear();
            tagList.addAll(list);
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
            for (int j = 0; j < tagSize && j < tagList.size(); hotIndex++, j++)
            {
                tags[j] = tagList.get(hotIndex % tagList.size());
            }
            List<TagColor> colors = TagColor.getRandomColors(tagSize);
            mTagGroup.setTags(colors, tags);
        }

        public void showAutoCompleteList(List<String> list)
        {
            mAutoList.clear();
            mAutoList.addAll(list);

            if (!mListPopupWindow.isShowing())
            {
                mListPopupWindow.setInputMethodMode(PopupWindow.INPUT_METHOD_NEEDED);
                mListPopupWindow.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_ADJUST_RESIZE);
                mListPopupWindow.show();
            }
            mAutoAdapter.notifyDataSetChanged();

        }

        public void showSearchResultList(List<SearchDetail.SearchBooks> list)
        {
            mAdapter.clear();
            mAdapter.addAll(list);
            mAdapter.notifyDataSetChanged();
            initSearchResult();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater inflater = getMenuInflater();
            inflater.inflate(R.menu.menu_search, menu);

            searchMenuItem = menu.findItem(R.id.action_search);//在菜单中找到对应控件的item
            searchView = (SearchView) MenuItemCompat.getActionView(searchMenuItem);
            //searchView.setOnQueryTextListener(new SearchView.OnQueryTextListener() {

            //    @Override
            //    public boolean onQueryTextSubmit(String query) {
            //        key = query;
            //        mPresenter.getSearchResultList(query);
            //        saveSearchHistory(query);
            //        return false;
            //    }

            //    @Override
            //    public boolean onQueryTextChange(String newText) {
            //        if (TextUtils.isEmpty(newText)) {
            //            if (mListPopupWindow.isShowing())
            //                mListPopupWindow.dismiss();
            //            initTagGroup();
            //        } else {
            //            mPresenter.getAutoCompleteList(newText);
            //        }
            //        return false;
            //    }
            //});
            search(key); // 外部调用搜索，则打开页面立即进行搜索
            //MenuItemCompat.setOnActionExpandListener(searchMenuItem,
            //    new MenuItemCompat.OnActionExpandListener() {//设置打开关闭动作监听
            //        @Override
            //        public boolean onMenuItemActionExpand(MenuItem item) {
            //            return true;
            //        }

            //        @Override
            //        public boolean onMenuItemActionCollapse(MenuItem item) {
            //            initTagGroup();
            //            return true;
            //        }
            //    });
            return true;
        }


        /**
         * 保存搜索记录.不重复，最多保存20条
         *
         * @param query
         */
        private void saveSearchHistory(String query)
        {
            List<String> list = CacheManager.getInstance().getSearchHistory();
            if (list == null)
            {
                list = new ArrayList<>();
                list.add(query);
            }
            else
            {
                Iterator<String> iterator = list.iterator();
                while (iterator.hasNext())
                {
                    String item = iterator.next();
                    if (TextUtils.equals(query, item))
                    {
                        iterator.remove();
                    }
                }
                list.add(0, query);
            }
            int size = list.size();
            if (size > 20)
            { // 最多保存20条
                for (int i = size - 1; i >= 20; i--)
                {
                    list.remove(i);
                }
            }
            CacheManager.getInstance().saveSearchHistory(list);
            initSearchHistory();
        }

        private void initSearchHistory()
        {
            List<String> list = CacheManager.getInstance().getSearchHistory();
            mHisAdapter.clear();
            if (list != null && list.size() > 0)
            {
                tvClear.setEnabled(true);
                mHisAdapter.addAll(list);
            }
            else
            {
                tvClear.setEnabled(false);
            }
            mHisAdapter.notifyDataSetChanged();
        }

        /**
         * 展开SearchView进行查询
         *
         * @param key
         */
        private void search(String key)
        {
            MenuItemCompat.expandActionView(searchMenuItem);
            if (!TextUtils.isEmpty(key))
            {
                searchView.setQuery(key, true);
                saveSearchHistory(key);
            }
        }

        private void initSearchResult()
        {
            gone(mTagGroup, mLayoutHotWord, rlHistory);
            visible(mRecyclerView);
            if (mListPopupWindow.isShowing())
                mListPopupWindow.dismiss();
        }

        private void initTagGroup()
        {
            visible(mTagGroup, mLayoutHotWord, rlHistory);
            gone(mRecyclerView);
            if (mListPopupWindow.isShowing())
                mListPopupWindow.dismiss();
        }

        public override void onItemClick(int position)
        {
            SearchDetail.SearchBooks data = mAdapter.getItem(position);
            BookDetailActivity.startActivity(this, data._id);
        }

        //@OnClick(R.id.tvClear)
        public void clearSearchHistory() {
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
    }
}