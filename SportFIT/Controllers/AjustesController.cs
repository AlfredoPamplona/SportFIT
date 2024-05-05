using SportFIT.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SportFIT.Controllers
{
    public class AjustesController
    {
        private readonly string connectionString;

        public AjustesController(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public List<string> ObtenerRol()
        {
            List<string> nombresRoles = new List<string>();
            string query = @"SELECT nombre_rol FROM Rol";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string nombreRol = reader["nombre_rol"].ToString();
                        nombresRoles.Add(nombreRol);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener nombres de rol: " + ex.Message);
            }

            return nombresRoles;
        }

        public int ObtenerRolSelected(string rol)
        {
            int idRol = 1; //Modificar variable
            string query = @"SELECT id_rol FROM rol where nombre_rol = @RolNombre";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@RolNombre", rol);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        idRol = Convert.ToInt32(reader["id_rol"]);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener el ID del rol: " + ex.Message);
            }
            return idRol;
        }
        public List<AjustesViewModel> ObtenerUsuarios()
        {
            List<AjustesViewModel> usuarios = new List<AjustesViewModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT u.id_usuario, u.nombre_usuario, u.apellidos_usuario, u.email, r.nombre_rol " +
                                "FROM Usuario u INNER JOIN Rol r ON u.id_rol = r.id_rol";

                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    AjustesViewModel usuario = new AjustesViewModel
                    {
                        IdUsuario = Convert.ToInt32(reader["id_usuario"]),
                        NombreUsuario = reader["nombre_usuario"].ToString(),
                        EmailUsuario = reader["email"].ToString(),
                        RolUsuario = reader["nombre_rol"].ToString()
                    };

                    // Verificar si apellidos_usuario es nulo
                    if (reader["apellidos_usuario"] != DBNull.Value)
                    {
                        usuario.ApellidoUsuario = reader["apellidos_usuario"].ToString();
                    }
                    else
                    {
                        usuario.ApellidoUsuario = "---";
                    }

                    usuarios.Add(usuario);
                }
            }

            return usuarios;
        }
    }
}
