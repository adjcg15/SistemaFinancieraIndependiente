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
    /// Lógica de interacción para CreditAuthorizationDictumPolicyControl.xaml
    /// </summary>
    public partial class CreditAuthorizationDictumPolicyControl : UserControl
    {
        public event EventHandler CheckCreditGrantingPolicy;
        public event EventHandler UncheckCreditGrantingPolicy;
        public CreditAuthorizationDictumPolicyControl()
        {
            InitializeComponent();
        }

        private void CkbSelectCreditGrantingPolicyChecked(object sender, RoutedEventArgs e)
        {
            CheckCreditGrantingPolicy?.Invoke(this, EventArgs.Empty);
        }

        private void CkbSelectCreditGrantingPolicyUnchecked(object sender, RoutedEventArgs e)
        {
            UncheckCreditGrantingPolicy?.Invoke(this, EventArgs.Empty);
        }
    }
}
