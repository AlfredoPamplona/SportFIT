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

        public List<string> ObtenerNombresPueblos()
        {
            List<string> nombresPueblos = new List<string>();

            // Consulta SQL para obtener los nombres de los pueblos
            string query = "SELECT nombre_pueblo FROM pueblo";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string nombrePueblo = reader["Nombre"].ToString();
                        nombresPueblos.Add(nombrePueblo);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción adecuadamente (por ejemplo, registrando o lanzando una excepción personalizada)
                Console.WriteLine("Error al obtener nombres de pueblos: " + ex.Message);
            }

            return nombresPueblos;
        }
    }
}
