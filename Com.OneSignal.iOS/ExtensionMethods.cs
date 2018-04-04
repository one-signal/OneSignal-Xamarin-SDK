using System;
using System.Collections.Generic;
using System.Text;

using Com.OneSignal.Abstractions;

namespace Com.OneSignal
{
   public static class ExtensionMethods
   {
      public static OSNotificationOpenedResult ToAbstract(this iOS.OSNotificationOpenedResult @this)
      {
         var openresult = new OSNotificationOpenedResult();
         openresult.action = new OSNotificationAction();
         iOS.OSNotificationAction action = @this.Action;
         openresult.action.actionID = action.ActionID;
         openresult.action.type = (OSNotificationAction.ActionType)(int)action.Type;

         openresult.notification = @this.Notification.ToAbstract();

         return openresult;
      }

      public static OSNotification ToAbstract(this iOS.OSNotification @this)
      {
         var notification = new OSNotification();
         notification.displayType = (OSNotification.DisplayType)@this.DisplayType;
         notification.shown = @this.Shown;
         notification.silentNotification = @this.SilentNotification;
         notification.isAppInFocus = @this.IsAppInFocus;
         notification.payload = @this.Payload.ToAbstract();

         return notification;
      }

      public static OSNotificationPayload ToAbstract(this iOS.OSNotificationPayload @this)
      {
         var payload = new OSNotificationPayload();


         payload.actionButtons = new List<Dictionary<string, object>>();
         if (@this.ActionButtons != null)
         {
            for (int i = 0; i < (int)@this.ActionButtons.Count; ++i)
            {
               Foundation.NSDictionary element = @this.ActionButtons.GetItem<Foundation.NSDictionary>((uint)i);
               payload.actionButtons.Add(OneSignalImplementation.NSDictToPureDict(element));
            }
         }

         payload.additionalData = new Dictionary<string, object>();
         if (@this.AdditionalData != null)
         {
            foreach (KeyValuePair<Foundation.NSObject, Foundation.NSObject> element in @this.AdditionalData)
            {
               payload.additionalData.Add((Foundation.NSString)element.Key, element.Value);
            }
         }

         payload.badge = (int)@this.Badge;
         payload.body = @this.Body;
         payload.contentAvailable = @this.ContentAvailable;
         payload.launchURL = @this.LaunchURL;
         payload.notificationID = @this.NotificationID;
         payload.sound = @this.Sound;
         payload.subtitle = @this.Subtitle;
         payload.title = @this.Title;

         return payload;
      }
   }
}
