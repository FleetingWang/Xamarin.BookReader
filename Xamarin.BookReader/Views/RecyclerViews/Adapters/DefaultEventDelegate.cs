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
using Android.Util;

namespace Xamarin.BookReader.Views.RecyclerViews.Adapters
{
    public class DefaultEventDelegate<T> : Java.Lang.Object, IEventDelegate
    {
        private Adapters.RecyclerArrayAdapter<T> adapter;
        private EventFooter footer;

        private IOnLoadMoreListener onLoadMoreListener;

        private bool hasData = false;
        private bool isLoadingMore = false;

        private bool hasMore = false;
        private bool hasNoMore = false;
        private bool hasError = false;

        private int status = STATUS_INITIAL;
        private const int STATUS_INITIAL = 291;
        private const int STATUS_MORE = 260;
        private const int STATUS_NOMORE = 408;
        private const int STATUS_ERROR = 732;

        public DefaultEventDelegate(RecyclerArrayAdapter<T> adapter)
        {
            this.adapter = adapter;
            footer = new EventFooter(this);
            adapter.addFooter(footer);
        }

        public void onMoreViewShowed()
        {
            log("onMoreViewShowed");
            if (!isLoadingMore && onLoadMoreListener != null)
            {
                isLoadingMore = true;
                onLoadMoreListener.onLoadMore();
            }
        }

        public void onErrorViewShowed()
        {
            resumeLoadMore();
        }

        public void addData(int length)
        {
            log("addData" + length);
            if (hasMore)
            {
                if (length == 0)
                {
                    //当添加0个时，认为已结束加载到底
                    if (status == STATUS_INITIAL || status == STATUS_MORE)
                    {
                        footer.showNoMore();
                    }
                }
                else
                {
                    //当Error或初始时。添加数据，如果有More则还原。
                    if (hasMore && (status == STATUS_INITIAL || status == STATUS_ERROR))
                    {
                        footer.showMore();
                    }
                    hasData = true;
                }
            }
            else
            {
                if (hasNoMore)
                {
                    footer.showNoMore();
                    status = STATUS_NOMORE;
                }
            }
            isLoadingMore = false;
        }

        public void clear()
        {
            log("clear");
            hasData = false;
            status = STATUS_INITIAL;
            footer.hide();
            isLoadingMore = false;
        }

        public void pauseLoadMore()
        {
            log("pauseLoadMore");
            footer.showError();
            status = STATUS_ERROR;
            isLoadingMore = false;
        }

        public void resumeLoadMore()
        {
            isLoadingMore = false;
            footer.showMore();
            onMoreViewShowed();
        }

        public void setErrorMore(View view)
        {
            this.footer.setErrorView(view);
            hasError = true;
            log("setErrorMore");
        }

        public void setMore(View view, IOnLoadMoreListener listener)
        {
            this.footer.setMoreView(view);
            this.onLoadMoreListener = listener;
            hasMore = true;
            log("setMore");
        }

        public void setNoMore(View view)
        {
            this.footer.setNoMoreView(view);
            hasNoMore = true;
            log("setNoMore");
        }

        public void stopLoadMore()
        {
            log("stopLoadMore");
            footer.showNoMore();
            status = STATUS_NOMORE;
            isLoadingMore = false;
        }

        private class EventFooter : RecyclerArrayAdapter<T>.ItemView
        {
            private DefaultEventDelegate<T> _eventDelegate;

            private FrameLayout container;
            private View moreView;
            private View noMoreView;
            private View errorView;

            private int flag = Hide;
            public const int Hide = 0;
            public const int ShowMore = 1;
            public const int ShowError = 2;
            public const int ShowNoMore = 3;


            public EventFooter(DefaultEventDelegate<T> eventDelegate)
            {
                _eventDelegate = eventDelegate;
                container = new FrameLayout(_eventDelegate.adapter.getContext());
                container.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            }

            public View onCreateView(ViewGroup parent)
            {
                log("onCreateView");
                return container;
            }

            public void onBindView(View headerView)
            {
                log("onBindView");
                switch (flag)
                {
                    case ShowMore:
                        _eventDelegate.onMoreViewShowed();
                        break;
                    case ShowError:
                        _eventDelegate.onErrorViewShowed();
                        break;
                }
            }

            public void refreshStatus()
            {
                if (container != null)
                {
                    if (flag == Hide)
                    {
                        container.Visibility = ViewStates.Gone;
                        return;
                    }
                    if (container.Visibility != ViewStates.Visible) container.Visibility = ViewStates.Visible;
                    View view = null;
                    switch (flag)
                    {
                        case ShowMore: view = moreView; break;
                        case ShowError: view = errorView; break;
                        case ShowNoMore: view = noMoreView; break;
                    }
                    if (view == null)
                    {
                        hide();
                        return;
                    }
                    if (view.Parent == null) container.AddView(view);
                    for (int i = 0; i < container.ChildCount; i++)
                    {
                        if (container.GetChildAt(i) == view) view.Visibility = ViewStates.Visible;
                        else container.GetChildAt(i).Visibility = ViewStates.Gone;
                    }
                }
            }

            public void showError()
            {
                flag = ShowError;
                refreshStatus();
            }
            public void showMore()
            {
                flag = ShowMore;
                refreshStatus();
            }
            public void showNoMore()
            {
                flag = ShowNoMore;
                refreshStatus();
            }

            //初始化
            public void hide()
            {
                flag = Hide;
                refreshStatus();
            }

            public void setMoreView(View moreView)
            {
                this.moreView = moreView;
            }

            public void setNoMoreView(View noMoreView)
            {
                this.noMoreView = noMoreView;
            }

            public void setErrorView(View errorView)
            {
                this.errorView = errorView;
            }
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