using System;
using System.Collections.Generic;
using System.Data;
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
using Stimulsoft.Report.Viewer;
using Stimulsoft.Report;
using Stimulsoft.Report.WpfDesign;


namespace SportFIT.Views.UserControls
{
    /// <summary>
    /// Lógica de interacción para EstadisticasControl.xaml
    /// </summary>
    public partial class EstadisticasControl : UserControl
    {
        private readonly EstadisticasController estadisticasController;
        private readonly PueblosController pueblosController;
        private string usuario;
        private string password;
        public EstadisticasControl(string connectionString, string usuario, string password)
        {
            InitializeComponent();

            this.usuario = usuario;
            this.password = password;

            estadisticasController = new EstadisticasController(connectionString);

            // Crear una instancia del controlador de pueblos con la cadena de conexión adecuada
            pueblosController = new PueblosController(connectionString);

            // Llenar el ComboBox con los nombres de los pueblos
            CargarNombresPueblos();

            //Seleccionar el primer item por defecto
            comboBoxPueblos.SelectedIndex = 0;

            //// Cargar datos de prueba en el gráfico de barras
            //SeriesCollection barSeries = new SeriesCollection
            //{
            //    new ColumnSeries
            //    {
            //        Title = "Reservas Por Instalación",
            //        Values = new ChartValues<int> { 10, 20, 30, 40 }
            //    }
            //};
            //BarChart.Series = barSeries;
            //// Cargar datos de prueba en el gráfico circular
            //SeriesCollection pieSeries = new SeriesCollection
            //{
            //    new PieSeries
            //    {
            //        Title = "Media duración reservas Usuario",
            //        Values = new ChartValues<double> { 30 },
            //        DataLabels = true
            //    },
            //    new PieSeries
            //    {
            //        Title = "Media duración reservas Administrador",
            //        Values = new ChartValues<double> { 50 },
            //        DataLabels = true
            //    },
            //    new PieSeries
            //    {
            //        Title = "Media duración reservas Monitor",
            //        Values = new ChartValues<double> { 20 },
            //        DataLabels = true
            //    }
            //};
            //PieChart.Series = pieSeries;
            //// Cargar datos de prueba en el gráfico de líneas
            //SeriesCollection lineSeries = new SeriesCollection
            //{
            //    new LineSeries
            //    {
            //        Title = "Tendencia mensual reservas",
            //        Values = new ChartValues<double> { 10, 20, 15, 40, 80, 65, 95 } // Valores de ejemplo para la serie de línea
            //    }
            //};
            //LineChart.Series = lineSeries;
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
                CargarDatosGraficoBarras(); // Llamar al método para cargar los datos del gráfico de barras
                CargarDatosGraficoRedondo(); // Llamar al método para cargar los datos del gráfico circular
                CargarDatosGraficoLineas(); // Llamar al método para cargar los datos del gráfico de lineas
            }
        }

        private void CargarDatosGraficoBarras()
        {
            string selectedPueblo = comboBoxPueblos.SelectedItem.ToString();
            DataTable reservasPorInstalacion = estadisticasController.ObtenerReservasPorInstalacion(selectedPueblo);

            SeriesCollection barSeries = new SeriesCollection();

            foreach (DataRow row in reservasPorInstalacion.Rows)
            {
                string nombreInstalacion = row["Instalacion"].ToString();
                int totalReservas = Convert.ToInt32(row["Total_Reservas"]);

                ColumnSeries series = new ColumnSeries
                {

                    Title = "Reservas por Instalación: " + nombreInstalacion,
                    Values = new ChartValues<int> { totalReservas }
                };

                barSeries.Add(series);
            }

            BarChart.Series = barSeries;
        }

        private void CargarDatosGraficoRedondo()
        {
            string selectedPueblo = comboBoxPueblos.SelectedItem.ToString();
            DataTable porcentajesReservas = estadisticasController.ObtenerPorcentajesReservas(selectedPueblo);

            SeriesCollection pieSeries = new SeriesCollection();

            foreach (DataRow row in porcentajesReservas.Rows)
            {
                string nombreUsuario = row["Usuario"].ToString();
                double porcentajeReservas = Convert.ToDouble(row["Porcentaje_Reservas"]);

                PieSeries series = new PieSeries
                {
                    Title = nombreUsuario,
                    Values = new ChartValues<double> { porcentajeReservas },
                    DataLabels = true
                };

                pieSeries.Add(series);
            }

            PieChart.Series = pieSeries;
        }
        private void CargarDatosGraficoLineas()
        {
            try
            {
                string selectedPueblo = comboBoxPueblos.SelectedItem.ToString();
                DataTable tendenciaReservas = estadisticasController.TendenciaReservasMes(selectedPueblo);

                LineChart.Series.Clear();

                LineSeries series = new LineSeries
                {
                    Title = "Tendencia mensual de reservas",
                    Values = new ChartValues<int>()

                };

                foreach (DataRow row in tendenciaReservas.Rows)
                {
                    string mes = row["Mes"].ToString();
                    int totalReservas = Convert.ToInt32(row["Total_Reservas"]);

                    series.Values.Add(totalReservas);
                }

                LineChart.Series.Add(series);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos para el gráfico de líneas: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void InformeAgrupacionInstalaciones_Click(object sender, RoutedEventArgs e)
        {
            // Crear una instancia del informe diseñado en Stimulsoft
            StiReport report = new StiReport();

            // Cargar el informe desde el archivo
            report.Load("..\\..\\Reports\\InformeAgrupacionReservas.mrt");
            report["PuebloId"] = pueblosController.ObtenerPuebloSelected(comboBoxPueblos.Text);
            report.Compile();
            
            report.Show();
        }
    }
}
