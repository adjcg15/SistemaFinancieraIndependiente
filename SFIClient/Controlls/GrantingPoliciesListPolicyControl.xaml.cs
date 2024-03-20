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

    public partial class GrantingPoliciesListPolicyControl : UserControl
    {
        public CreditGrantingPolicy BindedGrantingPolicy { get; }
        public event EventHandler<CreditGrantingPolicy> BtnEditPolicyClick;

        public GrantingPoliciesListPolicyControl(CreditGrantingPolicy policy)
        {
            InitializeComponent();

            BindedGrantingPolicy = policy;

            ShowCreditGrantingPolicyInformation();
        }

        private void ShowCreditGrantingPolicyInformation()
        {
            ShowPolicyStatus();

            SpnEffectiveDate.Inlines.Clear();
            SpnEffectiveDate.Inlines.Add(new Run(DateToolkit.FormatAsDDMMYYY(BindedGrantingPolicy.EffectiveDate)));

            TbkTitle.Text = BindedGrantingPolicy.Title;
            TbkDescription.Text = BindedGrantingPolicy.Description;
        }

        private void ShowPolicyStatus()
        {
            SolidColorBrush primaryColor = (SolidColorBrush)Application.Current.Resources["PrimaryColor"];
            SolidColorBrush darkPrimaryColor = (SolidColorBrush)Application.Current.Resources["DarkPrimaryColor"];
            SolidColorBrush gray = (SolidColorBrush)Application.Current.Resources["Gray"];

            if(BindedGrantingPolicy.IsActive)
            {
                BdrDecorator.Background = primaryColor;

                TbkStatus.Foreground = darkPrimaryColor;
                TbkStatus.Text = "Activa";
            }
            else
            {
                BdrDecorator.Background = gray;

                TbkStatus.Foreground = gray;
                TbkStatus.Text = "Inactiva";
            }
        }

        private void BtnEditGrantingPolicyClick(object sender, RoutedEventArgs e)
        {
            BtnEditPolicyClick?.Invoke(this, BindedGrantingPolicy);
        }
    }
}
