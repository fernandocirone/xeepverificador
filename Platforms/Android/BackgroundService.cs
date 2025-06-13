using Android.App;
using Android.OS;
using AndroidX.Core.App;
using Android.Content;
using verificador.Services;

namespace verificador.Platforms.Android
{
    [Service]
    internal class BackgroundService : Service
    {
        Timer timer = null;
        int myId = (new object()).GetHashCode();
        int BadgeNumber = 0;
        private readonly IBinder binder = new LocalBinder();
        NotificationCompat.Builder notification;
        Funciones Funciones;


        public class LocalBinder : Binder
        {
            public BackgroundService GetService()
            {
                return this.GetService();
            }
        }

        public override IBinder OnBind(Intent intent)
        {
            return binder;
        }

        public override StartCommandResult OnStartCommand(Intent intent,
            StartCommandFlags flags, int startId)
        {
            var input = intent.GetStringExtra("inputExtra");

            var notificationIntent = new Intent(this, typeof(MainActivity));
            notificationIntent.SetAction("USER_TAPPED_NOTIFIACTION");

            var pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent,
                PendingIntentFlags.Immutable);

            // Increment the BadgeNumber
            BadgeNumber++;

            notification = new NotificationCompat.Builder(this,
                    MainApplication.ChannelId)
                .SetContentText(input)
                .SetSmallIcon(Resource.Drawable.dotnet_bot)
                .SetAutoCancel(false)
                .SetContentTitle("Xeep Verificador")
                .SetPriority(NotificationCompat.PriorityDefault)
                .SetContentIntent(pendingIntent);

            notification.SetNumber(BadgeNumber);

            // build and notify
            StartForeground(myId, notification.Build());

            timer = new Timer(Timer_Elapsed, null, 0, 10000);

            // Obtener la ruta de la base de datos
            string dbPath = System.IO.Path.Combine(FileSystem.AppDataDirectory, "verificador.db3");

            // Crear una instancia de ConsultaCopiaDatosService
            Funciones = new Funciones(dbPath);

            return StartCommandResult.Sticky;
        }

        /// <summary>
        /// Timer elapsed event handler
        /// </summary>
        /// <param name="state"></param>
        async void Timer_Elapsed(object state)
        {
            try
            {
                // Realizar operaciones asincrónicas dentro del temporizador
                // Asegúrate de que estas operaciones no bloqueen el subproceso
                await Funciones.CargarDatosDesdeOrigenAsync();

            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir durante la ejecución del temporizador
                Console.WriteLine($"Error en el temporizador: {ex.Message}");
            }
        }
    }
}
