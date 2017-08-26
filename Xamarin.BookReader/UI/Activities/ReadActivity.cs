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
using Xamarin.BookReader.UI.Adapters;
using Xamarin.BookReader.Models.Support;
using Xamarin.BookReader.Views.ReadViews;
using Xamarin.BookReader.UI.EasyAdapters;
using Java.Text;
using Newtonsoft.Json;
using Android.Support.V4.Content;
using Uri = Android.Net.Uri;
using Xamarin.BookReader.Managers;
using Java.IO;
using Xamarin.BookReader.Utils;
using System.Reactive.Linq;
using Xamarin.BookReader.Helpers;
using Xamarin.BookReader.Datas;
using Android.Support.V7.App;
using Android.Graphics.Drawables;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Xamarin.BookReader.Services;
using Java.Util;
using DSoft.Messaging;
using System.Reactive.Concurrency;
using Android.Content.PM;

namespace Xamarin.BookReader.UI.Activities
{

    /**
     * Created by lfh on 2016/9/18.
     */
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class ReadActivity : BaseActivity
    {

        ImageView mIvBack;
        TextView mTvBookReadReading;
        TextView mTvBookReadCommunity;
        TextView mTvBookReadChangeSource;
        TextView mTvBookReadSource;

        FrameLayout flReadWidget;

        LinearLayout mLlBookReadTop;
        TextView mTvBookReadTocTitle;
        TextView mTvBookReadMode;
        TextView mTvBookReadSettings;
        TextView mTvBookReadDownload;
        TextView mTvBookReadToc;
        LinearLayout mLlBookReadBottom;
        RelativeLayout mRlBookReadRoot;
        TextView mTvDownloadProgress;

        LinearLayout rlReadAaSet;
        ImageView ivBrightnessMinus;
        SeekBar seekbarLightness;
        ImageView ivBrightnessPlus;
        TextView tvFontsizeMinus;
        SeekBar seekbarFontSize;
        TextView tvFontsizePlus;

        LinearLayout rlReadMark;
        TextView tvAddMark;
        ListView lvMark;

        CheckBox cbVolume;
        CheckBox cbAutoBrightness;
        GridView gvTheme;

        private View decodeView;

        private List<BookMixAToc.MixToc.Chapters> mChapterList = new List<BookMixAToc.MixToc.Chapters>();
        private ListPopupWindow mTocListPopupWindow;
        private TocListAdapter mTocListAdapter;

        private List<BookMark> mMarkList;
        private BookMarkAdapter mMarkAdapter;

        private int currentChapter = 1;

        /**
         * 是否开始阅读章节
         **/
        private bool startRead = false;

        /**
         * 朗读 播放器
         */
        //TODO: 朗读 播放器
        //private TTSPlayer mTtsPlayer;
        //private TtsConfig ttsConfig;

        private BaseReadView mPageWidget;
        private int curTheme = -1;
        private List<ReadTheme> themes;
        private ReadThemeAdapter gvAdapter;
        private Receiver receiver;
        private IntentFilter intentFilter = new IntentFilter();
        private SimpleDateFormat sdf = new SimpleDateFormat("HH:mm");

        public static String INTENT_BEAN = "recommendBooksBean";
        public static String INTENT_SD = "isFromSD";

        private Recommend.RecommendBooks recommendBooks;
        private String bookId;

        private bool isAutoLightness = false; // 记录其他页面是否自动调整亮度
        private bool isFromSD = false;

        //添加收藏需要，所以跳转的时候传递整个实体类
        public static void startActivity(Context context, Recommend.RecommendBooks recommendBooks)
        {
            startActivity(context, recommendBooks, false);
        }

        public static void startActivity(Context context, Recommend.RecommendBooks recommendBooks, bool isFromSD)
        {
            var recommendBooksStr = JsonConvert.SerializeObject(recommendBooks);
            context.StartActivity(new Intent(context, typeof(ReadActivity))
                    .PutExtra(INTENT_BEAN, recommendBooksStr)
                    .PutExtra(INTENT_SD, isFromSD));
        }

        public override int getLayoutId()
        {
            Window.SetFlags(WindowManagerFlags.Fullscreen,
                    WindowManagerFlags.Fullscreen);
            statusBarColor = ContextCompat.GetColor(this, Resource.Color.reader_menu_bg_color);
            return Resource.Layout.activity_read;
        }

        public override void bindViews()
        {
            receiver = new Receiver(this);
            mIvBack = FindViewById<ImageView>(Resource.Id.ivBack);
            mTvBookReadReading = FindViewById<TextView>(Resource.Id.tvBookReadReading);
            mTvBookReadCommunity = FindViewById<TextView>(Resource.Id.tvBookReadCommunity);
            mTvBookReadChangeSource = FindViewById<TextView>(Resource.Id.tvBookReadIntroduce);
            mTvBookReadSource = FindViewById<TextView>(Resource.Id.tvBookReadSource);
            flReadWidget = FindViewById<FrameLayout>(Resource.Id.flReadWidget);
            mLlBookReadTop = FindViewById<LinearLayout>(Resource.Id.llBookReadTop);
            mTvBookReadTocTitle = FindViewById<TextView>(Resource.Id.tvBookReadTocTitle);
            mTvBookReadMode = FindViewById<TextView>(Resource.Id.tvBookReadMode);
            mTvBookReadSettings = FindViewById<TextView>(Resource.Id.tvBookReadSettings);
            mTvBookReadDownload = FindViewById<TextView>(Resource.Id.tvBookReadDownload);
            mTvBookReadToc = FindViewById<TextView>(Resource.Id.tvBookReadToc);
            mLlBookReadBottom = FindViewById<LinearLayout>(Resource.Id.llBookReadBottom);
            mRlBookReadRoot = FindViewById<RelativeLayout>(Resource.Id.rlBookReadRoot);
            mTvDownloadProgress = FindViewById<TextView>(Resource.Id.tvDownloadProgress);
            rlReadAaSet = FindViewById<LinearLayout>(Resource.Id.rlReadAaSet);
            ivBrightnessMinus = FindViewById<ImageView>(Resource.Id.ivBrightnessMinus);
            seekbarLightness = FindViewById<SeekBar>(Resource.Id.seekbarLightness);
            ivBrightnessPlus = FindViewById<ImageView>(Resource.Id.ivBrightnessPlus);
            tvFontsizeMinus = FindViewById<TextView>(Resource.Id.tvFontsizeMinus);
            seekbarFontSize = FindViewById<SeekBar>(Resource.Id.seekbarFontSize);
            tvFontsizePlus = FindViewById<TextView>(Resource.Id.tvFontsizePlus);
            rlReadMark = FindViewById<LinearLayout>(Resource.Id.rlReadMark);
            tvAddMark = FindViewById<TextView>(Resource.Id.tvAddMark);
            lvMark = FindViewById<ListView>(Resource.Id.lvMark);
            cbVolume = FindViewById<CheckBox>(Resource.Id.cbVolume);
            cbAutoBrightness = FindViewById<CheckBox>(Resource.Id.cbAutoBrightness);
            gvTheme = FindViewById<GridView>(Resource.Id.gvTheme);

            mIvBack.Click += onClickBack;
            mTvBookReadReading.Click += readBook;
            mTvBookReadCommunity.Click += onClickCommunity;
            mTvBookReadChangeSource.Click += onClickIntroduce;
            mTvBookReadSource.Click += onClickSource;
            mTvBookReadMode.Click += onClickChangeMode;
            mTvBookReadSettings.Click += setting;
            mTvBookReadDownload.Click += downloadBook;
            var tvBookMark = FindViewById(Resource.Id.tvBookMark);
            tvBookMark.Click += onClickMark;
            mTvBookReadToc.Click += onClickToc;
            ivBrightnessMinus.Click += brightnessMinus;
            ivBrightnessPlus.Click += brightnessPlus;
            tvFontsizeMinus.Click += fontsizeMinus;
            tvFontsizePlus.Click += fontsizePlus;
            var tvClear = FindViewById(Resource.Id.tvClear);
            tvClear.Click += clearBookMark;
            tvAddMark.Click += addBookMark;
        }

        public override void initToolBar()
        {
        }

        public override void initDatas()
        {
            var recommendBooksStr = Intent.GetStringExtra(INTENT_BEAN);
            recommendBooks = JsonConvert.DeserializeObject<Recommend.RecommendBooks>(recommendBooksStr); ;
            bookId = recommendBooks._id;
            isFromSD = Intent.GetBooleanExtra(INTENT_SD, false);

            if (Intent.ActionView.Equals(Intent.Action))
            {
                String filePath = Uri.Decode(Intent.DataString.Replace("file://", ""));
                String fileName;
                if (filePath.LastIndexOf(".") > filePath.LastIndexOf("/"))
                {
                    fileName = filePath.Substring(filePath.LastIndexOf("/") + 1, filePath.LastIndexOf("."));
                }
                else
                {
                    fileName = filePath.Substring(filePath.LastIndexOf("/") + 1);
                }

                CollectionsManager.getInstance().remove(fileName);
                // 转存
                File desc = FileUtils.createWifiTranfesFile(fileName);
                FileUtils.fileChannelCopy(new File(filePath), desc);
                // 建立
                recommendBooks = new Recommend.RecommendBooks();
                recommendBooks.isFromSD = true;
                recommendBooks._id = fileName;
                recommendBooks.title = fileName;

                isFromSD = true;
            }

            MessageBus.Default.Register<DownloadProgress>(showDownProgress);
            MessageBus.Default.Register<DownloadMessage>(downloadMessage);
            showDialog();

            mTvBookReadTocTitle.Text = (recommendBooks.title);

            //mTtsPlayer = TTSPlayerUtils.getTTSPlayer();
            //ttsConfig = TTSPlayerUtils.getTtsConfig();

            intentFilter.AddAction(Intent.ActionBatteryChanged);
            intentFilter.AddAction(Intent.ActionTimeTick);

            CollectionsManager.getInstance().setRecentReadingTime(bookId);
            System.Reactive.Linq.Observable.Timer(new TimeSpan(0, 0, 1)) // 1s
                .Subscribe(_ =>
                {
                    //延迟1秒刷新书架
                    EventManager.refreshCollectionList();
                });
        }

        public override void configViews()
        {
            hideStatusBar();
            decodeView = Window.DecorView;
            RelativeLayout.LayoutParams @params = (RelativeLayout.LayoutParams)mLlBookReadTop.LayoutParameters;
            @params.TopMargin = ScreenUtils.getStatusBarHeight(this) - 2;
            mLlBookReadTop.LayoutParameters = @params;

            initTocList();

            initAASet();

            initPagerWidget();

            // 本地收藏  直接打开
            if (isFromSD)
            {
                BookMixAToc.MixToc.Chapters chapters = new BookMixAToc.MixToc.Chapters();
                chapters.title = recommendBooks.title;
                mChapterList.Add(chapters);
                showChapterRead(null, currentChapter);
                //本地书籍隐藏社区、简介、缓存按钮
                gone(mTvBookReadCommunity, mTvBookReadChangeSource, mTvBookReadDownload);
                return;
            }
            getBookMixAToc(bookId, "chapters");
        }

        private void getBookMixAToc(string bookId, string viewChapters)
        {
            BookApi.Instance.getBookMixAToc(bookId, viewChapters)
                .SubscribeOn(DefaultScheduler.Instance)
                .ObserveOn(Application.SynchronizationContext)
                .Select(s => s.mixToc)
                .Subscribe(data => {
                    List<BookMixAToc.MixToc.Chapters> list = data.chapters;
                    if (list != null && list.Any())
                    {
                        showBookToc(list);
                    }
                }, e => {
                    LogUtils.e("onError: " + e);
                    netError(0);
                }, () => {

                });
        }

        private void initTocList()
        {
            mTocListAdapter = new TocListAdapter(this, mChapterList, bookId, currentChapter);
            mTocListPopupWindow = new ListPopupWindow(this);
            mTocListPopupWindow.SetAdapter(mTocListAdapter);
            mTocListPopupWindow.Width = (ViewGroup.LayoutParams.MatchParent);
            mTocListPopupWindow.Height = (ViewGroup.LayoutParams.WrapContent);
            mTocListPopupWindow.AnchorView = (mLlBookReadTop);
            mTocListPopupWindow.ItemClick += (sender, e) => {
                mTocListPopupWindow.Dismiss();
                currentChapter = e.Position + 1;
                mTocListAdapter.setCurrentChapter(currentChapter);
                startRead = false;
                showDialog();
                readCurrentChapter();
                hideReadBar();
            };
            mTocListPopupWindow.DismissEvent += (sender, e) => {
                gone(mTvBookReadTocTitle);
                visible(mTvBookReadReading, mTvBookReadCommunity, mTvBookReadChangeSource);
            };
        }

        private void initAASet()
        {
            curTheme = Settings.ReadTheme;
            ThemeManager.setReaderTheme(curTheme, mRlBookReadRoot);

            seekbarFontSize.Max = (10);
            //int fontSizePx = Settings.getReadFontSize(bookId);
            int fontSizePx = Settings.FontSize;
            int progress = (int)((ScreenUtils.pxToDpInt(fontSizePx) - 12) / 1.7f);
            seekbarFontSize.Progress = (progress);
            seekbarFontSize.ProgressChanged += SeekbarFontSize_ProgressChanged;

            seekbarLightness.Max = 100;
            seekbarLightness.ProgressChanged += SeekbarFontSize_ProgressChanged;
            seekbarLightness.Progress = (Settings.ReadBrightness);
            isAutoLightness = ScreenUtils.isAutoBrightness(this);
            if (Settings.AutoBrightness)
            {
                startAutoLightness();
            }
            else
            {
                stopAutoLightness();
            }

            cbVolume.Checked = (Settings.VolumeFlipEnable);
            cbVolume.CheckedChange += CbVolume_CheckedChange;

            cbAutoBrightness.Checked = (Settings.AutoBrightness);
            cbAutoBrightness.CheckedChange += CbVolume_CheckedChange;

            gvAdapter = new ReadThemeAdapter(this, (themes = ThemeManager.getReaderThemeData(curTheme)), curTheme);
            gvTheme.Adapter = (gvAdapter);
            gvTheme.ItemClick += (sender, e) => {
                if (e.Position < themes.Count() - 1)
                {
                    changedMode(false, e.Position);
                }
                else
                {
                    changedMode(true, e.Position);
                }
            };
        }

        private void CbVolume_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            var buttonView = sender as CheckBox;
            if (buttonView.Id == cbVolume.Id)
            {
                Settings.VolumeFlipEnable = (e.IsChecked);
            }
            else if (buttonView.Id == cbAutoBrightness.Id)
            {
                if (e.IsChecked)
                {
                    startAutoLightness();
                }
                else
                {
                    stopAutoLightness();
                }
            }
        }

        private void SeekbarFontSize_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            var seekBar = sender as SeekBar;
            if (seekBar.Id == seekbarFontSize.Id && e.FromUser)
            {
                calcFontSize(e.Progress);
            }
            else if (seekBar.Id == seekbarLightness.Id && e.FromUser
                  && !Settings.AutoBrightness)
            { // 非自动调节模式下 才可调整屏幕亮度
                ScreenUtils.setScreenBrightness(e.Progress, this);
                Settings.ReadBrightness = (e.Progress);
            }
        }

        private void initPagerWidget()
        {
            if (SharedPreferencesUtil.getInstance().getInt(Constant.FLIP_STYLE, 0) == 0)
            {
                mPageWidget = new PageWidget(this, bookId, mChapterList, new ReadListener(this));
            }
            else
            {
                mPageWidget = new OverlappedWidget(this, bookId, mChapterList, new ReadListener(this));
            }
            RegisterReceiver(receiver, intentFilter);
            if (SharedPreferencesUtil.getInstance().getBoolean(Constant.ISNIGHT, false))
            {
                mPageWidget.setTextColor(ContextCompat.GetColor(this, Resource.Color.chapter_content_night),
                        ContextCompat.GetColor(this, Resource.Color.chapter_title_night));
            }
            flReadWidget.RemoveAllViews();
            flReadWidget.AddView(mPageWidget);
        }

        /**
         * 加载章节列表
         *
         * @param list
         */
        public void showBookToc(List<BookMixAToc.MixToc.Chapters> list)
        {
            mChapterList.Clear();
            mChapterList.AddRange(list);

            readCurrentChapter();
        }

        /**
         * 获取当前章节。章节文件存在则直接阅读，不存在则请求
         */
        public void readCurrentChapter()
        {
            if (CacheManager.GetChapterFile(bookId, currentChapter) != null)
            {
                showChapterRead(null, currentChapter);
            }
            else
            {
                getChapterRead(mChapterList[currentChapter - 1].link, currentChapter);
            }
        }

        private void getChapterRead(string url, int chapter)
        {
            BookApi.Instance.getChapterRead(url)
                .SubscribeOn(DefaultScheduler.Instance)
                .ObserveOn(Application.SynchronizationContext)
                .Subscribe(data => {
                    if (data.chapter != null)
                    {
                        showChapterRead(data.chapter, chapter);
                    }
                    else
                    {
                        netError(chapter);
                    }
                }, e => {
                    LogUtils.e("onError: " + e);
                    netError(chapter);
                }, () => {

                });
        }

        public /*synchronized*/ void showChapterRead(ChapterRead.Chapter data, int chapter)
        { // 加载章节内容
            if (data != null)
            {
                CacheManager.SaveChapterFile(bookId, chapter, data);
            }

            if (!startRead)
            {
                startRead = true;
                currentChapter = chapter;
                if (!mPageWidget.isPrepared)
                {
                    mPageWidget.init(curTheme);
                }
                else
                {
                    mPageWidget.jumpToChapter(currentChapter);
                }
                hideDialog();
            }
        }

        public void netError(int chapter)
        {
            hideDialog();//防止因为网络问题而出现dialog不消失
            if (Math.Abs(chapter - currentChapter) <= 1)
            {
                ToastUtils.showToast(Resource.String.net_error);
            }
        }

        public void showError()
        {
            hideDialog();
        }

        public void complete()
        {
            hideDialog();
        }

        private /*synchronized*/ void hideReadBar()
        {
            gone(mTvDownloadProgress, mLlBookReadBottom, mLlBookReadTop, rlReadAaSet, rlReadMark);
            hideStatusBar();

            decodeView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LowProfile;
        }

        private /*synchronized*/ void showReadBar()
        { // 显示工具栏
            visible(mLlBookReadBottom, mLlBookReadTop);
            showStatusBar();
            decodeView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LayoutFullscreen;
        }

        private /*synchronized*/ void toggleReadBar()
        { // 切换工具栏 隐藏/显示 状态
            if (isVisible(mLlBookReadTop))
            {
                hideReadBar();
            }
            else
            {
                showReadBar();
            }
        }

        /***************Title Bar*****************/

        public void onClickBack(object sender, EventArgs e)
        {
            if (mTocListPopupWindow.IsShowing)
            {
                mTocListPopupWindow.Dismiss();
            }
            else
            {
                Finish();
            }
        }

        public void readBook(object sender, EventArgs e)
        {
            gone(rlReadAaSet, rlReadMark);
            ToastUtils.showToast("正在拼命开发中...");
        }

        public void onClickCommunity(object sender, EventArgs e)
        {
            gone(rlReadAaSet, rlReadMark);
            BookDetailCommunityActivity.startActivity(this, bookId, mTvBookReadTocTitle.Text.ToString(), 0);
        }

        public void onClickIntroduce(object sender, EventArgs e)
        {
            gone(rlReadAaSet, rlReadMark);
            BookDetailActivity.startActivity(mContext, bookId);
        }

        public void onClickSource(object sender, EventArgs e)
        {
            BookSourceActivity.startActivityForResult(this, bookId, 1);
        }

        /***************Bottom Bar*****************/

        public void onClickChangeMode(object sender, EventArgs e)
        { // 日/夜间模式切换
            gone(rlReadAaSet, rlReadMark);

            bool isNight = !SharedPreferencesUtil.getInstance().getBoolean(Constant.ISNIGHT, false);
            changedMode(isNight, -1);
        }

        private void changedMode(bool isNight, int position)
        {
            SharedPreferencesUtil.getInstance().putBoolean(Constant.ISNIGHT, isNight);
            AppCompatDelegate.DefaultNightMode = (isNight ? AppCompatDelegate.ModeNightYes
                    : AppCompatDelegate.ModeNightNo);

            if (position >= 0)
            {
                curTheme = position;
            }
            else
            {
                curTheme = Settings.ReadTheme;
            }
            gvAdapter.select(curTheme);

            mPageWidget.setTheme(isNight ? ThemeManager.NIGHT : curTheme);
            mPageWidget.setTextColor(ContextCompat.GetColor(mContext, isNight ? Resource.Color.chapter_content_night : Resource.Color.chapter_content_day),
                    ContextCompat.GetColor(mContext, isNight ? Resource.Color.chapter_title_night : Resource.Color.chapter_title_day));

            mTvBookReadMode.Text = (GetString(isNight ? Resource.String.book_read_mode_day_manual_setting
                    : Resource.String.book_read_mode_night_manual_setting));
            Drawable drawable = ContextCompat.GetDrawable(this, isNight ? Resource.Drawable.ic_menu_mode_day_manual
                    : Resource.Drawable.ic_menu_mode_night_manual);
            drawable.SetBounds(0, 0, drawable.MinimumWidth, drawable.MinimumHeight);
            mTvBookReadMode.SetCompoundDrawables(null, drawable, null, null);

            ThemeManager.setReaderTheme(curTheme, mRlBookReadRoot);
        }

        public void setting(object sender, EventArgs e)
        {
            if (isVisible(mLlBookReadBottom))
            {
                if (isVisible(rlReadAaSet))
                {
                    gone(rlReadAaSet);
                }
                else
                {
                    visible(rlReadAaSet);
                    gone(rlReadMark);
                }
            }
        }

        public void downloadBook(object senderr, EventArgs ee)
        {
            gone(rlReadAaSet);
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("缓存多少章？")
                    .SetItems(new String[] { "后面五十章", "后面全部", "全部" }, (sender, e) =>
                    {
                        switch (e.Which)
                        {
                            case 0:
                                DownloadBookService.Post(new DownloadQueue(bookId, mChapterList, currentChapter + 1, currentChapter + 50));
                                break;
                            case 1:
                                DownloadBookService.Post(new DownloadQueue(bookId, mChapterList, currentChapter + 1, mChapterList.Count()));
                                break;
                            case 2:
                                DownloadBookService.Post(new DownloadQueue(bookId, mChapterList, 1, mChapterList.Count()));
                                break;
                            default:
                                break;
                        }
                    });
            builder.Show();
        }

        public void onClickMark(object sender, EventArgs e)
        {
            if (isVisible(mLlBookReadBottom))
            {
                if (isVisible(rlReadMark))
                {
                    gone(rlReadMark);
                }
                else
                {
                    gone(rlReadAaSet);

                    updateMark();

                    visible(rlReadMark);
                }
            }
        }

        public void onClickToc(object sender, EventArgs e)
        {
            gone(rlReadAaSet, rlReadMark);
            if (!mTocListPopupWindow.IsShowing)
            {
                visible(mTvBookReadTocTitle);
                gone(mTvBookReadReading, mTvBookReadCommunity, mTvBookReadChangeSource);
                mTocListPopupWindow.InputMethodMode = ListPopupWindowInputMethodMode.Needed;
                mTocListPopupWindow.SoftInputMode = SoftInput.AdjustResize;
                mTocListPopupWindow.Show();
                mTocListPopupWindow.SetSelection(currentChapter - 1);
                mTocListPopupWindow.ListView.FastScrollEnabled = (true);
            }
        }

        /***************Setting Menu*****************/

        public void brightnessMinus(object sender, EventArgs e)
        {
            int curBrightness = Settings.ReadBrightness;
            if (curBrightness > 2 && !Settings.AutoBrightness)
            {
                seekbarLightness.Progress = ((curBrightness = curBrightness - 2));
                ScreenUtils.setScreenBrightness(curBrightness, this);
                Settings.ReadBrightness = (curBrightness);
            }
        }

        public void brightnessPlus(object sender, EventArgs e)
        {
            int curBrightness = Settings.ReadBrightness;
            if (curBrightness < 99 && !Settings.AutoBrightness)
            {
                seekbarLightness.Progress = ((curBrightness = curBrightness + 2));
                ScreenUtils.setScreenBrightness(curBrightness, this);
                Settings.ReadBrightness = (curBrightness);
            }
        }

        public void fontsizeMinus(object sender, EventArgs e)
        {
            calcFontSize(seekbarFontSize.Progress - 1);
        }

        public void fontsizePlus(object sender, EventArgs e)
        {
            calcFontSize(seekbarFontSize.Progress + 1);
        }

        public void clearBookMark(object sender, EventArgs e)
        {
            Settings.ClearBookMarks(bookId);

            updateMark();
        }

        /***************Book Mark*****************/

        public void addBookMark(object sender, EventArgs e)
        {
            int[] readPos = mPageWidget.getReadPos();
            BookMark mark = new BookMark();
            mark.chapter = readPos[0];
            mark.startPos = readPos[1];
            mark.endPos = readPos[2];
            if (mark.chapter >= 1 && mark.chapter <= mChapterList.Count())
            {
                mark.title = mChapterList[mark.chapter - 1].title;
            }
            mark.desc = mPageWidget.getHeadLine();
            if (Settings.AddBookMark(bookId, mark))
            {
                ToastUtils.showSingleToast("添加书签成功");
                updateMark();
            }
            else
            {
                ToastUtils.showSingleToast("书签已存在");
            }
        }

        private void updateMark()
        {
            if (mMarkAdapter == null)
            {
                mMarkAdapter = new BookMarkAdapter(this, new List<BookMark>());
                lvMark.Adapter = (mMarkAdapter);
                lvMark.ItemClick += (sender, e) => {
                    BookMark mark = mMarkAdapter.getData(e.Position);
                    if (mark != null)
                    {
                        mPageWidget.setPosition(new int[] { mark.chapter, mark.startPos, mark.endPos });
                        hideReadBar();
                    }
                    else
                    {
                        ToastUtils.showSingleToast("书签无效");
                    }
                };
            }
            mMarkAdapter.clear();

            mMarkList = Settings.GetBookMarks(bookId);
            if (mMarkList != null && mMarkList.Count() > 0)
            {
                mMarkList.Reverse();
                mMarkAdapter.addAll(mMarkList);
            }
        }

        /***************Event*****************/

        public void showDownProgress(object sender, MessageBusEvent e)
        {
            var progress = e as DownloadProgress;
            if (bookId.Equals(progress.bookId))
            {
                RunOnUiThread(() => {
                    if (isVisible(mLlBookReadBottom))
                    { // 如果工具栏显示，则进度条也显示
                        visible(mTvDownloadProgress);
                        // 如果之前缓存过，就给提示
                        mTvDownloadProgress.Text = (progress.message);
                    }
                    else
                    {
                        gone(mTvDownloadProgress);
                    }
                });
            }
        }

        public void downloadMessage(object sender, MessageBusEvent e)
        {
            DownloadMessage msg = e as DownloadMessage;
            if (isVisible(mLlBookReadBottom))
            { // 如果工具栏显示，则进度条也显示
                if (bookId.Equals(msg.bookId))
                {
                    RunOnUiThread(() => {
                        visible(mTvDownloadProgress);
                        mTvDownloadProgress.Text = (msg.message);
                        if (msg.isComplete)
                        {
                            mTvDownloadProgress.PostDelayed(() =>
                            {
                                gone(mTvDownloadProgress);
                            }, 2500);
                        }
                    });
                }
            }
        }

        /**
         * 显示加入书架对话框
         *
         * @param bean
         */
        private void showJoinBookShelfDialog(Recommend.RecommendBooks bean)
        {
            new AlertDialog.Builder(mContext)
                    .SetTitle(GetString(Resource.String.book_read_add_book))
                    .SetMessage(GetString(Resource.String.book_read_would_you_like_to_add_this_to_the_book_shelf))
                    .SetPositiveButton(GetString(Resource.String.book_read_join_the_book_shelf), (sender, e) =>
                    {
                        var dialog = sender as AlertDialog;
                        dialog.Dismiss();
                        bean.recentReadingTime = FormatUtils.getCurrentTimeString(FormatUtils.FORMAT_DATE_TIME);
                        CollectionsManager.getInstance().add(bean);
                        Finish();
                    })
                    .SetNegativeButton(GetString(Resource.String.book_read_not), (sender, e) =>
                    {
                        var dialog = sender as AlertDialog;
                        dialog.Dismiss();
                        Finish();
                    })
                    .Create()
                    .Show();
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            switch (requestCode)
            {
                case 1:
                    if (resultCode == Result.Ok)
                    {
                        BookSource bookSource = (BookSource)data.GetSerializableExtra("source");
                        bookId = bookSource._id;
                    }
                    //mPresenter.getBookMixAToc(bookId, "chapters");
                    break;
                default:
                    break;
            }
        }

        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            switch (keyCode)
            {
                case Keycode.Back:
                    if (mTocListPopupWindow != null && mTocListPopupWindow.IsShowing)
                    {
                        mTocListPopupWindow.Dismiss();
                        gone(mTvBookReadTocTitle);
                        visible(mTvBookReadReading, mTvBookReadCommunity, mTvBookReadChangeSource);
                        return true;
                    }
                    else if (isVisible(rlReadAaSet))
                    {
                        gone(rlReadAaSet);
                        return true;
                    }
                    else if (isVisible(mLlBookReadBottom))
                    {
                        hideReadBar();
                        return true;
                    }
                    else if (!CollectionsManager.getInstance().isCollected(bookId))
                    {
                        showJoinBookShelfDialog(recommendBooks);
                        return true;
                    }
                    break;
                case Keycode.Menu:
                    toggleReadBar();
                    return true;
                case Keycode.VolumeDown:
                    if (Settings.VolumeFlipEnable)
                    {
                        return true;
                    }
                    break;
                case Keycode.VolumeUp:
                    if (Settings.VolumeFlipEnable)
                    {
                        return true;
                    }
                    break;
                default:
                    break;
            }
            return base.OnKeyDown(keyCode, e);
        }

        public override bool OnKeyUp([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.VolumeDown)
            {
                if (Settings.VolumeFlipEnable)
                {
                    mPageWidget.nextPage();
                    return true;// 防止翻页有声音
                }
            }
            else if (keyCode == Keycode.VolumeUp)
            {
                if (Settings.VolumeFlipEnable)
                {
                    mPageWidget.prePage();
                    return true;
                }
            }
            return base.OnKeyUp(keyCode, e);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            //if (mTtsPlayer.getPlayerState() == TTSCommonPlayer.PLAYER_STATE_PLAYING)
            //    mTtsPlayer.stop();

            EventManager.refreshCollectionIcon();
            EventManager.refreshCollectionList();
            MessageBus.Default.DeRegister<DownloadProgress>(showDownProgress);
            MessageBus.Default.DeRegister<DownloadMessage>(downloadMessage);

            try
            {
                UnregisterReceiver(receiver);
            }
            catch (Exception e)
            {
                LogUtils.e("Receiver not registered");
            }

            if (isAutoLightness)
            {
                ScreenUtils.startAutoBrightness(this);
            }
            else
            {
                ScreenUtils.stopAutoBrightness(this);
            }
        }

        private class ReadListener : Java.Lang.Object, IOnReadStateChangeListener
        {
            private ReadActivity readActivity;

            public ReadListener(ReadActivity readActivity)
            {
                this.readActivity = readActivity;
            }



            public void onChapterChanged(int chapter)
            {
                LogUtils.i("onChapterChanged:" + chapter);
                readActivity.currentChapter = chapter;
                //TODO：mTocListAdapter.setCurrentChapter(currentChapter);
                // 加载前一节 与 后三节
                for (int i = chapter - 1; i <= chapter + 3 && i <= readActivity.mChapterList.Count(); i++)
                {
                    if (i > 0 && i != chapter
                            && CacheManager.GetChapterFile(readActivity.bookId, i) == null)
                    {
                        readActivity.getChapterRead(readActivity.mChapterList[i - 1].link, i);
                    }
                }
            }

            public void onPageChanged(int chapter, int page)
            {
                LogUtils.i("onPageChanged:" + chapter + "-" + page);
            }

            public void onLoadChapterFailure(int chapter)
            {
                LogUtils.i("onLoadChapterFailure:" + chapter);
                readActivity.startRead = false;
                if (CacheManager.GetChapterFile(readActivity.bookId, chapter) == null)
                {
                    readActivity.getChapterRead(readActivity.mChapterList[chapter - 1].link, chapter);
                }
            }

            public void onCenterClick()
            {
                LogUtils.i("onCenterClick");
                readActivity.toggleReadBar();
            }

            public void onFlip()
            {
                readActivity.hideReadBar();
            }
        }

        class Receiver : BroadcastReceiver
        {
            private ReadActivity readActivity;

            public Receiver(ReadActivity readActivity)
            {
                this.readActivity = readActivity;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                if (readActivity.mPageWidget != null)
                {
                    if (Intent.ActionBatteryChanged.Equals(intent.Action))
                    {
                        int level = intent.GetIntExtra("level", 0);
                        readActivity.mPageWidget.setBattery(100 - level);
                    }
                    else if (Intent.ActionTimeTick.Equals(intent.Action))
                    {
                        readActivity.mPageWidget.setTime(readActivity.sdf.Format(new Date()));
                    }
                }
            }
        }

        private void startAutoLightness()
        {
            Settings.AutoBrightness = (true);
            ScreenUtils.startAutoBrightness(this);
            seekbarLightness.Enabled = (false);
        }

        private void stopAutoLightness()
        {
            Settings.AutoBrightness = (false);
            ScreenUtils.stopAutoBrightness(this);
            int value = Settings.ReadBrightness;
            seekbarLightness.Progress = (value);
            ScreenUtils.setScreenBrightness(value, this);
            seekbarLightness.Enabled = (true);
        }

        private void calcFontSize(int progress)
        {
            // progress range 1 - 10
            if (progress >= 0 && progress <= 10)
            {
                seekbarFontSize.Progress = (progress);
                mPageWidget.setFontSize(ScreenUtils.dpToPxInt(12 + 1.7f * progress));
            }
        }

    }

}