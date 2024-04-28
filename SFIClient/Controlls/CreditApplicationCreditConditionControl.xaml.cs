using SFIClient.SFIServices;
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
    public partial class CreditApplicationCreditConditionControl : UserControl
    {
        public CreditCondition BindedCondition { get; }
        public event EventHandler<CreditCondition> CardClick;
        public bool IsSelected { get; private set; }
        public CreditApplicationCreditConditionControl(CreditCondition applicableCondition)
        {
            InitializeComponent();
            BindedCondition = applicableCondition;

            ShowCreditConditionInformation();
            if(!applicableCondition.IsActive)
            {
                DisableCreditCondition();
            }
        }

        private void ShowCreditConditionInformation()
        {
            TbkIdentifier.Text = BindedCondition.Identifier;
            TbkIva.Text = BindedCondition.IsIvaApplied ? "Aplica IVA" : "";

            SpnPaymentMonths.Inlines.Clear();
            SpnPaymentMonths.Inlines.Add(new Run(BindedCondition.PaymentMonths.ToString()));

            SpnInterestRate.Inlines.Clear();
            SpnInterestRate.Inlines.Add(new Run((BindedCondition.InterestRate * 100).ToString("0.0")));

            double interestOnArrears = BindedCondition.InterestOnArrears * 100;
            BldInterestOnArrears.Inlines.Clear();
            BldInterestOnArrears.Inlines.Add(new Run(interestOnArrears.ToString("0.0")));

            double advancedPaymentReduction = BindedCondition.AdvancePaymentReduction * 100;
            BldAdvancePaymentReduction.Inlines.Clear();
            BldAdvancePaymentReduction.Inlines.Add(new Run(advancedPaymentReduction.ToString("0.0")));
        }

        private void DisableCreditCondition()
        {
            BdrCreditConditionCard.Cursor = null;
            BdrCreditConditionCard.Opacity = 0.5;
            TbkInterestRate.Foreground = Brushes.Black;
            TbkPaymentMonths.Foreground = Brushes.Black;
        }
        private void BdrCreditConditionCardMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (BindedCondition.IsActive)
            {
                IsSelected = true;
                CardClick?.Invoke(this, BindedCondition);
            }
        }
    }
}
