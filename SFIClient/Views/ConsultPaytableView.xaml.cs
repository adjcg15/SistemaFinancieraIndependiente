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

namespace SFIClient.Views
{
    /// <summary>
    /// Lógica de interacción para ConsultPaytableView.xaml
    /// </summary>
    public partial class ConsultPaytableController : Page
    {
        private Credit credit;
        private void PageLoaded(object sender, RoutedEventArgs e)
        {
        }
        public ConsultPaytableController(Credit credit)
        {
            this.credit = credit;
            InitializeComponent();
            ShowCreditInformation();
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

        private void BtnRegisterPaymentClick(object sender, RoutedEventArgs e)
        {

        }

        private void BtnReturnCreditsListClick(object sender, RoutedEventArgs e)
        {
            CreditsListController creditList = new CreditsListController();
            this.NavigationService.Navigate(creditList);
            NavigationService.RemoveBackEntry();
        }
    }
}
