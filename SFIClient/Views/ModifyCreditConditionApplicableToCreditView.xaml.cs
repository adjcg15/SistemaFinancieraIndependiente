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
        private CreditApplicationCreditConditionControl selectedConditionControl;
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
            CreditCondition currentAssociatedCondition = GetCurrentAssociatedCondition();

            foreach (var condition in applicableCreditConditions)
            {
                var creditConditionCard = new CreditApplicationCreditConditionControl(condition);
                creditConditionCard.CardClick += (sender, e) =>
                {
                    HighlightCreditConditionCard(creditConditionCard);
                    selectedConditionControl = creditConditionCard;
                };

                if (currentAssociatedCondition != null && creditConditionCard.BindedCondition.Identifier == currentAssociatedCondition.Identifier)
                {
                    HighlightCreditConditionCard(creditConditionCard);
                }

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
        }
        private CreditCondition GetCurrentAssociatedCondition()
        {
            CreditConditionsServiceClient creditConditionsService = new CreditConditionsServiceClient();
            CreditCondition currentAssociatedCondition = creditConditionsService.GetCurrentCreditConditionByCreditInvoice(credit.Invoice);
            Console.WriteLine($"Current associated condition: {currentAssociatedCondition?.Identifier}");
            return currentAssociatedCondition;
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
                message,
                "Condiciones de crédito no disponibles",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );

            if (buttonClicked == MessageBoxResult.OK)
            {
                RedirectToConsultCreditsList();
            }
        }
        private void ShowErrorSameConditionSelectedDialog(string message)
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                message,
               "Avertencia",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
        }
        private void RedirectToConsultCreditsList()
        {
            try
            {
                if (NavigationService != null)
                {
                    CreditsListController creditList = new CreditsListController();
                    NavigationService.Navigate(creditList);
                    NavigationService.RemoveBackEntry();
                }
            }
            catch (FaultException<ServiceFault> fault)
            {
                ShowErrorChangeCreditConditionDialog(fault.Detail.Message);
            }
            catch (EndpointNotFoundException)
            {
                string errorMessage = "El servidor no se encuentra disponible, intente más tarde";
                ShowErrorChangeCreditConditionDialog(errorMessage);
            }
            catch (CommunicationException)
            {
                string errorMessage = "No fue posible acceder a la información debido a un error de conexión";
                ShowErrorChangeCreditConditionDialog(errorMessage);
            }
        }
        private void BtnSaveChangesClick(object sender, RoutedEventArgs e)
        {
            CreditApplicationCreditConditionControl selectedConditionControl = SkpCreditConditions.Children.OfType<CreditApplicationCreditConditionControl>()
                                                                        .FirstOrDefault(ccc => ccc.IsSelected);
            if (selectedConditionControl != null)
            {
                CreditCondition currentAssociatedCondition = GetCurrentAssociatedCondition();
                if (selectedConditionControl.BindedCondition.Identifier != currentAssociatedCondition?.Identifier)
                {
                    ShowModifyCreditConditionDialog();
                }
                else
                {
                    ShowErrorSameConditionSelectedDialog("Por favor, seleccione una condición de crédito diferente a la actual.");
                }
            }
            else
            {
                ShowErrorSameConditionSelectedDialog("Por favor, seleccione una condición de crédito diferente a la actual.");
            }
        }
        private void ShowModifyCreditConditionDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                $"¿Está seguro de cambiar la condición aplicable al crédito?",
                "Confirmación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (buttonClicked == MessageBoxResult.Yes)
            {
                try
                {
                    bool isFirstPaymentReconciled = creditsService.VerifyFirstPaymentReconciled(credit.Invoice);

                    if (!isFirstPaymentReconciled)
                    {
                        ChangeCreditCondition();
                    }
                    else
                    {
                        ShowErrorChangeCreditConditionDialog("El primer pago de esta factura de crédito ya ha sido reconciliado.");
                    }
                }
                catch (Exception ex)
                {
                    ShowErrorChangeCreditConditionDialog(ex.Message);
                }
            }
        }


        private void ShowErrorChangeCreditConditionDialog(string message)
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                message,
                "Error al cambiar la condición de crédito",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );

            if (buttonClicked == MessageBoxResult.OK)
            {
                RedirectToConsultCreditsList();
            }
        }
        private void ChangeCreditCondition()
        {
            CreditsServiceClient creditService = new CreditsServiceClient();

            try
            {
                bool success = creditService.AssociateNewCreditCondition(credit.Invoice, selectedConditionControl.BindedCondition.Identifier);

                if (success)
                {
                    ShowSuccessChangeCreditConditionDialog();
                }
                else
                {
                    ShowErrorChangeCreditConditionDialog("No fue posible cambiar la condición de crédito.");
                }
            }
            catch (FaultException<ServiceFault> fault)
            {
                ShowErrorChangeCreditConditionDialog(fault.Detail.Message);
            }
            catch (EndpointNotFoundException)
            {
                string errorMessage = "El servidor no se encuentra disponible, intente más tarde";
                ShowErrorChangeCreditConditionDialog(errorMessage);
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex.Message);
                string errorMessage = "No fue posible guardar la información debido a un error de conexión";
                ShowErrorChangeCreditConditionDialog(errorMessage);
            }
        }
        private void ShowSuccessChangeCreditConditionDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                "La nueva condición de crédito ha sido asociada correctamente",
                "Modiciación de condición de crédito",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            if (buttonClicked == MessageBoxResult.OK)
            {
                RedirectToConsultCreditsList();
            }
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