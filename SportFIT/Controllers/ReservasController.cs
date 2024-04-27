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
        public List<string> ObtenerUsuarios()
        {
            List<string> nombresUsuarios = new List<string>();
            // Consulta SQL para obtener los pueblos
            string query = "SELECT nombre_usuario FROM usuario";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
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



    }
}
