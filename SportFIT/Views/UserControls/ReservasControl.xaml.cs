using System.Windows.Controls;
using SportFIT.Controllers;

namespace SportFIT.Views.UserControls
{
    public partial class ReservasControl : UserControl
    {
        private readonly PueblosController pueblosController;

        public ReservasControl(string connectionString)
        {
            InitializeComponent();

            // Crear una instancia del controlador de pueblos con la cadena de conexión adecuada
            pueblosController = new PueblosController(connectionString);

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
    }
}
