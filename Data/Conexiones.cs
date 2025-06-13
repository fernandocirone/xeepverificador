
namespace verificador.Data
{
    public class Conexiones
    {
        public static string ObtenerPathBD(string nombreArchivo)
        {
            return System.IO.Path.Combine(FileSystem.AppDataDirectory, nombreArchivo);
        }
    }
}
