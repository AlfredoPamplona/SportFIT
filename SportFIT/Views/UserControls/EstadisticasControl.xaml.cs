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

                    Title = "Reservas " + nombreInstalacion,
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

            string selectedPueblo = comboBoxPueblos.SelectedItem.ToString();
            DataTable tendenciaReservas = estadisticasController.TendenciaReservasMes(selectedPueblo);
            List<string> meses = new List<string>();

            LineChart.Series.Clear();

            LineSeries series = new LineSeries
            {
                Title = "Reservas por mes",
                Values = new ChartValues<int>(),
                LabelPoint = point => $"{meses[(int)point.X]}: {point.Y} reservas" 
            };

            foreach (DataRow row in tendenciaReservas.Rows)
            {
                string mes = row["Mes"].ToString();
                int totalReservas = Convert.ToInt32(row["Total_Reservas"]);

                series.Values.Add(totalReservas);
                meses.Add(mes);
            }

            LineChart.Series.Add(series);

            LineChart.AxisX.Clear();
            LineChart.AxisX.Add(new Axis
            {
                Labels = meses
            });
        }

        private void InformeAgrupacionInstalaciones_Click(object sender, RoutedEventArgs e)
        {
            StiReport report = new StiReport();

            report.Load("..\\..\\Reports\\InformeAgrupacionReservas.mrt");
            report["PuebloId"] = pueblosController.ObtenerPuebloSelected(comboBoxPueblos.Text);
            report.Compile();
            
            report.Show();
        }
        private void AbrirPopUpFechas_Click(object sender,RoutedEventArgs e)
        {
            if (!popupFechas.IsOpen)
            {
                popupFechas.IsOpen = true;
            }
            else
            {
                popupFechas.IsOpen = false;
            }
        }

        private void rangoFechas_Click(object sender, RoutedEventArgs e)
        {
            txtError.Text = "";

            if (datePickerFechaDesde.SelectedDate == null || datePickerFechaHasta.SelectedDate == null)
            {
                txtError.Text = "Ambas fechas son obligatorias.";
                return;
            }

            DateTime fechaDesde = datePickerFechaDesde.SelectedDate.Value;
            DateTime fechaHasta = datePickerFechaHasta.SelectedDate.Value;

            if (fechaHasta < fechaDesde)
            {
                txtError.Text = "La fecha fin no puede ser anterior a la fecha inicio.";
                return;
            }

            StiReport report = new StiReport();

            report.Load("..\\..\\Reports\\InformeRangoFechas.mrt");
            report["IdPueblo"] = pueblosController.ObtenerPuebloSelected(comboBoxPueblos.Text);
            report["FechaInicio"] = datePickerFechaDesde.SelectedDate.Value;
            report["FechaFin"] = datePickerFechaHasta.SelectedDate.Value;
            report.Compile();

            report.Show();
            popupFechas.IsOpen = false;
        }


        private void InformeHorasUsuario_Click(object sender, RoutedEventArgs e)
        {
            StiReport report = new StiReport();

            // Cargar el informe desde el archivo
            report.Load("..\\..\\Reports\\InformeHorasUsuario.mrt");
            report["PuebloId"] = pueblosController.ObtenerPuebloSelected(comboBoxPueblos.Text);
            report.Compile();

            report.Show();
        }
    }
}
