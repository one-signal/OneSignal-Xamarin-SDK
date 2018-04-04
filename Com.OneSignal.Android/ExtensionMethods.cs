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
using Com.OneSignal.Abstractions;

namespace Com.OneSignal
{
   public static class ExtensionMethods
   {
      public static OSNotificationOpenedResult ToAbstract(this Android.OSNotificationOpenResult @this)
      {

         OSNotificationAction.ActionType actionType = OSNotificationAction.ActionType.Opened;
         if (@this.Action.Type == Android.OSNotificationAction.ActionType.Opened)
            actionType = OSNotificationAction.ActionType.Opened;
         else
            actionType = OSNotificationAction.ActionType.ActionTaken;

         var openresult = new OSNotificationOpenedResult();
         openresult.action = new OSNotificationAction();
         Android.OSNotificationAction action = @this.Action;
         openresult.action.actionID = action.ActionID;
         openresult.action.type = actionType;

         openresult.notification = @this.Notification.ToAbstract();

         return openresult;
      }

      public static OSNotification ToAbstract(this Android.OSNotification @this)
      {
         var notification = new OSNotification();
         notification.shown = @this.Shown;
         notification.androidNotificationId = @this.AndroidNotificationId;
         @this.GroupedNotifications = @this.GroupedNotifications;
         @this.IsAppInFocus = @this.IsAppInFocus;

         notification.payload = @this.Payload.ToAbstract();

         return notification;
      }

      public static OSNotificationPayload ToAbstract(this Android.OSNotificationPayload @this)
      {
         var payload = new OSNotificationPayload();


         payload.actionButtons = new List<Dictionary<string, object>>();
         if (@this.ActionButtons != null)
         {
            foreach (Android.OSNotificationPayload.ActionButton button in @this.ActionButtons)
            {
               var d = new Dictionary<string, object>();
               d.Add(button.Id, button.Text);
               payload.actionButtons.Add(d);
            }
         }

         payload.additionalData = new Dictionary<string, object>();
         if (@this.AdditionalData != null)
         {
            var iterator = @this.AdditionalData.Keys();
            while (iterator.HasNext)
            {
               var key = (string)iterator.Next();
               payload.additionalData.Add(key, @this.AdditionalData.Get(key));
            }
         }

         payload.body = @this.Body;
         payload.launchURL = @this.LaunchURL;
         payload.notificationID = @this.NotificationID;
         payload.sound = @this.Sound;
         payload.title = @this.Title;
         payload.bigPicture = @this.BigPicture;
         payload.fromProjectNumber = @this.FromProjectNumber;
         payload.groupMessage = @this.GroupKey;
         payload.groupMessage = @this.GroupMessage;
         payload.largeIcon = @this.LargeIcon;
         payload.ledColor = @this.LedColor;
         payload.lockScreenVisibility = @this.LockScreenVisibility;
         payload.smallIcon = @this.SmallIcon;
         payload.smallIconAccentColor = @this.SmallIconAccentColor;

         return payload;
      }
   }
}