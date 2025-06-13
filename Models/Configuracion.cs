using SQLite;

namespace verificador.Models
{
    public class Configuracion
    {
        [PrimaryKey]
        public string Clave { get; set; }
        public string Valor { get; set; }
    }
}
