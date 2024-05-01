using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using SportFIT.Controllers;
using SportFIT.Views;

namespace SportFIT.Views
{
    // Clase parcial que representa la ventana de inicio de sesión
    public partial class LoginView : Window
    {
        private LoginController _viewModel; // Instancia del controlador de inicio de sesión

        // Constructor de la clase LoginView
        public LoginView()
        {
            InitializeComponent();
            _viewModel = new LoginController(); // Crea una nueva instancia del LoginController
            DataContext = _viewModel; // Establece el contexto de datos de la ventana a la instancia del LoginController
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _viewModel.Usuario = txtUsuario.Text; // Obtiene el nombre de usuario desde el TextBox txtUsuario y lo asigna al controlador
                _viewModel.Password = txtPassword.Password; // Obtiene la contraseña desde el PasswordBox txtPassword y la asigna al controlador

                // Verifica si el nombre de usuario o la contraseña están en blanco o nulos
                if (string.IsNullOrWhiteSpace(_viewModel.Usuario) || string.IsNullOrWhiteSpace(_viewModel.Password))
                {
                    _viewModel.ErrorMessage = "Por favor ingresa usuario y contraseña.";
                    return; // Sale del método sin continuar con el proceso de inicio de sesión
                }

                bool loginSuccessful = _viewModel.Login(); // Intenta iniciar sesion utilizando el controlador

                if (loginSuccessful)
                {
                    // Obtener el rol del usuario autenticado
                    RoleController roleController = new RoleController(ConfigurationManager.ConnectionStrings["DBContextSportFIT"].ConnectionString);
                    string userRole = roleController.GetUserRole(_viewModel.Usuario);

                    // Crear una instancia de MainView pasando el rol del usuario, nombre de usuario y contraseña
                    var mainView = new MainView(userRole, _viewModel.Usuario, _viewModel.Password);
                    mainView.Title = $"SportFIT | {_viewModel.Usuario}";
                    mainView.Show();

                    this.Close(); // Cierra la ventana de inicio de sesión
                }
                else
                {
                    _viewModel.ErrorMessage = _viewModel.ErrorMessage; // Establece un mensaje de error en el controlador si falla el inicio de sesion
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al iniciar sesión: {ex.Message}"); // Muestra un mensaje de error en caso de excepción
            }
        }
    }
}
