using SFIClient.SFIServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
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
    public partial class CredditApplicationController : Page
    {
        string lastRequestedAmount = string.Empty;
        string lastMinimumAcceptedAmount = string.Empty;

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
                ShowCreditTypesInCombobox(creditTypesRecovered);
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

        private void ShowCreditTypesInCombobox(List<CreditType> creditTypes)
        {
            
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
