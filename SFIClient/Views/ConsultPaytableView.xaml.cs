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
    public partial class ConsultPaytableController : Page
    {
        private readonly Credit credit;
        private PaymentControl selectedPayment;
        private Payment payment;
        private string invoice;
        private List<Payment> paymentsList;

        public ConsultPaytableController(Credit credit)
        {
            this.credit = credit;
            InitializeComponent();
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            ShowCreditInformation();
            LoadPayments(credit.Invoice);
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
                List<Payment> loadedPayments = creditsServiceClient.GetPaymentsByCreditInvoice(creditInvoice).ToList();
                paymentsList = loadedPayments;

                ShowPayments();
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
            NavigationService.Navigate(new CreditsListController());
        }

        private void ShowPayments()
        {
            GrdEmptyPaymentsMessage.Visibility = paymentsList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            SkpApplicablePayments.Visibility = paymentsList.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            SkpApplicablePayments.Children.Clear();

            foreach (var payment in paymentsList)
            {
                var paymentCard = new PaymentControl(payment);
                paymentCard.CardClick += (sender, e) =>
                {
                    selectedPayment = paymentCard;
                };

                SkpApplicablePayments.Children.Add(paymentCard);
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
                    if(!IsSmallFile(selectedFilePath))
                    {
                        string errorMessage = "El archivo seleccionado es demasiado grande. " +
                            "Por favor seleccione un archivo más pequeño.";
                        ShowInvalidUploadCsvErrorDialog(errorMessage);
                    }
                    else
                    {
                        string[] expectedColumns = { "amount", "invoice", "planned_date", "credit_invoice", "reconciliation_date" };
                        using (var reader = new StreamReader(selectedFilePath))
                        {
                            if(!validFileFormat(reader, expectedColumns))
                            {
                                string errorMessage = "Por favor, asegúrese de que el archivo tenga las columnas 'amount', " +
                                    "'invoice', 'planned_date', 'credit_invoice' y 'reconciliation_date'.";
                                ShowInvalidUploadCsvErrorDialog(errorMessage);
                            }
                            else
                            {
                                ProcessPaymentData(reader);
                            }
                        }
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
        }

        private void ShowInvalidUploadCsvErrorDialog(string message)
        {
            MessageBox.Show(
                message,
                "Archivo inválido",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }

        private bool IsSmallFile(string filePath)
        {
            const long MAX_BYTES_ALLOWED = 524288;
            FileInfo fileInfo = new FileInfo(filePath);
            long fileSizeInBytes = fileInfo.Length;

            return fileSizeInBytes < MAX_BYTES_ALLOWED;
        }

        private bool validFileFormat(StreamReader reader, string[] expectedColumns)
        {
            string[] columns = reader.ReadLine()?.Split(',');

            return columns != null && columns.SequenceEqual(expectedColumns);
        }

        private void ProcessPaymentData(StreamReader reader)
        {
            CreditsServiceClient creditsServiceClient = new CreditsServiceClient();

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] values = line.Split(',');
                string invoice = values[1].Trim().Replace("\"", "");

                Payment existingPayment = creditsServiceClient.GetPaymentByInvoice(invoice);
                if (existingPayment != null &&
                    existingPayment.reconciliation_date.HasValue)
                {
                    MessageBox.Show("El pago para la factura especificada ya ha sido conciliado. No se puede actualizar.",
                        "Pago ya conciliado", MessageBoxButton.OK, MessageBoxImage.Warning);
                    continue;
                }

                if (existingPayment != null)
                {
                    double amountFromDB = existingPayment.amount;
                    double amountFromFile = Convert.ToDouble(values[0]);
                    double tolerance = 0.01;
                    if (Math.Abs(amountFromDB - amountFromFile) <= tolerance)
                    {
                        ClosePayment(existingPayment.invoice);
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

        private void ClosePayment(string paymentInvoice)
        {
            CreditsServiceClient creditsServiceClient = new CreditsServiceClient();

            decimal interestPercentage = creditsServiceClient.ClosePayment(paymentInvoice);
            Payment updatedPayment = paymentsList.Find(payment => payment.invoice == paymentInvoice);
            updatedPayment.Interest = interestPercentage;
            updatedPayment.reconciliation_date = DateTime.Now;

            ShowPayments();
        }

        private void BtnReturnCreditsListClick(object sender, RoutedEventArgs e)
        {
            RedirectToConsultCreditsList();
        }
    }
}