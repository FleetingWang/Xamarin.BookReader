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
using Android.Graphics;
using Xamarin.BookReader.Utils;
using Java.IO;
using Xamarin.BookReader.Models;
using Java.Text;
using Android.Support.V4.Content;
using Java.Util;
using Java.Nio.Channels;
using Java.Nio;
using Settings = Xamarin.BookReader.Helpers.Settings;

namespace Xamarin.BookReader.Views.ReadViews
{
    public class PageFactory
    {
        private Context mContext;
        /**
         * 屏幕宽高
         */
        private int mHeight, mWidth;
        /**
         * 文字区域宽高
         */
        private int mVisibleHeight, mVisibleWidth;
        /**
         * 间距
         */
        private int marginHeight, marginWidth;
        /**
         * 字体大小
         */
        private int mFontSize, mNumFontSize;
        /**
         * 每页行数
         */
        private int mPageLineCount;
        /**
         * 行间距
         **/
        private int mLineSpace;
        /**
         * 字节长度
         */
        private int mbBufferLen;
        /**
         * MappedByteBuffer：高效的文件内存映射
         */
        private MappedByteBuffer mbBuff;
        /**
         * 页首页尾的位置
         */
        private int curEndPos = 0, curBeginPos = 0, tempBeginPos, tempEndPos;
        private int currentChapter, tempChapter;
        private Vector mLines = new Vector();

        private Paint mPaint;
        private Paint mTitlePaint;
        private Bitmap mBookPageBg;

        private DecimalFormat decimalFormat = new DecimalFormat("#0.00");
        private SimpleDateFormat dateFormat = new SimpleDateFormat("HH:mm");
        private int timeLen = 0, percentLen = 0;
        private string time;
        private int battery = 40;
        private Rect rectF;
        private ProgressBar batteryView;
        private Bitmap batteryBitmap;

        private string bookId;
        private List<BookMixAToc.MixToc.Chapters> chaptersList;
        private int chapterSize = 0;
        private int currentPage = 1;

        private IOnReadStateChangeListener listener;
        private string charset = "UTF-8";

        public PageFactory(Context context, string bookId, List<BookMixAToc.MixToc.Chapters> chaptersList)
            : this(context, ScreenUtils.getScreenWidth(), ScreenUtils.getScreenHeight(),
                    //Settings.GetFontSize(bookId),
                    Settings.FontSize,
                    bookId, chaptersList)
        {

        }

        public PageFactory(Context context, int width, int height, int fontSize, string bookId,
                           List<BookMixAToc.MixToc.Chapters> chaptersList)
        {
            mContext = context;
            mWidth = width;
            mHeight = height;
            mFontSize = fontSize;
            mLineSpace = mFontSize / 5 * 2;
            mNumFontSize = ScreenUtils.dpToPxInt(16);
            marginWidth = ScreenUtils.dpToPxInt(15);
            marginHeight = ScreenUtils.dpToPxInt(15);
            mVisibleHeight = mHeight - marginHeight * 2 - mNumFontSize * 2 - mLineSpace * 2;
            mVisibleWidth = mWidth - marginWidth * 2;
            mPageLineCount = mVisibleHeight / (mFontSize + mLineSpace);
            rectF = new Rect(0, 0, mWidth, mHeight);

            mPaint = new Paint() { Flags = PaintFlags.AntiAlias };
            mPaint.TextSize = (mFontSize);
            //mPaint.SetTextSize(ContextCompat.GetColor(context, Resource.Color.chapter_content_day));
            mPaint.Color = (Color.Black);
            mTitlePaint = new Paint() { Flags = PaintFlags.AntiAlias };
            mTitlePaint.TextSize = (mNumFontSize);
            mTitlePaint.Color = new Color(ContextCompat.GetColor(AppUtils.getAppContext(), Resource.Color.chapter_title_day));
            timeLen = (int)mTitlePaint.MeasureText("00:00");
            percentLen = (int)mTitlePaint.MeasureText("00.00%");
            // Typeface typeface = Typeface.createFromAsset(context.getAssets(),"fonts/FZBYSK.TTF");
            // mPaint.setTypeface(typeface);
            // mNumPaint.setTypeface(typeface);

            this.bookId = bookId;
            this.chaptersList = chaptersList;

            time = dateFormat.Format(new Date());
        }

        public File getBookFile(int chapter)
        {
            File file = FileUtils.getChapterFile(bookId, chapter);
            if (file != null && file.Length() > 10)
            {
                // 解决空文件造成编码错误的问题
                charset = FileUtils.getCharset(file.AbsolutePath);
            }
            LogUtils.i("charset=" + charset);
            return file;
        }

        public void openBook()
        {
            openBook(new int[] { 0, 0 });
        }

        public void openBook(int[] position)
        {
            openBook(1, position);
        }

        /**
         * 打开书籍文件
         *
         * @param chapter  阅读章节
         * @param position 阅读位置
         * @return 0：文件不存在或打开失败  1：打开成功
         */
        public int openBook(int chapter, int[] position)
        {
            this.currentChapter = chapter;
            this.chapterSize = chaptersList.Count();
            if (currentChapter > chapterSize)
                currentChapter = chapterSize;
            string path = getBookFile(currentChapter).Path;
            try
            {
                File file = new File(path);
                long length = file.Length();
                if (length > 10)
                {
                    mbBufferLen = (int)length;
                    // 创建文件通道，映射为MappedByteBuffer
                    mbBuff = new RandomAccessFile(file, "r")
                            .Channel
                            .Map(FileChannel.MapMode.ReadOnly, 0, length);
                    curBeginPos = position[0];
                    curEndPos = position[1];
                    onChapterChanged(chapter);
                    mLines.Clear();
                    return 1;
                }
            }
            catch (IOException e)
            {
                e.PrintStackTrace();
            }
            return 0;
        }

        /**
         * 绘制阅读页面
         *
         * @param canvas
         */
        public /*synchronized*/ void onDraw(Canvas canvas)
        {
            if (mLines.Size() == 0)
            {
                curEndPos = curBeginPos;
                mLines = pageDown();
            }
            if (mLines.Size() > 0)
            {
                int y = marginHeight + (mLineSpace << 1);
                // 绘制背景
                if (mBookPageBg != null)
                {
                    canvas.DrawBitmap(mBookPageBg, null, rectF, null);
                }
                else
                {
                    canvas.DrawColor(Color.White);
                }
                // 绘制标题
                canvas.DrawText(chaptersList[currentChapter - 1].title, marginWidth, y, mTitlePaint);
                y += mLineSpace + mNumFontSize;
                // 绘制阅读页面文字
                foreach (string line in mLines.ToEnumerable())
                {
                    y += mLineSpace;
                    if (line.EndsWith("@"))
                    {
                        canvas.DrawText(line.Substring(0, line.Count() - 1), marginWidth, y, mPaint);
                        y += mLineSpace;
                    }
                    else
                    {
                        canvas.DrawText(line, marginWidth, y, mPaint);
                    }
                    y += mFontSize;
                }
                // 绘制提示内容
                if (batteryBitmap != null)
                {
                    canvas.DrawBitmap(batteryBitmap, marginWidth + 2,
                            mHeight - marginHeight - ScreenUtils.dpToPxInt(12), mTitlePaint);
                }

                float percent = (float)currentChapter * 100 / chapterSize;
                canvas.DrawText(decimalFormat.Format(percent) + "%", (mWidth - percentLen) / 2,
                        mHeight - marginHeight, mTitlePaint);

                string mTime = dateFormat.Format(new Date());
                canvas.DrawText(mTime, mWidth - marginWidth - timeLen, mHeight - marginHeight, mTitlePaint);

                // 保存阅读进度
                Settings.SaveReadProgress(bookId, currentChapter, curBeginPos, curEndPos);
            }
        }

        /**
         * 指针移到上一页页首
         */
        private void pageUp()
        {
            Java.Lang.String strParagraph = new Java.Lang.String();
            Vector lines = new Vector(); // 页面行
            int paraSpace = 0;
            mPageLineCount = mVisibleHeight / (mFontSize + mLineSpace);
            while ((lines.Size() < mPageLineCount) && (curBeginPos > 0))
            {
                Vector paraLines = new Vector(); // 段落行
                byte[] parabuffer = readParagraphBack(curBeginPos); // 1.读取上一个段落

                curBeginPos -= parabuffer.Length; // 2.变换起始位置指针
                try
                {
                    strParagraph = new Java.Lang.String(parabuffer, charset);
                }
                catch (UnsupportedEncodingException e)
                {
                    e.PrintStackTrace();
                }
                strParagraph = new Java.Lang.String(strParagraph.ReplaceAll("\r\n", "  "));
                strParagraph = new Java.Lang.String(strParagraph.ReplaceAll("\n", " "));

                while (strParagraph.Length() > 0)
                { // 3.逐行添加到lines
                    int paintSize = mPaint.BreakText(strParagraph.ToString(), true, mVisibleWidth, null);
                    paraLines.Add(strParagraph.Substring(0, paintSize));
                    strParagraph = new Java.Lang.String(strParagraph.Substring(paintSize));
                }
                lines.AddAll(0, paraLines.ToArray());

                while (lines.Size() > mPageLineCount)
                { // 4.如果段落添加完，但是超出一页，则超出部分需删减
                    try
                    {
                        curBeginPos += ((Java.Lang.String)lines.Get(0)).GetBytes(charset).Length; // 5.删减行数同时起始位置指针也要跟着偏移
                        lines.Remove(0);
                    }
                    catch (UnsupportedEncodingException e)
                    {
                        e.PrintStackTrace();
                    }
                }
                curEndPos = curBeginPos; // 6.最后结束指针指向下一段的开始处
                paraSpace += mLineSpace;
                mPageLineCount = (mVisibleHeight - paraSpace) / (mFontSize + mLineSpace); // 添加段落间距，实时更新容纳行数
            }
        }

        /**
         * 根据起始位置指针，读取一页内容
         *
         * @return
         */
        private Vector pageDown()
        {
            Java.Lang.String strParagraph = new Java.Lang.String();
            Vector lines = new Vector();
            int paraSpace = 0;
            mPageLineCount = mVisibleHeight / (mFontSize + mLineSpace);
            while ((lines.Size() < mPageLineCount) && (curEndPos < mbBufferLen))
            {
                byte[] parabuffer = readParagraphForward(curEndPos);
                curEndPos += parabuffer.Length;
                try
                {
                    strParagraph = new Java.Lang.String(parabuffer, charset);
                }
                catch (UnsupportedEncodingException e)
                {
                    e.PrintStackTrace();
                }
                strParagraph = new Java.Lang.String(strParagraph.ReplaceAll("\r\n", "  "));
                strParagraph = new Java.Lang.String(strParagraph.ReplaceAll("\n", " ")); // 段落中的换行符去掉，绘制的时候再换行

                while (strParagraph.Length() > 0)
                {
                    int paintSize = mPaint.BreakText(strParagraph.ToString(), true, mVisibleWidth, null);
                    lines.Add(strParagraph.Substring(0, paintSize));
                    strParagraph = new Java.Lang.String(strParagraph.Substring(paintSize));
                    if (lines.Size() >= mPageLineCount)
                    {
                        break;
                    }
                }
                lines.Set(lines.Size() - 1, lines.Get(lines.Size() - 1) + "@");
                if (strParagraph.Length() != 0)
                {
                    try
                    {
                        curEndPos -= (strParagraph).GetBytes(charset).Length;
                    }
                    catch (UnsupportedEncodingException e)
                    {
                        e.PrintStackTrace();
                    }
                }
                paraSpace += mLineSpace;
                mPageLineCount = (mVisibleHeight - paraSpace) / (mFontSize + mLineSpace);
            }
            return lines;
        }

        /**
         * 获取最后一页的内容。比较繁琐，待优化
         *
         * @return
         */
        public Vector pageLast()
        {
            Java.Lang.String strParagraph = new Java.Lang.String();
            Vector lines = new Vector();
            currentPage = 0;
            while (curEndPos < mbBufferLen)
            {
                int paraSpace = 0;
                mPageLineCount = mVisibleHeight / (mFontSize + mLineSpace);
                curBeginPos = curEndPos;
                while ((lines.Size() < mPageLineCount) && (curEndPos < mbBufferLen))
                {
                    byte[] parabuffer = readParagraphForward(curEndPos);
                    curEndPos += parabuffer.Length;
                    try
                    {
                        strParagraph = new Java.Lang.String(parabuffer, charset);
                    }
                    catch (UnsupportedEncodingException e)
                    {
                        e.PrintStackTrace();
                    }
                    strParagraph = new Java.Lang.String(strParagraph.ReplaceAll("\r\n", "  "));
                    strParagraph = new Java.Lang.String(strParagraph.ReplaceAll("\n", " ")); // 段落中的换行符去掉，绘制的时候再换行

                    while (strParagraph.Length() > 0)
                    {
                        int paintSize = mPaint.BreakText(strParagraph.ToString(), true, mVisibleWidth, null);
                        lines.Add(strParagraph.Substring(0, paintSize));
                        strParagraph = new Java.Lang.String(strParagraph.Substring(paintSize));
                        if (lines.Size() >= mPageLineCount)
                        {
                            break;
                        }
                    }
                    lines.Set(lines.Size() - 1, lines.Get(lines.Size() - 1) + "@");

                    if (strParagraph.Length() != 0)
                    {
                        try
                        {
                            curEndPos -= (strParagraph).GetBytes(charset).Length;
                        }
                        catch (UnsupportedEncodingException e)
                        {
                            e.PrintStackTrace();
                        }
                    }
                    paraSpace += mLineSpace;
                    mPageLineCount = (mVisibleHeight - paraSpace) / (mFontSize + mLineSpace);
                }
                if (curEndPos < mbBufferLen)
                {
                    lines.Clear();
                }
                currentPage++;
            }
            Settings.SaveReadProgress(bookId, currentChapter, curBeginPos, curEndPos);
            return lines;
        }

        /**
         * 读取下一段落
         *
         * @param curEndPos 当前页结束位置指针
         * @return
         */
        private byte[] readParagraphForward(int curEndPos)
        {
            byte b0;
            int i = curEndPos;
            while (i < mbBufferLen)
            {
                b0 = Convert.ToByte(mbBuff.Get(i++));
                if (b0 == 0x0a)
                {
                    break;
                }
            }
            int nParaSize = i - curEndPos;
            byte[] buf = new byte[nParaSize];
            for (i = 0; i < nParaSize; i++)
            {
                buf[i] = Convert.ToByte(mbBuff.Get(curEndPos + i));
            }
            return buf;
        }

        /**
         * 读取上一段落
         *
         * @param curBeginPos 当前页起始位置指针
         * @return
         */
        private byte[] readParagraphBack(int curBeginPos)
        {
            byte b0;
            int i = curBeginPos - 1;
            while (i > 0)
            {
                b0 = Convert.ToByte(mbBuff.Get(i));
                if (b0 == 0x0a && i != curBeginPos - 1)
                {
                    i++;
                    break;
                }
                i--;
            }
            int nParaSize = curBeginPos - i;
            byte[] buf = new byte[nParaSize];
            for (int j = 0; j < nParaSize; j++)
            {
                buf[j] = Convert.ToByte(mbBuff.Get(i + j));
            }
            return buf;
        }

        public bool hasNextPage()
        {
            return currentChapter < chaptersList.Count() || curEndPos < mbBufferLen;
        }

        public bool hasPrePage()
        {
            return currentChapter > 1 || (currentChapter == 1 && curBeginPos > 0);
        }

        /**
         * 跳转下一页
         */
        public BookStatus nextPage()
        {
            if (!hasNextPage())
            { // 最后一章的结束页
                return BookStatus.NO_NEXT_PAGE;
            }
            else
            {
                tempChapter = currentChapter;
                tempBeginPos = curBeginPos;
                tempEndPos = curEndPos;
                if (curEndPos >= mbBufferLen)
                { // 中间章节结束页
                    currentChapter++;
                    int ret = openBook(currentChapter, new int[] { 0, 0 }); // 打开下一章
                    if (ret == 0)
                    {
                        onLoadChapterFailure(currentChapter);
                        currentChapter--;
                        curBeginPos = tempBeginPos;
                        curEndPos = tempEndPos;
                        return BookStatus.NEXT_CHAPTER_LOAD_FAILURE;
                    }
                    else
                    {
                        currentPage = 0;
                        onChapterChanged(currentChapter);
                    }
                }
                else
                {
                    curBeginPos = curEndPos; // 起始指针移到结束位置
                }
                mLines.Clear();
                mLines = pageDown(); // 读取一页内容
                onPageChanged(currentChapter, ++currentPage);
            }
            return BookStatus.LOAD_SUCCESS;
        }

        /**
         * 跳转上一页
         */
        public BookStatus prePage()
        {
            if (!hasPrePage())
            { // 第一章第一页
                return BookStatus.NO_PRE_PAGE;
            }
            else
            {
                // 保存当前页的值
                tempChapter = currentChapter;
                tempBeginPos = curBeginPos;
                tempEndPos = curEndPos;
                if (curBeginPos <= 0)
                {
                    currentChapter--;
                    int ret = openBook(currentChapter, new int[] { 0, 0 });
                    if (ret == 0)
                    {
                        onLoadChapterFailure(currentChapter);
                        currentChapter++;
                        return BookStatus.PRE_CHAPTER_LOAD_FAILURE;
                    }
                    else
                    { // 跳转到上一章的最后一页
                        mLines.Clear();
                        mLines = pageLast();
                        onChapterChanged(currentChapter);
                        onPageChanged(currentChapter, currentPage);
                        return BookStatus.LOAD_SUCCESS;
                    }
                }
                mLines.Clear();
                pageUp(); // 起始指针移到上一页开始处
                mLines = pageDown(); // 读取一页内容
                onPageChanged(currentChapter, --currentPage);
            }
            return BookStatus.LOAD_SUCCESS;
        }

        public void cancelPage()
        {
            currentChapter = tempChapter;
            curBeginPos = tempBeginPos;
            curEndPos = curBeginPos;

            int ret = openBook(currentChapter, new int[] { curBeginPos, curEndPos });
            if (ret == 0)
            {
                onLoadChapterFailure(currentChapter);
                return;
            }
            mLines.Clear();
            mLines = pageDown();
        }

        /**
         * 获取当前阅读位置
         *
         * @return index 0：起始位置 1：结束位置
         */
        public int[] getPosition()
        {
            return new int[] { currentChapter, curBeginPos, curEndPos };
        }

        public string getHeadLineStr()
        {
            if (mLines != null && mLines.Size() > 1)
            {
                return mLines.Get(0).ToString();
            }
            return "";
        }

        /**
         * 设置字体大小
         *
         * @param fontsize 单位：px
         */
        public void setTextFont(int fontsize)
        {
            LogUtils.i("fontSize=" + fontsize);
            mFontSize = fontsize;
            mLineSpace = mFontSize / 5 * 2;
            mPaint.TextSize = mFontSize;
            mPageLineCount = mVisibleHeight / (mFontSize + mLineSpace);
            curEndPos = curBeginPos;
            nextPage();
        }

        /**
         * 设置字体颜色
         *
         * @param textColor
         * @param titleColor
         */
        public void setTextColor(int textColor, int titleColor)
        {
            mPaint.Color = new Color(textColor);
            mTitlePaint.Color = new Color(titleColor);
        }

        public int getTextFont()
        {
            return mFontSize;
        }

        /**
         * 根据百分比，跳到目标位置
         *
         * @param persent
         */
        public void setPercent(int persent)
        {
            float a = (float)(mbBufferLen * persent) / 100;
            curEndPos = (int)a;
            if (curEndPos == 0)
            {
                nextPage();
            }
            else
            {
                nextPage();
                prePage();
                nextPage();
            }
        }

        public void setBgBitmap(Bitmap BG)
        {
            mBookPageBg = BG;
        }

        public void setOnReadStateChangeListener(IOnReadStateChangeListener listener)
        {
            this.listener = listener;
        }

        private void onChapterChanged(int chapter)
        {
            if (listener != null)
                listener.onChapterChanged(chapter);
        }

        private void onPageChanged(int chapter, int page)
        {
            if (listener != null)
                listener.onPageChanged(chapter, page);
        }

        private void onLoadChapterFailure(int chapter)
        {
            if (listener != null)
                listener.onLoadChapterFailure(chapter);
        }

        public void convertBetteryBitmap()
        {
            batteryView = (ProgressBar)LayoutInflater.From(mContext).Inflate(Resource.Layout.layout_battery_progress, null);
            batteryView.ProgressDrawable = (ContextCompat.GetDrawable(mContext,
                    Settings.ReadTheme < 4 ? 
                            Resource.Drawable.seekbar_battery_bg : Resource.Drawable.seekbar_battery_night_bg));
            batteryView.Progress = (battery);
            batteryView.DrawingCacheEnabled = (true);
            batteryView.Measure(View.MeasureSpec.MakeMeasureSpec(ScreenUtils.dpToPxInt(26), MeasureSpecMode.Exactly),
                    View.MeasureSpec.MakeMeasureSpec(ScreenUtils.dpToPxInt(14), MeasureSpecMode.Exactly));
            batteryView.Layout(0, 0, batteryView.MeasuredWidth, batteryView.MeasuredHeight);
            batteryView.BuildDrawingCache();
            //batteryBitmap = batteryView.getDrawingCache();
            // tips: @link{https://github.com/JustWayward/BookReader/issues/109}
            batteryBitmap = Bitmap.CreateBitmap(batteryView.DrawingCache);
            batteryView.DrawingCacheEnabled = (false);
            batteryView.DestroyDrawingCache();
        }

        public void setBattery(int battery)
        {
            this.battery = battery;
            convertBetteryBitmap();
        }

        public void setTime(string time)
        {
            this.time = time;
        }

        public void recycle()
        {
            if (mBookPageBg != null && !mBookPageBg.IsRecycled)
            {
                mBookPageBg.Recycle();
                mBookPageBg = null;
                LogUtils.d("mBookPageBg recycle");
            }

            if (batteryBitmap != null && !batteryBitmap.IsRecycled)
            {
                batteryBitmap.Recycle();
                batteryBitmap = null;
                LogUtils.d("batteryBitmap recycle");
            }
        }
    }
}