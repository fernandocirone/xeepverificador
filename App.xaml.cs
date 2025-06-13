using verificador.Services;

namespace verificador
{
    public partial class App : Application
    {
        string Message = string.Empty;


        public App(IFunciones funciones)
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            base.OnStart();
#if ANDROID
            if (!verificador.Platforms.Android.AndroidServiceManager.IsRunning)
            {
                verificador.Platforms.Android.AndroidServiceManager.IniciarServicio();
                Message = "Servicio Iniciado";
            }
            else
            {
                Message = "El Servicio ya fue Iniciado";
            }
#endif

        }



    }
}
