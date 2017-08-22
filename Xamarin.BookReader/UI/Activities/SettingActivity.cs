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
using Android.Support.V7.Widget;
using System.Threading.Tasks;
using Xamarin.BookReader.Datas;
using Xamarin.BookReader.Utils;
using Xamarin.BookReader.Helpers;
using Xamarin.BookReader.Managers;
using Android.Content.PM;

namespace Xamarin.BookReader.UI.Activities
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class SettingActivity : BaseActivity
    {
        public static void startActivity(Context context)
        {
            context.StartActivity(new Intent(context, typeof(SettingActivity)));
        }

        TextView mTvSort;
        TextView mTvFlipStyle;
        TextView mTvCacheSize;
        SwitchCompat noneCoverCompat;

        public override int getLayoutId()
        {
            return Resource.Layout.activity_setting;
        }

        public override void bindViews()
        {
            mTvSort = FindViewById<TextView>(Resource.Id.mTvSort);
            mTvFlipStyle = FindViewById<TextView>(Resource.Id.tvFlipStyle);
            mTvCacheSize = FindViewById<TextView>(Resource.Id.tvCacheSize);
            noneCoverCompat = FindViewById<SwitchCompat>(Resource.Id.noneCoverCompat);

            var bookshelfSort = FindViewById(Resource.Id.bookshelfSort);
            bookshelfSort.Click += (sender, e) => onClickBookShelfSort();
            var rlFlipStyle = FindViewById(Resource.Id.rlFlipStyle);
            rlFlipStyle.Click += (sender, e) => onClickFlipStyle();
            var feedBack = FindViewById(Resource.Id.feedBack);
            feedBack.Click += (sender, e) => onClickFeedBack();
            var cleanCache = FindViewById(Resource.Id.cleanCache);
            cleanCache.Click += (sender, e) => onClickCleanCache();
        }

        public override void initToolBar()
        {
            mCommonToolbar.Title = ("设置");
            mCommonToolbar.SetNavigationIcon(Resource.Drawable.ab_back);
        }
        public override void initDatas()
        {
            Task.Factory.StartNew(() =>
            {
                String cachesize = CacheManager.GetCacheSize();
                RunOnUiThread(() =>
                {
                    mTvCacheSize.Text = (cachesize);
                });
            });
            mTvSort.Text = (Resources.GetStringArray(Resource.Array.setting_dialog_sort_choice)[
                SharedPreferencesUtil.getInstance().getBoolean(Constant.ISBYUPDATESORT, true) ? 0 : 1]);
            mTvFlipStyle.Text = (Resources.GetStringArray(Resource.Array.setting_dialog_style_choice)[
                    SharedPreferencesUtil.getInstance().getInt(Constant.FLIP_STYLE, 0)]);
        }
        public override void configViews()
        {
            noneCoverCompat.Checked = Settings.IsNoneCover;
            noneCoverCompat.CheckedChange += (sender, e) =>
            {
                Settings.IsNoneCover = e.IsChecked;
            };
        }

        public void onClickBookShelfSort()
        {
            new AlertDialog.Builder(mContext)
                    .SetTitle("书架排序方式")
                    .SetSingleChoiceItems(Resources.GetStringArray(Resource.Array.setting_dialog_sort_choice),
                        SharedPreferencesUtil.getInstance().getBoolean(Constant.ISBYUPDATESORT, true) ? 0 : 1,
                        (sender, e) =>
                        {
                            var which = e.Which;
                            var dialog = sender as AlertDialog;
                            mTvSort.Text = (Resources.GetStringArray(Resource.Array.setting_dialog_sort_choice)[which]);
                            SharedPreferencesUtil.getInstance().putBoolean(Constant.ISBYUPDATESORT, which == 0);
                            EventManager.refreshCollectionList();
                            dialog?.Dismiss();
                        })
                    .Create().Show();
        }

        public void onClickFlipStyle()
        {
            new AlertDialog.Builder(mContext)
                    .SetTitle("阅读页翻页效果")
                    .SetSingleChoiceItems(Resources.GetStringArray(Resource.Array.setting_dialog_style_choice),
                        SharedPreferencesUtil.getInstance().getInt(Constant.FLIP_STYLE, 0),
                        (sender, e) =>
                        {
                            var which = e.Which;
                            var dialog = sender as AlertDialog;
                            mTvFlipStyle.Text = (Resources.GetStringArray(Resource.Array.setting_dialog_style_choice)[which]);
                            SharedPreferencesUtil.getInstance().putInt(Constant.FLIP_STYLE, which);
                            dialog.Dismiss();
                        })
                    .Create().Show();
        }

        public void onClickFeedBack()
        {
            FeedbackActivity.startActivity(this);
        }

        public void onClickCleanCache()
        {
            //默认不勾选清空书架列表，防手抖！！
            bool[] selected = { true, false };
            new AlertDialog.Builder(mContext)
                    .SetTitle("清除缓存")
                    .SetCancelable(true)
                    .SetMultiChoiceItems(new String[] { "删除阅读记录", "清空书架列表" }, selected,
                        (sender, e) =>
                        {
                            selected[e.Which] = e.IsChecked;
                        })
                    .SetPositiveButton("确定", (sender, e) =>
                    {
                        Task.Factory.StartNew(() =>
                        {
                            CacheManager.clearCache(selected[0], selected[1]);
                            String cacheSize = CacheManager.GetCacheSize();
                            RunOnUiThread(() =>
                            {
                                mTvCacheSize.Text = (cacheSize);
                                EventManager.refreshCollectionList();
                            });
                        });
                        var dialog = sender as AlertDialog;
                        dialog?.Dismiss();
                    })
                    .SetNegativeButton("取消", (sender, e) =>
                    {
                        var dialog = sender as AlertDialog;
                        dialog?.Dismiss();
                    })
                    .Create().Show();
        }

    }
}