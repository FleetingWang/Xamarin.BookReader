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

namespace Xamarin.BookReader.UI.Activities
{
    public class SettingActivity: BaseActivity
    {
        public static void startActivity(Context context)
        {
            context.StartActivity(new Intent(context, typeof(SettingActivity)));
        }
        //@Bind(R.id.mTvSort)
        TextView mTvSort;
        //@Bind(R.id.tvFlipStyle)
        TextView mTvFlipStyle;
        //@Bind(R.id.tvCacheSize)
        TextView mTvCacheSize;
        //@Bind(R.id.noneCoverCompat)
        SwitchCompat noneCoverCompat;

        public override int getLayoutId()
        {
            return R.layout.activity_setting;
        }

        public override void bindViews()
        {

        }

        public override void initToolBar()
        {
            mCommonToolbar.setTitle("设置");
            mCommonToolbar.setNavigationIcon(R.drawable.ab_back);
        }
        public override void initDatas()
        {
            Task.Factory.StartNew(() => {
                String cachesize = CacheManager.GetCacheSize();
                RunOnUiThread(() => {
                    mTvCacheSize.Text = (cachesize);
                });
            });
            mTvSort.setText(getResources().getStringArray(R.array.setting_dialog_sort_choice)[
                SharedPreferencesUtil.getInstance().getBoolean(Constant.ISBYUPDATESORT, true) ? 0 : 1]);
            mTvFlipStyle.setText(getResources().getStringArray(R.array.setting_dialog_style_choice)[
                    SharedPreferencesUtil.getInstance().getInt(Constant.FLIP_STYLE, 0)]);
        }
        public override void configViews()
        {
            noneCoverCompat.setChecked(SettingManager.getInstance().isNoneCover());
            //noneCoverCompat.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            //    @Override
            //    public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
            //        SettingManager.getInstance().saveNoneCover(isChecked);
            //    }
            //});
        }

        //@OnClick(R.id.bookshelfSort)
        public void onClickBookShelfSort() {
            new AlertDialog.Builder(mContext)
                    .setTitle("书架排序方式")
                    //.setSingleChoiceItems(getResources().getStringArray(R.array.setting_dialog_sort_choice),
                    //        SharedPreferencesUtil.getInstance().getBoolean(Constant.ISBYUPDATESORT, true) ? 0 : 1,
                    //        new DialogInterface.OnClickListener() {
                    //            @Override
                    //            public void onClick(DialogInterface dialog, int which) {
                    //                mTvSort.setText(getResources().getStringArray(R.array.setting_dialog_sort_choice)[which]);
                    //                SharedPreferencesUtil.getInstance().putBoolean(Constant.ISBYUPDATESORT, which == 0);
                    //                EventManager.refreshCollectionList();
                    //                dialog.dismiss();
                    //            }
                    //        })
                    .create().show();
        }

        //@OnClick(R.id.rlFlipStyle)
        public void onClickFlipStyle() {
            new AlertDialog.Builder(mContext)
                    .setTitle("阅读页翻页效果")
                    //.setSingleChoiceItems(getResources().getStringArray(R.array.setting_dialog_style_choice),
                    //        SharedPreferencesUtil.getInstance().getInt(Constant.FLIP_STYLE, 0),
                    //        new DialogInterface.OnClickListener() {
                    //            @Override
                    //            public void onClick(DialogInterface dialog, int which) {
                    //                mTvFlipStyle.setText(getResources().getStringArray(R.array.setting_dialog_style_choice)[which]);
                    //                SharedPreferencesUtil.getInstance().putInt(Constant.FLIP_STYLE, which);
                    //                dialog.dismiss();
                    //            }
                    //        })
                    .create().show();
        }

        //@OnClick(R.id.feedBack)
        public void feedBack()
        {
            FeedbackActivity.startActivity(this);
        }
        //@OnClick(R.id.cleanCache)
        public void onClickCleanCache() {
            //默认不勾选清空书架列表，防手抖！！
            boolean selected[] = {true, false};
            new AlertDialog.Builder(mContext)
                    .setTitle("清除缓存")
                    .setCancelable(true)
                    //.setMultiChoiceItems(new String[]{"删除阅读记录", "清空书架列表"}, selected, new DialogInterface.OnMultiChoiceClickListener() {
                    //    @Override
                    //    public void onClick(DialogInterface dialog, int which, boolean isChecked) {
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
                    //                        mTvCacheSize.setText(cacheSize);
                    //                        EventManager.refreshCollectionList();
                    //                    }
                    //                });
                    //            }
                    //        }).start();
                    //        dialog.dismiss();
                    //    }
                    //})
                    //.setNegativeButton("取消", new DialogInterface.OnClickListener() {
                    //    @Override
                    //    public void onClick(DialogInterface dialog, int which) {
                    //        dialog.dismiss();
                    //    }
                    //})
                    .create().show();
        }

    }
}