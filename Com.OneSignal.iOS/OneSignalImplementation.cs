using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Com.OneSignal.Abstractions;
using OSNotificationOpenedResult = Com.OneSignal.Abstractions.OSNotificationOpenedResult;
using OSNotification = Com.OneSignal.Abstractions.OSNotification;
using OSNotificationAction = Com.OneSignal.Abstractions.OSNotificationAction;
using OSNotificationPayload = Com.OneSignal.Abstractions.OSNotificationPayload;
using OSInFocusDisplayOption = Com.OneSignal.Abstractions.OSInFocusDisplayOption;

namespace Com.OneSignal
{
   public class OneSignalImplementation : OneSignalShared, IOneSignal
   {
      public static Dictionary<string, object> NSDictToPureDict(Foundation.NSDictionary nsDict)
      {
         if (nsDict == null)
            return null;
         Foundation.NSError error;
         Foundation.NSData jsonData = Foundation.NSJsonSerialization.Serialize(nsDict, 0, out error);
         Foundation.NSString jsonNSString = Foundation.NSString.FromData(jsonData, Foundation.NSStringEncoding.UTF8);
         string jsonString = jsonNSString.ToString();
         return Json.Deserialize(jsonString) as Dictionary<string, object>;
      }

      private OSNotificationOpenedResult OSNotificationOpenedResultToNative(iOS.OSNotificationOpenedResult result)
      {
         var openresult = new OSNotificationOpenedResult();
         openresult.action = new OSNotificationAction();
         iOS.OSNotificationAction action = result.Action;
         openresult.action.actionID = action.ActionID;
         openresult.action.type = (OSNotificationAction.ActionType)(int)action.Type;

         openresult.notification = OSNotificationToNative(result.Notification);

         return openresult;
      }

      private OSNotification OSNotificationToNative(iOS.OSNotification notif)
      {
         var notification = new OSNotification();
         notification.displayType = (OSNotification.DisplayType)notif.DisplayType;
         notification.shown = notif.Shown;
         notification.silentNotification = notif.SilentNotification;

         notification.payload = new OSNotificationPayload();


         notification.payload.actionButtons = new List<Dictionary<string, object>>();
         if (notif.Payload.ActionButtons != null)
         {
            for (int i = 0; i < (int)notif.Payload.ActionButtons.Count; ++i)
            {
               Foundation.NSDictionary element = notif.Payload.ActionButtons.GetItem<Foundation.NSDictionary>((uint)i);
               notification.payload.actionButtons.Add(NSDictToPureDict(element));
            }
         }

         notification.payload.additionalData = new Dictionary<string, object>();
         if (notif.Payload.AdditionalData != null)
         {
            foreach (KeyValuePair<Foundation.NSObject, Foundation.NSObject> element in notif.Payload.AdditionalData)
            {
               notification.payload.additionalData.Add((Foundation.NSString)element.Key, element.Value);
            }
         }

         notification.payload.badge = (int)notif.Payload.Badge;
         notification.payload.body = notif.Payload.Body;
         notification.payload.contentAvailable = notif.Payload.ContentAvailable;
         notification.payload.launchURL = notif.Payload.LaunchURL;
         notification.payload.notificationID = notif.Payload.NotificationID;
         notification.payload.sound = notif.Payload.Sound;
         notification.payload.subtitle = notif.Payload.Subtitle;
         notification.payload.title = notif.Payload.Title;

         return notification;
      }

      // Init - Only required method you call to setup OneSignal to receive push notifications.
      public override void InitPlatform()
      {
         //extract settings
         bool autoPrompt = true, inAppLaunchURL = true;

         if (builder.iOSSettings != null)
         {
            if (builder.iOSSettings.ContainsKey(IOSSettings.kOSSettingsKeyAutoPrompt))
               autoPrompt = builder.iOSSettings[IOSSettings.kOSSettingsKeyAutoPrompt];
            if (builder.iOSSettings.ContainsKey(IOSSettings.kOSSettingsKeyInAppLaunchURL))
               inAppLaunchURL = builder.iOSSettings[IOSSettings.kOSSettingsKeyInAppLaunchURL];
         }
         Init(builder.mAppId, autoPrompt, inAppLaunchURL, builder.displayOption, logLevel, visualLogLevel);
      }

      public void Init(string appId, bool autoPrompt, bool inAppLaunchURLs, OSInFocusDisplayOption displayOption, LOG_LEVEL logLevel, LOG_LEVEL visualLevel)
      {
         var convertedLogLevel = (iOS.OneSLogLevel)((int)logLevel);
         var convertedVisualLevel = (iOS.OneSLogLevel)((int)visualLevel);

         iOS.OneSignal.SetLogLevel(convertedLogLevel, convertedVisualLevel);
         var dict = new Foundation.NSDictionary("kOSSettingsKeyInAppLaunchURL", new Foundation.NSNumber(inAppLaunchURLs),
                                                "kOSSettingsKeyAutoPrompt", new Foundation.NSNumber(autoPrompt),
                                                "kOSSettingsKeyInFocusDisplayOption", new Foundation.NSNumber((int)displayOption)
                                               );
         iOS.OneSignal.SetMSDKType("xam");
         iOS.OneSignal.InitWithLaunchOptions(new Foundation.NSDictionary(), appId, NotificationReceivedHandler, NotificationOpenedHandler, dict);

      }

      public override void RegisterForPushNotifications()
      {
         iOS.OneSignal.RegisterForPushNotifications();
      }

      public override void SendTag(string tagName, string tagValue)
      {
         iOS.OneSignal.SendTag(tagName, tagValue);
      }

      public override void SendTags(IDictionary<string, string> tags)
      {
         string jsonString = Json.Serialize(tags);
         iOS.OneSignal.SendTagsWithJsonString(jsonString);
      }

      public override void GetTags()
      {
         iOS.OneSignal.GetTags(GetTagsHandler);
      }

      public override void DeleteTag(string key)
      {
         iOS.OneSignal.DeleteTag(key);
      }

      public override void DeleteTags(IList<string> keys)
      {
         Foundation.NSObject[] objs = new Foundation.NSObject[keys.Count];
         for (int i = 0; i < keys.Count; i++)
         {
            objs[i] = (Foundation.NSString)keys[i];
         }
         iOS.OneSignal.DeleteTags(objs);
      }

      public override void IdsAvailable()
      {
         iOS.OneSignal.IdsAvailable(IdsAvailableHandler);
      }


      public override void SetSubscription(bool enable)
      {
         iOS.OneSignal.SetSubscription(enable);
      }

      public override void PostNotification(Dictionary<string, object> data)
      {
         string jsonString = Json.Serialize(data);
         iOS.OneSignal.PostNotificationWithJsonString(jsonString, PostNotificationSuccessHandler, PostNotificationFailureHandler);
      }

      public override void SetLogLevel(LOG_LEVEL logLevel, LOG_LEVEL visualLevel)
      {
         base.SetLogLevel(logLevel, visualLevel);

         var convertedLogLevel = (iOS.OneSLogLevel)((ulong)((int)logLevel));
         var convertedVisualLevel = (iOS.OneSLogLevel)((ulong)((int)visualLevel));
         iOS.OneSignal.SetLogLevel(convertedLogLevel, convertedVisualLevel);
      }

      public void IdsAvailableHandler(string playerID, string pushToken)
      {
         onIdsAvailable(playerID, pushToken);
      }

       public Task<IdsResult> IdsAvailableAsync()
       {
           var taskCompletionSource = new TaskCompletionSource<IdsResult>();

           Action<string, string> action = (id, push) =>
           {
               var result = new IdsResult()
               {
                   PlayerId = id,
                   PushToken = push
               };
               taskCompletionSource.SetResult(result);
           };

           iOS.OneSignal.IdsAvailable(new IdsAvailableCallback(action));

           return taskCompletionSource.Task;
       }

        public void NotificationOpenedHandler(iOS.OSNotificationOpenedResult result)
      {
         onPushNotificationOpened(OSNotificationOpenedResultToNative(result));
      }
      public void NotificationReceivedHandler(iOS.OSNotification notification)
      {
         onPushNotificationReceived(OSNotificationToNative(notification));
      }

      public void GetTagsHandler(Foundation.NSDictionary result)
      {
         Dictionary<string, object> dict = NSDictToPureDict(result);
         onTagsReceived(dict);
      }

      public void PostNotificationSuccessHandler(Foundation.NSDictionary result)
      {
         Dictionary<string, object> dict = NSDictToPureDict(result);
         onPostNotificationSuccess(dict);
      }

      public void PostNotificationFailureHandler(Foundation.NSError error)
      {
         if (error.UserInfo != null && error.UserInfo["returned"] != null)
         {
            Dictionary<string, object> dict = NSDictToPureDict(error.UserInfo);
            onPostNotificationFailed(dict);
         }
         else
            onPostNotificationFailed(new Dictionary<string, object> { { "error", "HTTP no response error" } });
      }

      public override void SyncHashedEmail(string email)
      {
         iOS.OneSignal.SyncHashedEmail(email);
      }

      public override void PromptLocation()
      {
         iOS.OneSignal.PromptLocation();
      }
   }
}
