using SFIClient.Controlls;
using SFIClient.SFIServices;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace SFIClient.Views
{
    /// <summary>
    /// Lógica de interacción para ModifyCreditConditionApplicableToCreditView.xaml
    /// </summary>
    public partial class ModifyCreditConditionApplicableToCreditController : Page
    {
        private Credit credit;
        private int creditTypeId;
        CreditsServiceClient creditsService = new CreditsServiceClient();
        public ModifyCreditConditionApplicableToCreditController(Credit credit)
        {
            InitializeComponent();
            this.credit = credit;
            this.creditTypeId = creditsService.GetCreditTypeIdByCreditInvoice(credit.Invoice);
            ShowCreditAndClientInformation();
            LoadCreditConditionsByCreditType(this.creditTypeId);
        }
        private void ShowCreditAndClientInformation()
        {
            TbkCreditInvoice.Text = credit.Invoice;
            TbkClientName.Inlines.Clear();
            TbkClientName.Inlines.Add(new Run($"{credit.Client.Name} {credit.Client.Surname} {credit.Client.LastName}"));

            BldApprovedAmountCredit.Inlines.Clear();
            BldApprovedAmountCredit.Inlines.Add(new Run($"{credit.AmountApproved} MXN"));

            SpnClientSalaryAmmount.Inlines.Clear();
        }
        private void ShowReturnPreviousPageConfirmationDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                $"¿Está seguro de que desea regresar a la lista de créditos? Todos los cambios sin guardar se perderán",
                "Regresar a ventana previa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );
            if (buttonClicked == MessageBoxResult.Yes)
            {
                RedirectToConsultCreditsList();
            }
        }
        private void LoadCreditConditionsByCreditType(int creditConditionId)
        {
            CreditConditionsServiceClient creditConditionsService = new CreditConditionsServiceClient();
            try
            {
                List<CreditCondition> applicableCreditConditions = creditConditionsService
                    .RecoverCreditConditionsByCreditType(creditConditionId).ToList();
                ShowApplicableCreditConditions(applicableCreditConditions);
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
            GrdEmptyConditionsMessage.Visibility = applicableCreditConditions.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            SkpCreditConditions.Visibility = applicableCreditConditions.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            SkpCreditConditions.Children.Clear();

            foreach (var condition in applicableCreditConditions)
            {
                var creditConditionCard = new CreditApplicationCreditConditionControl(condition);
                creditConditionCard.CardClick += (sender, e) => HighlightCreditConditionCard(sender as CreditApplicationCreditConditionControl);

                // TODO resaltar condicion de credito asociada actualmente, posible solucion
                /*if (condition == currentAssociatedCondition)
                {
                    HighlightCreditConditionCard(creditConditionCard);
                }*/

                SkpCreditConditions.Children.Add(creditConditionCard);
            }
        }
        private void HighlightCreditConditionCard(CreditApplicationCreditConditionControl creditConditionCard)
        {
            SolidColorBrush primaryColor = (SolidColorBrush)Application.Current.Resources["PrimaryColor"];
            SolidColorBrush lightGray = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ECECEC"));
            SolidColorBrush defaultBorderColor = (SolidColorBrush)Application.Current.Resources["LightGray"];
            SolidColorBrush defaultBgColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FAFAFA"));

            foreach (var child in SkpCreditConditions.Children)
            {
                if (child is CreditApplicationCreditConditionControl conditionControl && conditionControl != creditConditionCard)
                {
                    conditionControl.BdrCreditConditionCard.BorderBrush = defaultBorderColor;
                    conditionControl.BdrCreditConditionCard.Background = defaultBgColor;
                }
            }

            creditConditionCard.BdrCreditConditionCard.BorderBrush = primaryColor;
            creditConditionCard.BdrCreditConditionCard.Background = lightGray;

            // Aquí puedes realizar cualquier otra lógica necesaria después de resaltar la condición de crédito
        }
        private CreditCondition GetCurrentAssociatedCondition()
        {
            //TODO
            return new CreditCondition { /* Datos de la condición de crédito asociada actualmente */ };
        }
        private void ShowDiscardChangesConfirmationDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                $"¿Está seguro de que desea cancelar modificacion de la condición de crédito aplicable a crédito? " +
                $"Todos los cambios sin guardar se perderán",
                "Descartar cambios",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );
            if (buttonClicked == MessageBoxResult.Yes)
            {
                RedirectToConsultCreditsList();
            }
        }
        private void ShowErrorRecoveringCreditConditionsDialog(string message)
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                $"¿Está seguro de que desea cancelar modificacion de la condición de crédito aplicable a crédito? " +
                $"Todos los cambios sin guardar se perderán",
                "Descartar cambios",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );
            if (buttonClicked == MessageBoxResult.Yes)
            {
                RedirectToConsultCreditsList();
            }
        }
        private void RedirectToConsultCreditsList()
        {
            CreditsListController creditList = new CreditsListController();
            this.NavigationService.Navigate(creditList);
            NavigationService.RemoveBackEntry();
        }
        private void BtnSaveChangesClick(object sender, RoutedEventArgs e)
        {

        }
        private void BtnCancelChangesClick(object sender, RoutedEventArgs e)
        {
            ShowDiscardChangesConfirmationDialog();
        }

        private void BtnBackPreviousPageClick(object sender, RoutedEventArgs e)
        {
            ShowReturnPreviousPageConfirmationDialog();
        }
    }
}
