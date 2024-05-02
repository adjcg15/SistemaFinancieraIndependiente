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
    /// <summary>
    /// Lógica de interacción para PaymentControl.xaml
    /// </summary>
    public partial class PaymentControl : UserControl
    {
        public  Payments BindedPayment { get; }
        public event EventHandler<Payments> CardClick;
        public bool IsSelected { get; private set; }
        public PaymentControl(Payments payment)
        {
            InitializeComponent();
            BindedPayment = payment;
            ShowCreditConditionInformation();
        }
        private void ShowCreditConditionInformation()
        {
            TbkPaymentInvoice.Text = BindedPayment.invoice;
            TbkPlannedDate.Text = BindedPayment.planned_date.ToString("dd-MM-yyyy");
            TbkAmount.Text = BindedPayment.amount.ToString("C");
            TbkAmount.Inlines.Clear();
            TbkAmount.Inlines.Add(new Run(BindedPayment.amount.ToString("C", new System.Globalization.CultureInfo("es-MX"))));
            //TbkPlannedDate.Text = BindedPayment.reconciliation_date.ToString("dd-MM-yyyy");
        }
        private void BdrCreditConditionCardMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
                IsSelected = true;
                CardClick?.Invoke(this, BindedPayment);
        }

        private void BtnDownloadLayoutClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
