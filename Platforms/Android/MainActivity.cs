using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using verificador.Platforms.Android;

namespace verificador
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true,
            LaunchMode = LaunchMode.SingleTop, // 👈 Agregá esto
            ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                               ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        public MainActivity()
        {
            AndroidServiceManager.MainActivity = this;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Mantener pantalla encendida y activar fullscreen
            EnterFullScreenMode();

            Window.AddFlags(WindowManagerFlags.KeepScreenOn);

            // Forzar pantalla completa sin barra de estado ni navegación
            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(
                  SystemUiFlags.ImmersiveSticky
                | SystemUiFlags.Fullscreen
                | SystemUiFlags.HideNavigation
                | SystemUiFlags.LayoutStable
                | SystemUiFlags.LayoutFullscreen
                | SystemUiFlags.LayoutHideNavigation);
        }

        private void EnterFullScreenMode()
        {
            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(
                  SystemUiFlags.ImmersiveSticky
                | SystemUiFlags.Fullscreen
                | SystemUiFlags.HideNavigation
                | SystemUiFlags.LayoutStable
                | SystemUiFlags.LayoutFullscreen
                | SystemUiFlags.LayoutHideNavigation);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            ProcessIntent(intent);
        }

        private void ProcessIntent(Intent intent)
        {
            if (intent != null)
            {
                var action = intent.Action;
                if (action == "USER_TAPPED_NOTIFIACTION")
                {
                    // Acción personalizada
                }
            }
        }

        public void IniciarServicio()
        {
            var serviceIntent = new Intent(this, typeof(BackgroundService));
            serviceIntent.PutExtra("inputExtra", "Servicio de Verificador de Precios Xeep");
            StartService(serviceIntent);
        }

        public void DetenerServicio()
        {
            var serviceIntent = new Intent(this, typeof(BackgroundService));
            StopService(serviceIntent);
        }
    }
}
