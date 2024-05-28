using SFIClient.SFIServices;
using SFIClient.Utilities;
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

namespace SFIClient.Controlls
{
    public partial class PaymentControl : UserControl
    {
        public Payment BindedPayment { get; }
        public Credit BindedCredit { get; }
        public event EventHandler<Payment> CardClick;
        public bool IsSelected { get; private set; }
        private int index;
        private Action<int> disableButtonAction;
        private string clientName;

        public PaymentControl(Payment payment, int index, Action<int> disableButtonAction, bool isEnabled, string clientName)
        {
            InitializeComponent();
            BindedPayment = payment;
            this.index = index;
            this.disableButtonAction = disableButtonAction;
            this.clientName = clientName;
            ShowCreditConditionInformation();
            BtnDownloadLayout.IsEnabled = isEnabled;
        }

        private void ShowCreditConditionInformation()
        {
            TbkPaymentInvoice.Text = BindedPayment.Invoice;
            TbkPlannedDate.Text = BindedPayment.PlannedDate.ToString("dd-MM-yyyy");
            TbkAmount.Text = BindedPayment.Amount.ToString("C", new System.Globalization.CultureInfo("es-MX"));
            TbkInterest.Text = BindedPayment.Interest.ToString();
            TbkReconciliationDate.Text = BindedPayment.ReconciliationDate.HasValue
                ? BindedPayment.ReconciliationDate.Value.ToString("dd-MM-yyyy")
                : "-";
        }

        private void BtnDownloadLayoutClick(object sender, RoutedEventArgs e)
        {
            string client = clientName;

            string captureLine = GenerateCaptureLine(BindedPayment.Invoice, BindedPayment.PlannedDate);
            string creditInvoice = BindedPayment.Invoice;
            string plannedDate = BindedPayment.PlannedDate.ToString("dd-MM-yyyy");
            double amount = BindedPayment.Amount;

            HandleDownloadLayoutRequest(BindedPayment, captureLine, client, creditInvoice, plannedDate, amount);
            if (BindedPayment.ReconciliationDate.HasValue)
            {
                disableButtonAction(index);
            }
        }

        private string GenerateCaptureLine(string invoice, DateTime plannedDate)
        {
            Random random = new Random();
            StringBuilder captureBuilder = new StringBuilder();

            captureBuilder.Append(invoice.Substring(0, 6));
            captureBuilder.Append(plannedDate.Day.ToString("D2"));
            captureBuilder.Append(plannedDate.Month.ToString("D2"));

            for (int i = 0; i < 6; i++)
            {
                captureBuilder.Append(random.Next(0, 10));
            }

            for (int i = 0; i < 2; i++)
            {
                char randomChar = (char)random.Next('A', 'Z' + 1);
                captureBuilder.Append(randomChar);
            }

            return captureBuilder.ToString();
        }

        private void HandleDownloadLayoutRequest(
             Payment payment,
             string captureLine,
             string client,
             string creditInvoice,
             string plannedDate,
             double amount
            )
        {
            CreditsServiceClient creditsServiceClient = new CreditsServiceClient();
            var existingLayout = creditsServiceClient.GetPaymentLayoutByPaymentId(payment.Id);

            if (existingLayout != null)
            {
                PDFLayoutGenerator.GeneratePDF(client, existingLayout.CaptureLine, plannedDate, amount, captureLine);
                ShowSuccessMessage("El archivo se ha descargado correctamente en la carpeta Documentos con el nombre SFLayout.");
            }
            else
            {
                creditsServiceClient.InsertIntoPaymentLayouts(captureLine, payment);
                PDFLayoutGenerator.GeneratePDF(client, creditInvoice, plannedDate, amount, captureLine);
                ShowSuccessMessage("El archivo se ha descargado correctamente en la carpeta Documentos con el nombre SFLayout.");
            }

            if (payment.ReconciliationDate.HasValue)
            {
                disableButtonAction(index);
            }
        }

        private void ShowSuccessMessage(string message)
        {
            MessageBox.Show(message, "Descarga exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

}
