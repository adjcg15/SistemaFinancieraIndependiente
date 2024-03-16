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
        private CreditCondition applicableCondition;

        public CreditApplicationCreditConditionControl(CreditCondition applicableCondition)
        {
            InitializeComponent();
            this.applicableCondition = applicableCondition;

            ShowCreditConditionInformation();
        }

        private void ShowCreditConditionInformation()
        {
            TbkIdentifier.Text = applicableCondition.Identifier;
            TbkIva.Text = applicableCondition.IsIvaApplied ? "Aplica IVA" : "";

            SpnPaymentMonths.Inlines.Clear();
            SpnPaymentMonths.Inlines.Add(new Run(applicableCondition.PaymentMonths.ToString()));

            SpnInterestRate.Inlines.Clear();
            SpnInterestRate.Inlines.Add(new Run((applicableCondition.InterestRate * 100).ToString("0.0")));

            double interestOnArrears = applicableCondition.InterestOnArrears * 100;
            BldInterestOnArrears.Inlines.Clear();
            BldInterestOnArrears.Inlines.Add(new Run(interestOnArrears.ToString("0.0")));

            double advancedPaymentReduction = applicableCondition.AdvancePaymentReduction * 100;
            BldAdvancePaymentReduction.Inlines.Clear();
            BldAdvancePaymentReduction.Inlines.Add(new Run(advancedPaymentReduction.ToString("0.0")));
        }
    }
}
