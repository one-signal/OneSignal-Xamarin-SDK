using System;
using System.Collections.Generic;
using Com.OneSignal.Abstractions;

namespace Com.OneSignal
{
    public class NotificationOpenedHandler : Java.Lang.Object, Android.OneSignal.INotificationOpenedHandler
    {
        public void NotificationOpened(Android.OSNotificationOpenResult result)
        {
            (OneSignal.Current as OneSignalImplementation).OnPushNotificationOpened(OSNotificationOpenedResultToNative(result));
        }

        public static OSNotificationOpenedResult OSNotificationOpenedResultToNative(Android.OSNotificationOpenResult result)
        {

            OSNotificationAction.ActionType actionType = OSNotificationAction.ActionType.Opened;
            if (result.Action.Type == Android.OSNotificationAction.ActionType.Opened)
                actionType = OSNotificationAction.ActionType.Opened;
            else
                actionType = OSNotificationAction.ActionType.ActionTaken;

            var openresult = new OSNotificationOpenedResult();
            openresult.action = new OSNotificationAction();
            Android.OSNotificationAction action = result.Action;
            openresult.action.actionID = action.ActionID;
            openresult.action.type = actionType;

            openresult.notification = OSNotificationToNative(result.Notification);

            return openresult;
        }

        public static OSNotification OSNotificationToNative(Android.OSNotification notif)
        {
            var notification = new OSNotification();

            notification.groupedNotifications = new List<OSNotificationPayload>();
            if (notif.GroupedNotifications != null)
            {
               foreach (Android.OSNotificationPayload payload in notif.GroupedNotifications)
               {
                  OSNotificationPayload nativePayload = OSNotificationPayloadToNative(payload);
                  notification.groupedNotifications.Add(nativePayload);
               }
            }

            notification.shown = notif.Shown;
            notification.androidNotificationId = notif.AndroidNotificationId;
            notification.isAppInFocus = notif.IsAppInFocus;
            notification.payload = OSNotificationPayloadToNative(notif.Payload);

            return notification;
        }

        public static OSNotificationPayload OSNotificationPayloadToNative(Android.OSNotificationPayload payload)
        {
            OSNotificationPayload nativePayload = new OSNotificationPayload();

            nativePayload.actionButtons = new List<Dictionary<string, object>>();
            if (payload.ActionButtons != null)
            {
                foreach (Android.OSNotificationPayload.ActionButton button in payload.ActionButtons)
                {
                    var d = new Dictionary<string, object>();
                    d.Add(button.Id, button.Text);
                    nativePayload.actionButtons.Add(d);
                }
            }

            nativePayload.additionalData = new Dictionary<string, object>();
            if (payload.AdditionalData != null)
            {
                var iterator = payload.AdditionalData.Keys();
                while (iterator.HasNext)
                {
                    var key = (string)iterator.Next();
                    nativePayload.additionalData.Add(key, payload.AdditionalData.Get(key));
                }
            }

            nativePayload.body = payload.Body;
            nativePayload.launchURL = payload.LaunchURL;
            nativePayload.notificationID = payload.NotificationID;
            nativePayload.sound = payload.Sound;
            nativePayload.title = payload.Title;
            nativePayload.bigPicture = payload.BigPicture;
            nativePayload.fromProjectNumber = payload.FromProjectNumber;
            nativePayload.groupMessage = payload.GroupKey;
            nativePayload.groupMessage = payload.GroupMessage;
            nativePayload.largeIcon = payload.LargeIcon;
            nativePayload.ledColor = payload.LedColor;
            nativePayload.lockScreenVisibility = payload.LockScreenVisibility;
            nativePayload.smallIcon = payload.SmallIcon;
            nativePayload.smallIconAccentColor = payload.SmallIconAccentColor;

            return nativePayload;
        }
    }
}