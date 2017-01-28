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

#if __IOS__ || __ANDROID__
#define ONESIGNAL_PLATFORM
#define SUPPORTS_LOGGING
#endif 

#if !__IOS__ && __ANDROID__
 #define ANDROID_ONLY
 #endif

using System.Collections.Generic;

namespace Com.OneSignal
{

	public class OSNotificationPayload {
	  	public string notificationID;
	  	public string sound;
	   	public string title;
	   	public string body;
	   	public string subtitle;
	   	public string launchURL;
         public Dictionary<string, object> additionalData;
		   public List<Dictionary<string, object>> actionButtons;
	   	public bool contentAvailable;
	   	public int badge;
	   	public string smallIcon;
	   	public string largeIcon;
	   	public string bigPicture;
	   	public string smallIconAccentColor;
	   	public string ledColor;
	   	public int lockScreenVisibility = 1;
	   	public string groupKey;
	   	public string groupMessage;
	   	public string fromProjectNumber;
	}

	public class OSNotification {
	   	public enum DisplayType {
	      	// Notification shown in the notification shade.
	    	Notification,

	      	// Notification shown as an in app alert.
	      	InAppAlert,

	      	// Notification was silent and not displayed.
	      	None
	   	}

		public bool isAppInFocus;
	   	public bool shown;
	   	public bool silentNotification;
	   	public int androidNotificationId;
	   	public DisplayType displayType;
	   	public OSNotificationPayload payload;
	}

	public class OSNotificationAction {
		public enum ActionType {
	    	// Notification was tapped on.
	    	Opened,

	    	// User tapped on an action from the notification.
	    	ActionTaken
		}

	   	public string actionID;
	   	public ActionType type;
	}

	public class OSNotificationOpenedResult {
	   	public OSNotificationAction action;
	   	public OSNotification notification;
	}

	public static class OneSignal {

		// NotificationReceived - Delegate is called when a push notification is received when the user is in your game.
	   // notification = The Notification dictionary filled from a serialized native OSNotification object
	   public delegate void NotificationReceived(OSNotification notification);

	   // NotificationOpened - Delegate is called when a push notification is opened.
	   // result = The Notification open result describing : 1. The notification opened 2. The action taken by the user
	   public delegate void NotificationOpened(OSNotificationOpenedResult result);
	   
	   public delegate void IdsAvailableCallback(string playerID, string pushToken);
	   public delegate void TagsReceived(Dictionary<string, object> tags);

	   public delegate void OnPostNotificationSuccess(Dictionary<string, object> response);
	   public delegate void OnPostNotificationFailure(Dictionary<string, object> response);

		// delegates
		static IdsAvailableCallback idsAvailableDelegate;
		public static TagsReceived tagsReceivedDelegate;

		public const string kOSSettingsKeyAutoPrompt = "kOSSettingsKeyAutoPrompt";
   	public const string kOSSettingsKeyInAppLaunchURL = "kOSSettingsKeyInAppLaunchURL";

		// logging
		public enum LOG_LEVEL { NONE, FATAL, ERROR, WARN, INFO, DEBUG, VERBOSE }

		public enum OSInFocusDisplayOption {
      		None, InAppAlert, Notification
   		}

		#if ONESIGNAL_PLATFORM
   			#if SUPPORTS_LOGGING
      			private static LOG_LEVEL logLevel = LOG_LEVEL.INFO, visualLogLevel = LOG_LEVEL.NONE;
			#endif

			// shared platform member
			static OneSignalPlatform oneSignalPlatform;

			internal static OnPostNotificationSuccess postNotificationSuccessDelegate;
			internal static OnPostNotificationFailure postNotificationFailureDelegate;

			// Name of the GameObject that gets automaticly created in your game scene.
			private const string gameObjectName = "OneSignalRuntimeObject_KEEP";

		#endif

		public class XamarinBuilder {
	      public string appID;
         public string googleProjectNumber;
	      public Dictionary<string, bool> iOSSettings;
	      public OSInFocusDisplayOption displayOption = OSInFocusDisplayOption.InAppAlert;
	      public NotificationReceived _notificationReceivedDelegate;
	      public NotificationOpened _notificationOpenedDelegate;

	      // inNotificationReceivedDelegate   = Calls this delegate when a notification is received.
	      public XamarinBuilder HandleNotificationReceived(NotificationReceived inNotificationReceivedDelegate) {
	         _notificationReceivedDelegate = inNotificationReceivedDelegate;
	         return this;
	      }

	      // inNotificationOpenedDelegate     = Calls this delegate when a push notification is opened.
	      public XamarinBuilder HandleNotificationOpened(NotificationOpened inNotificationOpenedDelegate) {
	         _notificationOpenedDelegate = inNotificationOpenedDelegate;
	         return this;
	      }

	      public XamarinBuilder InFocusDisplaying(OSInFocusDisplayOption display) {
	         displayOption = display;
	         return this;
	      }

	      // Pass one if the define kOSSettings strings as keys only. Only affects iOS platform.
	      // autoPrompt                       = Set false to delay the iOS accept notification system prompt. Defaults true.
	      //                                    You can then call RegisterForPushNotifications at a better point in your game to prompt them.
	      // inAppLaunchURL                   = (iOS) Set false to force a ULRL to launch through Safari instead of in-app webview.
         
	      public XamarinBuilder Settings(Dictionary<string, bool> settings) {
            iOSSettings = settings;
            return this;
	      }

	      public void EndInit() {
	         OneSignal.Init();
	      }

   		}

   		internal static XamarinBuilder builder;

		// Init - Only required method you call to setup OneSignal to recieve push notifications.
	   	//        Call this on the first scene that is loaded.
	   	// appId                            = Your OneSignal AppId from onesignal.com

		public static XamarinBuilder StartInit(string appID, string googleProjectNumber = "") {
      		if (builder == null)
            	builder = new XamarinBuilder();
      		#if ONESIGNAL_PLATFORM
         		builder.appID = appID;
               builder.googleProjectNumber = googleProjectNumber;
      		#endif
      		return builder;
   		}

		// Init - Only required method you call to setup OneSignal to receive push notifications.
		private static void Init() {	

			#if ONESIGNAL_PLATFORM
				if (oneSignalPlatform != null || builder == null) return;
         
				#if __ANDROID__
               oneSignalPlatform = new OneSignalAndroid(builder.appID, builder.googleProjectNumber, builder.displayOption, logLevel, visualLogLevel);
            	#elif __IOS__
            		//extract settings
               		bool autoPrompt = true, inAppLaunchURL = true;

               		if (builder.iOSSettings != null) {
		                  if(builder.iOSSettings.ContainsKey(kOSSettingsKeyAutoPrompt))
		                     autoPrompt = builder.iOSSettings[kOSSettingsKeyAutoPrompt];
		                  if (builder.iOSSettings.ContainsKey(kOSSettingsKeyInAppLaunchURL))
		                     inAppLaunchURL = builder.iOSSettings[kOSSettingsKeyInAppLaunchURL];
		            }
		            oneSignalPlatform = new OneSignalIOS(builder.appID, autoPrompt, inAppLaunchURL, builder.displayOption, logLevel, visualLogLevel);				
				#endif
			#endif
		}

		public static void SetLogLevel (LOG_LEVEL ll, LOG_LEVEL vll) {
			#if SUPPORTS_LOGGING
				logLevel = ll;
				visualLogLevel = vll;
			#endif
		}

		// Tag player with a key value pair to later create segments on them at onesignal.com.
		public static void SendTag (string tagName, string tagValue) {
			#if ONESIGNAL_PLATFORM
				oneSignalPlatform.SendTag (tagName, tagValue);
			#endif
		}

		// Tag player with a key value pairs to later create segments on them at onesignal.com.
		public static void SendTags (IDictionary<string, string> tags) {
			#if ONESIGNAL_PLATFORM
				oneSignalPlatform.SendTags (tags);
			#endif
		}

		// Makes a request to onesignal.com to get current tags set on the player and then run the callback passed in.
		public static void GetTags(TagsReceived inTagsReceivedDelegate) {
			#if ONESIGNAL_PLATFORM
				tagsReceivedDelegate = inTagsReceivedDelegate;
				oneSignalPlatform.GetTags();
			#endif
		}
			
		public static void GetTags() {
			#if ONESIGNAL_PLATFORM
				oneSignalPlatform.GetTags();
			#endif
		}

		public static void DeleteTag (string key) {
			#if ONESIGNAL_PLATFORM
				oneSignalPlatform.DeleteTag(key);
			#endif
		}

		public static void DeleteTags(IList<string> keys) {
			#if ONESIGNAL_PLATFORM
				oneSignalPlatform.DeleteTags(keys);
			#endif
		}
			
		public static void RegisterForPushNotifications () {
			#if ONESIGNAL_PLATFORM
				oneSignalPlatform.RegisterForPushNotifications();
			#endif
		}
			
		public static void IdsAvailable(IdsAvailableCallback inIdsAvailableDelegate) {
			#if ONESIGNAL_PLATFORM
				idsAvailableDelegate = inIdsAvailableDelegate;
         		oneSignalPlatform.IdsAvailable();
			#endif
		}
			
		public static void IdsAvailable() {
			#if ONESIGNAL_PLATFORM
				oneSignalPlatform.IdsAvailable();
			#endif
		}

		public static void EnableVibrate (bool enable) {
			#if __ANDROID__
				((OneSignalAndroid)oneSignalPlatform).EnableVibrate(enable);
			#endif
		}

		public static void EnableSound (bool enable) {
			#if __ANDROID__
				((OneSignalAndroid)oneSignalPlatform).EnableSound(enable);
			#endif
		}

		public static void ClearOneSignalNotifications() {
    		#if ANDROID_ONLY
         		((OneSignalAndroid)oneSignalPlatform).ClearOneSignalNotifications();
      		#endif
   		}

		public static void SetSubscription(bool enable) {
			#if ONESIGNAL_PLATFORM
				oneSignalPlatform.SetSubscription(enable);
			#endif
		}

        public static void PostNotification(Dictionary<string, object> data)
        {
#if ONESIGNAL_PLATFORM
				PostNotification (data, null, null);
#endif
        }

        public static void PostNotification(Dictionary<string, object> data, OnPostNotificationSuccess inOnPostNotificationSuccess, OnPostNotificationFailure inOnPostNotificationFailure)
        {
#if ONESIGNAL_PLATFORM
         		postNotificationSuccessDelegate = inOnPostNotificationSuccess;
         		postNotificationFailureDelegate = inOnPostNotificationFailure;
         		oneSignalPlatform.PostNotification(data);
#endif
        }

        public static void PostNotification(string jsonString)
        {
#if ONESIGNAL_PLATFORM
				PostNotification (jsonString, null, null);
#endif
        }

        public static void PostNotification(string jsonString, OnPostNotificationSuccess inOnPostNotificationSuccess, OnPostNotificationFailure inOnPostNotificationFailure)
        {
#if ONESIGNAL_PLATFORM
         		postNotificationSuccessDelegate = inOnPostNotificationSuccess;
         		postNotificationFailureDelegate = inOnPostNotificationFailure;
         		oneSignalPlatform.PostNotification(jsonString);
#endif
        }

        public static void SyncHashedEmail(string email) {
     		#if ONESIGNAL_PLATFORM
         		oneSignalPlatform.SyncHashedEmail(email);
      		#endif
   		}

    	public static void PromptLocation() {
        	#if ONESIGNAL_PLATFORM
            	oneSignalPlatform.PromptLocation();
        	#endif
    	}

   		/*** protected and private methods ****/
		#if ONESIGNAL_PLATFORM
      
   		// Called from the native SDK - Called when a push notification received.
      public static void onPushNotificationReceived(OSNotification notification) {
   			if (builder._notificationReceivedDelegate != null)
            {
               builder._notificationReceivedDelegate(notification);
      		}
   		}

	      // Called from the native SDK - Called when a push notification is opened by the user
      public static void onPushNotificationOpened(OSNotificationOpenedResult result) {
	      if (builder._notificationOpenedDelegate != null) {
	         builder._notificationOpenedDelegate(result);
	      }
	   }
	      
	   // Called from the native SDK - Called when device is registered with onesignal.com service or right after IdsAvailable
	   //                          if already registered.
	   public static void onIdsAvailable(string userId, string pushToken) {
	      if (idsAvailableDelegate != null) {
            idsAvailableDelegate(userId, pushToken);
	      }
	   }

	   // Called from the native SDK - Called After calling GetTags(...)
	   public static void onTagsReceived(Dictionary<string, object> dict) {
	      if (tagsReceivedDelegate != null)
	         tagsReceivedDelegate(dict);
	   }

	   // Called from the native SDK
      public static void onPostNotificationSuccess(Dictionary<string, object> response) {
	      if (postNotificationSuccessDelegate != null) {
	         OnPostNotificationSuccess tempPostNotificationSuccessDelegate = postNotificationSuccessDelegate;
	         postNotificationFailureDelegate = null;
	         postNotificationSuccessDelegate = null;
	         tempPostNotificationSuccessDelegate(response);
	      }
	   }

	   // Called from the native SDK
	   public static void onPostNotificationFailed(Dictionary<string, object> response) {
	      if (postNotificationFailureDelegate != null) {
	         OnPostNotificationFailure tempPostNotificationFailureDelegate = postNotificationFailureDelegate;
	         postNotificationFailureDelegate = null;
	         postNotificationSuccessDelegate = null;
	         tempPostNotificationFailureDelegate(response);
	      }
	   }

	#endif

	}
}