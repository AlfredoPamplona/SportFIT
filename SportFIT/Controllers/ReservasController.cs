using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SportFIT.Controllers
{
    public class ReservasController
    {
        private readonly string connectionString; // Cadena de conexión a la base de datos

        // Constructor que recibe la cadena de conexión a la base de datos
        public ReservasController(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<string> ObtenerReservasDetalladas()
        {
            List<string> reservasDetalladas = new List<string>();

            // Consulta SQL para obtener los detalles de las reservas
            string query = @"SELECT r.id_reserva, u.nombre_usuario, r.fecha_reserva, r.hora_inicio, r.duracion, a.nombre_actividad, i.nombre_instalacion
                             FROM Reserva r
                             INNER JOIN Usuario u ON r.id_usuario = u.id_usuario
                             INNER JOIN Instalacion i ON r.id_instalacion = i.id_instalacion
                             INNER JOIN Actividad a ON i.id_instalacion = a.id_instalacion";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int idReserva = Convert.ToInt32(reader["id_reserva"]);
                        string nombreUsuario = reader["nombre_usuario"].ToString();
                        DateTime fechaReserva = Convert.ToDateTime(reader["fecha_reserva"]);
                        TimeSpan horaInicio = TimeSpan.Parse(reader["hora_inicio"].ToString());
                        int duracion = Convert.ToInt32(reader["duracion"]);
                        string nombreActividad = reader["nombre_actividad"].ToString();
                        string nombreInstalacion = reader["nombre_instalacion"].ToString();

                        // Formatear los detalles de la reserva como texto y agregar a la lista
                        string reservaDetallada = $"ID: {idReserva}, Usuario: {nombreUsuario}, Fecha: {fechaReserva.ToShortDateString()}, Hora: {horaInicio}, Duración: {duracion} horas, Actividad: {nombreActividad}, Instalación: {nombreInstalacion}";
                        reservasDetalladas.Add(reservaDetallada);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción adecuadamente (por ejemplo, registrando o lanzando una excepción personalizada)
                Console.WriteLine("Error al obtener reservas detalladas: " + ex.Message);
            }

            return reservasDetalladas;
        }
    }
}
