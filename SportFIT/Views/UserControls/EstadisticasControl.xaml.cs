using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;
using SportFIT.Controllers;

namespace SportFIT.Views.UserControls
{
    /// <summary>
    /// Lógica de interacción para EstadisticasControl.xaml
    /// </summary>
    public partial class EstadisticasControl : UserControl
    {
        private readonly PueblosController pueblosController;
        private string usuario;
        private string password;
        public EstadisticasControl(string connectionString, string usuario, string password)
        {
            InitializeComponent();

            this.usuario = usuario;
            this.password = password;

            // Crear una instancia del controlador de pueblos con la cadena de conexión adecuada
            pueblosController = new PueblosController(connectionString);

            // Llenar el ComboBox con los nombres de los pueblos
            CargarNombresPueblos();

            //Seleccionar el primer item por defecto
            comboBoxPueblos.SelectedIndex = 0;

            // Cargar datos de prueba en el gráfico de barras
            SeriesCollection barSeries = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Reservas Por Instalación",
                    Values = new ChartValues<int> { 10, 20, 30, 40 }
                }
            };
            BarChart.Series = barSeries;
            // Cargar datos de prueba en el gráfico circular
            SeriesCollection pieSeries = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Media duración reservas Usuario",
                    Values = new ChartValues<double> { 30 },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Media duración reservas Administrador",
                    Values = new ChartValues<double> { 50 },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Media duración reservas Monitor",
                    Values = new ChartValues<double> { 20 },
                    DataLabels = true
                }
            };
            PieChart.Series = pieSeries;
            // Cargar datos de prueba en el gráfico de líneas
            SeriesCollection lineSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Tendencia mensual reservas",
                    Values = new ChartValues<double> { 10, 20, 15, 40, 80, 65, 95 } // Valores de ejemplo para la serie de línea
                }
            };
            LineChart.Series = lineSeries;
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

            }
        }
    }
}
