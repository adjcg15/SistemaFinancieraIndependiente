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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SFIClient.Controlls
{
    /// <summary>
    /// Lógica de interacción para ClientControll.xaml
    /// </summary>
    public partial class ClientControll : UserControl
    {
        public event EventHandler ButtonSeePersonalInformation;
        public event EventHandler ButtonModifyBankAccount;
        public event EventHandler ButtonModifyPersonalInformation;
        public event EventHandler ButtonModifyWorkCenter;
        public event EventHandler ButtonModifyPersonalReferences;
        public event EventHandler ButtonApplyForCredit;
        public ClientControll()
        {
            InitializeComponent();
        }

        private void BtnSeePersonalInformationClick(object sender, RoutedEventArgs e)
        {
            ButtonSeePersonalInformation?.Invoke(this, EventArgs.Empty);
        }

        private void BtnModifyBankAccountClick(object sender, RoutedEventArgs e)
        {
            ButtonModifyBankAccount?.Invoke(this, EventArgs.Empty);
        }

        private void BtnModifyPersonalInformationClick(object sender, RoutedEventArgs e)
        {
            ButtonModifyPersonalInformation?.Invoke(this, EventArgs.Empty);
        }

        private void BtnModifyWorkCenterClick(object sender, RoutedEventArgs e)
        {
            ButtonModifyWorkCenter?.Invoke(this, EventArgs.Empty);
        }

        private void BtnModifyPersonalReferencesClick(object sender, RoutedEventArgs e)
        {
            ButtonModifyPersonalReferences?.Invoke(this, EventArgs.Empty);
        }

        private void BtnApplyForCreditClick(object sender, RoutedEventArgs e)
        {
            ButtonApplyForCredit?.Invoke(this, EventArgs.Empty);
        }
    }
}