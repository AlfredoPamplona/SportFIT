using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using SportFIT.Controllers;
using SportFIT.Views.UserControls;

namespace SportFIT.Views
{
    public partial class MainView : Window
    {
        private string currentUserRole;
        private string usuario;
        private string password;

        public MainView(string userRole, string usuario, string password)
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
            currentUserRole = userRole;
            this.usuario = usuario;
            this.password = password;
            DisableEnableButtonsBasedOnRole(currentUserRole);
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;
            if (button != null && button.Tag != null)
            {
                UserControl userControlToShow = null;
                switch (button.Name)
                {
                    case "reservasButton":
                        userControlToShow = new ReservasControl(ConfigurationManager.ConnectionStrings["DBContextSportFIT"].ConnectionString, usuario, password);
                        mainGrid.Children.Clear(); // Limpiar cualquier contenido existente en la segunda columna
                        mainGrid.Children.Add(userControlToShow);
                        break;
                    case "instalacionesButton":
                        MessageBox.Show("Se ha pulsado el botón de Instalaciones.");
                        break;
                    case "actividadesButton":
                        MessageBox.Show("Se ha pulsado el botón de Actividades.");
                        break;
                    case "estadisticasButton":
                        MessageBox.Show("Se ha pulsado el botón de Estadísticas.");
                        break;
                    case "ajustesButton":
                        MessageBox.Show("Se ha pulsado el botón de Ajustes.");
                        break;
                    case "cambiarUsuarioButton":
                        var loginView = new LoginView();
                        loginView.Show();
                        this.Close();
                        break;
                    case "cerrarSesionButton":
                        this.Close();
                        break;
                }
            }
        }

        private void DisableEnableButtonsBasedOnRole(string role)
        {
            switch (role)
            {
                case "admin":
                case "adminPueblo":
                    // Habilitar todos los botones
                    break;
                case "monitor":
                    ajustesButton.IsEnabled = false;
                    instalacionesButton.IsEnabled = false;
                    break;
                case "usuario":
                    instalacionesButton.IsEnabled = false;
                    actividadesButton.IsEnabled = false;
                    estadisticasButton.IsEnabled = false;
                    ajustesButton.IsEnabled = false;
                    break;
                default:
                    break;
            }
        }
    }
}
