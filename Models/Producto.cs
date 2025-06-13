using SQLite;
namespace verificador.Models
{
    [Table("producto")]
    public class Producto
    {
        [PrimaryKey, AutoIncrement, Column("idproducto")]
        public int Id { get; set; }
        [Unique]
        public string ? codigodebarras { get; set; }
        public string ?  descripcion { get; set; }
        public decimal precioventa { get; set; }
    }
}
