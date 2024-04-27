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
            int idPueblo=0;
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
        public List<string> ObtenerNombresPueblos()
        {
            List<string> nombresPueblos = new List<string>();

            // Consulta SQL para obtener los pueblos
            string query = "SELECT * FROM pueblo";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
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
                // Manejar la excepción adecuadamente (por ejemplo, registrando o lanzando una excepción personalizada)
                Console.WriteLine("Error al obtener nombres de pueblos: " + ex.Message);
            }

            return nombresPueblos;
        }
    }
}
