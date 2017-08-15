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
using Xamarin.BookReader.Models;
using Android.Text;
using Xamarin.BookReader.Utils;
using Xamarin.BookReader.Bases;
using Java.Text;
using Settings = Xamarin.BookReader.Helpers.Settings;

namespace Xamarin.BookReader.UI.EasyAdapters
{
    public class RecommendAdapter : RecyclerArrayAdapter<Recommend.RecommendBooks>
    {
        public RecommendAdapter(Context context) :base(context)
        {

        }
        public override BaseViewHolder<Recommend.RecommendBooks> onCreateViewHolder(ViewGroup parent, int viewType)
        {
            return new CustomBaseViewHolder(parent, Resource.Layout.item_recommend_list);
        }

        class CustomBaseViewHolder: BaseViewHolder<Recommend.RecommendBooks>
        {
            public CustomBaseViewHolder(ViewGroup parent, int layoutId)
                :base(parent, layoutId)
            {

            }

            public override void setData(Recommend.RecommendBooks item)
            {
                base.setData(item);
                String latelyUpdate = "";
                if (!TextUtils.IsEmpty(FormatUtils.getDescriptionTimeFromDateString(item.updated))) {
                    latelyUpdate = FormatUtils.getDescriptionTimeFromDateString(item.updated) + ":";
                }

                holder.setText(Resource.Id.tvRecommendTitle, item.title)
                        .setText(Resource.Id.tvLatelyUpdate, latelyUpdate)
                        .setText(Resource.Id.tvRecommendShort, item.lastChapter)
                        .setVisible(Resource.Id.ivTopLabel, item.isTop)
                        .setVisible(Resource.Id.ckBoxSelect, item.showCheckBox)
                        .setVisible(Resource.Id.ivUnReadDot, FormatUtils.formatZhuiShuDateString(item.updated)
                                .CompareTo(item.recentReadingTime) > 0);

                if (item.path != null && item.path.EndsWith(Constant.SUFFIX_PDF)) {
                    holder.setImageResource(Resource.Id.ivRecommendCover, Resource.Drawable.ic_shelf_pdf);
                } else if (item.path != null && item.path.EndsWith(Constant.SUFFIX_EPUB)) {
                    holder.setImageResource(Resource.Id.ivRecommendCover, Resource.Drawable.ic_shelf_epub);
                } else if (item.path != null && item.path.EndsWith(Constant.SUFFIX_CHM)) {
                    holder.setImageResource(Resource.Id.ivRecommendCover, Resource.Drawable.ic_shelf_chm);
                } else if (item.isFromSD) {
                    holder.setImageResource(Resource.Id.ivRecommendCover, Resource.Drawable.ic_shelf_txt);
                    long fileLen = FileUtils.getChapterFile(item._id, 1).Length();
                    if (fileLen > 10) {
                        double progress = ((double)Settings.GetReadProgress(item._id)[2]) / fileLen;
                        NumberFormat fmt = NumberFormat.PercentInstance;
                        fmt.MaximumFractionDigits = 2;
                        holder.setText(Resource.Id.tvRecommendShort, "当前阅读进度：" + fmt.Format(progress));
                    }
                } else if (!Settings.IsNoneCover) {
                    holder.setRoundImageUrl(Resource.Id.ivRecommendCover, Constant.IMG_BASE_URL + item.cover,
                            Resource.Drawable.cover_default);
                } else {
                    holder.setImageResource(Resource.Id.ivRecommendCover, Resource.Drawable.cover_default);
                }

                CheckBox ckBoxSelect = holder.getView<CheckBox>(Resource.Id.ckBoxSelect);
                ckBoxSelect.Checked = (item.isSeleted);
                ckBoxSelect.CheckedChange += (sender, e) => {
                    item.isSeleted = e.IsChecked;
                };
            }
        }
    }
}