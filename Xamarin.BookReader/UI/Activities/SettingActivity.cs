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

namespace Xamarin.BookReader.UI.Activities
{
    public class SettingActivity: BaseActivity
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
            Task.Factory.StartNew(() => {
                String cachesize = CacheManager.GetCacheSize();
                RunOnUiThread(() => {
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
            noneCoverCompat.CheckedChange += (sender, e) => {
                Settings.IsNoneCover = e.IsChecked;
            };
        }
        
        public void onClickBookShelfSort() {
            new AlertDialog.Builder(mContext)
                    .SetTitle("书架排序方式")
                    //.setSingleChoiceItems(Resources.GetStringArray(Resource.Array.setting_dialog_sort_choice),
                    //        SharedPreferencesUtil.getInstance().getBoolean(Constant.ISBYUPDATESORT, true) ? 0 : 1,
                    //        new DialogInterface.OnClickListener() {
                    //            @Override
                    //            public void onClick(DialogInterface dialog, int which) {
                    //                mTvSort.Text = (Resources.GetStringArray(Resource.Array.setting_dialog_sort_choice)[which]);
                    //                SharedPreferencesUtil.getInstance().putBoolean(Constant.ISBYUPDATESORT, which == 0);
                    //                EventManager.refreshCollectionList();
                    //                dialog.Dismiss();
                    //            }
                    //        })
                    .Create().Show();
        }

        public void onClickFlipStyle() {
            new AlertDialog.Builder(mContext)
                    .SetTitle("阅读页翻页效果")
                    //.setSingleChoiceItems(Resources.GetStringArray(Resource.Array.setting_dialog_style_choice),
                    //        SharedPreferencesUtil.getInstance().getInt(Constant.FLIP_STYLE, 0),
                    //        new DialogInterface.OnClickListener() {
                    //            @Override
                    //            public void onClick(DialogInterface dialog, int which) {
                    //                mTvFlipStyle.Text = (Resources.GetStringArray(Resource.Array.setting_dialog_style_choice)[which]);
                    //                SharedPreferencesUtil.getInstance().putInt(Constant.FLIP_STYLE, which);
                    //                dialog.Dismiss();
                    //            }
                    //        })
                    .Create().Show();
        }

        public void onClickFeedBack()
        {
            FeedbackActivity.startActivity(this);
        }

        public void onClickCleanCache() {
            //默认不勾选清空书架列表，防手抖！！
            bool[] selected = {true, false};
            new AlertDialog.Builder(mContext)
                    .SetTitle("清除缓存")
                    .SetCancelable(true)

                    //.setMultiChoiceItems(new String[]{"删除阅读记录", "清空书架列表"}, selected, new DialogInterface.OnMultiChoiceClickListener() {
                    //    @Override
                    //    public void onClick(DialogInterface dialog, int which, bool isChecked) {
                    //        selected[which] = isChecked;
                    //    }
                    //})
                    //.setPositiveButton("确定", new DialogInterface.OnClickListener() {
                    //    @Override
                    //    public void onClick(DialogInterface dialog, int which) {
                    //        new Thread(new Runnable() {
                    //            @Override
                    //            public void run() {
                    //                CacheManager.getInstance().clearCache(selected[0], selected[1]);
                    //                final String cacheSize = CacheManager.getInstance().getCacheSize();
                    //                runOnUiThread(new Runnable() {
                    //                    @Override
                    //                    public void run() {
                    //                        mTvCacheSize.Text = (cacheSize);
                    //                        EventManager.refreshCollectionList();
                    //                    }
                    //                });
                    //            }
                    //        }).start();
                    //        dialog.Dismiss();
                    //    }
                    //})
                    //.SetNegativeButton("取消", new DialogInterface.OnClickListener() {
                    //    @Override
                    //    public void onClick(DialogInterface dialog, int which) {
                    //        dialog.Dismiss();
                    //    }
                    //})
                    .Create().Show();
        }

    }
}