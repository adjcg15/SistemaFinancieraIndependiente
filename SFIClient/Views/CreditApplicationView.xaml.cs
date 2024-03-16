using SFIClient.Controlls;
using SFIClient.SFIServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SFIClient.Views
{
    public partial class CredditApplicationController : Page
    {
        private string lastRequestedAmount = string.Empty;
        private string lastMinimumAcceptedAmount = string.Empty;
        private ObservableCollection<CreditType> LoadedCreditTypes { get; set; }

        public CredditApplicationController()
        {
            InitializeComponent();

            LoadCreditTypes();
        }

        private void LoadCreditTypes()
        {
            CreditsServiceClient creditsService = new CreditsServiceClient();
            
            try
            {
                List<CreditType> creditTypesRecovered = creditsService.GetAllCreditTypes().ToList();
                FillCreditTypesCombobox(creditTypesRecovered);
            }
            catch (FaultException<ServiceFault> fault)
            {
                ShowErrorRecoveringCreditTypesDialog(fault.Detail.Message);
            }
            catch (EndpointNotFoundException)
            {
                string errorMessage = "El servidor no se encuentra disponible, intente más tarde";
                ShowErrorRecoveringCreditTypesDialog(errorMessage);
            }
            catch (CommunicationException)
            {
                string errorMessage = "No fue posible acceder a la información debido a un error de conexión";
                ShowErrorRecoveringCreditTypesDialog(errorMessage);
            }
        }

        private void FillCreditTypesCombobox(List<CreditType> creditTypes)
        {
            ObservableCollection<CreditType> creditTypesPresented = new ObservableCollection<CreditType>();
            creditTypes.ForEach(creditType => creditTypesPresented.Add(creditType));

            CbCreditTypes.ItemsSource = creditTypesPresented;
        }

        private void ShowErrorRecoveringCreditTypesDialog(string message)
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                message,
                "Tipos de créditos no disponibles",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );

            if(buttonClicked == MessageBoxResult.OK)
            {
                RedirectToMainMenu();
            }
        }

        private void RedirectToMainMenu()
        {
            //TODO: redireccionar cuando exista el menú principal
            Console.WriteLine("Redireccionando al menú principal...");
        }

        private void CbCreditTypesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(CbCreditTypes.SelectedIndex != -1)
            {
                CreditType selectedCreditType = (CreditType)CbCreditTypes.SelectedItem;
                //TODO: settear tipo de crédito en nueva solicitud
                LoadCreditConditionsByCreditType(selectedCreditType.Identifier);
            }
        }

        private void LoadCreditConditionsByCreditType(int creditConditionId)
        {
            CreditConditionsServiceClient creditConditionsService = new CreditConditionsServiceClient();

            try
            {
                List<CreditCondition> creditConditionsRecovered = 
                    creditConditionsService.RecoverCreditConditionsByCreditType(creditConditionId).ToList();
                ShowApplicableCreditConditions(creditConditionsRecovered);
            }
            catch (FaultException<ServiceFault> fault)
            {
                ShowErrorRecoveringCreditConditionsDialog(fault.Detail.Message);
            }
            catch (EndpointNotFoundException)
            {
                string errorMessage = "El servidor no se encuentra disponible, intente más tarde";
                ShowErrorRecoveringCreditConditionsDialog(errorMessage);
            }
            catch (CommunicationException)
            {
                string errorMessage = "No fue posible acceder a la información debido a un error de conexión";
                ShowErrorRecoveringCreditConditionsDialog(errorMessage);
            }
        }

        private void ShowApplicableCreditConditions(List<CreditCondition> applicableCreditConditions)
        {
            GrdEmptyConditionsMessage.Visibility = Visibility.Collapsed;
            SkpApplicableCreditConditions.Visibility = Visibility.Visible;
            SkpApplicableCreditConditions.Children.Clear();

            applicableCreditConditions.ForEach(conditon =>
            {
                var creditConditionCard = new CreditApplicationCreditConditionControl(conditon);
                creditConditionCard.CardClick += ApplicableCreditConditionClick;

                SkpApplicableCreditConditions.Children.Add(creditConditionCard);
            });
        }

        private void ApplicableCreditConditionClick(object sender, CreditCondition selectedCondition)
        {
            foreach (var child in SkpApplicableCreditConditions.Children)
            {
                if (child is CreditApplicationCreditConditionControl creditConditionCard)
                {

                    if(creditConditionCard.BindedCondition.Identifier == selectedCondition.Identifier)
                    {
                        SolidColorBrush primaryColor = (SolidColorBrush)Application.Current.Resources["PrimaryColor"];
                        SolidColorBrush lightGray = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ECECEC"));

                        creditConditionCard.BdrCreditConditionCard.BorderBrush = primaryColor;
                        creditConditionCard.BdrCreditConditionCard.Background = lightGray;

                        //TODO: asignar condición seleccionada a crédito
                    }
                    else
                    {
                        SolidColorBrush defaultBorderColor = (SolidColorBrush)Application.Current.Resources["LightGray"];
                        SolidColorBrush defaultBgColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FAFAFA"));

                        creditConditionCard.BdrCreditConditionCard.BorderBrush = defaultBorderColor;
                        creditConditionCard.BdrCreditConditionCard.Background = defaultBgColor;
                    }
                }
            }
        }

        private void ShowErrorRecoveringCreditConditionsDialog(string message)
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                message,
                "Condiciones de crédito no disponibles",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );

            if (buttonClicked == MessageBoxResult.OK)
            {
                RedirectToMainMenu();
            }
        }

        private void TbRequestedAmountTextChanged(object sender, TextChangedEventArgs e)
        {
            string newAmount = TbRequestedAmount.Text;
            bool isWritingNewAmmount = newAmount != lastRequestedAmount;

            if (isWritingNewAmmount)
            {
                if (newAmount != string.Empty && !IsValidMoneyAmount(newAmount))
                {
                    TbRequestedAmount.Text = lastRequestedAmount;
                    TbRequestedAmount.CaretIndex = TbRequestedAmount.Text.Length;
                }
                else
                {
                    lastRequestedAmount = newAmount;
                }
            }
        }

        private void TbMinimumAcceptedAmountTextChanged(object sender, TextChangedEventArgs e)
        {
            string newAmount = TbMinimumAcceptedAmount.Text;
            bool isWritingNewAmmount = newAmount != lastMinimumAcceptedAmount;

            if (isWritingNewAmmount)
            {
                if (newAmount != string.Empty && !IsValidMoneyAmount(newAmount))
                {
                    TbMinimumAcceptedAmount.Text = lastMinimumAcceptedAmount;
                    TbMinimumAcceptedAmount.CaretIndex = TbMinimumAcceptedAmount.Text.Length;
                }
                else
                {
                    lastMinimumAcceptedAmount = newAmount;
                }
            }
        }

        //TODO: ver si se puede crear un helper
        private bool IsValidMoneyAmount(string amount)
        {
            return amount.Length <= 7 
                && double.TryParse(amount, out double result) 
                && result >= 0 
                && result <= 1000000;
        }
    }
}
