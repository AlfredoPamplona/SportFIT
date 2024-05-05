using SportFIT.Controllers;
using SportFIT.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Controls;

namespace SportFIT.Views.UserControls
{
    public partial class ReservasControl : UserControl
    {
        private readonly PueblosController pueblosController;
        private readonly ReservasController reservasController;
        private readonly RoleController roleController;
        private string usuario;
        private string password;
        private bool editingMode = false; // Variable para controlar si estamos en modo edición

        public ReservasControl(string connectionString, string usuario, string password)
        {
            InitializeComponent();

            this.usuario = usuario;
            this.password = password;

            // Crear una instancia del controlador de pueblos con la cadena de conexión adecuada
            pueblosController = new PueblosController(connectionString);

            // Crear una instancia del controlador de reservas con la cadena de conexión adecuada
            reservasController = new ReservasController(connectionString);

            // Crear una instancia del controlador de roles con la cadena de conexión adecuada
            roleController = new RoleController(connectionString);

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

                //LLenar el ComboBox con las instalaciones
                CargarNombreInstalaciones(selectedPuebloId);
            }
        }

        private void CargarReservas(int selectedPuebloId)
        {
            List<ReservaViewModel> reservas = reservasController.ObtenerReservas(selectedPuebloId);
            dataGridReservas.ItemsSource = reservas;
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
            ComboBoxInstalaciones.SelectedIndex = 0;
            datePickerFecha.Text = "";
            TextBoxDuracion.Text = "";
            TextBoxHoraReserva.Text = "";
            txtError.Text = "";
            btnEditAdd.Content = "Añadir";
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
            return DateTime.TryParseExact(input, "H:mm", null, System.Globalization.DateTimeStyles.None, out time);
        }

        private void AddReserva_Click(object sender, RoutedEventArgs e)
        {
            txtError.Text = ""; // Limpiar mensaje de error

            // Validar que se hayan seleccionado todos los elementos necesarios
            if (ComboBoxInstalaciones.SelectedIndex <= 0 ||
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

            if (editingMode)
            {
                editingMode = false;
                if (dataGridReservas.SelectedItem != null && dataGridReservas.SelectedItem is ReservaViewModel reserva)
                {
                    // Obtener datos necesarios para la reserva actualizada
                    string selectedInstalacion = ComboBoxInstalaciones.SelectedItem.ToString();
                    int idInstalacion = reservasController.ObtenerInstalacionSelected(selectedInstalacion);
                    DateTime fechaReserva = datePickerFecha.SelectedDate.Value;
                    TimeSpan horaInicio = TimeSpan.Parse(TextBoxHoraReserva.Text);
                    TimeSpan duracion = TimeSpan.Parse(TextBoxDuracion.Text);

                    // Verificar disponibilidad utilizando el método ComprobarDisponibilidad
                    string disponibilidad = reservasController.ComprobarDisponibilidad(idInstalacion, fechaReserva, horaInicio, duracion);
                    
                    // Si estamos editando la misma reserva, entonces debemos ignorar esa reserva al comprobar disponibilidad
                    if (reserva.NombreInstalacion == selectedInstalacion && reserva.FechaReserva == fechaReserva
                        && reserva.HoraInicio == horaInicio)
                    {
                        disponibilidad = "Disponible"; // Ignorar esta reserva al verificar disponibilidad
                    }
                    if (disponibilidad == "Disponible")
                    {
                        // La instalación está disponible, proceder con la actualización de la reserva
                        // Crear una instancia de ReservaViewModel con los datos actualizados
                        ReservaViewModel reservaActualizada = new ReservaViewModel
                        {
                            IdReserva = reserva.IdReserva,
                            NombreInstalacion = selectedInstalacion,
                            FechaReserva = fechaReserva,
                            HoraInicio = horaInicio,
                            Duracion = duracion
                        };

                        // Intentar actualizar la reserva en la base de datos
                        bool reservaModificada = reservasController.ActualizarReserva(reservaActualizada);

                        if (reservaModificada)
                        {
                            MessageBox.Show("Reserva actualizada correctamente.");
                            // Recargar las reservas del pueblo seleccionado en el DataGrid
                            if (comboBoxPueblos.SelectedItem != null)
                            {
                                string selectedPueblo = comboBoxPueblos.SelectedItem.ToString();
                                int selectedPuebloId = pueblosController.ObtenerPuebloSelected(selectedPueblo);
                                CargarReservas(selectedPuebloId); // Recargar reservas en el DataGrid
                            }

                            // Limpiar los campos
                            LimpiarCampos();
                        }
                        else
                        {
                            MessageBox.Show("Error al actualizar la reserva. Por favor, intenta nuevamente.");
                        }
                    }
                    else
                    {
                        txtError.Text = $"La instalación no está disponible el {fechaReserva.ToShortDateString()} a las {horaInicio.ToString()}";
                    }
                }
                else
                {
                    MessageBox.Show("Selecciona una reserva para editar.");
                }
            }
            else
            {
                editingMode = false;

                // Obtener datos necesarios para la reserva
                string selectedInstalacion = ComboBoxInstalaciones.SelectedItem.ToString();
                int idInstalacion = reservasController.ObtenerInstalacionSelected(selectedInstalacion);
                DateTime fechaReserva = datePickerFecha.SelectedDate.Value;
                TimeSpan horaInicio = TimeSpan.Parse(TextBoxHoraReserva.Text);
                TimeSpan duracion = TimeSpan.Parse(TextBoxDuracion.Text);

                // Verificar disponibilidad utilizando el método ComprobarDisponibilidad
                string disponibilidad = reservasController.ComprobarDisponibilidad(idInstalacion, fechaReserva, horaInicio, duracion);

                if (disponibilidad == "Disponible")
                {
                    // La instalación está disponible, proceder con la inserción de la reserva
                    // Crear una instancia de LoginController y configurar usuario y contraseña
                    LoginController loginController = new LoginController();
                    loginController.Usuario = usuario;
                    loginController.Password = password;

                    // Intentar iniciar sesión
                    if (loginController.Login())
                    {
                        // Obtener el idUsuario del usuario autenticado
                        int idUsuario = loginController.ObtenerIdUsuario(usuario);

                        // Insertar la reserva utilizando el idUsuario obtenido
                        bool reservaInsertada = reservasController.InsertarReserva(idUsuario, idInstalacion, fechaReserva, horaInicio, duracion);

                        // Mostrar mensaje de éxito o error según el resultado de la inserción
                        if (reservaInsertada)
                        {
                            MessageBox.Show("Reserva agregada correctamente.");
                            // Recargar las reservas del pueblo seleccionado en el DataGrid
                            if (comboBoxPueblos.SelectedItem != null)
                            {
                                string selectedPueblo = comboBoxPueblos.SelectedItem.ToString();
                                int selectedPuebloId = pueblosController.ObtenerPuebloSelected(selectedPueblo);
                                CargarReservas(selectedPuebloId); // Recargar reservas en el DataGrid
                            }
                            LimpiarCampos();
                        }
                        else
                        {
                            MessageBox.Show("Error al agregar la reserva. Por favor, intenta nuevamente.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error al iniciar sesión. Verifica usuario y contraseña.");
                    }
                }
                else
                {
                    txtError.Text = $"La instalación no está disponible el {fechaReserva.ToShortDateString()} a las {horaInicio.ToString()}";
                }
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            btnEditAdd.Content = "Editar";
            if (sender is Button btn && btn.DataContext is ReservaViewModel reserva)
            {
                string userRole = roleController.GetUserRole(usuario);
                if(userRole == "admin" || userRole == "adminPueblo")
                {
                    // Llenar los campos del Popup con los datos de la reserva seleccionada
                    ComboBoxInstalaciones.SelectedItem = reserva.NombreInstalacion;
                    datePickerFecha.SelectedDate = reserva.FechaReserva;
                    TextBoxDuracion.Text = reserva.Duracion.ToString(@"hh\:mm");
                    TextBoxHoraReserva.Text = reserva.HoraInicio.ToString(@"hh\:mm");

                    //Activar modo edicion
                    editingMode = true;

                    // Abre el Popup para editar la reserva
                    popupAgregar.IsOpen = true;
                }
                else if(userRole == "monitor" || userRole == "usuario")
                {
                    if(reserva.NombreUsuario == usuario)
                    {
                        // Llenar los campos del Popup con los datos de la reserva seleccionada
                        ComboBoxInstalaciones.SelectedItem = reserva.NombreInstalacion;
                        datePickerFecha.SelectedDate = reserva.FechaReserva;
                        TextBoxDuracion.Text = reserva.Duracion.ToString(@"hh\:mm");
                        TextBoxHoraReserva.Text = reserva.HoraInicio.ToString(@"hh\:mm");

                        //Activar modo edicion
                        editingMode = true;

                        // Abre el Popup para editar la reserva
                        popupAgregar.IsOpen = true;
                    }
                    else
                    {
                        MessageBox.Show("No tienes permisos para editar esta reserva.");
                    }
                }
            }
            
        }
        private void LimpiarCampos()
        {
            ComboBoxInstalaciones.SelectedIndex = 0;
            datePickerFecha.SelectedDate = null;
            TextBoxDuracion.Text = "";
            TextBoxHoraReserva.Text = "";
        }
        private void DeleteReserva_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ReservaViewModel reserva)
            {
                // Obtener el ID de la reserva a eliminar
                int idReserva = reserva.IdReserva;

                string username = usuario;

                string userRole = roleController.GetUserRole(username);


                if (userRole == "admin" || userRole == "adminPueblo")
                {
                    // El usuario tiene permisos para borrar todas las reservas
                    ConfirmarEliminacionReserva(idReserva);
                }
                else if (userRole == "monitor" || userRole == "usuario")
                {
                    // El usuario solo puede borrar sus propias reservas
                    if (reserva.NombreUsuario == username)
                    {
                        ConfirmarEliminacionReserva(idReserva);
                    }
                    else
                    {
                        MessageBox.Show("No tienes permisos para eliminar esta reserva.");
                    }
                }
            }
            
        }
        
        private void ConfirmarEliminacionReserva(int idReserva)
        {

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