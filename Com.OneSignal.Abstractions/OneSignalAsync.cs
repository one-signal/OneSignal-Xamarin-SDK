using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Com.OneSignal.Abstractions
{
   public sealed class PlayerIds
   {
      public string PlayerId { get; }
      public string PushToken { get; }

      public PlayerIds(string playerId, string pushToken)
      {
         PlayerId = playerId;
         PushToken = pushToken;
      }
   }

   public sealed class OneSignalResponse
   {
      public bool Success { get; }
      public Dictionary<string, object> Response { get; }

      public OneSignalResponse(bool success, Dictionary<string, object> response)
      {
         Success = success;
         Response = response;
      }
   }

   public static class OneSignalExtensions
   {
      public static Task<Dictionary<string, object>> GetTagsAsync(this IOneSignal @this)
      {
         var tcs = new TaskCompletionSource<Dictionary<string, object>>();
         @this.GetTags(tags => tcs.SetResult(tags));
         return tcs.Task;
      }

      public static Task<PlayerIds> IdsAvailableAsync(this IOneSignal @this)
      {
         var tcs = new TaskCompletionSource<PlayerIds>();
         @this.IdsAvailable((playerId, pushToken) => tcs.SetResult(new PlayerIds(playerId, pushToken)));
         return tcs.Task;
      }

      public static Task<OneSignalResponse> PostNotificationAsync(this IOneSignal @this, Dictionary<string, object> data)
      {
         var tcs = new TaskCompletionSource<OneSignalResponse>();
         @this.PostNotification(data,
             response => tcs.SetResult(new OneSignalResponse(true, response)),
             response => tcs.SetResult(new OneSignalResponse(false, response)));
         return tcs.Task;
      }

      public static Task<OneSignalResponse> SetEmailAsync(this IOneSignal @this, string email, string emailAuthToken)
      {
         var tcs = new TaskCompletionSource<OneSignalResponse>();
         @this.SetEmail(email, emailAuthToken,
             () => tcs.SetResult(new OneSignalResponse(true, null)),
             response => tcs.SetResult(new OneSignalResponse(false, response)));
         return tcs.Task;
      }

      public static Task<OneSignalResponse> SetEmailAsync(this IOneSignal @this, string email)
      {
         var tcs = new TaskCompletionSource<OneSignalResponse>();
         @this.SetEmail(email,
             () => tcs.SetResult(new OneSignalResponse(true, null)),
             response => tcs.SetResult(new OneSignalResponse(false, response)));
         return tcs.Task;
      }

      public static Task<OneSignalResponse> LogoutEmailAsync(this IOneSignal @this)
      {
         var tcs = new TaskCompletionSource<OneSignalResponse>();
         @this.LogoutEmail(
             () => tcs.SetResult(new OneSignalResponse(true, null)),
             response => tcs.SetResult(new OneSignalResponse(false, response)));
         return tcs.Task;
      }
   }
}