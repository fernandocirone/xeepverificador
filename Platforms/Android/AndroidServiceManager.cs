
namespace verificador.Platforms.Android
{
    public static class AndroidServiceManager
    {
        public static MainActivity MainActivity { get; set; }

        public static bool IsRunning { get; set; }
        public static void IniciarServicio()
        {
            if (MainActivity == null) return;
            MainActivity.IniciarServicio();
        }

        public static void DetenerServicio()
        {
            if (MainActivity == null) return;
            MainActivity.DetenerServicio();
        }
    }
}
