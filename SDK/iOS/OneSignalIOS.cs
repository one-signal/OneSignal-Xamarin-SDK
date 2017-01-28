/**
 * Modified MIT License
 * 
 * Copyright 2016 OneSignal
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * 1. The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * 2. All copies of substantial portions of the Software may only be used in connection
 * with services provided by OneSignal.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System.Collections.Generic;
using OneSignalPush.MiniJSON;

namespace Com.OneSignal
{
   public class OneSignalIOS : OneSignalPlatform {

      public static Dictionary<string, object> NSDictToPureDict(Foundation.NSDictionary nsDict) {
         if(nsDict == null)
            return null;
         Foundation.NSError error;
         Foundation.NSData jsonData = Foundation.NSJsonSerialization.Serialize (nsDict, 0, out error);
         Foundation.NSString jsonNSString = Foundation.NSString.FromData (jsonData, Foundation.NSStringEncoding.UTF8);
         string jsonString = jsonNSString.ToString ();
         return Json.Deserialize (jsonString) as Dictionary<string, object>;
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

      public OneSignalIOS(string appId, bool autoPrompt, bool inAppLaunchURLs, OneSignal.OSInFocusDisplayOption displayOption, OneSignal.LOG_LEVEL logLevel, OneSignal.LOG_LEVEL visualLevel)
      {
         var convertedLogLevel = (iOS.OneSLogLevel)((int)logLevel);
         var convertedVisualLevel = (iOS.OneSLogLevel)((int)visualLevel);

         iOS.OneSignal.SetLogLevel(convertedLogLevel, convertedVisualLevel);
         var dict = new Foundation.NSDictionary("kOSSettingsKeyInAppLaunchURL", new Foundation.NSNumber(inAppLaunchURLs), 
                                                "kOSSettingsKeyAutoPrompt", new Foundation.NSNumber(autoPrompt),
                                                "kOSSettingsKeyInFocusDisplayOption", new Foundation.NSNumber((int)displayOption)
                                               );
         iOS.OneSignal.InitWithLaunchOptions(new Foundation.NSDictionary(), appId, NotificationReceivedHandler, NotificationOpenedHandler, dict);

      }

      public void RegisterForPushNotifications() {
         iOS.OneSignal.RegisterForPushNotifications();
      }

      public void SendTag(string tagName, string tagValue) {
         iOS.OneSignal.SendTag (tagName, tagValue);
      }

      public void SendTags(IDictionary<string, string> tags) {
         string jsonString = Json.Serialize (tags);
         iOS.OneSignal.SendTagsWithJsonString (jsonString);
      }

      public void GetTags() {
         iOS.OneSignal.GetTags (GetTagsHandler);
      }

      public void DeleteTag(string key)
      {
         iOS.OneSignal.DeleteTag (key);
      }

      public void DeleteTags (IList<string> keys)
      {
         Foundation.NSObject[] objs = new Foundation.NSObject[keys.Count];
         for (int i = 0; i < keys.Count; i++)
         {
            objs [i] = (Foundation.NSString)keys[i];
         }
         iOS.OneSignal.DeleteTags (objs);
      }

      public void IdsAvailable ()
      {
         iOS.OneSignal.IdsAvailable (IdsAvailableHandler);
      }

      public void SetSubscription (bool enable)
      {
         iOS.OneSignal.SetSubscription (enable);
      }

      public void PostNotification (Dictionary<string, object> data)
      {
         string jsonString = Json.Serialize (data);
         iOS.OneSignal.PostNotificationWithJsonString (jsonString, PostNotificationSuccessHandler, PostNotificationFailureHandler);
      }
	  
      public void PostNotification (string jsonString)
      {
         iOS.OneSignal.PostNotificationWithJsonString (jsonString, PostNotificationSuccessHandler, PostNotificationFailureHandler);
      }

      public void SetLogLevel (OneSignal.LOG_LEVEL logLevel, OneSignal.LOG_LEVEL visualLevel)
      {
         var convertedLogLevel = (iOS.OneSLogLevel)((ulong)((int)logLevel));
         var convertedVisualLevel = (iOS.OneSLogLevel)((ulong)((int)visualLevel));
         iOS.OneSignal.SetLogLevel (convertedLogLevel, convertedVisualLevel);
      }

      public void IdsAvailableHandler (string playerID, string pushToken)
      {
         OneSignal.onIdsAvailable(playerID, pushToken);
      }
                                                           
      public void NotificationOpenedHandler (iOS.OSNotificationOpenedResult result)
      {
         OneSignal.onPushNotificationOpened(OSNotificationOpenedResultToNative(result));
      }
      public void NotificationReceivedHandler(iOS.OSNotification notification)
      {
         OneSignal.onPushNotificationReceived(OSNotificationToNative(notification));
      }

      public void GetTagsHandler (Foundation.NSDictionary result)
      {
         Dictionary<string, object> dict = NSDictToPureDict(result);
         OneSignal.onTagsReceived(dict);
      }

      public void PostNotificationSuccessHandler (Foundation.NSDictionary result)
      {
         Dictionary<string, object> dict = NSDictToPureDict (result);
         OneSignal.onPostNotificationSuccess (dict);
      }

      public void PostNotificationFailureHandler (Foundation.NSError error)
      {
         if (error.UserInfo != null && error.UserInfo ["returned"] != null)
         {
            Dictionary<string, object> dict = NSDictToPureDict (error.UserInfo);
            OneSignal.onPostNotificationFailed (dict);
         }
         else
            OneSignal.onPostNotificationFailed (new Dictionary<string, object> { {"error", "HTTP no response error"} });
      }

      public void SyncHashedEmail(string email)
      {
            iOS.OneSignal.SyncHashedEmail(email);
      }

      public void PromptLocation()
      {
         iOS.OneSignal.PromptLocation();
      }
   }
}