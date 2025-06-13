using Microsoft.Extensions.Logging;
using verificador.Data;
using verificador.Services;
using EmbedIO;
using verificador.Api;
using EmbedIO.WebApi;

namespace verificador
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            string dbPath = Conexiones.ObtenerPathBD("verificador.db3");
            builder.Services.AddSingleton<IFunciones>(s => ActivatorUtilities.CreateInstance<Funciones>(s, dbPath));

            // 🔥 Iniciar servidor EmbedIO
            Task.Run(() =>
            {
                // Ruta del archivo original (del paquete de la app)
                var origen = Path.Combine(AppContext.BaseDirectory, "wwwroot", "config.html");
                // Ruta de destino donde EmbedIO servirá los archivos
                var destino = Path.Combine(FileSystem.Current.AppDataDirectory, "wwwroot", "config.html");

                // Crear carpeta destino si no existe
                var destinoDir = Path.GetDirectoryName(destino);
                if (!Directory.Exists(destinoDir))
                    Directory.CreateDirectory(destinoDir);

                // Copiar archivo solo si no está ya en destino
                if (!File.Exists(destino))
                    File.Copy(origen, destino);

                // Iniciar servidor web
                var server = new WebServer(o => o
                        .WithUrlPrefix("http://*:5000")
                        .WithMode(HttpListenerMode.EmbedIO))
                    .WithWebApi("/", m => m.WithController<ConfiguracionController>())
                    .WithStaticFolder("/", Path.Combine(FileSystem.Current.AppDataDirectory, "wwwroot"), true);

                server.RunAsync();
            });

            return builder.Build();
        }
    }
}
