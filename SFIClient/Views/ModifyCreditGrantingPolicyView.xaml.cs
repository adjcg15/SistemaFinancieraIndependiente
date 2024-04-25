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
    public partial class ModifyCreditGrantingPolicyController : Page
    {
        private CreditGrantingPolicy editedPolicy;

        public ModifyCreditGrantingPolicyController(CreditGrantingPolicy policy)
        {
            InitializeComponent();

            this.editedPolicy = policy;
        }

        private void BtnReturnToPreviousPageClick(object sender, RoutedEventArgs e)
        {
            ShowReturnToPreviousPageConfirmationDialog();
        }

        private void ShowReturnToPreviousPageConfirmationDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                "¿Está seguro que desea regresar a la ventana previa? " +
                "Todos los cambios sin guardar se perderá",
                "Regresar a ventana previa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (buttonClicked == MessageBoxResult.Yes)
            {
                RedirectToCreditGrantingPolicyList();
            }
        }

        private void RedirectToCreditGrantingPolicyList()
        {
            NavigationService.Navigate(new CreditGrantingPolicyListController());
        }

        private void BtnCancelClick(object sender, RoutedEventArgs e)
        {
            ShowDiscardChangesConfirmationDialog();
        }

        private void ShowDiscardChangesConfirmationDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                "¿Está seguro que desea cancelar la actualización de la política? " +
                "Todos los cambios sin guardar se perderán",
                "Descartar cambios",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (buttonClicked == MessageBoxResult.Yes)
            {
                RedirectToCreditGrantingPolicyList();
            }
        }

        private void BtnSaveChangesClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
