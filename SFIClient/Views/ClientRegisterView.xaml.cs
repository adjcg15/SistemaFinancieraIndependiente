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
    /// Lógica de interacción para ClientRegisterView.xaml
    /// </summary>
    public partial class ClientRegisterView : Page
    {
        public ClientRegisterView()
        {
            InitializeComponent();
        }

        private void BtnGoBackClick(object sender, RoutedEventArgs e)
        {
            SearchClientByRFCView searchClientByRFCView = new SearchClientByRFCView();
            this.NavigationService.Navigate(searchClientByRFCView);
        }
    }
}
