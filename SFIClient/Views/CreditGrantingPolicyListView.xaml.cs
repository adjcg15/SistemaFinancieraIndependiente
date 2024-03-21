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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SFIClient.Views
{
    public partial class CreditGrantingPolicyListController : Page
    {
        public CreditGrantingPolicyListController()
        {
            InitializeComponent();

            SizeChanged += CreditGrantingPolicyListViewSizeChanged;
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            LoadAllCreditGrantingPolicies();
        }

        private void CreditGrantingPolicyListViewSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Page page = (Page)sender;
            double pageWidth = page.ActualWidth;

            ItemsPanelTemplate grantingPoliciesPanelTemplate = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(UniformGrid)));
            int totalPanelColumns = 1;
            if(pageWidth > 800)
            {
                totalPanelColumns = 2;
            }
            
            if(pageWidth > 1200)
            {
                totalPanelColumns = 3;
            }

            if (pageWidth > 1700)
            {
                totalPanelColumns = 4;
            }

            grantingPoliciesPanelTemplate.VisualTree.SetValue(UniformGrid.ColumnsProperty, totalPanelColumns);
            ItcGrantingPolicies.ItemsPanel = grantingPoliciesPanelTemplate;
        }

        private void LoadAllCreditGrantingPolicies()
        {
            CreditGrantingPoliciesClient policiesService = new CreditGrantingPoliciesClient();

            try
            {
                List<CreditGrantingPolicy> policiesList = policiesService.GetAllCreditGrantingPolicies().ToList();
                if (policiesList.Count == 0)
                {
                    ShowEmptyPoliciesListMessage();
                }
                else
                {
                    ShowPoliciesList(policiesList);
                }
            }
            catch (FaultException<ServiceFault> fault)
            {
                ShowErrorRecoveringPoliciesDialog(fault.Detail.Message);
            }
            catch (EndpointNotFoundException)
            {
                string errorMessage = "El servidor no se encuentra disponible, intente más tarde";
                ShowErrorRecoveringPoliciesDialog(errorMessage);
            }
            catch (CommunicationException)
            {
                string errorMessage = "No fue posible acceder a la información debido a un error de conexión";
                ShowErrorRecoveringPoliciesDialog(errorMessage);
            }
        }

        private void ShowErrorRecoveringPoliciesDialog(string message)
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                message,
                "Políticas de otorgamiento no disponibles",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );

            if (buttonClicked == MessageBoxResult.OK)
            {
                RedirectToMainMenu();
            }
        }

        private void RedirectToMainMenu()
        {
            NavigationService.Navigate(new MainMenuController());
        }

        private void ShowEmptyPoliciesListMessage()
        {
            SkpEmptyPoliciesListMessage.Visibility = Visibility.Visible;
            SkpMainView.Visibility = Visibility.Collapsed;
        }

        private void ShowPoliciesList(List<CreditGrantingPolicy> policiesList)
        {
            SkpEmptyPoliciesListMessage.Visibility = Visibility.Collapsed;
            SkpMainView.Visibility = Visibility.Visible;

            policiesList.ForEach(policy =>
            {
                GrantingPoliciesListPolicyControl policyCard = new GrantingPoliciesListPolicyControl(policy);
                policyCard.BtnEditPolicyClick += RedirectToEditPolicy;

                ItcGrantingPolicies.Items.Add(policyCard);
            });
        }

        public void BtnReturnToPreviousScreenClick(object sender, RoutedEventArgs e)
        {
            RedirectToMainMenu();
        }

        private void BtnRegisterPolicyClick(object sender, RoutedEventArgs e)
        {
            //TODO: redirect to create policy view
        }

        private void RedirectToEditPolicy(object sender, CreditGrantingPolicy selectedPolicy)
        {
            //TODO: implementar redirección
        }
    }
}
