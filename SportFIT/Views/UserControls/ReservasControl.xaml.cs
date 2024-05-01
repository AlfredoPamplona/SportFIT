using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using SportFIT.Controllers;
using SportFIT.Models;

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

                //LLenar el ComboBox con los nombres de los usuarios, instalaciones y actividades
                CargarNombreUsuarios(selectedPuebloId);
                CargarNombreInstalaciones(selectedPuebloId);
                CargarNombreActividades();
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
        private void CargarNombreActividades()
        {
            var nombreActividades = reservasController.ObtenerActividad();

            // Limpiar ComboBox antes de asignar nuevos elementos
            ComboBoxActividades.ItemsSource = null;

            // Agregar texto fijo al inicio de la lista
            nombreActividades.Insert(0, "Actividades");

            // Asignar la lista como ItemsSource del ComboBox
            ComboBoxActividades.ItemsSource = nombreActividades;

            // Seleccionar el primer elemento (el texto fijo)
            ComboBoxActividades.SelectedIndex = 0;
        }
        private void ComboBoxActividades_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Verificar si se ha seleccionado una actividad (ignorar el primer elemento)
            if (ComboBoxActividades.SelectedIndex > 0)
            {
                // Aquí puedes acceder a la actividad seleccionada
                string selectedActividad = ComboBoxActividades.SelectedItem.ToString();

                int selectedActividadId = reservasController.ObtenerActividadSelected(selectedActividad);
            }
        }
        private void btnAdd_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ComboBoxActividades.SelectedIndex = 0;
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

        private void btnAddReserva(object sender, RoutedEventArgs e)
        {
            txtError.Text = "";

            // Check if ComboBox selections are valid
            if (ComboBoxUser.SelectedIndex <= 0 ||
                ComboBoxInstalaciones.SelectedIndex <= 0 ||
                ComboBoxActividades.SelectedIndex <= 0)
            {
                txtError.Text = "";
                txtError.Text = "Por favor, selecciona todas las opciones antes de agregar la reserva.";
            }else if (string.IsNullOrWhiteSpace(TextBoxDuracion.Text) ||
                string.IsNullOrWhiteSpace(TextBoxHoraReserva.Text) ||
                datePickerFecha.SelectedDate == null)
            {
                txtError.Text = "";
                txtError.Text = "Por favor, completa todos los campos antes de agregar la reserva.";
            }else if (!string.IsNullOrWhiteSpace(TextBoxDuracion.Text) &&
                !IsValidDuration(TextBoxDuracion.Text))
            {
                txtError.Text = "";
                txtError.Text = "Por favor, introduce la duración en el formato correcto (ejemplo: '2:00').";
            }else if (!string.IsNullOrWhiteSpace(TextBoxHoraReserva.Text) &&
                !IsValidTime(TextBoxHoraReserva.Text))
            {
                txtError.Text = "";
                txtError.Text = "Por favor, introduce la hora de reserva en el formato correcto (ejemplo: '11:30').";           
            }
            else
            {
                txtError.Text = "";
                MessageBox.Show("Todo Correcto");
            }
            // Reset fields after adding the reservation (if necessary)
            // ComboBoxUser.SelectedIndex = 0;
            // ComboBoxInstalaciones.SelectedIndex = 0;
            // TextBoxDuracion.Text = string.Empty;
            // TextBoxHoraReserva.Text = string.Empty;
        }

    }
}
