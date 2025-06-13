using Android.App;
using Android.Content;
using Android.Widget;
using AndroidX.Core.Content;
using verificador.Platforms.Android;


namespace verificador.Platforms.Android
{
    [BroadcastReceiver(Enabled = true, Exported = true, DirectBootAware = true)]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    public class BootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == Intent.ActionBootCompleted)
            {
                Toast.MakeText(context, "Boot completed event received",
                    ToastLength.Short).Show();
                var serviceIntent = new Intent(context,
                    typeof(BackgroundService));

                ContextCompat.StartForegroundService(context,
                    serviceIntent);
            }
        }
    }
}
