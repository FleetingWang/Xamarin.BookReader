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

namespace Xamarin.BookReader.UI.Activities
{
    
public class ReadEPubActivity : BaseActivity implements ReaderCallback {

    public static void start(Context context, String filePath) {
        Intent intent = new Intent(context, ReadEPubActivity.class);
        intent.SetAction(Intent.ActionView);
        intent.SetData(Uri.FromFile(new File(filePath)));
        context.StartActivity(intent);
    }

    //@Bind(R.id.epubViewPager)
    DirectionalViewpager viewpager;

    //@Bind(R.id.toolbar_menu)
    View ivMenu;
    //@Bind(R.id.toolbar_title)
    TextView tvTitle;

    private EPubReaderAdapter mAdapter;

    private String mFileName;
    private String mFilePath;

    private Book mBook;
    private List<TOCReference> mTocReferences;
    private List<SpineReference> mSpineReferences;
    public bool mIsSmilParsed = false;

    private List<BookMixAToc.MixToc.Chapters> mChapterList = new List<>();
    private ListPopupWindow mTocListPopupWindow;
    private TocListAdapter mTocListAdapter;

    private bool mIsActionBarVisible = true;
    private int currentChapter;

    @Override
    public int getLayoutId() {
        return Resource.Layout.activity_read_epub;
    }

    @Override
    protected void setupActivityComponent(AppComponent appComponent) {

    }

    @Override
    public void initToolBar() {

        mCommonToolbar.getViewTreeObserver()
                .addOnGlobalLayoutListener(new ViewTreeObserver.OnGlobalLayoutListener() {
                    @Override
                    public void onGlobalLayout() {
                        if (Build.VERSION.SDK_INT > Build.VERSION_CODES.JELLY_BEAN) {
                            mCommonToolbar.getViewTreeObserver().removeOnGlobalLayoutListener(this);
                        } else {
                            mCommonToolbar.getViewTreeObserver().removeGlobalOnLayoutListener(this);
                        }
                        hideToolBarIfVisible();
                    }
                });

        showDialog();
    }

    @Override
    public void initDatas() {
        mFilePath = Uri.Decode(Intent.DataString.Replace("file://", ""));
        mFileName = mFilePath.Substring(mFilePath.LastIndexOf("/") + 1, mFilePath.LastIndexOf("."));
    }

    @Override
    public void configViews() {

        new AsyncTask<Void, Void, Void>() {

            @Override
            protected Void doInBackground(Void... params) {
                loadBook();
                return null;
            }

            @Override
            protected void onPostExecute(Void aVoid) {
                initPager();

                initTocList();
            }
        }.execute();

    }

    private void loadBook() {

        try {
            // 打开书籍
            EpubReader reader = new EpubReader();
            InputStream is = new FileInputStream(mFilePath);
            mBook = reader.readEpub(is);

            mTocReferences = (List<TOCReference>) mBook.getTableOfContents().getTocReferences();
            mSpineReferences = mBook.getSpine().getSpineReferences();

            setSpineReferenceTitle();

            // 解压epub至缓存目录
            FileUtils.unzipFile(mFilePath, Constant.PATH_EPUB + "/" + mFileName);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    private void initPager() {
        viewpager.setOnPageChangeListener(new DirectionalViewpager.OnPageChangeListener() {
            @Override
            public void onPageScrolled(int position, float positionOffset, int positionOffsetPixels) {
            }

            @Override
            public void onPageSelected(int position) {
                mTocListAdapter.setCurrentChapter(position + 1);
            }

            @Override
            public void onPageScrollStateChanged(int state) {
            }
        });

        if (mBook != null && mSpineReferences != null && mTocReferences != null) {

            mAdapter = new EPubReaderAdapter(getSupportFragmentManager(),
                    mSpineReferences, mBook, mFileName, mIsSmilParsed);
            viewpager.setAdapter(mAdapter);
        }

        hideDialog();
    }

    private void initTocList() {
        mTocListAdapter = new TocListAdapter(this, mChapterList, "", 1);
        mTocListAdapter.setEpub(true);
        mTocListPopupWindow = new ListPopupWindow(this);
        mTocListPopupWindow.setAdapter(mTocListAdapter);
        mTocListPopupWindow.setWidth(ViewGroup.LayoutParams.MatchParent);
        mTocListPopupWindow.setHeight(ViewGroup.LayoutParams.WrapContent);
        mTocListPopupWindow.setAnchorView(mCommonToolbar);
        mTocListPopupWindow.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                mTocListPopupWindow.dismiss();
                currentChapter = position + 1;
                mTocListAdapter.setCurrentChapter(currentChapter);
                viewpager.setCurrentItem(position);
            }
        });
        mTocListPopupWindow.setOnDismissListener(new PopupWindow.OnDismissListener() {
            @Override
            public void onDismiss() {
                toolbarAnimateHide();
            }
        });
    }

    private void setSpineReferenceTitle() {
        int srSize = mSpineReferences.size();
        int trSize = mTocReferences.size();
        for (int j = 0; j < srSize; j++) {
            String href = mSpineReferences.get(j).getResource().getHref();
            for (int i = 0; i < trSize; i++) {
                if (mTocReferences.get(i).getResource().getHref().equalsIgnoreCase(href)) {
                    mSpineReferences.get(j).getResource().Title = (mTocReferences.get(i).getTitle());
                    break;
                } else {
                    mSpineReferences.get(j).getResource().Title = ("");
                }
            }
        }

        for (int i = 0; i < trSize; i++) {
            Resource resource = mTocReferences.get(i).getResource();
            if (resource != null) {
                mChapterList.add(new BookMixAToc.MixToc.Chapters(resource.getTitle(), resource.getHref()));
            }
        }
    }

    @Override
    public String getPageHref(int position) {
        String pageHref = mTocReferences.get(position).getResource().getHref();
        String opfpath = FileUtils.getPathOPF(FileUtils.getEpubFolderPath(mFileName));
        if (FileUtils.checkOPFInRootDirectory(FileUtils.getEpubFolderPath(mFileName))) {
            pageHref = FileUtils.getEpubFolderPath(mFileName) + "/" + pageHref;
        } else {
            pageHref = FileUtils.getEpubFolderPath(mFileName) + "/" + opfpath + "/" + pageHref;
        }
        return pageHref;
    }

    @Override
    public void toggleToolBarVisible() {
        if (mIsActionBarVisible) {
            toolbarAnimateHide();
        } else {
            toolbarAnimateShow(1);
        }
    }

    @Override
    public void hideToolBarIfVisible() {
        if (mIsActionBarVisible) {
            toolbarAnimateHide();
        }
    }

    private void toolbarAnimateShow(int verticalOffset) {
        showStatusBar();
        mCommonToolbar.animate()
                .translationY(0)
                .setInterpolator(new LinearInterpolator())
                .setDuration(180)
                .setListener(new AnimatorListenerAdapter() {

                    @Override
                    public void onAnimationStart(Animator animation) {
                        toolbarSetElevation(verticalOffset == 0 ? 0 : 1);
                    }
                });

        new Handler().postDelayed(new Runnable() {
            @Override
            public void run() {
                runOnUiThread(new Runnable() {
                    @Override
                    public void run() {
                        if (mIsActionBarVisible) {
                            toolbarAnimateHide();
                        }
                    }
                });
            }
        }, 10000);

        mIsActionBarVisible = true;
    }

    private void toolbarAnimateHide() {
        if (mIsActionBarVisible) {
            mCommonToolbar.animate()
                    .translationY(-mCommonToolbar.getHeight())
                    .setInterpolator(new LinearInterpolator())
                    .setDuration(180)
                    .setListener(new AnimatorListenerAdapter() {
                        @Override
                        public void onAnimationEnd(Animator animation) {
                            toolbarSetElevation(0);
                            hideStatusBar();
                            if (mTocListPopupWindow != null && mTocListPopupWindow.IsShowing) {
                                mTocListPopupWindow.dismiss();
                            }
                        }
                    });
            mIsActionBarVisible = false;
        }
    }

    @TargetApi(Build.VERSION_CODES.LOLLIPOP)
    private void toolbarSetElevation(float elevation) {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP) {
            mCommonToolbar.setElevation(elevation);
        }
    }

    @OnClick(R.id.toolbar_menu)
    public void showMenu() {
        if (!mTocListPopupWindow.IsShowing) {
            mTocListPopupWindow.setInputMethodMode(PopupWindow.INPUT_METHOD_NEEDED);
            mTocListPopupWindow.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_ADJUST_RESIZE);
            mTocListPopupWindow.show();
            mTocListPopupWindow.setSelection(currentChapter - 1);
            mTocListPopupWindow.getListView().setFastScrollEnabled(true);
        }
    }

    @Override
    public void onBackPressed() {
        if (mTocListPopupWindow != null && mTocListPopupWindow.IsShowing) {
            mTocListPopupWindow.dismiss();
        } else {
            super.onBackPressed();
        }
    }
}

}