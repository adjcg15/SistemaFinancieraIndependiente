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
            //TODO: implementar redirección a menú principal
        }

        private void DisableFilterOptions()
        {
            BtnSearchCredits.IsEnabled = false;
            BtnApplyFilters.IsEnabled = false;
            BtnRestartFilters.IsEnabled = false;
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
            SkpCreditsList.Children.Clear();

            showedCreditsList.ForEach(credit =>
            {
                var creditCard = new CreditsListCreditControl(credit);
                //creditConditionCard.CardClick += HighlightCreditConditionCard;

                SkpCreditsList.Children.Add(creditCard);
            });
        }

        private void BtnReturnToPreviousScreenClick(object sender, RoutedEventArgs e)
        {
            //TODO: implementar redirección a pantalla previa
        }
    }
}
