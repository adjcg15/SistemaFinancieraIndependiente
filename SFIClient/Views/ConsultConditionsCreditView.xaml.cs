using SFIClient.Controlls;
using SFIClient.SFIServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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
    /// Lógica de interacción para ConsultConditionsCreditView.xaml
    /// </summary>
    public partial class ConsultConditionsCreditView : Page
    {
        private readonly CreditConditionsServiceClient creditServiceClient = new CreditConditionsServiceClient();
        private List<CreditCondition> creditConditions = new List<CreditCondition>();
        public ConsultConditionsCreditView()
        {
            InitializeComponent();
            LoadCreditConditions();
        }
        private void LoadCreditConditions()
        {
            try
            {
                CreditConditionsServiceClient creditConditiionClient = new CreditConditionsServiceClient();
                creditConditions = creditConditiionClient.RecoverAllCreditConditions().ToList();
                if (creditConditions.Count != 0)
                {
                    AddCreditConditions(creditConditions);
                }
                else
                {
                    SkpRegisterCreditCondition.Visibility = Visibility.Collapsed;
                    SkpNoRegisteredCreditConditions.Visibility = Visibility.Visible;
                }
            }
            catch (FaultException fe)
            {
                MessageBox.Show(fe.Message);
            }
            catch (EndpointNotFoundException)
            {
                MessageBox.Show("No fue posible establecer la conexión con el servicio, intente más tarde");
                RedirectToMainMenu();
            }
            catch (CommunicationException)
            {
                MessageBox.Show("No fue posible establecer la conexión con el servicio, intente más tarde");
                RedirectToMainMenu();
            }
        }
        private void AddCreditConditions(List<CreditCondition> clientsList)
        {
            SkpNoRegisteredCreditConditions.Visibility = Visibility.Collapsed;
            SkpRegisterCreditCondition.Visibility = Visibility.Collapsed;
            for (int i = 0; i < clientsList.Count; i++)
            {
                ShowCreditCondition(clientsList[i]);
            }
        }
        private void ShowCreditCondition(CreditCondition creditCondition)
        {
            CreditConditionControl creditConditionControl = new CreditConditionControl();
            creditConditionControl.LblCreditConditionName.Content = creditCondition.Identifier;
            creditConditionControl.LblIsActive.Content = creditCondition.IsActive ? "Activa" : "Inactiva";
            creditConditionControl.LblApplyIVA.Content = creditCondition.IsIvaApplied ? "Aplica IVA" : "No aplica IVA";
            creditConditionControl.LblPaymentMonths.Content = creditCondition.PaymentMonths;
            creditConditionControl.LblInterestRate.Content = creditCondition.InterestRate;
            creditConditionControl.LblInterestOnArrears.Content = creditCondition.InterestOnArrears;
            creditConditionControl.LblAdvancePaymentReduction.Content = creditCondition.AdvancePaymentReduction;
            creditConditionControl.DataContext = creditCondition;
            ItcCreditCondition.Items.Add(creditConditionControl);
        }
        private void BtnNewCreditConditionClick(object sender, RoutedEventArgs e)
        {
            RegisterCreditConditionView registerCreditCondition = new RegisterCreditConditionView();
            this.NavigationService.Navigate(registerCreditCondition);
            NavigationService.RemoveBackEntry();
        }
        private void RedirectToMainMenu()
        {
            NavigationService.Navigate(new MainMenuController());
        }

        private void BtnReturnToPreviousPageClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainMenuController());
        }
    }
}
