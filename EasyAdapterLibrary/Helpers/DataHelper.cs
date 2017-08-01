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

namespace EasyAdapterLibrary.Helpers
{
    public interface DataHelper<T>
    {

        void addAll(List<T> list);

        void addAll(int position, List<T> list);

        void add(T data);

        void add(int position, T data);

        void clear();

        bool contains(T data);

        T getData(int index);

        void modify(T oldData, T newData);

        void modify(int index, T newData);

        bool remove(T data);

        void remove(int index);
    }
}