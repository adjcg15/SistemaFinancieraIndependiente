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
    public partial class CreditApplicationsListController : Page
    {
        public CreditApplicationsListController()
        {
            InitializeComponent();
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            LoadAllCreditApplications();
        }

        private void LoadAllCreditApplications()
        {

        }

        private void BtnRestartFiltersClick(object sender, RoutedEventArgs e)
        {

        }

        private void BtnSearchCreditApplicationsClick(object sender, RoutedEventArgs e)
        {

        }

        private void BtnReturnToPreviousScreenClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
