using SportFIT.Views.UserControls;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;

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
            HiddenButtonsBasedOnRole(currentUserRole);
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
                        mainGrid.Children.Clear();
                        mainGrid.Children.Add(userControlToShow);
                        break;
                    case "instalacionesButton":
                        userControlToShow = new InstalacionesControl(ConfigurationManager.ConnectionStrings["DBContextSportFIT"].ConnectionString, usuario, password);
                        mainGrid.Children.Clear();
                        mainGrid.Children.Add(userControlToShow);
                        break;
                    case "estadisticasButton":
                        mainGrid.Children.Clear();
                        break;
                    case "ajustesButton":
                        mainGrid.Children.Clear();
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

        private void HiddenButtonsBasedOnRole(string role)
        {
            switch (role)
            {
                case "admin":
                case "adminPueblo":
                    // Habilitar todos los botones
                    break;
                case "monitor":
                    estadisticasButton.Visibility = Visibility.Hidden;
                    ajustesButton.Visibility = Visibility.Hidden;                   
                    break;
                case "usuario":
                    instalacionesButton.Visibility = Visibility.Hidden;
                    estadisticasButton.Visibility = Visibility.Hidden;
                    ajustesButton.Visibility = Visibility.Hidden;
                    break;
                default:
                    break;
            }
        }
    }
}
