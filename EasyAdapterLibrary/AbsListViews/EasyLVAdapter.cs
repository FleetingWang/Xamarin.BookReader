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
using EasyAdapterLibrary.Helpers;
using Java.Lang;

namespace EasyAdapterLibrary.AbsListViews
{
    public abstract class EasyLVAdapter<T> : BaseAdapter, DataHelper<T> where T : class
    {

        protected Context mContext;
        protected List<T> mList;
        protected int[] layoutIds;
        protected LayoutInflater mLInflater;

        protected EasyLVHolder holder = new EasyLVHolder();

        public EasyLVAdapter(Context context, List<T> list, params int[] layoutIds)
        {
            this.mContext = context;
            this.mList = list;
            this.layoutIds = layoutIds;
            this.mLInflater = LayoutInflater.From(mContext);
        }

        public EasyLVAdapter(Context context, List<T> list)
        {
            this.mContext = context;
            this.mList = list;
            this.mLInflater = LayoutInflater.From(mContext);
        }

        public override int Count => mList == null ? 0 : mList.Count();
        public override Java.Lang.Object GetItem(int position)
        {
            return null;//TODO: mList == null ? null : mList[position];
        }
        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            int layoutId = getViewCheckLayoutId(position);
            holder = holder.get<EasyLVHolder>(mContext, position, convertView, parent, layoutId);
            convert(holder, position, mList[position]);
            return holder.getConvertView(layoutId);
        }

        private int getViewCheckLayoutId(int position)
        {
            int layoutId;
            if (layoutIds == null || layoutIds.Length == 0)
            {
                layoutId = getLayoutId(position, mList[position]);
            }
            else
            {
                layoutId = layoutIds[getLayoutIndex(position, mList[position])];
            }
            return layoutId;
        }

        /**
         * 若构造函数没有指定layoutIds, 则必须重写该方法
         *
         * @param position
         * @param item
         * @return layoutId
         */
        public int getLayoutId(int position, T item)
        {
            return 0;
        }

        /**
         * 指定item布局样式在layoutIds的索引。默认为第一个
         *
         * @param position
         * @param item
         * @return
         */
        public int getLayoutIndex(int position, T item)
        {
            return 0;
        }

        public abstract void convert(EasyLVHolder holder, int position, T t);

        public void addAll(List<T> list)
        {
            mList.AddRange(list);
            NotifyDataSetChanged();
        }

        public void addAll(int position, List<T> list)
        {
            mList.InsertRange(position, list);
            NotifyDataSetChanged();
        }

        public void add(T data)
        {
            mList.Add(data);
            NotifyDataSetChanged();
        }

        public void add(int position, T data)
        {
            mList.Insert(position, data);
            NotifyDataSetChanged();
        }

        public void clear()
        {
            mList.Clear();
            NotifyDataSetChanged();
        }

        public bool contains(T data)
        {
            return mList.Contains(data);
        }

        public T getData(int index)
        {
            return mList[index];
        }

        public void modify(T oldData, T newData)
        {
            modify(mList.IndexOf(oldData), newData);
        }

        public void modify(int index, T newData)
        {
            mList[index] = newData;
            NotifyDataSetChanged();
        }

        public bool remove(T data)
        {
            bool result = mList.Remove(data);
            NotifyDataSetChanged();
            return result;
        }

        public void remove(int index)
        {
            mList.RemoveAt(index);
            NotifyDataSetChanged();
        }
    }

}