using SFIClient.SFIServices;
using SFIClient.Utilities;
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

                ShowCreditInformation();
                ShowClientInformation();
                ShowEfficiencyInformation();
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

        private void ShowCreditInformation()
        {
            TbkCreditInvoice.Text = credit.Invoice;
            TbkCreditAmountApproved.Text = credit.AmountApproved.ToString("C", new System.Globalization.CultureInfo("es-MX"));
        }

        private void ShowClientInformation()
        {
            Client client = credit.Client;
            TbkClientName.Text = client.Name + " " + client.LastName + " " + client.Surname;
            TbkClientRFC.Text = client.Rfc;
        }

        private void ShowEfficiencyInformation()
        {
            List<Payment> payments = credit.Payments.ToList();
            int totalPayments = payments.Count;

            List<Payment> paymentsPlannedToDate = payments
                .Where(payment => payment.planned_date <= DateTime.Now)
                .ToList();
            int totalPaymentPlannedToDate = paymentsPlannedToDate.Count;
            int totalPaidPayments = paymentsPlannedToDate
                .Where(payment => payment.reconciliation_date != null)
                .ToList()
                .Count;

            TbkTotalPayments.Text = totalPayments.ToString();
            TbkTotalPaymentPlannedToDate.Text = totalPaymentPlannedToDate.ToString();

            if(totalPaymentPlannedToDate > 0) {
                double efficiencyPercentage = (totalPaidPayments / totalPaymentPlannedToDate) * 100.0;
                TbkEfficiency.Text = efficiencyPercentage.ToString("0.0") + "%";
            } else
            {
                TbkEfficiency.Text = "No aplica";
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
