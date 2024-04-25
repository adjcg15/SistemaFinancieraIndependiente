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
    /// Lógica de interacción para CreditAuthorizationDictumView.xaml
    /// </summary>
    public partial class CreditAuthorizationDictumController : Page
    {
        private readonly CreditGrantingPoliciesClient creditGrantingPoliciesClient = new CreditGrantingPoliciesClient();
        private CreditGrantingPolicy[] creditGrantingPolicesList;
        public CreditAuthorizationDictumController()
        {
            InitializeComponent();
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            LoadCreditGrantingPolices();
        }

        private void LoadCreditGrantingPolices()
        {
            try
            {
                creditGrantingPolicesList = creditGrantingPoliciesClient.GetAllCreditGrantingPolicies();
                if (creditGrantingPolicesList.Length != 0)
                {
                    LoadCreditApplicationContent("1234567890");
                }
                else
                {
                    ShowNoPolicesExistsAlertDialog();
                    RedirectToCreditApplicationsListView();
                }
            }
            catch (FaultException<ServiceFault> fe)
            {
                ShowErrorRecoveringCreditGrantingPolices(fe.Message);
            }
            catch (EndpointNotFoundException)
            {
                string errorMessage = "Por el momento el servidor no se encuentra disponible, intente más tarde";
                ShowErrorRecoveringCreditGrantingPolices(errorMessage);
                RedirectToCreditApplicationsListView();
            }
            catch (CommunicationException)
            {
                string errorMessage = "Por el momento el servidor no se encuentra disponible, intente más tarde";
                ShowErrorRecoveringCreditGrantingPolices(errorMessage);
                RedirectToCreditApplicationsListView();
            }
        }

        private void ShowNoPolicesExistsAlertDialog()
        {
            MessageBoxResult messageBoxResult = MessageBox.Show(
                "Por el momento no se puede generar ningún dictamen",
                "No existen políticas registradas",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }

        private void ShowErrorRecoveringCreditGrantingPolices(string message)
        {

        }

        private void RedirectToCreditApplicationsListView()
        {
            NavigationService.Navigate(new SearchClientByRFCController());
        }

        private void LoadCreditApplicationContent(string invoice)
        {

        }

        private void ShowErrorRecoveringCreditApplicationContent(string message)
        {

        }

        private void ShowCreditApplicationContent(CreditApplication creditApplication)
        {

        }

        private void BtnDownloadINEClick(object sender, RoutedEventArgs e)
        {

        }

        private void BtnDownloadProofOfAddressSClick(object sender, RoutedEventArgs e)
        {

        }

        private void BtnDownloadProofOfIncomeClick(object sender, RoutedEventArgs e)
        {

        }

        private void SaveDocumentToPath()
        {

        }
    }
}
