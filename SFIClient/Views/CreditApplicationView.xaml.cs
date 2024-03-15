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
    public partial class CredditApplicationController : Page
    {
        string lastRequestedAmount = string.Empty;
        string lastMinimumAcceptedAmount = string.Empty;

        public CredditApplicationController()
        {
            InitializeComponent();
        }

        private void TbRequestedAmountTextChanged(object sender, TextChangedEventArgs e)
        {
            string newAmount = TbRequestedAmount.Text;
            bool isWritingNewAmmount = newAmount != lastRequestedAmount;

            if (isWritingNewAmmount)
            {
                if (newAmount != string.Empty && !IsValidMoneyAmount(newAmount))
                {
                    TbRequestedAmount.Text = lastRequestedAmount;
                    TbRequestedAmount.CaretIndex = TbRequestedAmount.Text.Length;
                }
                else
                {
                    lastRequestedAmount = newAmount;
                }
            }
        }

        private void TbMinimumAcceptedAmountTextChanged(object sender, TextChangedEventArgs e)
        {
            string newAmount = TbMinimumAcceptedAmount.Text;
            bool isWritingNewAmmount = newAmount != lastMinimumAcceptedAmount;

            if (isWritingNewAmmount)
            {
                if (newAmount != string.Empty && !IsValidMoneyAmount(newAmount))
                {
                    TbMinimumAcceptedAmount.Text = lastMinimumAcceptedAmount;
                    TbMinimumAcceptedAmount.CaretIndex = TbMinimumAcceptedAmount.Text.Length;
                }
                else
                {
                    lastMinimumAcceptedAmount = newAmount;
                }
            }
        }

        //TODO: ver si se puede crear un helper
        private bool IsValidMoneyAmount(string amount)
        {
            return amount.Length <= 7 
                && double.TryParse(amount, out double result) 
                && result >= 0 
                && result <= 1000000;
        }
    }
}
