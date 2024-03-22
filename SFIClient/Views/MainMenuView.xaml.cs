using SFIClient.Session;
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
    public partial class MainMenuController : Page
    {
        public MainMenuController()
        {
            InitializeComponent();

            ShowEmployeeName();
            ShowMenuAccordingEmployeeRole();
        }

        private void ShowEmployeeName()
        {
            SpnEmployeeName.Inlines.Clear();
            SpnEmployeeName.Inlines.Add(new Run($"{SystemSession.Employee.Name} {SystemSession.Employee.LastName}"));
        }

        private void ShowMenuAccordingEmployeeRole()
        {
            switch (SystemSession.Employee.EmployeeRole.Name)
            {
                case Security.Roles.MANAGER:
                    SkpManagerOptions.Visibility = Visibility.Visible;
                    break;
                case Security.Roles.DEBT_COLLECTOR:
                    SkpDebtCollectorOptions.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void BdrGrantingPoliciesClick(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new CreditGrantingPolicyListController());
        }

        private void BdrCreditConditionsClick(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new ConsultConditionsCreditView());
        }

        private void BdrGeneralEfficiencyClick(object sender, MouseButtonEventArgs e)
        {
            //TODO: redirect to generate general efficiency
        }

        private void BdrCreditsListClick(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new CreditsListController());
        }

        private void BtnLogOutClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginController());
            SystemSession.Employee = null;
        }
    }
}
