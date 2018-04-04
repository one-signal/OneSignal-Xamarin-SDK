using System.Collections.Generic;
using Android.App;
using System;
using Com.OneSignal.Abstractions;

namespace Com.OneSignal
{
   public class OneSignalImplementation : OneSignalShared, IOneSignal
   {
      public void Init(string appid, OSInFocusDisplayOption displayOption, LOG_LEVEL logLevel, LOG_LEVEL visualLevel)
      {
         SetLogLevel(logLevel, visualLevel);

         //Convert OneSignal.OSInFocusDisplayOptions to Android.OneSignal.OSInFocusDisplayOption
         Android.OneSignal.OSInFocusDisplayOption option = Android.OneSignal.OSInFocusDisplayOption.InAppAlert;
         switch (displayOption)
         {
            case OSInFocusDisplayOption.None: option = Android.OneSignal.OSInFocusDisplayOption.None; break;
            case OSInFocusDisplayOption.Notification: option = Android.OneSignal.OSInFocusDisplayOption.Notification; break;
            case OSInFocusDisplayOption.InAppAlert: option = Android.OneSignal.OSInFocusDisplayOption.InAppAlert; break;
         }

         Android.OneSignal.SdkType = "xam";
         Android.OneSignal.Init(Application.Context, "", appid, new NotificationOpenedHandler(), new NotificationReceivedHandler());
         Android.OneSignal.SetInFocusDisplaying(option);
      }

      // Init - Only required method you call to setup OneSignal to receive push notifications.
      public override void InitPlatform()
      {
         Init(builder.mAppId, builder.displayOption, logLevel, visualLogLevel);
      }

      public override void SendTag(string tagName, string tagValue)
      {
         Android.OneSignal.SendTag(tagName, tagValue);
      }

      public override void SendTags(IDictionary<string, string> tags)
      {
         Android.OneSignal.SendTags(Json.Serialize(tags));
      }

      public override void GetTags(TagsReceived tagsReceived)
      {
         if (tagsReceived == null)
            throw new ArgumentNullException(nameof(tagsReceived));
         Android.OneSignal.GetTags(new TagsHandler(tagsReceived));
      }

      public override void DeleteTag(string key)
      {
         Android.OneSignal.DeleteTag(key);
      }

      public override void DeleteTags(IList<string> keys)
      {
         Android.OneSignal.DeleteTags(Json.Serialize(keys));
      }

      public override void IdsAvailable(IdsAvailableCallback idsAvailable)
      {
         if (idsAvailable == null)
            throw new ArgumentNullException(nameof(idsAvailable));
         Android.OneSignal.IdsAvailable(new IdsAvailableHandler(idsAvailable));
      }

      public override void RegisterForPushNotifications() { } // Doesn't apply to Android as the Native SDK always registers with GCM.

      public void EnableVibrate(bool enable)
      {
         Android.OneSignal.EnableVibrate(enable);
      }

      public void EnableSound(bool enable)
      {
         Android.OneSignal.EnableSound(enable);
      }

      public void SetInFocusDisplaying(OSInFocusDisplayOption display)
      {
         Android.OneSignal.SetInFocusDisplaying((int)display);
      }

      public override void SetSubscription(bool enable)
      {
         Android.OneSignal.SetSubscription(enable);
      }

      public override void PostNotification(Dictionary<string, object> data, OnPostNotificationSuccess success, OnPostNotificationFailure failure)
      {
         Android.OneSignal.PostNotification(Json.Serialize(data), new PostNotificationResponseHandler(success, failure));
      }

      [Obsolete("SyncHashedEmail has been deprecated. Please use SetEmail() instead.")]
      public override void SyncHashedEmail(string email)
      {
         Android.OneSignal.SyncHashedEmail(email);
      }

      public override void PromptLocation()
      {
         Android.OneSignal.PromptLocation();
      }

		public override void ClearAndroidOneSignalNotifications()
		{
			Android.OneSignal.ClearOneSignalNotifications();
		}

		public override void SetLogLevel(LOG_LEVEL logLevel, LOG_LEVEL visualLevel)
      {
         base.SetLogLevel(logLevel, visualLevel);

         Android.OneSignal.SetLogLevel((int)logLevel, (int)visualLevel);
      }

      public override void SetEmail(string email, string emailAuthCode, OnSetEmailSuccess success, OnSetEmailFailure failure)
      {
         Android.OneSignal.SetEmail(email, emailAuthCode, new EmailUpdateHandler(success, failure));
      }

      public override void SetEmail(string email, OnSetEmailSuccess success, OnSetEmailFailure failure)
      {
         Android.OneSignal.SetEmail(email, null, new EmailUpdateHandler(success, failure));
      }

      public override void LogoutEmail(OnSetEmailSuccess success, OnSetEmailFailure failure)
      {
         Android.OneSignal.LogoutEmail(new EmailUpdateHandler(success, failure));
      }
   }
}