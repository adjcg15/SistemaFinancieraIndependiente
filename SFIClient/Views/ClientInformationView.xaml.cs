using SFIClient.SFIServices;
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

namespace SFIClient.Views
{
    /// <summary>
    /// Lógica de interacción para ClientInformationView.xaml
    /// </summary>
    public partial class ClientInformationController : Page
    {
        private readonly Client client;
        public ClientInformationController(Client client)
        {
            InitializeComponent();
            this.client = client;
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void LoadClient()
        {

        }

        private void BtnGoToSearchClientByRFCViewClick(object sender, RoutedEventArgs e)
        {
            RedirectToSearchClientByRFCView();
        }

        private void RedirectToSearchClientByRFCView()
        {
            NavigationService.Navigate(new SearchClientByRFCController());
        }

        private void ShowErrorRecoveringClient(string message)
        {
            MessageBox.Show(
                message,
                "Sistema no disponible",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
    }
}
