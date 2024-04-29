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
                string query = @"SELECT r.id_reserva, u.nombre_usuario, r.fecha_reserva, r.hora_inicio, r.duracion, a.nombre_actividad, i.nombre_instalacion
                                FROM Reserva r
                                INNER JOIN Usuario u ON r.id_usuario = u.id_usuario
                                INNER JOIN Instalacion i ON r.id_instalacion = i.id_instalacion
                                INNER JOIN Actividad a ON i.id_instalacion = a.id_instalacion
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
                        Duracion = Convert.ToInt32(reader["duracion"]),
                        NombreActividad = reader["nombre_actividad"].ToString(),
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
        public int ObtenerActividadSelected(string instalacion)
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

        public int ObtenerInstalacionSelected(string actividad)
        {
            int idActividad = 1; //Modificar variable
            string query = @"SELECT id_actividad FROM actividad where nombre_actividad = @ActividadNombre";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ActividadNombre", actividad);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        idActividad = Convert.ToInt32(reader["id_actividad"]);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener el ID de la actividad: " + ex.Message);
            }
            return idActividad;
        }
        public List<string> ObtenerActividad()
        {
            List<string> nombresActividades = new List<string>();
            // Consulta SQL para obtener las instalaciones
            string query = @"SELECT nombre_actividad FROM actividad";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string nombreActividad = reader["nombre_actividad"].ToString();
                        nombresActividades.Add(nombreActividad);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener nombres de actividades: " + ex.Message);
            }

            return nombresActividades;
        }
    }
}
