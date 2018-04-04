using System;
using System.Collections.Generic;
using Com.OneSignal.Abstractions;

namespace Com.OneSignal
{
   public class NotificationOpenedHandler : Java.Lang.Object, Android.OneSignal.INotificationOpenedHandler
   {
      public void NotificationOpened(Android.OSNotificationOpenResult result)
      {
         (OneSignal.Current as OneSignalImplementation).onPushNotificationOpened(result.ToAbstract());
      }
   }
}