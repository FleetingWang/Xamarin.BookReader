using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using Android.Views.Animations;
using EasyAdapterLibrary.AbsListViews;
using Android.Support.V4.Content;
using Java.Lang;
using System;
using ListPopupWindow = Android.Support.V7.Widget.ListPopupWindow;

namespace Xamarin.BookReader.Views
{
    [Register("xamarin.bookreader.views.SelectionLayout")]
    public class SelectionLayout : LinearLayout
    {
        private Context mContext;
        private LinearLayout parent;

        private OnSelectListener listener;

        public SelectionLayout(Context context)
            : this(context, null)
        {
        }

        public SelectionLayout(Context context, IAttributeSet attrs)
            : this(context, attrs, 0)
        {

        }

        public SelectionLayout(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {
            parent = this;
            this.mContext = context;
        }

        public void setData(List<string>[] data)
        {
            if (data != null && data.Length > 0)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    List<string> list = data[i];
                    ChildView childView = new ChildView(mContext, this);
                    LayoutParams layoutParams = new LayoutParams(0, ViewGroup.LayoutParams.WrapContent);
                    layoutParams.Weight = 1;
                    childView.LayoutParameters = layoutParams;
                    childView.setData(list);
                    childView.Tag = i;
                    AddView(childView);
                }
            }
        }


        private void closeAll()
        {
            int childCount = ChildCount;
            for (int i = 0; i < childCount; i++)
            {
                ChildView childView = (ChildView)GetChildAt(i);
                childView.closePopWindow();
            }
        }

        class ChildView : LinearLayout, IOnClickListener, AdapterView.IOnItemClickListener
        {

            private LinearLayout layout;

            private SelectionLayout _parent;

            private ImageView ivArrow;
            private TextView tvTitle;

            private bool isOpen = false;

            private List<string> data = new List<string>();
            private ListPopupWindow mListPopupWindow;
            private SelAdapter mAdapter;

            Animation operatingAnim1;
            Animation operatingAnim2;
            LinearInterpolator lin1 = new LinearInterpolator();
            LinearInterpolator lin2 = new LinearInterpolator();

            public ChildView(Context context, SelectionLayout parent)
                    : this(context, parent, null)
            {
            }

            public ChildView(Context context, SelectionLayout parent, IAttributeSet attrs)
                    : this(context, parent, attrs, 0)
            {
            }

            public ChildView(Context context, SelectionLayout parent, IAttributeSet attrs, int defStyleAttr)
                    : base(context, attrs, defStyleAttr)
            {
                _parent = parent;
                operatingAnim1 = AnimationUtils.LoadAnimation(_parent.mContext, Resource.Animation.roate_0_180);
                operatingAnim2 = AnimationUtils.LoadAnimation(_parent.mContext, Resource.Animation.roate_180_360);

                LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
                layout = (LinearLayout)inflater.Inflate(Resource.Layout.view_selection, this);

                initView();
            }

            private void initView()
            {
                ivArrow = FindViewById<ImageView>(Resource.Id.ivSelArrow);
                ivArrow.SetScaleType(ImageView.ScaleType.Matrix);   //required
                tvTitle = FindViewById<TextView>(Resource.Id.tvSelTitle);
                SetOnClickListener(this);
                operatingAnim1.Interpolator = lin1;
                operatingAnim1.FillAfter = true;
                operatingAnim2.Interpolator = lin2;
                operatingAnim2.FillAfter = true;
            }

            public void setData(List<string> list)
            {
                if (list != null && list.Any())
                {
                    data.AddRange(list);
                    tvTitle.Text = list[0];
                }
            }

            public void openPopupWindow()
            {
                if (mListPopupWindow == null)
                {
                    createPopupWindow();
                }
                mListPopupWindow.Show();
            }

            private void createPopupWindow()
            {
                mListPopupWindow = new ListPopupWindow(_parent.mContext);
                mAdapter = new SelAdapter(_parent.mContext, data.Select(s => new Java.Lang.String(s)).ToList());
                mListPopupWindow.SetAdapter(mAdapter);
                mListPopupWindow.Width = ViewGroup.LayoutParams.MatchParent;
                mListPopupWindow.Height = ViewGroup.LayoutParams.WrapContent;
                mListPopupWindow.AnchorView = _parent.parent.GetChildAt(0);
                mListPopupWindow.SetForceIgnoreOutsideTouch(false);
                mListPopupWindow.SetOnItemClickListener(this);
                mListPopupWindow.SetOnDismissListener(new CustomPopupWindowOnDismissListener(this));
                mListPopupWindow.Modal = true;
            }

            public void closePopWindow()
            {
                if (mListPopupWindow != null && mListPopupWindow.IsShowing)
                {
                    mListPopupWindow.Dismiss();
                }
            }

            public void OnClick(View v)
            {
                if (isOpen)
                {
                    ivArrow.StartAnimation(operatingAnim2);
                    closePopWindow();
                    isOpen = false;
                }
                else
                {
                    ivArrow.StartAnimation(operatingAnim1);
                    openPopupWindow();
                    isOpen = true;
                }
            }

            public void OnItemClick(AdapterView parent, View view, int position, long id)
            {
                mAdapter.setSelPosition(position);
                tvTitle.Text = data[position];
                if (_parent.listener != null)
                {
                    _parent.listener.onSelect((int)this.Tag, position, data[position]);
                }
                mListPopupWindow.Dismiss();
            }

            class SelAdapter : EasyLVAdapter<Java.Lang.String>
            {

                int selPosition = 0;

                public SelAdapter(Context context, List<Java.Lang.String> list)
                        : base(context, list, Resource.Layout.item_selection_view)
                {

                }

                public override void convert(EasyLVHolder holder, int position, Java.Lang.String s)
                {
                    holder.setText(Resource.Id.tvSelTitleItem, s.ToString());
                    if (selPosition == position)
                    {
                        holder.setTextColor(Resource.Id.tvSelTitleItem, ContextCompat.GetColor(mContext, Resource.Color.light_pink));
                    }
                    else
                    {
                        holder.setTextColor(Resource.Id.tvSelTitleItem, ContextCompat.GetColor(mContext, Resource.Color.light_black));
                    }
                }

                public void setSelPosition(int position)
                {
                    selPosition = position;
                    NotifyDataSetChanged();
                }
            }

            class CustomPopupWindowOnDismissListener : Java.Lang.Object, PopupWindow.IOnDismissListener
            {
                private ChildView _childView;
                public CustomPopupWindowOnDismissListener(ChildView childView)
                {
                    _childView = childView;
                }
                public void OnDismiss()
                {
                    _childView.ivArrow.StartAnimation(_childView.operatingAnim2);
                    _childView.isOpen = false;
                }
            }
        }

        public interface OnSelectListener
        {
            void onSelect(int index, int position, string title);
        }

        public void setOnSelectListener(OnSelectListener listener)
        {
            this.listener = listener;
        }
    }
}