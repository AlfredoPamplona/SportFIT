using System;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace SportFIT.Controllers
{
    // Clase que maneja la lógica de inicio de sesión y notifica cambios en las propiedades
    public class LoginController : INotifyPropertyChanged
    {
        private string _usuario;
        public string Usuario
        {
            get { return _usuario; }
            set
            {
                _usuario = value;
                OnPropertyChanged(nameof(Usuario)); // Notificar cambios en la propiedad Usuario
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password)); // Notificar cambios en la propiedad Password
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage)); // Notificar cambios en la propiedad ErrorMessage
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Método protegido para invocar el evento PropertyChanged
        protected virtual void OnPropertyChanged(string propertyName)
        {
            // Verificar si PropertyChanged no es nulo, luego invocar el evento
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Método para cifrar la contraseña
        public static string HashPassword(string password)
        {
            using (var sha256 = new SHA256Managed())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        // Método que intenta autenticar al usuario
        public bool Login()
        {
            try
            {
                string hashedPassword = HashPassword(Password);

                // Obtener la cadena de conexión desde la configuración de la aplicación
                string connectionString = ConfigurationManager.ConnectionStrings["DBContextSportFIT"].ConnectionString;

                // Establecer una conexión a la base de datos utilizando SqlConnection
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Abrir la conexión a la base de datos
                    connection.Open();

                    // Definir la consulta para contar las filas que coinciden con el usuario y la contraseña cifrada
                    string query = "SELECT COUNT(*) FROM Usuario WHERE nombre_usuario = @Usuario AND password = @Password";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Usuario", Usuario);
                        command.Parameters.AddWithValue("@Password", hashedPassword); // Utilizar la contraseña cifrada

                        // Ejecutar la consulta y obtener el resultado (número de filas)
                        int count = (int)command.ExecuteScalar();

                        // Verificar si se encontró al menos una fila (usuario autenticado)
                        if (count > 0)
                        {
                            return true;
                        }
                        else
                        {
                            // Usuario o contraseña incorrectos, establecer mensaje de error
                            ErrorMessage = "Usuario o contraseña incorrectos.";
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Capturar excepciones y establecer el mensaje de error correspondiente
                ErrorMessage = $"Error al iniciar sesión: {ex.Message}";
                return false; // Error al intentar iniciar sesión
            }
        }
    }
}
