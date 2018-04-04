using System;
using System.Collections.Generic;
using Com.OneSignal.Abstractions;
using Org.Json;

namespace Com.OneSignal
{
    public class PostNotificationResponseHandler : Java.Lang.Object, Android.OneSignal.IPostNotificationResponseHandler
    {
		readonly OnPostNotificationSuccess _success;
		readonly OnPostNotificationFailure _failure;

		public PostNotificationResponseHandler(OnPostNotificationSuccess success, OnPostNotificationFailure failure)
		{
			_success = success;
			_failure = failure;
		}

        public void OnSuccess(JSONObject jsonObject)
        {
			if (_success == null)
				return;

            Dictionary<string, object> dict = null;
            if (jsonObject != null)
                dict = Json.Deserialize(jsonObject.ToString()) as Dictionary<string, object>;
			_success(dict);
        }

        public void OnFailure(JSONObject jsonObject)
        {
			if (_failure == null)
				return;

            Dictionary<string, object> dict = null;
            if (jsonObject != null)
                dict = Json.Deserialize(jsonObject.ToString()) as Dictionary<string, object>;
			_failure(dict);
        }
    }
}
