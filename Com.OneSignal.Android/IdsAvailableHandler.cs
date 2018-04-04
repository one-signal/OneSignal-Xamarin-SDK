using System;

using Com.OneSignal.Abstractions;
using Com.OneSignal.Android;

namespace Com.OneSignal
{
    public class IdsAvailableHandler : Java.Lang.Object, Android.OneSignal.IIdsAvailableHandler
    {
		readonly IdsAvailableCallback _idsAvailable;

		public IdsAvailableHandler(IdsAvailableCallback idsAvailable) => _idsAvailable = idsAvailable;

        public void IdsAvailable(string p0, string p1)
        {
			_idsAvailable?.Invoke(p0, p1);
        }
    }
}
