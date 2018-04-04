using System;
using System.Collections.Generic;
using Com.OneSignal.Abstractions;
using Org.Json;

namespace Com.OneSignal
{
    public class TagsHandler : Java.Lang.Object, Android.OneSignal.IGetTagsHandler
    {
		readonly TagsReceived _tagsReceived;

		public TagsHandler(TagsReceived tagsReceived) => _tagsReceived = tagsReceived;

        public void TagsAvailable(JSONObject jsonObject)
        {
			if (_tagsReceived == null)
				return;

            Dictionary<string, object> dict = null;
            if (jsonObject != null)
                dict = Json.Deserialize(jsonObject.ToString()) as Dictionary<string, object>;
			_tagsReceived(dict);
        }
    }
}
