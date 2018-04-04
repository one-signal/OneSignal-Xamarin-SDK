using System;
using System.Collections.Generic;

namespace Com.OneSignal.Abstractions
{
   public abstract class OneSignalShared : IOneSignal
   {
      public XamarinBuilder StartInit(string appId)
      {
         if (builder == null)
            builder = new XamarinBuilder(appId, this);

         return builder;
      }

      public abstract void RegisterForPushNotifications();
      public abstract void SendTag(string tagName, string tagValue);
      public abstract void SendTags(IDictionary<string, string> tags);


      public abstract void GetTags(TagsReceived inTagsReceivedDelegate);

      public abstract void DeleteTag(string key);
      public abstract void DeleteTags(IList<string> keys);
      public abstract void SetSubscription(bool enable);
      public abstract void PromptLocation();

      public abstract void ClearAndroidOneSignalNotifications();

      [Obsolete("SyncHashedEmail has been deprecated. Please use SetEmail() instead.")]
      public abstract void SyncHashedEmail(string email);



      // logging
      public LOG_LEVEL logLevel = LOG_LEVEL.INFO, visualLogLevel = LOG_LEVEL.NONE;

      public XamarinBuilder builder;

      // Init - Only required method you call to setup OneSignal to receive push notifications.
      public abstract void InitPlatform();

      public abstract void IdsAvailable(IdsAvailableCallback inIdsAvailableDelegate);

      public virtual void SetLogLevel(LOG_LEVEL ll, LOG_LEVEL vll)
      {
         logLevel = ll;
         visualLogLevel = vll;
      }

      public abstract void PostNotification(Dictionary<string, object> data, OnPostNotificationSuccess inOnPostNotificationSuccess, OnPostNotificationFailure inOnPostNotificationFailure);

      public abstract void SetEmail(string email, string emailAuthCode, OnSetEmailSuccess inSetEmailSuccess, OnSetEmailFailure inSetEmailFailure);


      public abstract void SetEmail(string email, OnSetEmailSuccess inSetEmailSuccess, OnSetEmailFailure inSetEmailFailure);

      public abstract void LogoutEmail(OnSetEmailSuccess inSetEmailSuccess, OnSetEmailFailure inSetEmailFailure);

      // Called from the native SDK - Called when a push notification received.
      public void onPushNotificationReceived(OSNotification notification)
      {
         if (builder._notificationReceivedDelegate != null)
         {
            builder._notificationReceivedDelegate(notification);
         }
      }

      // Called from the native SDK - Called when a push notification is opened by the user
      public void onPushNotificationOpened(OSNotificationOpenedResult result)
      {
         if (builder._notificationOpenedDelegate != null)
         {
            builder._notificationOpenedDelegate(result);
         }
      }

	  public void PostNotification(Dictionary<string, object> data) => PostNotification(data, null, null);
		 
	  public void SetEmail(string email, string emailAuthToken) => SetEmail(email, emailAuthToken, null, null);

	  public void SetEmail(string email) => SetEmail(email, null, null);

	  public void LogoutEmail() => LogoutEmail(null, null);
	}
}
