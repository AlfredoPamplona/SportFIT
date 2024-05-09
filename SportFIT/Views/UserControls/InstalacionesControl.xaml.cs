using SportFIT.Controllers;
using SportFIT.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SportFIT.Views.UserControls
{
    public partial class InstalacionesControl : UserControl
    {
        private readonly PueblosController pueblosController;
        private readonly InstalacionesController instalacionesController;
        private string usuario;
        private string password;

        public InstalacionesControl(string connectionString, string usuario, string password)
        {
            InitializeComponent();

            this.usuario = usuario;
            this.password = password;

            // Crear una instancia del controlador de pueblos con la cadena de conexión adecuada
            pueblosController = new PueblosController(connectionString);

            // Crear una instancia del controlador de reservas con la cadena de conexión adecuada
            instalacionesController = new InstalacionesController(connectionString);

            // Llenar el ComboBox con los nombres de los pueblos
            CargarNombresPueblos();

            //Seleccionar el primer item por defecto
            comboBoxPueblos.SelectedIndex = 0;
        }
        private void CargarNombresPueblos()
        {
            //Recoger contraseña cifrada
            string hashedPassword = LoginController.HashPassword(password);

            // Obtener los nombres de los pueblos desde el controlador
            var nombresPueblos = pueblosController.ObtenerNombresPueblos(usuario, hashedPassword);

            // Asignar la lista de nombres como origen de datos del ComboBox
            comboBoxPueblos.ItemsSource = nombresPueblos;
        }

        private void comboBoxPueblos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxPueblos.SelectedItem != null)
            {
                // Obtener el nombre del pueblo seleccionado
                string selectedPueblo = comboBoxPueblos.SelectedItem.ToString();

                // Obtener el ID del pueblo seleccionado
                int selectedPuebloId = pueblosController.ObtenerPuebloSelected(selectedPueblo);
                // Cargar las instalaciones del pueblo seleccionado en el DataGrid
                CargarInstalaciones(selectedPuebloId);

            }
        }
        private void CargarInstalaciones(int selectedPuebloId)
        {
            List<InstalacionViewModel> instalaciones = instalacionesController.ObtenerInstalaciones(selectedPuebloId);
            dataGridInstalaciones.ItemsSource = instalaciones;
        }
        private void btnAddInstalaciones_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            txtError.Text = "";
            if (!popupAgregarInstalacion.IsOpen)
            {
                popupAgregarInstalacion.IsOpen = true;
            }
            else
            {
                popupAgregarInstalacion.IsOpen = false;
            }
        }
        private void btnAddInstalacion(object sender, RoutedEventArgs e)
        {
            txtError.Text = ""; // Limpiar mensaje de error

            // Validar que se hayan seleccionado todos los elementos necesarios
            if (string.IsNullOrWhiteSpace(TextBoxNombre.Text) ||
                string.IsNullOrWhiteSpace(TextBoxTipoInstalacion.Text) ||
                string.IsNullOrWhiteSpace(TextBoxDireccion.Text))
            {
                txtError.Text = "Por favor, completa todos los campos antes de agregar la reserva.";
                return;
            }
            try
            {
                // Obtener el nombre del pueblo seleccionado
                string selectedPueblo = comboBoxPueblos.SelectedItem.ToString();

                // Obtener el ID del pueblo seleccionado
                int selectedPuebloId = pueblosController.ObtenerPuebloSelected(selectedPueblo);

                // Obtener los datos de la instalación desde los TextBox
                string instalacion = TextBoxNombre.Text;
                string tipo = TextBoxTipoInstalacion.Text;
                string direccion = TextBoxDireccion.Text;

                // Insertar la nueva instalación utilizando el InstalacionesController
                bool instalacionInsertada = instalacionesController.InsertarInstalacion(selectedPuebloId, instalacion, direccion, tipo);

                if (instalacionInsertada)
                {
                    // Limpiar los campos después de una inserción exitosa
                    TextBoxNombre.Text = "";
                    TextBoxTipoInstalacion.Text = "";
                    TextBoxDireccion.Text = "";

                    // Cerrar el popup de agregar instalación
                    popupAgregarInstalacion.IsOpen = false;

                    // Actualizar el DataGrid con las instalaciones del pueblo seleccionado
                    CargarInstalaciones(selectedPuebloId);
                }
                else
                {
                    MessageBox.Show("Error al insertar la instalación. Por favor, intenta nuevamente.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al insertar la instalación: {ex.Message}");
            }
        }

        private void DeleteInstalacion_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is InstalacionViewModel instalacion)
            {
                // Obtener el ID de la instalacion a eliminar
                int idInstalacion = instalacion.IdInstalacion;

                // Mostrar un MessageBox de confirmación
                MessageBoxResult result = MessageBox.Show("¿Estás seguro de que deseas eliminar esta instalación?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // El usuario confirmó la eliminación, procede a eliminar la instalacion
                    bool instalacionEliminada = instalacionesController.EliminarInstalacion(idInstalacion);

                    if (instalacionEliminada)
                    {
                        // Volver a cargar las instalaciones del pueblo seleccionado en el DataGrid
                        if (comboBoxPueblos.SelectedItem != null)
                        {
                            string selectedPueblo = comboBoxPueblos.SelectedItem.ToString();
                            int selectedPuebloId = pueblosController.ObtenerPuebloSelected(selectedPueblo);
                            CargarInstalaciones(selectedPuebloId);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error al eliminar la instalacion. Por favor, intenta nuevamente.");
                    }
                }
            }
        }

    }
}