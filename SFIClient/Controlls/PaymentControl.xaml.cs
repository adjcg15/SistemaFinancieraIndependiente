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
        public  Payment BindedPayment { get; }
        public Credit BindedCredit { get; }
        public event EventHandler<Payment> CardClick;
        public bool IsSelected { get; private set; }
        public PaymentControl(Payment payment)
        {
            InitializeComponent();
            BindedPayment = payment;
            ShowCreditConditionInformation();
        }
        private void ShowCreditConditionInformation()
        {
            TbkPaymentInvoice.Text = BindedPayment.invoice;
            TbkPlannedDate.Text = BindedPayment.planned_date.ToString("dd-MM-yyyy");
            TbkAmount.Text = BindedPayment.amount.ToString("C", new System.Globalization.CultureInfo("es-MX"));
            TbkInterest.Text = BindedPayment.Interest.ToString();
            TbkReconciliationDate.Text = BindedPayment.reconciliation_date.HasValue 
                ? BindedPayment.reconciliation_date.Value.ToString("dd-MM-yyyy") 
                : "-";
            BtnDownloadLayout.IsEnabled = !BindedPayment.reconciliation_date.HasValue;
        }

        private void BtnDownloadLayoutClick(object sender, RoutedEventArgs e)
        {
            //TODO: change client name
            string client = "Andres Manuel López Obrador";

            string captureLine = GenerateCaptureLine(BindedPayment.invoice, BindedPayment.planned_date);
            string creditInvoice = BindedPayment.invoice;
            string plannedDate = BindedPayment.planned_date.ToString("dd-MM-yyyy");
            double amount = BindedPayment.amount;

            HandleDownloadLayoutRequest(BindedPayment, captureLine, client, creditInvoice, plannedDate, amount);
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
            var existingLayout = creditsServiceClient.GetPaymentLayoutByPaymentId(payment.id);

            if (existingLayout == null)
            {
                PDFLayoutGenerator.GeneratePDF(client, existingLayout.capture_line, plannedDate, amount, captureLine);
                ShowSuccessMessage("El archivo se ha descargado correctamente en la carpeta Documentos con el nombre SFLayout.");
            } 
            else
            {
                creditsServiceClient.InsertIntoPaymentLayouts(captureLine, payment);
                PDFLayoutGenerator.GeneratePDF(client, creditInvoice, plannedDate, amount, captureLine);
                ShowSuccessMessage("El archivo se ha descargado correctamente en la carpeta Documentos con el nombre SFLayout.");
            }
        }

        private void ShowSuccessMessage(string message)
        {
            MessageBox.Show(message, "Descarga exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
