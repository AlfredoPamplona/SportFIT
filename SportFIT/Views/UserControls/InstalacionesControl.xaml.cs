using SportFIT.Controllers;
using SportFIT.Models;
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
            MessageBox.Show("CLICK");
            //txtError.Text = ""; // Limpiar mensaje de error

            //// Validar que se hayan seleccionado todos los elementos necesarios
            //if (ComboBoxUser.SelectedIndex <= 0 ||
            //    ComboBoxInstalaciones.SelectedIndex <= 0 ||
            //    string.IsNullOrWhiteSpace(TextBoxDuracion.Text) ||
            //    string.IsNullOrWhiteSpace(TextBoxHoraReserva.Text) ||
            //    datePickerFecha.SelectedDate == null)
            //{
            //    txtError.Text = "Por favor, completa todos los campos antes de agregar la reserva.";
            //    return;
            //}

            //// Validar el formato de la duración
            //if (!IsValidDuration(TextBoxDuracion.Text))
            //{
            //    txtError.Text = "Por favor, introduce la duración en el formato correcto (ejemplo: '2:00').";
            //    return;
            //}

            //// Validar el formato de la hora de reserva
            //if (!IsValidTime(TextBoxHoraReserva.Text))
            //{
            //    txtError.Text = "Por favor, introduce la hora de reserva en el formato correcto (ejemplo: '11:30').";
            //    return;
            //}

            //// Obtener los valores seleccionados de los ComboBox
            //string selectedUser = ComboBoxUser.SelectedItem.ToString();
            //string selectedInstalacion = ComboBoxInstalaciones.SelectedItem.ToString();

            //// Obtener los IDs correspondientes a partir de los nombres seleccionados
            //int idUsuario = reservasController.ObtenerUserSelected(selectedUser);
            //int idInstalacion = reservasController.ObtenerInstalacionSelected(selectedInstalacion);

            //// Obtener la fecha de reserva y la hora de inicio
            //DateTime fechaReserva = datePickerFecha.SelectedDate.Value;
            //TimeSpan horaInicio = TimeSpan.Parse(TextBoxHoraReserva.Text);
            //int duracion = Convert.ToInt32(TextBoxDuracion.Text);

            //// Insertar la reserva utilizando el controlador de reservas
            //bool reservaInsertada = reservasController.InsertarReserva(idUsuario, idInstalacion, fechaReserva, horaInicio, duracion);

            //// Mostrar mensaje de éxito o error según el resultado de la inserción
            //if (reservaInsertada)
            //{
            //    MessageBox.Show("Reserva agregada correctamente.");

            //    // Volver a cargar las reservas del pueblo seleccionado en el DataGrid
            //    if (comboBoxPueblos.SelectedItem != null)
            //    {
            //        string selectedPueblo = comboBoxPueblos.SelectedItem.ToString();
            //        int selectedPuebloId = pueblosController.ObtenerPuebloSelected(selectedPueblo);
            //        CargarReservas(selectedPuebloId);
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Error al agregar la reserva. Por favor, intenta nuevamente.");
            //}
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
                        MessageBox.Show("Error al eliminar la reserva. Por favor, intenta nuevamente.");
                    }
                }
            }
        }

    }
}