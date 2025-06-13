using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;

public class ConfiguracionServer : WebApiController
{
    private static string _cadena = "";
    private static string _consulta = "";

    [Route(HttpVerbs.Get, "/configuracion")]
    public object GetConfiguracion() => new
    {
        CadenaConexion = _cadena,
        ConsultaSql = _consulta
    };

    [Route(HttpVerbs.Post, "/configuracion")]
    public async Task SaveConfiguracion()
    {
        var body = await HttpContext.GetRequestBodyAsStringAsync();
        var datos = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(body);
        _cadena = datos["cadena"];
        _consulta = datos["consulta"];
    }
}
