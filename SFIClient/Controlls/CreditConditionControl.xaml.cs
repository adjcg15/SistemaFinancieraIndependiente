using SFIClient.Views;
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
    /// Lógica de interacción para CreditConditionControl.xaml
    /// </summary>
    public partial class CreditConditionControl : UserControl
    {
        public event EventHandler ButtonEditCreditCondition;
        public CreditConditionControl()
        {
            InitializeComponent();
        }
        private void BtnEditCreditConditionClick(object sender, RoutedEventArgs e)
        {
            ButtonEditCreditCondition?.Invoke(this, EventArgs.Empty);
        }
    }
}
