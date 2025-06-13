using Npgsql;
using SQLite;
using verificador.Models;


namespace verificador.Services
{
    public interface IFunciones
    {
        Task CargarDatosDesdeOrigenAsync();
        Task<Producto> ObtenerProductoAsync(string c);
    }

    public class Funciones : IFunciones
    {
        private readonly string _conexionOrigen;
        private readonly string _consultaBD;

        private string _dbPath;
        public string StatusMessage { get; set; }
        private SQLiteAsyncConnection _connection;


        public Funciones(string dbPath)
        {
            _conexionOrigen = "Server=192.168.0.139;Port=5432;Database=aledosrv;User Id=postgres;Password=dlrpos;";
            //configuration.GetConnectionString("ConexionOrigen");
            _consultaBD = "SELECT ROUND(pr.precio, 2) AS precio, a.descripcio, cb.codigobarr FROM precioreal(0, 0) pr LEFT JOIN articulos a ON pr.marca = a.marca AND pr.codigo = a.codigo LEFT JOIN codigosbarra cb ON a.marca = cb.marca AND a.codigo = cb.codigo WHERE cb.codigobarr IS NOT NULL;";
            //configuration["Queries:ObtenerDatos"];
            _dbPath = dbPath;
        }

        public async Task GuardarConfiguracionAsync(string clave, string valor)
        {
            await IniciarBDSQLite();
            var config = new Configuracion { Clave = clave, Valor = valor };
            await _connection.InsertOrReplaceAsync(config);
        }

        public async Task<string> ObtenerConfiguracionAsync(string clave)
        {
            await IniciarBDSQLite();
            var config = await _connection.FindAsync<Configuracion>(clave);
            return config?.Valor;
        }

        private async Task IniciarBDSQLite()
        {
            try
            {
                // Verificar si ya hay una conexión establecida
                if (_connection == null)
                {
                    // Crear una nueva conexión
                    _connection = new SQLiteAsyncConnection(_dbPath);

                    // Crear la tabla Producto si no existe
                    await _connection.CreateTableAsync<Producto>();
                    await _connection.CreateTableAsync<Configuracion>();

                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción adecuadamente, como registrarla o lanzarla nuevamente
                Console.WriteLine($"Error al inicializar la base de datos SQLite: {ex}");
                throw;
            }
        }

        public async Task<Producto> ObtenerProductoAsync(string codbarras)
        {
            try
            {
                await IniciarBDSQLite(); // Asegurar que la conexión está abierta y la tabla está creada

                // Obtener el producto de la tabla basado en el ID y devolverlo
                return await _connection.Table<Producto>().FirstOrDefaultAsync(p => p.codigodebarras == codbarras);
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir durante la obtención del producto
                Console.WriteLine($"Error en ObtenerProductoAsync: {ex.Message}");
                throw;
            }
        }

        public async Task CargarDatosDesdeOrigenAsync()
        {
            try
            {
                await using (var sourceConnection = new NpgsqlConnection(_conexionOrigen))
                {
                    await sourceConnection.OpenAsync();
                    string sqlQuery = _consultaBD;

                    await using (var command = new NpgsqlCommand(sqlQuery, sourceConnection))
                    {
                        await using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                decimal valorPrecio = reader.GetDecimal(reader.GetOrdinal("precio"));
                                string valorDescripcion = reader.GetString(reader.GetOrdinal("descripcio"));
                                string valorCodigoBarra = reader.IsDBNull(reader.GetOrdinal("codigobarr"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("codigobarr")).Trim();

                                await ActualizarOInsertarAsync(valorCodigoBarra, valorPrecio, valorDescripcion);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CargarDatosDesdeOrigenAsync: {ex.Message}");
            }
        }

        private async Task ActualizarOInsertarAsync(string codigodebarras, decimal nuevoPrecio, string descripcion)
        {
            try
            {
                await IniciarBDSQLite();

                string existeQuery = "SELECT COUNT(*) FROM producto WHERE codigodebarras = @codigodebarras";
                var count = await _connection.ExecuteScalarAsync<int>(existeQuery, codigodebarras);

                if (count > 0)
                {
                    string existeQueryp = "SELECT COUNT(*) FROM producto WHERE codigodebarras = @codigodebarras and precioventa = @nuevoPrecio";
                    var countp = await _connection.ExecuteScalarAsync<int>(existeQueryp, codigodebarras, nuevoPrecio);

                    if (countp > 0)
                    {
                        return;
                    }
                    else
                    {
                        await ActualizarPrecioAsync(codigodebarras, nuevoPrecio);
                        Console.WriteLine($"Se encontró el producto: {codigodebarras} y el se actualizó el precio a {nuevoPrecio}.");
                    }
                }
                else
                {
                    // Si el producto no existe, insertarlo en la base de datos
                    await InsertarDatosAsync(nuevoPrecio, descripcion, codigodebarras);
                    Console.WriteLine($"Nuevo producto: {codigodebarras} y con precio $ {nuevoPrecio}.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActualizarOInsertarAsync: {ex.Message}");
                throw;
            }
        }

        private async Task ActualizarPrecioAsync(string codigodebarras, decimal nuevoPrecio)
        {
            try
            {
                await IniciarBDSQLite();

                // Obtener el producto existente de la base de datos local
                var producto = await _connection.Table<Producto>().FirstOrDefaultAsync(p => p.codigodebarras == codigodebarras);

                if (producto != null)
                {
                    // Actualizar el precio del producto
                    producto.precioventa = nuevoPrecio;
                    await _connection.UpdateAsync(producto);
                }
                else
                {
                    Console.WriteLine($"No se encontró el producto con el código de barras {codigodebarras} en la base de datos local.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActualizarPrecioAsync: {ex.Message}");
                throw;
            }
        }


        private async Task InsertarDatosAsync(decimal nuevoPrecio, string descripcion, string codigodebarras)
        {
            try
            {
                await IniciarBDSQLite();

                // Crear un objeto Producto con los datos proporcionados
                Producto producto = new Producto();
                producto.codigodebarras = codigodebarras;
                producto.descripcion = descripcion;
                producto.precioventa = nuevoPrecio;

                // Insertar el objeto Producto en la base de datos
                await _connection.InsertAsync(producto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en InsertarDatosAsync: {ex.Message}");
                throw;
            }
        }


    }
}
