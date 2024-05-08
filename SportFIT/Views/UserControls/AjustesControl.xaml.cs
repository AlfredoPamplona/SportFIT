using SportFIT.Controllers;
using SportFIT.Models;
using System.Collections.Generic;
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
        private void btnAddUsuarios_Click(object sender, RoutedEventArgs e)
        {
            //txtError.Text = "";
            if (!popupAgregarUsuario.IsOpen)
            {
                popupAgregarUsuario.IsOpen = true;
            }
            else
            {
                popupAgregarUsuario.IsOpen = false;
            }
        }
    }
}
    
