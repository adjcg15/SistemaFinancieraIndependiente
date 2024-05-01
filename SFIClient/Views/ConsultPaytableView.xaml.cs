using Microsoft.Win32;
using SFIClient.Controlls;
using SFIClient.SFIServices;
using SFIClient.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
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
            LoadPayments(this.credit.Invoice);
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
                foreach (var payment in applicablePayments)
                {
                    if (payment.reconciliation_date == null)
                    {
                        payment.reconciliation_date = DateTime.MinValue;
                    }
                }
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
            try
            {
                if (NavigationService != null)
                {
                    CreditsListController creditList = new CreditsListController();
                    NavigationService.Navigate(creditList);
                    NavigationService.RemoveBackEntry();
                }
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
        private void BtnRegisterPaymentClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Archivos CSV (*.csv)|*.csv|Todos los archivos (*.*)|*.*";
            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                string selectedFilePath = openFileDialog.FileName;

                try
                {
                    ValidateFileSize(selectedFilePath);

                    string[] expectedColumns = { "amount", "invoice", "planned_date", "credit_invoice", "reconciliation_date" };
                    using (var reader = new StreamReader(selectedFilePath))
                    {
                        ValidateFileFormat(reader, expectedColumns);
                        ProcessPaymentData(reader);
                    }

                    MessageBox.Show("El procesamiento del archivo CSV se completó con éxito.", "Procesamiento completado",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ValidateFileSize(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            long fileSizeInBytes = fileInfo.Length;
            const long maxSizeInBytes = 524288;

            if (fileSizeInBytes > maxSizeInBytes)
            {
                throw new Exception("El archivo seleccionado es demasiado grande. Por favor seleccione un archivo más pequeño.");
            }
        }

        private void ValidateFileFormat(StreamReader reader, string[] expectedColumns)
        {
            string[] columns = reader.ReadLine()?.Split(',');

            if (columns == null || !columns.SequenceEqual(expectedColumns))
            {
                throw new Exception("El archivo seleccionado no tiene el formato esperado. " +
                    "Por favor, asegúrese de que el archivo tenga las columnas 'amount', 'invoice', 'planned_date', 'credit_invoice' y 'reconciliation_date'.");
            }
        }
        private void ProcessPaymentData(StreamReader reader)
        {
            CreditsServiceClient creditsServiceClient = new CreditsServiceClient();

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] values = line.Split(',');
                string invoice = values[1].Trim().Replace("\"", "");
                Payments existingPayment = creditsServiceClient.GetPaymentByInvoice(invoice);
                if (existingPayment != null &&
                    (existingPayment.reconciliation_date != null && existingPayment.reconciliation_date != DateTime.MinValue))
                {
                    MessageBox.Show("El pago para la factura especificada ya ha sido conciliado. No se puede actualizar.",
                        "Pago ya conciliado", MessageBoxButton.OK, MessageBoxImage.Warning);
                    continue;
                }

                Payments payment = creditsServiceClient.GetPaymentByInvoice(invoice);

                if (payment != null)
                {
                    double amountFromDB = payment.amount;
                    double amountFromFile = Convert.ToDouble(values[0]);
                    double tolerance = 0.01;
                    if (Math.Abs(amountFromDB - amountFromFile) <= tolerance)
                    {
                        UpdatePayment(payment, values);
                    }
                    else
                    {
                        MessageBox.Show("El monto del pago realizado no corresponde con el pago planeado," +
                            " por lo que no será considerado. Por favor, realice el pago nuevamente.", "Cantidad incorrecta",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("No se encontró información de pago para la factura especificada.",
                        "Error de búsqueda de pago", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void UpdatePayment(Payments payment, string[] values)
        {
            CreditsServiceClient creditsServiceClient = new CreditsServiceClient();

            payment.reconciliation_date = DateTime.Parse(values[4]);
            creditsServiceClient.UpdatePayment(payment);
        }

        private void ShowErrorUploadCsvDialog(string message)
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                message,
                "Error con el pago",
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
