using System.Collections.Generic;
using System.Windows.Controls;
using SportFIT.Controllers;
using SportFIT.Models;

namespace SportFIT.Views.UserControls
{
    public partial class ReservasControl : UserControl
    {
        private readonly PueblosController pueblosController;
        private readonly ReservasController reservasController;

        public ReservasControl(string connectionString)
        {
            InitializeComponent();

            // Crear una instancia del controlador de pueblos con la cadena de conexión adecuada
            pueblosController = new PueblosController(connectionString);

            // Crear una instancia del controlador de reservas con la cadena de conexión adecuada
            reservasController = new ReservasController(connectionString);

            // Llenar el ComboBox con los nombres de los pueblos
            CargarNombresPueblos();

        }

        private void CargarNombresPueblos()
        {
            // Obtener los nombres de los pueblos desde el controlador
            var nombresPueblos = pueblosController.ObtenerNombresPueblos();

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
            }
        }

        private void CargarReservas(int selectedPuebloId)
        {
            List<ReservaViewModel> reservas = reservasController.ObtenerReservas(selectedPuebloId);
            dataGridReservas.ItemsSource = reservas;          
        }
    }
}
