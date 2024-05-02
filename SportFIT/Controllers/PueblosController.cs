using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SportFIT.Controllers
{
    public class PueblosController
    {
        private readonly string connectionString; // Cadena de conexion a la base de datos

        // Constructor que recibe la cadena de conexión a la base de datos
        public PueblosController(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public int ObtenerPuebloSelected(string pueblo)
        {
            int idPueblo = 0;
            string query = @"SELECT id_pueblo FROM pueblo where nombre_pueblo = @PuebloNombre";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@PuebloNombre", pueblo);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        idPueblo = Convert.ToInt32(reader["id_pueblo"]);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción adecuadamente (por ejemplo, registrando o lanzando una excepción personalizada)
                Console.WriteLine("Error al obtener nombres de pueblos: " + ex.Message);
            }
            return idPueblo;
        }
        public List<string> ObtenerNombresPueblos(string usuario, string password)
        {
            List<string> nombresPueblos = new List<string>();

            // Consulta SQL para obtener los pueblos asociados al usuario con nombre y contraseña específicos
            string query = @"SELECT p.nombre_pueblo 
                     FROM Usuario u 
                     INNER JOIN Usuario_Pueblo up ON u.id_usuario = up.id_usuario 
                     INNER JOIN Pueblo p ON up.id_pueblo = p.id_pueblo 
                     WHERE u.nombre_usuario = @usuario AND u.password = @password";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@usuario", usuario);
                    command.Parameters.AddWithValue("@password", password);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string nombrePueblo = reader["nombre_pueblo"].ToString();
                        nombresPueblos.Add(nombrePueblo);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener nombres de pueblos por usuario: " + ex.Message);
            }

            return nombresPueblos;
        }
    }
}
