using System;
using System.Configuration;
using System.Data.SqlClient;

namespace SportFIT.Controllers
{
    // Clase que gestiona la lógica relacionada con los roles de usuario
    public class RoleController
    {
        private readonly string connectionString; // Cadena de conexion a la base de datos

        // Constructor que recibe la cadena de conexión a la base de datos
        public RoleController(string connectionString)
        {
            this.connectionString = connectionString;
        }

        // Método para obtener el rol de un usuario dado su nombre de usuario
        public string GetUserRole(string username)
        {
            try
            {
                string role = "usuario"; // Valor por defecto si no se encuentra un rol específico

                // Consulta SQL para obtener el rol del usuario
                string query = @"
                SELECT distinct r.nombre_rol
                FROM Usuario u
                INNER JOIN Rol r ON u.id_rol = r.id_rol
                WHERE u.nombre_usuario = @Username";

                // Establecer una conexión a la base de datos utilizando SqlConnection
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open(); // Abrir la conexión

                    // Configurar y ejecutar la consulta SQL
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Establecer el parámetro para el nombre de usuario en la consulta
                        command.Parameters.AddWithValue("@Username", username);

                        // Ejecutar la consulta y obtener el resultado (nombre del rol)
                        object result = command.ExecuteScalar();

                        // Verificar si se obtuvo un resultado válido
                        if (result != null && result != DBNull.Value)
                        {
                            role = result.ToString(); // Asignar el nombre del rol obtenido
                        }
                    }
                }

                return role; // Devolver el nombre del rol del usuario
            }
            catch (Exception ex)
            {
                // Capturar cualquier excepción ocurrida durante la obtención del rol
                Console.WriteLine($"Error al obtener el rol del usuario: {ex.Message}");
                return "usuario"; // Devolver un valor predeterminado en caso de error
            }
        }
    }
}
