using SportFIT.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SportFIT.Controllers
{
    public class InstalacionesController
    {
        private readonly string connectionString;

        public InstalacionesController(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<InstalacionViewModel> ObtenerInstalaciones(int selectedPuebloId)
        {
            List<InstalacionViewModel> instalaciones = new List<InstalacionViewModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT DISTINCT i.id_instalacion, i.nombre_instalacion, i.tipo, i.direccion 
                                FROM Instalacion i INNER JOIN Pueblo p ON i.id_pueblo = p.id_pueblo 
                                WHERE i.id_pueblo = @selectedPuebloId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@selectedPuebloId", selectedPuebloId);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    // Crear objetos Reserva y llenarlos con los datos del lector
                    InstalacionViewModel instalacion = new InstalacionViewModel
                    {
                        IdInstalacion = Convert.ToInt32(reader["id_instalacion"]),
                        NombreInstalacion = reader["nombre_instalacion"].ToString(),
                        TipoInstalacion = reader["tipo"].ToString(),
                        DireccionInstalacion = reader["direccion"].ToString()
                    };

                    instalaciones.Add(instalacion);
                }
            }

            return instalaciones;
        }

        public bool EliminarInstalacion(int idInstalacion)
        {
            try
            {
                string query = @"DELETE FROM instalacion WHERE id_instalacion = @idInstalacion";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@idInstalacion", idInstalacion);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    // Verificar si se eliminó correctamente (al menos una fila afectada)
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al eliminar reserva: " + ex.Message);
                return false;
            }
        }

    }


}
