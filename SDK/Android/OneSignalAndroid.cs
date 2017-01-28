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
using Org.Json;
using OneSignalPush.MiniJSON;
using Android.App;

namespace Com.OneSignal
{
	public class OneSignalAndroid : OneSignalPlatform {
      
      static OSNotificationOpenedResult OSNotificationOpenedResultToNative(Android.OSNotificationOpenResult result)
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

      static OSNotification OSNotificationToNative(Android.OSNotification notif)
      {
         var notification = new OSNotification();
         notification.shown = notif.Shown;
         notification.androidNotificationId = notif.AndroidNotificationId;
         notif.GroupedNotifications = notif.GroupedNotifications;
         notif.IsAppInFocus = notif.IsAppInFocus;
         
         notification.payload = new OSNotificationPayload();


         notification.payload.actionButtons = new List<Dictionary<string, object>>();
         if (notif.Payload.ActionButtons != null)
         {
            foreach (Android.OSNotificationPayload.ActionButton button in notif.Payload.ActionButtons)
            {
               var d = new Dictionary<string, object>();
               d.Add(button.Id, button.Text);
               notification.payload.actionButtons.Add(d);
            }
         }

         notification.payload.additionalData = new Dictionary<string, object>();
         if (notif.Payload.AdditionalData != null)
         {
            var iterator = notif.Payload.AdditionalData.Keys();
            while (iterator.HasNext)
            {
               var key = (string)iterator.Next();
               notification.payload.additionalData.Add(key, notif.Payload.AdditionalData.Get(key));
            }
         }
         
         notification.payload.body = notif.Payload.Body;
         notification.payload.launchURL = notif.Payload.LaunchURL;
         notification.payload.notificationID = notif.Payload.NotificationID;
         notification.payload.sound = notif.Payload.Sound;
         notification.payload.title = notif.Payload.Title;
         notification.payload.bigPicture = notif.Payload.BigPicture; 
         notification.payload.fromProjectNumber = notif.Payload.FromProjectNumber; 
         notification.payload.groupMessage = notif.Payload.GroupKey; 
         notification.payload.groupMessage = notif.Payload.GroupMessage; 
         notification.payload.largeIcon = notif.Payload.LargeIcon; 
         notification.payload.ledColor = notif.Payload.LedColor; 
         notification.payload.lockScreenVisibility = notif.Payload.LockScreenVisibility; 
         notification.payload.smallIcon = notif.Payload.SmallIcon; 
         notification.payload.smallIconAccentColor = notif.Payload.SmallIconAccentColor;

         return notification;
      }

      public OneSignalAndroid(string appid, string googleProjectNumber, OneSignal.OSInFocusDisplayOption displayOption, OneSignal.LOG_LEVEL logLevel, OneSignal.LOG_LEVEL visualLevel)
      {  
			SetLogLevel (logLevel, visualLevel);

         //Convert OneSignal.OSInFocusDisplayOptions to Android.OneSignal.OSInFocusDisplayOption
         Android.OneSignal.OSInFocusDisplayOption option = Android.OneSignal.OSInFocusDisplayOption.InAppAlert;
         switch (displayOption)
         {
            case OneSignal.OSInFocusDisplayOption.None: option = Android.OneSignal.OSInFocusDisplayOption.None; break;
            case OneSignal.OSInFocusDisplayOption.Notification: option = Android.OneSignal.OSInFocusDisplayOption.Notification; break;
            case OneSignal.OSInFocusDisplayOption.InAppAlert: option = Android.OneSignal.OSInFocusDisplayOption.InAppAlert; break;
         }

         Android.OneSignal.Init(Application.Context, googleProjectNumber, appid, new NotificationOpenedHandler(), new NotificationReceivedHandler());
         Android.OneSignal.SetInFocusDisplaying(option);
		}
			
		public void SendTag (string tagName, string tagValue)
		{
			Android.OneSignal.SendTag (tagName, tagValue);
		}

		public void SendTags (IDictionary<string, string> tags)
		{
			Android.OneSignal.SendTags (Json.Serialize (tags));
		}

		public void GetTags ()
		{
		   Android.OneSignal.GetTags (new GetTagsHandler ());
		}

		public void DeleteTag (string key)
		{
			Android.OneSignal.DeleteTag (key);
		}

		public void DeleteTags (IList<string> keys)
		{
			Android.OneSignal.DeleteTags (Json.Serialize (keys));
		}

		public void IdsAvailable ()
		{
			Android.OneSignal.IdsAvailable (new IdsAvailableHandler ());
		}

		public void RegisterForPushNotifications () { } // Doesn't apply to Android as the Native SDK always registers with GCM.

		public void EnableVibrate (bool enable)
		{
			Android.OneSignal.EnableVibrate (enable);
		}

      public void EnableSound(bool enable)
      {
         Android.OneSignal.EnableSound(enable);
      }

		public void SetInFocusDisplaying(OneSignal.OSInFocusDisplayOption display)
		{
      		Android.OneSignal.SetInFocusDisplaying((int)display);
   	}

		public void SetSubscription (bool enable)
		{
			Android.OneSignal.SetSubscription (enable);
		}

		public void PostNotification (Dictionary<string, object> data)
		{
			Android.OneSignal.PostNotification (Json.Serialize (data), new PostNotificationResponseHandler ());
		}

		public void PostNotification (string jsonString)
		{
			Android.OneSignal.PostNotification (jsonString, new PostNotificationResponseHandler ());
		}

		public void SyncHashedEmail(string email)
		{
   		Android.OneSignal.SyncHashedEmail(email);
  		}

		public void PromptLocation()
		{
    		Android.OneSignal.PromptLocation();
  		}
  
		public void ClearOneSignalNotifications()
		{
			Android.OneSignal.ClearOneSignalNotifications();
		}

		public void SetLogLevel (OneSignal.LOG_LEVEL logLevel, OneSignal.LOG_LEVEL visualLevel)
		{
			Android.OneSignal.SetLogLevel ((int)logLevel, (int)visualLevel);
		}
			
		private class IdsAvailableHandler : Java.Lang.Object, Android.OneSignal.IIdsAvailableHandler
		{
			public void IdsAvailable (string p0, string p1)
			{
            OneSignal.onIdsAvailable(p0, p1);
			}
		}

      private class NotificationReceivedHandler : Java.Lang.Object, Android.OneSignal.INotificationReceivedHandler
		{
         public void NotificationReceived(Android.OSNotification notification)
			{
            OneSignal.onPushNotificationReceived(OSNotificationToNative(notification));
			}
		}
      
      private class NotificationOpenedHandler : Java.Lang.Object, Android.OneSignal.INotificationOpenedHandler
      {
         public void NotificationOpened(Android.OSNotificationOpenResult result)
         {
            OneSignal.onPushNotificationOpened(OSNotificationOpenedResultToNative(result));
         }
      }

		private class GetTagsHandler : Java.Lang.Object, Android.OneSignal.IGetTagsHandler
		{
			public void TagsAvailable (JSONObject jsonObject)
			{
				Dictionary<string, object> dict = null;
				if (jsonObject != null)
					dict = Json.Deserialize (jsonObject.ToString ()) as Dictionary<string, object>;
            OneSignal.onTagsReceived (dict);
			}
		}

		private class PostNotificationResponseHandler : Java.Lang.Object, Android.OneSignal.IPostNotificationResponseHandler
		{
			public void OnSuccess (JSONObject jsonObject)
			{
				Dictionary<string, object> dict = null;
				if (jsonObject != null)
					dict = Json.Deserialize (jsonObject.ToString ()) as Dictionary<string, object>;
            OneSignal.onPostNotificationSuccess (dict);
			}

			public void OnFailure (JSONObject jsonObject)
			{
				Dictionary<string, object> dict = null;
				if (jsonObject != null)
					dict = Json.Deserialize (jsonObject.ToString ()) as Dictionary<string, object>;
            OneSignal.onPostNotificationFailed (dict);
			}
		}
	}
}