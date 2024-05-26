using SFIClient.SFIServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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
    public partial class CreditCollectionEfficiencyController : Page
    {
        private string creditInvoice;
        private Credit credit;

        public CreditCollectionEfficiencyController(string creditInvoice)
        {
            InitializeComponent();

            this.creditInvoice = creditInvoice;
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            RecoverCreditInformation();
        }

        private void RecoverCreditInformation()
        {
            CreditsServiceClient creditsService = new CreditsServiceClient();

            try
            {
                Credit recoveredCredit = creditsService.GetCreditForCollectionEfficiency(creditInvoice);
                credit = recoveredCredit;
                //TODO: show credit information
            }
            catch (FaultException<ServiceFault> fault)
            {
                ShowErrorRecoveringCreditDialog(fault.Detail.Message);
            }
            catch (EndpointNotFoundException)
            {
                string errorMessage = "Servidor no disponible. No fue posible recuperar la información " +
                    "del cliente, por favor inténtelo más tarde";
                ShowErrorRecoveringCreditDialog(errorMessage);
            }
            catch (CommunicationException)
            {
                string errorMessage = "Error de conexión. Ocurrió un error de conexión, por favor compruebe " +
                    "su conexión de Internet e intente de nuevo";
                ShowErrorRecoveringCreditDialog(errorMessage);
            }
        }

        private void ShowErrorRecoveringCreditDialog(string message)
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                message,
                "No fue posible recuperar la información del crédito",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );

            if (buttonClicked == MessageBoxResult.OK)
            {
                RediectToLogin();
            }
        }

        private void RediectToLogin()
        {
            NavigationService.Navigate(new LoginController());
        }

        private void BtnReturnToPreviousPageClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
