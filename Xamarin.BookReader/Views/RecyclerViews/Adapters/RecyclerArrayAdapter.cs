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
using Android.Support.V7.Widget;
using Java.Lang;
using Android.Util;

namespace Xamarin.BookReader.Views.RecyclerViews.Adapters
{
    public abstract class RecyclerArrayAdapter<T> : RecyclerView.Adapter where T : class
    {
        /**
     * Contains the list of objects that represent the data of this ArrayAdapter.
     * The content of this list is referred to as "the array" in the documentation.
     */
        protected List<T> mObjects;
        protected IEventDelegate mEventDelegate;
        protected List<ItemView> headers = new List<ItemView>();
        protected List<ItemView> footers = new List<ItemView>();

        protected OnItemClickListener mItemClickListener;
        protected OnItemLongClickListener mItemLongClickListener;

        RecyclerView.AdapterDataObserver mObserver;

        public interface ItemView
        {
            View onCreateView(ViewGroup parent);

            void onBindView(View headerView);
        }

        public class GridSpanSizeLookup : GridLayoutManager.SpanSizeLookup
        {

            private int mMaxCount;
            private RecyclerArrayAdapter<T> _adapter;

            public GridSpanSizeLookup(RecyclerArrayAdapter<T> adapter, int maxCount)
            {
                _adapter = adapter;
                this.mMaxCount = maxCount;
            }

            public override int GetSpanSize(int position)
            {
                if (_adapter.headers.Count() != 0)
                {
                    if (position < _adapter.headers.Count()) return mMaxCount;
                }
                if (_adapter.footers.Count() != 0)
                {
                    int i = position - _adapter.headers.Count() - _adapter.mObjects.Count();
                    if (i >= 0)
                    {
                        return mMaxCount;
                    }
                }
                return 1;
            }
        }

        public GridSpanSizeLookup obtainGridSpanSizeLookUp(int maxCount)
        {
            return new GridSpanSizeLookup(this, maxCount);
        }

        /**
         * Lock used to modify the content of {@link #mObjects}. Any write operation
         * performed on the array should be lock on this lock.
         */
        private object mLock = new object();


        /**
         * Indicates whether or not {@link #NotifyDataSetChanged()} must be called whenever
         * {@link #mObjects} is modified.
         */
        private bool mNotifyOnChange = true;

        private Context mContext;


        /**
         * Constructor
         *
         * @param context The current context.
         */
        public RecyclerArrayAdapter(Context context)
        {
            init(context, new List<T>());
        }


        /**
         * Constructor
         *
         * @param context The current context.
         * @param objects The objects to represent in the ListView.
         */
        public RecyclerArrayAdapter(Context context, T[] objects)
        {
            init(context, objects.ToList());
        }

        /**
         * Constructor
         *
         * @param context The current context.
         * @param objects The objects to represent in the ListView.
         */
        public RecyclerArrayAdapter(Context context, List<T> objects)
        {
            init(context, objects);
        }


        private void init(Context context, List<T> objects)
        {
            mContext = context;
            mObjects = objects;
        }


        public void stopMore()
        {
            if (mEventDelegate == null)
                throw new NullPointerException("You should invoking setLoadMore() first");
            mEventDelegate.stopLoadMore();
        }

        public void pauseMore()
        {
            if (mEventDelegate == null)
                throw new NullPointerException("You should invoking setLoadMore() first");
            mEventDelegate.pauseLoadMore();
        }

        public void resumeMore()
        {
            if (mEventDelegate == null)
                throw new NullPointerException("You should invoking setLoadMore() first");
            mEventDelegate.resumeLoadMore();
        }


        public void addHeader(ItemView view)
        {
            if (view == null) throw new NullPointerException("ItemView can't be null");
            headers.Add(view);
            NotifyItemInserted(footers.Count() - 1);
        }

        public void addFooter(ItemView view)
        {
            if (view == null) throw new NullPointerException("ItemView can't be null");
            footers.Add(view);
            NotifyItemInserted(headers.Count() + getCount() + footers.Count() - 1);
        }

        public void removeAllHeader()
        {
            int count = headers.Count();
            headers.Clear();
            NotifyItemRangeRemoved(0, count);
        }

        public void removeAllFooter()
        {
            int count = footers.Count();
            footers.Clear();
            NotifyItemRangeRemoved(headers.Count() + getCount(), count);
        }

        public ItemView getHeader(int index)
        {
            return headers[index];
        }

        public ItemView getFooter(int index)
        {
            return footers[index];
        }

        public int getHeaderCount()
        {
            return headers.Count();
        }

        public int getFooterCount()
        {
            return footers.Count();
        }

        public void removeHeader(ItemView view)
        {
            int position = headers.IndexOf(view);
            headers.Remove(view);
            NotifyItemRemoved(position);
        }

        public void removeFooter(ItemView view)
        {
            int position = headers.Count() + getCount() + footers.IndexOf(view);
            footers.Remove(view);
            NotifyItemRemoved(position);
        }


        IEventDelegate getEventDelegate()
        {
            if (mEventDelegate == null) mEventDelegate = new Adapters.DefaultEventDelegate<T>(this);
            return mEventDelegate;
        }

        public View setMore(int res, IOnLoadMoreListener listener)
        {
            FrameLayout container = new FrameLayout(getContext());
            container.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            LayoutInflater.From(getContext()).Inflate(res, container);
            getEventDelegate().setMore(container, listener);
            return container;
        }

        public View setMore(View view, IOnLoadMoreListener listener)
        {
            getEventDelegate().setMore(view, listener);
            return view;
        }

        public View setNoMore(int res)
        {
            FrameLayout container = new FrameLayout(getContext());
            container.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            LayoutInflater.From(getContext()).Inflate(res, container);
            getEventDelegate().setNoMore(container);
            return container;
        }

        public View setNoMore(View view)
        {
            getEventDelegate().setNoMore(view);
            return view;
        }

        public View setError(int res)
        {
            FrameLayout container = new FrameLayout(getContext());
            container.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            LayoutInflater.From(getContext()).Inflate(res, container);
            getEventDelegate().setErrorMore(container);
            return container;
        }

        public View setError(View view)
        {
            getEventDelegate().setErrorMore(view);
            return view;
        }

        public override void RegisterAdapterDataObserver(RecyclerView.AdapterDataObserver observer)
        {
            if (observer is EasyRecyclerView.EasyDataObserver)
            {
                mObserver = observer;
            }
            else
            {
                base.RegisterAdapterDataObserver(observer);
            }
        }

        /**
         * Adds the specified object at the end of the array.
         *
         * @param object The object to add at the end of the array.
         */
        public void add(T obj)
        {
            if (mEventDelegate != null) mEventDelegate.addData(obj == null ? 0 : 1);
            if (obj != null)
            {
                lock (mLock)
                {
                    mObjects.Add(obj);
                }
            }
            if (mObserver != null) mObserver.OnItemRangeInserted(getCount() + 1, 1);
            if (mNotifyOnChange) NotifyItemInserted(headers.Count() + getCount() + 1);
            log("add NotifyItemInserted " + (headers.Count() + getCount() + 1));
        }

        /**
         * Adds the specified Collection at the end of the array.
         *
         * @param collection The Collection to add at the end of the array.
         */
        public void addAll(ICollection<T> collection)
        {
            if (mEventDelegate != null)
                mEventDelegate.addData(collection == null ? 0 : collection.Count());
            if (collection != null && collection.Count() != 0)
            {
                lock (mLock)
                {
                    mObjects.AddRange(collection);
                }
            }
            int dataCount = collection == null ? 0 : collection.Count();
            if (mObserver != null) mObserver.OnItemRangeInserted(getCount() - dataCount + 1, dataCount);
            if (mNotifyOnChange)
                NotifyItemRangeInserted(headers.Count() + getCount() - dataCount + 1, dataCount);
            log("addAll NotifyItemRangeInserted " + (headers.Count() + getCount() - dataCount + 1) + "," + (dataCount));

        }

        /**
         * Adds the specified items at the end of the array.
         *
         * @param items The items to add at the end of the array.
         */
        public void addAll(T[] items)
        {
            if (mEventDelegate != null) mEventDelegate.addData(items == null ? 0 : items.Length);
            if (items != null && items.Length != 0)
            {
                lock (mLock)
                {
                    mObjects.AddRange(items);
                }
            }
            int dataCount = items == null ? 0 : items.Length;
            if (mObserver != null) mObserver.OnItemRangeInserted(getCount() - dataCount + 1, dataCount);
            if (mNotifyOnChange)
                NotifyItemRangeInserted(headers.Count() + getCount() - dataCount + 1, dataCount);
            log("addAll NotifyItemRangeInserted " + ((headers.Count() + getCount() - dataCount + 1) + "," + (dataCount)));
        }

        /**
         * 插入，不会触发任何事情
         *
         * @param object The object to insert into the array.
         * @param index  The index at which the object must be inserted.
         */
        public void insert(T obj, int index)
        {
            lock (mLock)
            {
                mObjects.Insert(index, obj);
            }
            if (mObserver != null) mObserver.OnItemRangeInserted(index, 1);
            if (mNotifyOnChange) NotifyItemInserted(headers.Count() + index + 1);
            log("insert NotifyItemRangeInserted " + (headers.Count() + index + 1));
        }

        /**
         * 插入数组，不会触发任何事情
         *
         * @param object The object to insert into the array.
         * @param index  The index at which the object must be inserted.
         */
        public void insertAll(T[] objects, int index)
        {
            lock (mLock)
            {
                mObjects.InsertRange(index, objects);
            }
            int dataCount = objects == null ? 0 : objects.Length;
            if (mObserver != null) mObserver.OnItemRangeInserted(index + 1, dataCount);
            if (mNotifyOnChange) NotifyItemRangeInserted(headers.Count() + index + 1, dataCount);
            log("insertAll NotifyItemRangeInserted " + ((headers.Count() + index + 1) + "," + (dataCount)));
        }

        /**
         * 插入数组，不会触发任何事情
         *
         * @param object The object to insert into the array.
         * @param index  The index at which the object must be inserted.
         */
        public void insertAll(ICollection<T> obj, int index)
        {
            lock (mLock)
            {
                mObjects.InsertRange(index, obj);
            }
            int dataCount = obj == null ? 0 : obj.Count();
            if (mObserver != null) mObserver.OnItemRangeInserted(index + 1, dataCount);
            if (mNotifyOnChange) NotifyItemRangeInserted(headers.Count() + index + 1, dataCount);
            log("insertAll NotifyItemRangeInserted " + ((headers.Count() + index + 1) + "," + (dataCount)));
        }

        /**
         * 删除，不会触发任何事情
         *
         * @param object The object to remove.
         */
        public void remove(T obj)
        {
            int position = mObjects.IndexOf(obj);
            lock (mLock)
            {
                if (mObjects.Remove(obj))
                {
                    if (mObserver != null) mObserver.OnItemRangeRemoved(position, 1);
                    if (mNotifyOnChange) NotifyItemRemoved(headers.Count() + position);
                    log("remove NotifyItemRemoved " + (headers.Count() + position));
                }
            }
        }

        /**
         * 删除，不会触发任何事情
         *
         * @param position The position of the object to remove.
         */
        public void remove(int position)
        {
            lock (mLock)
            {
                mObjects.RemoveAt(position);
            }
            if (mObserver != null) mObserver.OnItemRangeRemoved(position, 1);
            if (mNotifyOnChange) NotifyItemRemoved(headers.Count() + position);
            log("remove NotifyItemRemoved " + (headers.Count() + position));
        }


        /**
         * 触发清空
         */
        public void clear()
        {
            int count = mObjects.Count();
            if (mEventDelegate != null) mEventDelegate.clear();
            lock (mLock)
            {
                mObjects.Clear();
            }
            if (mObserver != null) mObserver.OnItemRangeRemoved(0, count);
            if (mNotifyOnChange) NotifyItemRangeRemoved(headers.Count(), count);
            log("clear NotifyItemRangeRemoved " + (headers.Count()) + "," + (count));
        }

        /**
         * Sorts the content of this adapter using the specified comparator.
         *
         * @param comparator The comparator used to sort the objects contained
         *                   in this adapter.
         */
        public void sort(IComparer<T> comparator)
        {
            lock (mLock)
            {
                mObjects.Sort(comparator);
            }
            if (mNotifyOnChange) NotifyDataSetChanged();
        }


        /**
         * Control whether methods that change the list ({@link #add},
         * {@link #insert}, {@link #remove}, {@link #clear}) automatically call
         * {@link #NotifyDataSetChanged}.  If set to false, caller must
         * manually call NotifyDataSetChanged() to have the changes
         * reflected in the attached view.
         * <p>
         * The default is true, and calling NotifyDataSetChanged()
         * resets the flag to true.
         *
         * @param notifyOnChange if true, modifications to the list will
         *                       automatically call {@link
         *                       #NotifyDataSetChanged}
         */
        public void setNotifyOnChange(bool notifyOnChange)
        {
            mNotifyOnChange = notifyOnChange;
        }


        /**
         * Returns the context associated with this array adapter. The context is used
         * to create views from the resource passed to the constructor.
         *
         * @return The Context associated with this adapter.
         */
        public Context getContext()
        {
            return mContext;
        }

        public void setContext(Context ctx)
        {
            mContext = ctx;
        }

        /**
         * 这个函数包含了头部和尾部view的个数，不是真正的item个数。
         *
         * @return
         */
        [Deprecated]
        public override int ItemCount => mObjects.Count() + headers.Count() + footers.Count();

        /**
         * 应该使用这个获取item个数
         *
         * @return
         */
        public int getCount()
        {
            return mObjects.Count();
        }

        private View createSpViewByType(ViewGroup parent, int viewType)
        {
            foreach (ItemView headerView in headers)
            {
                if (headerView.GetHashCode() == viewType)
                {
                    View view = headerView.onCreateView(parent);
                    StaggeredGridLayoutManager.LayoutParams layoutParams;
                    if (view.LayoutParameters != null)
                        layoutParams = new StaggeredGridLayoutManager.LayoutParams(view.LayoutParameters);
                    else
                        layoutParams = new StaggeredGridLayoutManager.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                    layoutParams.FullSpan = true;
                    view.LayoutParameters = layoutParams;
                    return view;
                }
            }
            foreach (ItemView footerview in footers)
            {
                if (footerview.GetHashCode() == viewType)
                {
                    View view = footerview.onCreateView(parent);
                    StaggeredGridLayoutManager.LayoutParams layoutParams;
                    if (view.LayoutParameters != null)
                        layoutParams = new StaggeredGridLayoutManager.LayoutParams(view.LayoutParameters);
                    else
                        layoutParams = new StaggeredGridLayoutManager.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                    layoutParams.FullSpan = true;
                    view.LayoutParameters = layoutParams;
                    return view;
                }
            }
            return null;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = createSpViewByType(parent, viewType);
            if (view != null)
            {
                return new StateViewHolder(view);
            }

            Adapters.BaseViewHolder<T> viewHolder = onCreateViewHolder(parent, viewType);

            //itemView 的点击事件
            if (mItemClickListener != null)
            {
                viewHolder.ItemView.Click += (sender, e) => {
                    mItemClickListener.onItemClick(viewHolder.AdapterPosition - headers.Count());
                };
            }

            if (mItemLongClickListener != null)
            {
                viewHolder.ItemView.LongClick += (sender, e) => {
                    e.Handled = mItemLongClickListener.onItemLongClick(viewHolder.AdapterPosition - headers.Count());
                };
            }
            return viewHolder;
        }

        abstract public BaseViewHolder<T> onCreateViewHolder(ViewGroup parent, int viewType);

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var baseViewHolder = holder as BaseViewHolder<T>;
            baseViewHolder.ItemView.Id = position;
            if (headers.Count() != 0 && position < headers.Count())
            {
                headers[position].onBindView(baseViewHolder.ItemView);
                return;
            }

            int i = position - headers.Count() - mObjects.Count();
            if (footers.Count() != 0 && i >= 0)
            {
                footers[i].onBindView(baseViewHolder.ItemView);
                return;
            }
            OnBindViewHolder(baseViewHolder, position - headers.Count());
        }


        public void OnBindViewHolder(BaseViewHolder<T> holder, int position)
        {
            holder.setData(getItem(position));
        }

        [Deprecated]
        public override int GetItemViewType(int position)
        {
            if (headers.Count() != 0)
            {
                if (position < headers.Count()) return headers[position].GetHashCode();
            }
            if (footers.Count() != 0)
            {
                /*
                eg:
                0:header1
                1:header2   2
                2:object1
                3:object2
                4:object3
                5:object4
                6:footer1   6(position) - 2 - 4 = 0
                7:footer2
                 */
                int i = position - headers.Count() - mObjects.Count();
                if (i >= 0)
                {
                    return footers[i].GetHashCode();
                }
            }
            return getViewType(position - headers.Count());
        }

        public int getViewType(int position)
        {
            return 0;
        }


        public List<T> getAllData()
        {
            return new List<T>(mObjects);
        }

        /**
         * {@inheritDoc}
         */
        public T getItem(int position)
        {
            return mObjects[position];
        }

        /**
         * Returns the position of the specified item in the array.
         *
         * @param item The item to retrieve the position of.
         * @return The position of the specified item.
         */
        public int getPosition(T item)
        {
            return mObjects.IndexOf(item);
        }

        /**
         * {@inheritDoc}
         */
        public long getItemId(int position)
        {
            return position;
        }

        private class StateViewHolder : BaseViewHolder<T>
        {

            public StateViewHolder(View itemView)
                    : base(itemView)
            {
            }
        }

        public interface OnItemClickListener
        {
            void onItemClick(int position);
        }

        public interface OnItemLongClickListener
        {
            bool onItemLongClick(int position);
        }

        public void setOnItemClickListener(OnItemClickListener listener)
        {
            this.mItemClickListener = listener;
        }

        public void setOnItemLongClickListener(OnItemLongClickListener listener)
        {
            this.mItemLongClickListener = listener;
        }

        private static void log(string content)
        {
            if (EasyRecyclerView.DEBUG)
            {
                Log.Info(EasyRecyclerView.TAG, content);
            }
        }
    }
}