using SportFIT.Models;
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
        public List<ReservaViewModel> ObtenerReservas(int selectedPuebloId)
        {
            List<ReservaViewModel> reservas = new List<ReservaViewModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT r.id_reserva, u.nombre_usuario, r.fecha_reserva, r.hora_inicio, r.duracion, i.nombre_instalacion
                                FROM Reserva r
                                INNER JOIN Usuario u ON r.id_usuario = u.id_usuario
                                INNER JOIN Instalacion i ON r.id_instalacion = i.id_instalacion
                                WHERE i.id_pueblo = @selectedPuebloId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@selectedPuebloId", selectedPuebloId);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    // Crear objetos Reserva y llenarlos con los datos del lector
                    ReservaViewModel reserva = new ReservaViewModel
                    {
                        IdReserva = Convert.ToInt32(reader["id_reserva"]),
                        NombreUsuario = reader["nombre_usuario"].ToString(),
                        FechaReserva = Convert.ToDateTime(reader["fecha_reserva"]),
                        HoraInicio = TimeSpan.Parse(reader["hora_inicio"].ToString()),
                        Duracion = TimeSpan.Parse(reader["duracion"].ToString()),
                        NombreInstalacion = reader["nombre_instalacion"].ToString()
                    };

                    reservas.Add(reserva);
                }
            }

            return reservas;
        }

        public int ObtenerUserSelected(string user)
        {
            int idUser = 1; //Modificar variable
            string query = @"SELECT id_usuario FROM usuario where nombre_usuario = @UsuarioNombre";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@PuebloNombre", user);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        idUser = Convert.ToInt32(reader["id_usuario"]);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener el ID del usuario: " + ex.Message);
            }
            return idUser;
        }
        public List<string> ObtenerUsuarios(int selectedPuebloId)
        {
            List<string> nombresUsuarios = new List<string>();
            // Consulta SQL para obtener los pueblos
            string query = @"Select u.nombre_usuario " +
                            "From Usuario u inner join Usuario_Pueblo up on u.id_usuario = up.id_usuario " +
                            "where up.id_pueblo = @PuebloId";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@PuebloId", selectedPuebloId);
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string nombreUsuario = reader["nombre_usuario"].ToString();
                        nombresUsuarios.Add(nombreUsuario);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener nombres de usuarios: " + ex.Message);
            }

            return nombresUsuarios;
        }
        public int ObtenerInstalacionSelected(string instalacion)
        {
            int idInstalacion = 1; //Modificar variable
            string query = @"SELECT id_instalacion FROM instalacion where nombre_instalacion = @InstalacionNombre";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@InstalacionNombre", instalacion);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        idInstalacion = Convert.ToInt32(reader["id_instalacion"]);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener el ID de la instalacion: " + ex.Message);
            }
            return idInstalacion;
        }
        public List<string> ObtenerInstalaciones(int selectedPuebloId)
        {
            List<string> nombresInstalaciones = new List<string>();
            // Consulta SQL para obtener las instalaciones
            string query = @"SELECT nombre_instalacion FROM instalacion where id_pueblo= @PuebloId";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@PuebloId", selectedPuebloId);
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string nombreInstalacion = reader["nombre_instalacion"].ToString();
                        nombresInstalaciones.Add(nombreInstalacion);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener nombres de instalaciones: " + ex.Message);
            }

            return nombresInstalaciones;
        }

        public string ComprobarDisponibilidad(int idInstalacion, DateTime fechaReserva, TimeSpan horaInicio, TimeSpan duracion, int? idReservaExcluir = null)
        {
            string disponibilidad = "Disponible";

            try
            {
                // Calcular la hora de finalización de la reserva que quieres verificar
                TimeSpan horaFin = horaInicio.Add(duracion);

                // Consulta para verificar la disponibilidad
                string query = @"SELECT COUNT(*) FROM Reserva
                         WHERE id_instalacion = @idInstalacion
                         AND fecha_reserva = @fechaReserva
                         AND (
                             (@horaInicio >= hora_inicio AND @horaInicio < DATEADD(MINUTE, DATEDIFF(MINUTE, '00:00:00', duracion), hora_inicio))
                             OR
                             (@horaFin > hora_inicio AND @horaFin <= DATEADD(MINUTE, DATEDIFF(MINUTE, '00:00:00', duracion), hora_inicio)) 
                             OR
                             (@horaInicio <= hora_inicio AND @horaFin >= DATEADD(MINUTE, DATEDIFF(MINUTE, '00:00:00', duracion), hora_inicio)) 
                         )";

                if (idReservaExcluir.HasValue)
                {
                    query += " AND id_reserva <> @idReservaExcluir";
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@idInstalacion", idInstalacion);
                    command.Parameters.AddWithValue("@fechaReserva", fechaReserva);
                    command.Parameters.AddWithValue("@horaInicio", horaInicio);
                    command.Parameters.AddWithValue("@horaFin", horaFin);

                    if (idReservaExcluir.HasValue)
                    {
                        command.Parameters.AddWithValue("@idReservaExcluir", idReservaExcluir.Value);
                    }

                    connection.Open();

                    int reservaSuperpuesta = (int)command.ExecuteScalar();

                    if (reservaSuperpuesta > 0)
                    {
                        disponibilidad = "No disponible";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al verificar disponibilidad: " + ex.Message);
                disponibilidad = "Error";
            }

            return disponibilidad;
        }

        public bool InsertarReserva(int idUsuario, int idInstalacion, DateTime fechaReserva, TimeSpan horaInicio, TimeSpan duracion)
        {
            try
            {
                // Consulta SQL para insertar una reserva en la tabla Reserva
                string query = @"INSERT INTO Reserva (fecha_reserva, hora_inicio, duracion, id_usuario, id_instalacion)
                                VALUES (@fechaReserva, @horaInicio, @duracion, @idUsuario, @idInstalacion)";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@fechaReserva", fechaReserva);
                    command.Parameters.AddWithValue("@horaInicio", horaInicio);
                    command.Parameters.AddWithValue("@duracion", duracion);
                    command.Parameters.AddWithValue("@idUsuario", idUsuario);
                    command.Parameters.AddWithValue("@idInstalacion", idInstalacion);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    // Verificar si se insertó correctamente (al menos una fila afectada)
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al insertar reserva: " + ex.Message);
                return false;
            }
        }
        public bool ActualizarReserva(ReservaViewModel reservaActualizada)
        {
            try
            {
                // Consulta SQL para actualizar la reserva en la tabla Reserva
                string query = @"UPDATE Reserva
                                SET id_instalacion = (SELECT id_instalacion FROM Instalacion WHERE nombre_instalacion = @nombreInstalacion),
                                    fecha_reserva = @fechaReserva,
                                    hora_inicio = @horaInicio,
                                    duracion = @duracion
                                WHERE id_reserva = @idReserva";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@nombreInstalacion", reservaActualizada.NombreInstalacion);
                    command.Parameters.AddWithValue("@fechaReserva", reservaActualizada.FechaReserva);
                    command.Parameters.AddWithValue("@horaInicio", reservaActualizada.HoraInicio);
                    command.Parameters.AddWithValue("@duracion", reservaActualizada.Duracion);
                    command.Parameters.AddWithValue("@idReserva", reservaActualizada.IdReserva);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    // Verificar si se actualizó correctamente (al menos una fila afectada)
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al actualizar reserva: " + ex.Message);
                return false;
            }
        }

        public bool EliminarReserva(int idReserva)
        {
            try
            {
                string query = @"DELETE FROM Reserva WHERE id_reserva = @idReserva";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@idReserva", idReserva);

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
