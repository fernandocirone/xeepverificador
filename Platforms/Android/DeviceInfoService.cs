#if ANDROID
using Android.Content;
using Android.Net.Wifi;
using AndroidApp = Android.App.Application;

namespace verificador.Platforms.Android
{
    public static class DeviceInfoService
    {
        public static string ObtenerIp()
        {
            var wifiManager = (WifiManager)AndroidApp.Context.GetSystemService(Context.WifiService);
            if (wifiManager != null)
            {
                var ip = wifiManager.ConnectionInfo.IpAddress;
                var ipBytes = BitConverter.GetBytes(ip);
                return string.Join(".", ipBytes);
            }

            return "Desconocida";
        }
    }
}
#endif
