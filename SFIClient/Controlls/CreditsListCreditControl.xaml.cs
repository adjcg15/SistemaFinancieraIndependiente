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
    public partial class CreditsListCreditControl : UserControl
    {
        public Credit BindedCredit { get; }
        public event EventHandler<Credit> BtnPaymentsTableClick;
        public event EventHandler<Credit> BtnMonthlyEfficiencyClick;
        public event EventHandler<Credit> BtnChangeConditionClick;

        public CreditsListCreditControl(Credit credit)
        {
            InitializeComponent();
            BindedCredit = credit;

            ShowCreditInformation();
        }

        private void ShowCreditInformation()
        {
            ShowCreditStatus();
            ShowCreditExtraInformation();

            SpnCreditApprovalDate.Inlines.Clear();
            SpnCreditApprovalDate.Inlines.Add(new Run(DateToolkit.FormatAsDDMMYYY(BindedCredit.ApprovalDate)));

            TbkCreditInvoice.Text = BindedCredit.Invoice;
            TbkCreditClient.Text = $"{BindedCredit.Client.Name} {BindedCredit.Client.Surname} {BindedCredit.Client.LastName}";

            SpnCredirAmountApproved.Inlines.Clear();
            SpnCredirAmountApproved.Inlines.Add(new Run(BindedCredit.AmountApproved.ToString("C", new System.Globalization.CultureInfo("es-MX"))));

            SpnPaymentMonths.Inlines.Clear();
            SpnPaymentMonths.Inlines.Add(new Run(BindedCredit.CreditCondition.PaymentMonths.ToString()));

            SpnInterestRate.Inlines.Clear();
            SpnInterestRate.Inlines.Add(new Run((BindedCredit.CreditCondition.InterestRate * 100).ToString()));

            SpnIsIvaApplied.Inlines.Clear();
            SpnIsIvaApplied.Inlines.Add(new Run(BindedCredit.CreditCondition.IsIvaApplied ? " y aplicando IVA" : ""));

            SpnInterestOnArrears.Inlines.Clear();
            SpnInterestOnArrears.Inlines.Add(new Run((BindedCredit.CreditCondition.InterestOnArrears * 100).ToString()));

            SpnAdvancePaymentReduction.Inlines.Clear();
            SpnAdvancePaymentReduction.Inlines.Add(new Run((BindedCredit.CreditCondition.AdvancePaymentReduction * 100).ToString()));
        }

        private void ShowCreditStatus()
        {
            SolidColorBrush primaryColor = (SolidColorBrush)Application.Current.Resources["PrimaryColor"];
            SolidColorBrush gray = (SolidColorBrush)Application.Current.Resources["Gray"];

            if (!BindedCredit.SettlementDate.HasValue)
            {
                BdrDecorator.BorderBrush = primaryColor;

                if (DateTime.Now >= BindedCredit.WithdrawalDate)
                {
                    BdrDecorator.Background = primaryColor;
                    TbkCreditStatus.Text = "Crédito en curso";
                }
                else
                {
                    BdrDecorator.Background = Brushes.White;
                    TbkCreditStatus.Text = "Pendiente por liberar";
                    SkpCreditActions.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                BdrDecorator.BorderBrush = gray;
                BdrDecorator.Background = Brushes.White;
                TbkCreditStatus.Text = "Liquidado";
            }
        }

        private void ShowCreditExtraInformation()
        {
            string message;

            if (!BindedCredit.SettlementDate.HasValue)
            {
                if (DateTime.Now >= BindedCredit.WithdrawalDate)
                {
                    message = "El pago se liberó el día " + DateToolkit.FormatAsText(BindedCredit.WithdrawalDate);
                }
                else
                {
                    message = "El pago se liberará el día " + DateToolkit.FormatAsText(BindedCredit.WithdrawalDate);
                }
            }
            else
            {
                message = "El crédito se finalizó de pagar el día " + DateToolkit.FormatAsText(BindedCredit.SettlementDate.Value);
            }

            TbkCreditExtraInfo.Text = message;
        }

        private void BtnOpenCreditPaymentsTableClick(object sender, RoutedEventArgs e)
        {
            BtnPaymentsTableClick?.Invoke(this, BindedCredit);
        }

        private void BtnOpenCreditMonthlyEfficiencyClick(object sender, RoutedEventArgs e)
        {
            BtnMonthlyEfficiencyClick?.Invoke(this, BindedCredit);
        }

        private void BtnChangeApplicableCreditConditionsClick(object sender, RoutedEventArgs e)
        {
            BtnChangeConditionClick?.Invoke(this, BindedCredit);
        }
    }
}
