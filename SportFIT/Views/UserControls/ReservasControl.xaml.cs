using SportFIT.Controllers;
using SportFIT.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SportFIT.Views.UserControls
{
    public partial class ReservasControl : UserControl
    {
        private readonly PueblosController pueblosController;
        private readonly ReservasController reservasController;
        private string usuario;
        private string password;

        public ReservasControl(string connectionString, string usuario, string password)
        {
            InitializeComponent();

            this.usuario = usuario;
            this.password = password;

            // Crear una instancia del controlador de pueblos con la cadena de conexión adecuada
            pueblosController = new PueblosController(connectionString);

            // Crear una instancia del controlador de reservas con la cadena de conexión adecuada
            reservasController = new ReservasController(connectionString);

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
                // Cargar las reservas del pueblo seleccionado en el DataGrid
                CargarReservas(selectedPuebloId);

                //LLenar el ComboBox con los nombres de los usuarios, instalaciones
                CargarNombreUsuarios(selectedPuebloId);
                CargarNombreInstalaciones(selectedPuebloId);
            }
        }

        private void CargarReservas(int selectedPuebloId)
        {
            List<ReservaViewModel> reservas = reservasController.ObtenerReservas(selectedPuebloId);
            dataGridReservas.ItemsSource = reservas;
        }
        private void CargarNombreUsuarios(int selectedPuebloId)
        {
            var nombreUsuarios = reservasController.ObtenerUsuarios(selectedPuebloId);

            // Limpiar ComboBox antes de asignar nuevos elementos
            ComboBoxUser.ItemsSource = null;

            // Agregar texto fijo al inicio de la lista
            nombreUsuarios.Insert(0, "Usuario");

            // Asignar la lista como ItemsSource del ComboBox
            ComboBoxUser.ItemsSource = nombreUsuarios;

            // Seleccionar el primer elemento (el texto fijo)
            ComboBoxUser.SelectedIndex = 0;
        }
        private void ComboBoxUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Verificar si se ha seleccionado un usuario (ignorar el primer elemento)
            if (ComboBoxUser.SelectedIndex > 0)
            {
                // Aquí puedes acceder al usuario seleccionado
                string selectedUser = ComboBoxUser.SelectedItem.ToString();

                int selectedUserId = reservasController.ObtenerUserSelected(selectedUser);
            }
        }
        private void CargarNombreInstalaciones(int selectedPuebloId)
        {
            var nombreInstalaciones = reservasController.ObtenerInstalaciones(selectedPuebloId);

            // Limpiar ComboBox antes de asignar nuevos elementos
            ComboBoxInstalaciones.ItemsSource = null;

            // Agregar texto fijo al inicio de la lista
            nombreInstalaciones.Insert(0, "Instalaciones");

            // Asignar la lista como ItemsSource del ComboBox
            ComboBoxInstalaciones.ItemsSource = nombreInstalaciones;

            // Seleccionar el primer elemento (el texto fijo)
            ComboBoxInstalaciones.SelectedIndex = 0;
        }
        private void ComboBoxInstalaciones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Verificar si se ha seleccionado una instalacion (ignorar el primer elemento)
            if (ComboBoxInstalaciones.SelectedIndex > 0)
            {
                // Aquí puedes acceder ala instalacion seleccionada
                string selectedInstalacion = ComboBoxInstalaciones.SelectedItem.ToString();

                int selectedInstalacionId = reservasController.ObtenerInstalacionSelected(selectedInstalacion);
            }
        }
        private void btnAdd_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ComboBoxUser.SelectedIndex = 0;
            ComboBoxInstalaciones.SelectedIndex = 0;
            datePickerFecha.Text = "";
            TextBoxDuracion.Text = "";
            TextBoxHoraReserva.Text = "";
            txtError.Text = "";
            if (!popupAgregar.IsOpen)
            {
                popupAgregar.IsOpen = true;
            }
            else
            {
                popupAgregar.IsOpen = false;
            }
        }

        // Método para validar el formato de duración (ejemplo: "2:00")
        private bool IsValidDuration(string input)
        {
            TimeSpan duration;
            return TimeSpan.TryParse(input, out duration);
        }

        // Método para validar el formato de hora (ejemplo: "11:30")
        private bool IsValidTime(string input)
        {
            DateTime time;
            return DateTime.TryParseExact(input, "HH:mm", null, System.Globalization.DateTimeStyles.None, out time);
        }

        private void AddReserva_Click(object sender, RoutedEventArgs e)
        {
            txtError.Text = ""; // Limpiar mensaje de error

            // Validar que se hayan seleccionado todos los elementos necesarios
            if (ComboBoxUser.SelectedIndex <= 0 ||
                ComboBoxInstalaciones.SelectedIndex <= 0 ||
                string.IsNullOrWhiteSpace(TextBoxDuracion.Text) ||
                string.IsNullOrWhiteSpace(TextBoxHoraReserva.Text) ||
                datePickerFecha.SelectedDate == null)
            {
                txtError.Text = "Por favor, completa todos los campos antes de agregar la reserva.";
                return;
            }

            // Validar el formato de la duración
            if (!IsValidDuration(TextBoxDuracion.Text))
            {
                txtError.Text = "Por favor, introduce la duración en el formato correcto (ejemplo: '2:00').";
                return;
            }

            // Validar el formato de la hora de reserva
            if (!IsValidTime(TextBoxHoraReserva.Text))
            {
                txtError.Text = "Por favor, introduce la hora de reserva en el formato correcto (ejemplo: '11:30').";
                return;
            }

            // Obtener los valores seleccionados de los ComboBox
            string selectedUser = ComboBoxUser.SelectedItem.ToString();
            string selectedInstalacion = ComboBoxInstalaciones.SelectedItem.ToString();

            // Obtener los IDs correspondientes a partir de los nombres seleccionados
            int idUsuario = reservasController.ObtenerUserSelected(selectedUser);
            int idInstalacion = reservasController.ObtenerInstalacionSelected(selectedInstalacion);

            // Obtener la fecha de reserva y la hora de inicio
            DateTime fechaReserva = datePickerFecha.SelectedDate.Value;
            TimeSpan horaInicio = TimeSpan.Parse(TextBoxHoraReserva.Text);
            TimeSpan duracion = TimeSpan.Parse(TextBoxDuracion.Text);

            // Insertar la reserva utilizando el controlador de reservas
            bool reservaInsertada = reservasController.InsertarReserva(idUsuario, idInstalacion, fechaReserva, horaInicio, duracion);

            // Mostrar mensaje de éxito o error según el resultado de la inserción
            if (reservaInsertada)
            {
                // Cerrar el Popup después de agregar la reserva correctamente
                popupAgregar.IsOpen = false;

                // Volver a cargar las reservas del pueblo seleccionado en el DataGrid
                if (comboBoxPueblos.SelectedItem != null)
                {
                    string selectedPueblo = comboBoxPueblos.SelectedItem.ToString();
                    int selectedPuebloId = pueblosController.ObtenerPuebloSelected(selectedPueblo);
                    CargarReservas(selectedPuebloId);
                }
            }
            else
            {
                MessageBox.Show("Error al agregar la reserva. Por favor, intenta nuevamente.");
            }
        }

        private void DeleteReserva_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ReservaViewModel reserva)
            {
                // Obtener el ID de la reserva a eliminar
                int idReserva = reserva.IdReserva;

                // Mostrar un MessageBox de confirmación
                MessageBoxResult result = MessageBox.Show("¿Estás seguro de que deseas eliminar esta reserva?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // El usuario confirmó la eliminación, procede a eliminar la reserva
                    bool reservaEliminada = reservasController.EliminarReserva(idReserva);

                    if (reservaEliminada)
                    {
                        // Volver a cargar las reservas del pueblo seleccionado en el DataGrid
                        if (comboBoxPueblos.SelectedItem != null)
                        {
                            string selectedPueblo = comboBoxPueblos.SelectedItem.ToString();
                            int selectedPuebloId = pueblosController.ObtenerPuebloSelected(selectedPueblo);
                            CargarReservas(selectedPuebloId);
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
