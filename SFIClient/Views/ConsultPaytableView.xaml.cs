using Microsoft.Win32;
using SFIClient.Controlls;
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
    /// <summary>
    /// Lógica de interacción para ConsultPaytableView.xaml
    /// </summary>
    public partial class ConsultPaytableController : Page
    {
        private Credit credit;
        private PaymentControl selectedPayment;
        private Payments payment;
        private string invoice;
        private void PageLoaded(object sender, RoutedEventArgs e)
        {
        }
        public ConsultPaytableController(Credit credit)
        {
            this.credit = credit;
            InitializeComponent();
            ShowCreditInformation();
            LoadPayments(this.credit.Invoice);
        }
        private void ShowCreditInformation()
        {
            TbkCreditInvoice.Text = credit.Invoice;
            TbkClientName.Text = $"{credit.Client.Name} {credit.Client.Surname} {credit.Client.LastName}";

            SpnCredirAmountApproved.Inlines.Clear();
            SpnCredirAmountApproved.Inlines.Add(new Run(credit.AmountApproved.ToString("C", new System.Globalization.CultureInfo("es-MX"))));

            SpnPaymentMonths.Inlines.Clear();
            SpnPaymentMonths.Inlines.Add(new Run(credit.CreditCondition.PaymentMonths.ToString()));

            SpnInterestRate.Inlines.Clear();
            SpnInterestRate.Inlines.Add(new Run((credit.CreditCondition.InterestRate * 100).ToString()));

            SpnIsIvaApplied.Inlines.Clear();
            SpnIsIvaApplied.Inlines.Add(new Run(credit.CreditCondition.IsIvaApplied ? " y aplicando IVA" : ""));

            SpnInterestOnArrears.Inlines.Clear();
            SpnInterestOnArrears.Inlines.Add(new Run((credit.CreditCondition.InterestOnArrears * 100).ToString()));

            SpnAdvancePaymentReduction.Inlines.Clear();
            SpnAdvancePaymentReduction.Inlines.Add(new Run((credit.CreditCondition.AdvancePaymentReduction * 100).ToString()));
        }
        private void LoadPayments(string creditInvoice)
        {
            CreditsServiceClient creditsServiceClient = new CreditsServiceClient();
            try
            {
                List<Payments> applicablePayments = creditsServiceClient.GetPaymentsByCreditInvoice(creditInvoice).ToList();
                ShowPayments(applicablePayments);
            }
            catch (FaultException<ServiceFault> fault)
            {
                ShowErrorRecoveringEstablishedPaymentsDialog(fault.Detail.Message);
            }
            catch (EndpointNotFoundException)
            {
                string errorMessage = "El servidor no se encuentra disponible, intente más tarde";
                ShowErrorRecoveringEstablishedPaymentsDialog(errorMessage);
            }
            catch (CommunicationException)
            {
                string errorMessage = "No fue posible acceder a la información debido a un error de conexión";
                ShowErrorRecoveringEstablishedPaymentsDialog(errorMessage);
            }
        }
        private void ShowPayments(List<Payments> establishedPayments)
        {
            GrdEmptyPaymentsMessage.Visibility = establishedPayments.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            SkpApplicablePayments.Visibility = establishedPayments.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            SkpApplicablePayments.Children.Clear();
            foreach (var payment in establishedPayments)
            {
                var establishedPaymentCard = new PaymentControl(payment);
                establishedPaymentCard.CardClick += (sender, e) =>
                {
                    selectedPayment = establishedPaymentCard;
                };
                SkpApplicablePayments.Children.Add(establishedPaymentCard);
            }
        }
        private void ShowErrorRecoveringEstablishedPaymentsDialog(string message)
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                message,
                "Condiciones de crédito no disponibles",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );

            if (buttonClicked == MessageBoxResult.OK)
            {
                RedirectToConsultCreditsList();
            }
        }
        private void RedirectToConsultCreditsList()
        {
            CreditsListController creditList = new CreditsListController();
            this.NavigationService.Navigate(creditList);
            NavigationService.RemoveBackEntry();
        }
        private void BtnRegisterPaymentClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Archivos CSV (*.csv)|*.csv|Todos los archivos (*.*)|*.*";
            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                string selectedFilePath = openFileDialog.FileName;

                // Aquí puedes realizar las operaciones necesarias con la ruta del archivo seleccionado
                // Por ejemplo, puedes iniciar el proceso de carga y procesamiento del archivo CSV
            }
            else
            {
                string errorMessage = "No fue seleccionado algun archivo, intente nuevamente";
                ShowErrorRecoveringEstablishedPaymentsDialog(errorMessage);
            }
        }
        private void ShowErrorUploadCsvDialog(string message)
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                message,
                "Archivo no seleccionado",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
        private void BtnReturnCreditsListClick(object sender, RoutedEventArgs e)
        {
            CreditsListController creditList = new CreditsListController();
            this.NavigationService.Navigate(creditList);
            NavigationService.RemoveBackEntry();
        }
    }
}
