﻿using SFIClient.Controlls;
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
    public partial class CreditsListController : Page
    {
        private List<Credit> allCreditsList;
        private List<Credit> showedCreditsList;

        public CreditsListController()
        {
            InitializeComponent();
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            LoadAllCredits();
        }

        private void LoadAllCredits()
        {
            CreditsServiceClient creditsService = new CreditsServiceClient();

            try
            {
                List<Credit> creditsList = creditsService.GetAllCredits().ToList();
                
                if(creditsList.Count == 0)
                {
                    DisableFilterOptions();
                    ShowEmptyCreditsListMessage();
                }
                else
                {
                    allCreditsList = creditsList;
                    showedCreditsList = creditsList;
                    ShowCreditsList();
                }
            }
            catch (FaultException<ServiceFault> fault)
            {
                ShowErrorRecoveringCreditsDialog(fault.Detail.Message);
            }
            catch (EndpointNotFoundException)
            {
                string errorMessage = "El servidor no se encuentra disponible, intente más tarde";
                ShowErrorRecoveringCreditsDialog(errorMessage);
            }
            catch (CommunicationException)
            {
                string errorMessage = "No fue posible acceder a la información debido a un error de conexión";
                ShowErrorRecoveringCreditsDialog(errorMessage);
            }
        }

        private void ShowErrorRecoveringCreditsDialog(string message)
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                message,
                "Créditos no disponibles",
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

        private void DisableFilterOptions()
        {
            BtnSearchCredits.IsEnabled = false;
            BtnRestartFilters.IsEnabled = false;
            BtnApplyFilters.IsEnabled = false;
            DpkFromDate.IsEnabled = false;
            DpkToDate.IsEnabled = false;
            TbSearchbar.IsEnabled = false;
            CkbCreditsInProgress.IsEnabled = false;
            CkbPaidCredits.IsEnabled = false;
        }

        private void ShowEmptyCreditsListMessage()
        {
            SkpEmptyCreditsListMessage.Visibility = Visibility.Visible;
            SkpCreditsList.Visibility = Visibility.Hidden;
        }

        private void ShowCreditsList()
        {
            SkpEmptyCreditsListMessage.Visibility = Visibility.Collapsed;
            SkpCreditsList.Visibility = Visibility.Visible;

            SkpCreditsList.Children.Clear();

            showedCreditsList.ForEach(credit =>
            {
                var creditCard = new CreditsListCreditControl(credit);
                creditCard.BtnPaymentsTableClick += RedirectToPaymentsTable;
                creditCard.BtnMonthlyEfficiencyClick += RedirectToCreditMonthlyEfficiency;
                creditCard.BtnChangeConditionClick += RedirectToChangeApplicableCreditCondition;

                SkpCreditsList.Children.Add(creditCard);
            });
        }

        private void RedirectToPaymentsTable(object sender, Credit selectedCredit)
        {
            ConsultPaytableController consultPaytableView = new ConsultPaytableController(selectedCredit);
            NavigationService.Navigate(consultPaytableView);
        }

        private void RedirectToCreditMonthlyEfficiency(object sender, Credit selectedCredit)
        {
            NavigationService.Navigate(new CreditCollectionEfficiencyController(selectedCredit.Invoice));
        }

        private void RedirectToChangeApplicableCreditCondition(object sender, Credit selectedCredit)
        {
            ModifyCreditConditionApplicableToCreditController modifyCreditConditionWindow
                = new ModifyCreditConditionApplicableToCreditController(selectedCredit);
            NavigationService.Navigate(modifyCreditConditionWindow);
        }

        private void BtnSearchCreditsClick(object sender, RoutedEventArgs e)
        {
            List<Credit> filteredCreditsList = FilterCreditsList();
            showedCreditsList = filteredCreditsList;

            if(showedCreditsList.Count == 0)
            {
                ShowEmptyCreditsListMessage();
            }
            else
            {
                ShowCreditsList();
            }
        }

        private List<Credit> FilterCreditsList()
        {
            string searchQuery = TbSearchbar.Text.Trim();
            DateTime fromDate = DpkFromDate.SelectedDate ?? DateTime.MinValue;
            DateTime toDate = DpkToDate.SelectedDate ?? DateTime.MaxValue;
            bool includeCreditsInProgress = (bool)CkbCreditsInProgress.IsChecked;
            bool includePaidCredits = (bool)CkbPaidCredits.IsChecked;

            return allCreditsList
                .Where(credit =>
                {
                    string clientFullName = credit.Client.Name;
                    clientFullName += (credit.Client.Surname != "" ? $" {credit.Client.Surname} " : " ");
                    clientFullName += credit.Client.LastName;

                    return credit.Invoice.ToLower().Contains(searchQuery.ToLower())
                        || clientFullName.ToLower().Contains(searchQuery.ToLower());
                })
                .Where(credit => fromDate <= credit.ApprovalDate)
                .Where(credit => credit.ApprovalDate <= toDate)
                .Where(credit =>
                {
                    bool matchFilter = true;

                    if(includeCreditsInProgress && !includePaidCredits)
                    {
                        matchFilter = !credit.SettlementDate.HasValue;
                    }
                    else if(includePaidCredits && !includeCreditsInProgress)
                    {
                        matchFilter = credit.SettlementDate.HasValue;
                    }
                    
                    return matchFilter;
                })
                .ToList();
        }

        private void BtnRestartFiltersClick(object sender, RoutedEventArgs e)
        {
            string searchQuery = TbSearchbar.Text.Trim();
            List<Credit> creditsFilteredJustBySearch = allCreditsList.Where(credit => 
            {
                string clientFullName = credit.Client.Name;
                clientFullName += (credit.Client.Surname != "" ? $" {credit.Client.Surname} " : " ");
                clientFullName += credit.Client.LastName;

                return credit.Invoice.ToLower().Contains(searchQuery.ToLower())
                    || clientFullName.ToLower().Contains(searchQuery.ToLower());
            })
            .ToList();

            showedCreditsList = creditsFilteredJustBySearch;

            if (showedCreditsList.Count == 0)
            {
                ShowEmptyCreditsListMessage();
            }
            else
            {
                ShowCreditsList();
            }

            DpkFromDate.SelectedDate = null;
            DpkToDate.SelectedDate = null;
            CkbCreditsInProgress.IsChecked = false;
            CkbPaidCredits.IsChecked = false;
        }

        private void BtnReturnToPreviousScreenClick(object sender, RoutedEventArgs e)
        {
            RedirectToMainMenu();
        }
    }
}
