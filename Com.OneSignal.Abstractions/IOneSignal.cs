using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.OneSignal.Abstractions
{
   public interface IOneSignal
   {
      XamarinBuilder StartInit(string appID);

      void SetLogLevel(LOG_LEVEL logLevel, LOG_LEVEL visualLevel);
      void RegisterForPushNotifications();
      void SendTag(string tagName, string tagValue);
      void SendTags(IDictionary<string, string> tags);
      void GetTags(TagsReceived tagsReceivedDelegate);
      void DeleteTag(string key);
      void DeleteTags(IList<string> keys);
      [Obsolete]
      void IdsAvailable(IdsAvailableCallback idsAvailableCallback);
      Task<IdsResult> IdsAvailableAsync();
      void SetSubscription(bool enable);
      void PostNotification(Dictionary<string, object> data);
      void SyncHashedEmail(string email);
      void PromptLocation();
   }
}
