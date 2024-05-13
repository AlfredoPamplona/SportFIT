using SportFIT.Controllers;
using SportFIT.Models;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace SportFIT.Views.UserControls
{
    /// <summary>
    /// Lógica de interacción para AjustesControl.xaml
    /// </summary>
    public partial class AjustesControl : UserControl
    {
        private readonly AjustesController ajustesController;
        private readonly PueblosController pueblosController;
        private List<string> nombresPueblos;

        private string usuario;
        private string password;
        public AjustesControl(string connectionString, string usuario, string password)
        {
            InitializeComponent();

            this.usuario = usuario;
            this.password = password;
            
            ajustesController = new AjustesController(connectionString);
            pueblosController = new PueblosController(connectionString);
            
            Loaded += AjustesControl_Loaded;
            
            CargarNombresRoles();
            CargarUsuarios();
        }
        private void CargarNombresRoles()
        {

            var nombresRoles = ajustesController.ObtenerRol();
            ComboBoxRoles.ItemsSource = null;

            // Agregar texto fijo al inicio de la lista
            nombresRoles.Insert(0, "Rol");

            // Asignar la lista como ItemsSource del ComboBox
            ComboBoxRoles.ItemsSource = nombresRoles;

            // Seleccionar el primer elemento (el texto fijo)
            ComboBoxRoles.SelectedIndex = 0;
        }
        private void ComboBoxRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxRoles.SelectedIndex > 0)
            {
                string selectedInstalacion = ComboBoxRoles.SelectedItem.ToString();

                int selectedInstalacionId = ajustesController.ObtenerRolSelected(selectedInstalacion);
            }
        }

        private void AjustesControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Recoger contraseña cifrada
            string hashedPassword = LoginController.HashPassword(password);

            // Obtener los nombres de los pueblos desde el controlador
            nombresPueblos = pueblosController.ObtenerNombresPueblos(usuario, hashedPassword);

            // Limpiar el StackPanel antes de añadir nuevos CheckBoxes
            stackPanelPueblos.Children.Clear();

            // Crear y configurar CheckBoxes para cada nombre de pueblo
            foreach (string nombrePueblo in nombresPueblos)
            {
                CheckBox checkBoxPueblo = new CheckBox
                {
                    Content = nombrePueblo,
                    Foreground = FindResource("NavegationColor") as System.Windows.Media.SolidColorBrush,
                    Margin = new System.Windows.Thickness(10, 5, 10, 5)
                };

                // Añadir CheckBox al StackPanel
                stackPanelPueblos.Children.Add(checkBoxPueblo);
            }
        }
        private void CargarUsuarios()
        {
            List<AjustesViewModel> usuarios = ajustesController.ObtenerUsuarios();
            dataGridAjustes.ItemsSource = usuarios;
        }

        private void DeleteUsuario_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is AjustesViewModel usuario)
            {
                // Obtener el ID del usuario a eliminar
                int idUsuario = usuario.IdUsuario;

                // Mostrar un MessageBox de confirmación
                MessageBoxResult result = MessageBox.Show("¿Estás seguro de que deseas eliminar este usuario?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // El usuario confirmó la eliminación, procede a eliminar el usuario
                    bool usuarioEliminado = ajustesController.EliminarUsuario(idUsuario);

                    if (usuarioEliminado)
                    {
                        MessageBox.Show("Usuario eliminado correctamente.");
                        CargarUsuarios(); // Volver a cargar la lista de usuarios en el DataGrid
                    }
                    else
                    {
                        MessageBox.Show("Error al eliminar el usuario. Por favor, intenta nuevamente.");
                    }
                }
            }
        }

        private void btnPopUpAddUsuarios_Click(object sender, RoutedEventArgs e)
        {
            txtError.Text = "";
            if (!popupAgregarUsuario.IsOpen)
            {
                popupAgregarUsuario.IsOpen = true;
            }
            else
            {
                popupAgregarUsuario.IsOpen = false;
            }
        }

        private void btnAddUsuarios(object sender, RoutedEventArgs e)
        {
            txtError.Text = "";

            // Obtener datos del nuevo usuario desde los controles en la interfaz de usuario
            string nombre = TextBoxUsuario.Text.Trim();
            string apellidos = TextBoxApellidos.Text.Trim();
            string email = TextBoxEmail.Text.Trim();
            string password = new NetworkCredential(string.Empty, passwordBox.SecurePassword).Password.Trim(); // Obtener contraseña como string

            // Validar campos obligatorios
            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(email) || passwordBox.SecurePassword.Length == 0)
            {
                txtError.Text = "Por favor, completa todos los campos obligatorios.";
                return;
            }

            // Obtener el rol seleccionado (ComboBox)
            string rol = ComboBoxRoles.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(rol) || rol.Equals("Rol"))
            {
                txtError.Text = "Por favor, selecciona un rol.";
                return;
            }

            // Obtener los pueblos seleccionados (CheckBoxes)
            List<string> pueblosSeleccionados = new List<string>();
            foreach (var child in stackPanelPueblos.Children)
            {
                if (child is CheckBox checkBox && checkBox.IsChecked == true)
                {
                    pueblosSeleccionados.Add(checkBox.Content.ToString());
                }
            }

            // Validar que al menos se haya seleccionado un pueblo
            if (pueblosSeleccionados.Count == 0)
            {
                txtError.Text = "Por favor, selecciona al menos un pueblo.";
                return;
            }

            // Insertar el nuevo usuario en la base de datos
            bool usuarioInsertado = ajustesController.InsertarUsuario(nombre, apellidos, email, password, rol, pueblosSeleccionados);
            if (usuarioInsertado)
            {
                MessageBox.Show("Usuario insertado correctamente.");
                CargarUsuarios(); // Volver a cargar la lista de usuarios en el DataGrid
                popupAgregarUsuario.IsOpen = false; // Cerrar el popup de agregar usuario
            }
            else
            {
                MessageBox.Show("Error al insertar el usuario. Por favor, intenta nuevamente.");
            }
        }
    }
}
    
