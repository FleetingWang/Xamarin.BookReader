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
using Xamarin.BookReader.Views;
using Org.Greenrobot.Eventbus;
using Xamarin.BookReader.Models.Support;

namespace Xamarin.BookReader.Bases
{
    public abstract class BaseCommunitActivity : BaseActivity, SelectionLayout.OnSelectListener
    {
        SelectionLayout slOverall;

        protected List<List<String>> list;

        protected List<List<String>> list1 = new List<List<String>>() {
            new List<String>() {
                "全部",
                "精品"
            },
            new List<String>() {
                "默认排序",
                "最新发布",
                "最多评论"
            }
        };

        protected List<List<String>> list2 = new List<List<String>>() {
            new List<String>() {
                "全部",
                "精品",
            },
            new List<String>() {
                "全部类型",
                "玄幻奇幻",
                "武侠仙侠",
                "都市异能",
                "历史军事",
                "游戏竞技",
                "科幻灵异",
                "穿越架空",
                "豪门总裁",
                "现代言情",
                "古代言情",
                "幻想言情",
                "耽美同人"
            },
            new List<String>() {
                "默认排序",
                "最新发布",
                "最多评论",
                "最有用的"
            }
        };

        private Constant.Distillate distillate = Constant.Distillate.All;
        private Constant.BookType type = Constant.BookType.ALL;
        private Constant.SortType sort = Constant.SortType.Default;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            slOverall = FindViewById<SelectionLayout>(Resource.Id.slOverall);
        }

        public override void initDatas()
        {
            list = getTabList();
            if (slOverall != null)
            {
                slOverall.setData(list.ToArray());
                slOverall.setOnSelectListener(this);
            }
        }

        protected abstract List<List<String>> getTabList();

        public void onSelect(int index, int position, string title)
        {
            switch (index)
            {
                case 0:
                    switch (position)
                    {
                        case 0:
                            distillate = Constant.Distillate.All;
                            break;
                        case 1:
                            distillate = Constant.Distillate.Distillate;
                            break;
                        default:
                            break;
                    }
                    break;
                case 1:
                    if (list.Count == 2)
                    {
                        sort = Constant.sortTypeList[position];
                    }
                    else if (list.Count == 3)
                    {
                        type = Constant.bookTypeList[position];
                    }
                    break;
                case 2:
                    sort = Constant.sortTypeList[position];
                    break;
                default:
                    break;
            }

            // TODO: EventBus
            EventBus.Default.Post(new SelectionEvent(distillate, type, sort));
        }
    }
}