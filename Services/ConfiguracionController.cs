using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using verificador.Services;
using verificador.Data;

namespace verificador.Api
{
    public class ConfiguracionController : WebApiController
    {
        private static string _cadena = "";
        private static string _consulta = "";

        [Route(HttpVerbs.Get, "/configuracion")]
        public async Task<object> Obtener()
        {
            var funciones = new Funciones(Conexiones.ObtenerPathBD("verificador.db3"));
            var cadena = await funciones.ObtenerConfiguracionAsync("cadena");
            var consulta = await funciones.ObtenerConfiguracionAsync("consulta");

            return new { Cadena = cadena, Consulta = consulta };
        }

        [Route(HttpVerbs.Post, "/configuracion")]
        public async Task Guardar()
        {
            var datos = await HttpContext.GetRequestDataAsync<Dictionary<string, string>>();
            var funciones = new Funciones(Conexiones.ObtenerPathBD("verificador.db3"));

            if (datos.TryGetValue("cadena", out var nuevaCadena))
                await funciones.GuardarConfiguracionAsync("cadena", nuevaCadena);

            if (datos.TryGetValue("consulta", out var nuevaConsulta))
                await funciones.GuardarConfiguracionAsync("consulta", nuevaConsulta);
        }

    }
}
