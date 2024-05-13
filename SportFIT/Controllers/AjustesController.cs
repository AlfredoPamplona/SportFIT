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

        public bool InsertarUsuario(string nombre, string apellidos, string email, string password, string rol, List<string> pueblos)
        {
            string hashedPassword = LoginController.HashPassword(password);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Insertar el nuevo usuario en la tabla Usuario
                    string insertUsuarioQuery = "INSERT INTO Usuario (nombre_usuario, apellidos_usuario, email, password, id_rol) VALUES (@Nombre, @Apellidos, @Email, @Password, @IdRol); SELECT SCOPE_IDENTITY()";
                    SqlCommand insertUsuarioCommand = new SqlCommand(insertUsuarioQuery, connection);
                    insertUsuarioCommand.Parameters.AddWithValue("@Nombre", nombre);
                    insertUsuarioCommand.Parameters.AddWithValue("@Apellidos", string.IsNullOrEmpty(apellidos) ? DBNull.Value : (object)apellidos);
                    insertUsuarioCommand.Parameters.AddWithValue("@Email", email);
                    insertUsuarioCommand.Parameters.AddWithValue("@Password", hashedPassword); // Almacenar el hash de la contraseña
                    insertUsuarioCommand.Parameters.AddWithValue("@IdRol", ObtenerIdRol(rol)); // Obtener el ID del rol

                    int idUsuario = Convert.ToInt32(insertUsuarioCommand.ExecuteScalar()); // Obtener el ID del nuevo usuario insertado

                    // Insertar los pueblos asociados al nuevo usuario en la tabla Usuario_Pueblo
                    foreach (string pueblo in pueblos)
                    {
                        int idPueblo = ObtenerIdPueblo(pueblo); // Obtener el ID del pueblo

                        if (idPueblo > 0)
                        {
                            string insertUsuarioPuebloQuery = "INSERT INTO Usuario_Pueblo (id_usuario, id_pueblo) VALUES (@IdUsuario, @IdPueblo)";
                            SqlCommand insertUsuarioPuebloCommand = new SqlCommand(insertUsuarioPuebloQuery, connection);
                            insertUsuarioPuebloCommand.Parameters.AddWithValue("@IdUsuario", idUsuario);
                            insertUsuarioPuebloCommand.Parameters.AddWithValue("@IdPueblo", idPueblo);
                            insertUsuarioPuebloCommand.ExecuteNonQuery(); // Ejecutar la inserción del pueblo asociado al usuario
                        }
                        else
                        {
                            Console.WriteLine("No se encontró el ID del pueblo asociado: " + pueblo);
                            return false;
                        }
                    }

                    return true; // Indicar que la inserción fue exitosa
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al insertar usuario: " + ex.Message);
                return false;
            }
        }

        private int ObtenerIdRol(string rol)
        {
            int idRol = 0;
            string query = "SELECT id_rol FROM Rol WHERE nombre_rol = @RolNombre";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@RolNombre", rol);
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    idRol = Convert.ToInt32(result);
                }
            }
            return idRol;
        }

        private int ObtenerIdPueblo(string pueblo)
        {
            int idPueblo = 0;
            string query = "SELECT id_pueblo FROM Pueblo WHERE nombre_pueblo = @PuebloNombre";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@PuebloNombre", pueblo);
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    idPueblo = Convert.ToInt32(result);
                }
            }
            return idPueblo;
        }

        public bool EliminarUsuario(int idUsuario)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Eliminar el usuario de la tabla Usuario_Pueblo
                    string deleteUsuarioPuebloQuery = "DELETE FROM Usuario_Pueblo WHERE id_usuario = @IdUsuario";
                    SqlCommand deleteUsuarioPuebloCommand = new SqlCommand(deleteUsuarioPuebloQuery, connection);
                    deleteUsuarioPuebloCommand.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    deleteUsuarioPuebloCommand.ExecuteNonQuery();

                    // Eliminar el usuario de la tabla Usuario
                    string deleteUsuarioQuery = "DELETE FROM Usuario WHERE id_usuario = @IdUsuario";
                    SqlCommand deleteUsuarioCommand = new SqlCommand(deleteUsuarioQuery, connection);
                    deleteUsuarioCommand.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    int rowsAffected = deleteUsuarioCommand.ExecuteNonQuery();

                    // Verificar si se eliminó correctamente (al menos una fila afectada en Usuario)
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al eliminar usuario: " + ex.Message);
                return false;
            }
        }
    }
}
