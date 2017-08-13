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
//using Org.Greenrobot.Eventbus;
using Xamarin.BookReader.Models.Support;
using DSoft.Messaging;

namespace Xamarin.BookReader.Managers
{
    public class EventManager
    {
        public static void refreshCollectionList()
        {
            MessageBus.Default.Post(new RefreshCollectionListEvent());
        }

        public static void refreshCollectionIcon()
        {
            MessageBus.Default.Post(new RefreshCollectionIconEvent());
        }

        public static void refreshSubCategory(String minor, String type)
        {
            MessageBus.Default.Post(new SubEvent(minor, type));
        }
    }
}