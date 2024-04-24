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
    public partial class CreditApplicationsListController : Page
    {
        private List<CreditApplication> allCreditApplicationsList;
        private List<CreditApplication> showedCreditApplicationsList;

        public CreditApplicationsListController()
        {
            InitializeComponent();
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            LoadAllCreditApplications();
        }

        private void LoadAllCreditApplications()
        {
            CreditsServiceClient creditsService = new CreditsServiceClient();

            try
            {
                List<CreditApplication> creditApplicationsList = creditsService.GetAllCreditApplications().ToList();
                if (creditApplicationsList.Count == 0)
                {
                    DisableFilterOptions();
                    ShowEmptyCreditApplicationsListMessage();
                }
                else
                {
                    allCreditApplicationsList = creditApplicationsList;
                    showedCreditApplicationsList = creditApplicationsList;
                    ShowCreditApplicationsList();
                }
            }
            catch (FaultException<ServiceFault> fault)
            {
                ShowErrorRecoveringCreditApplicationsDialog(fault.Detail.Message);
            }
            catch (EndpointNotFoundException)
            {
                string errorMessage = "Servidor no disponible. " +
                    "Por el momento el servidor no se encuentra disponible, intente más tarde";
                ShowErrorRecoveringCreditApplicationsDialog(errorMessage);
            }
            catch (CommunicationException)
            {
                string errorMessage = "Error de conexión. " +
                    "Ocurrió un error de conexión, por favor compruebe su conexión de Internet e intente de nuevo";
                ShowErrorRecoveringCreditApplicationsDialog(errorMessage);
            }
        }

        private void ShowErrorRecoveringCreditApplicationsDialog(string message)
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                message,
                "Solicitudes de crédito no disponibles",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );

            if (buttonClicked == MessageBoxResult.OK)
            {
                RedirectToLogin();
            }
        }

        private void RedirectToLogin()
        {
            NavigationService.Navigate(new LoginController());
        }

        private void DisableFilterOptions()
        {
            BtnSearchCreditApplications.IsEnabled = false;
            BtnRestartFilters.IsEnabled = false;
            DpkFromDate.IsEnabled = false;
            DpkToDate.IsEnabled = false;
            TbSearchbar.IsEnabled = false;
            CkbCreditApplicationsWaitingDictum.IsEnabled = false;
            CkbCreditApplicationsRejected.IsEnabled = false;
            CkbPaidCreditApplicationsApproved.IsEnabled = false;
        }

        private void ShowEmptyCreditApplicationsListMessage()
        {
            SkpEmptyCreditApplicationsListMessage.Visibility = Visibility.Visible;
            SkpCreditApplicationsList.Visibility = Visibility.Hidden;
        }

        private void ShowCreditApplicationsList()
        {
            SkpEmptyCreditApplicationsListMessage.Visibility = Visibility.Collapsed;
            SkpCreditApplicationsList.Visibility = Visibility.Visible;

            SkpCreditApplicationsList.Children.Clear();

            showedCreditApplicationsList.ForEach(credit =>
            {
                var applicationCard = new ApplicationsListCreditApplicationControl(credit);
                applicationCard.WriteDictumClick += BtnGenerateDictumClick;

                SkpCreditApplicationsList.Children.Add(applicationCard);
            });
        }

        private void BtnGenerateDictumClick(object sender, CreditApplication selectedApplication)
        {
            //TODO: implementar redirección
        }

        private void BtnRestartFiltersClick(object sender, RoutedEventArgs e)
        {

        }

        private void BtnSearchCreditApplicationsClick(object sender, RoutedEventArgs e)
        {

        }

        private void BtnReturnToPreviousScreenClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
