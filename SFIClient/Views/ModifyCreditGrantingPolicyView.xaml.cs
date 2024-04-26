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
    public partial class ModifyCreditGrantingPolicyController : Page
    {
        private CreditGrantingPolicy editedPolicy;

        public ModifyCreditGrantingPolicyController(CreditGrantingPolicy policy)
        {
            InitializeComponent();

            editedPolicy = policy;
            ShowCreditGrantingPolicyInformation();
        }

        private void ShowCreditGrantingPolicyInformation()
        {
            TbPolicyTitle.Text = editedPolicy.Title;
            DpkEffectiveDate.SelectedDate = editedPolicy.EffectiveDate;
            TbPolicyDescription.Text = editedPolicy.Description;

            if (editedPolicy.IsActive)
            {
                RbActiveStatus.IsChecked = true;
            }
            else
            {
                RbInactiveStatus.IsChecked = true;
            }
        }

        private void BtnReturnToPreviousPageClick(object sender, RoutedEventArgs e)
        {
            ShowReturnToPreviousPageConfirmationDialog();
        }

        private void ShowReturnToPreviousPageConfirmationDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                "¿Está seguro que desea regresar a la ventana previa? " +
                "Todos los cambios sin guardar se perderá",
                "Regresar a ventana previa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (buttonClicked == MessageBoxResult.Yes)
            {
                RedirectToCreditGrantingPolicyList();
            }
        }

        private void RedirectToCreditGrantingPolicyList()
        {
            NavigationService.Navigate(new CreditGrantingPolicyListController());
        }

        private void BtnCancelClick(object sender, RoutedEventArgs e)
        {
            ShowDiscardChangesConfirmationDialog();
        }

        private void ShowDiscardChangesConfirmationDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                "¿Está seguro que desea cancelar la actualización de la política? " +
                "Todos los cambios sin guardar se perderán",
                "Descartar cambios",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (buttonClicked == MessageBoxResult.Yes)
            {
                RedirectToCreditGrantingPolicyList();
            }
        }

        private void BtnSaveChangesClick(object sender, RoutedEventArgs e)
        {
            HideFieldsErrors();

            bool isValidPolicyInformation = ValidateCreditGrantingPolicyInformation();
            if (!isValidPolicyInformation)
            {
                HighlightInvalidFields();
                ShowInvalidFieldsErrors();
                ShowInvalidFieldsAlertDialog();
            }
            else
            {
                ShowSaveChangesConfirmationDialog();
            }
        }

        private void HideFieldsErrors()
        {
            TbPolicyTitle.Style = (Style)FindResource("TextInput");
            TbkTitleError.Visibility = Visibility.Collapsed;

            TbPolicyDescription.Style = (Style)FindResource("TextInput");
            TbkDescriptionError.Visibility = Visibility.Collapsed;

            BdrEffectiveDate.BorderBrush = (Brush)FindResource("MediumGray");
            TbkEffectiveDateError.Visibility = Visibility.Collapsed;

            TbkStatusError.Visibility = Visibility.Collapsed;
        }

        private bool ValidateCreditGrantingPolicyInformation()
        {
            bool isValidPolicy = !string.IsNullOrWhiteSpace(TbPolicyTitle.Text)
                && (DpkEffectiveDate.SelectedDate.HasValue && DpkEffectiveDate.SelectedDate.Value > DateTime.Now)
                && ((RbActiveStatus.IsChecked.HasValue && RbActiveStatus.IsChecked.Value)
                || (RbInactiveStatus.IsChecked.HasValue && RbInactiveStatus.IsChecked.Value))
                && !string.IsNullOrWhiteSpace(TbPolicyDescription.Text);

            return isValidPolicy;
        }

        private void HighlightInvalidFields()
        {
            if (string.IsNullOrWhiteSpace(TbPolicyTitle.Text))
            {
                TbPolicyTitle.Style = (Style)FindResource("TextInputError");
            }

            if (!DpkEffectiveDate.SelectedDate.HasValue
                || DpkEffectiveDate.SelectedDate.Value <= DateTime.Now)
            {
                BdrEffectiveDate.BorderBrush = (Brush)FindResource("Red");
            }

            if (string.IsNullOrWhiteSpace(TbPolicyDescription.Text))
            {
                TbPolicyDescription.Style = (Style)FindResource("TextInputError");
            }
        }

        private void ShowInvalidFieldsErrors()
        {
            if (string.IsNullOrWhiteSpace(TbPolicyTitle.Text))
            {
                TbkTitleError.Text = "El título de la política de otorgamiento de crédito es obligatorio";
                TbkTitleError.Visibility = Visibility.Visible;
            }

            if ((!RbActiveStatus.IsChecked.HasValue || !RbActiveStatus.IsChecked.Value)
                && (!RbInactiveStatus.IsChecked.HasValue || !RbInactiveStatus.IsChecked.Value))
            {
                TbkStatusError.Visibility = Visibility.Visible;
            }

            if (!DpkEffectiveDate.SelectedDate.HasValue
                || DpkEffectiveDate.SelectedDate.Value <= DateTime.Now)
            {
                TbkEffectiveDateError.Visibility = Visibility.Visible;
            }

            if (string.IsNullOrWhiteSpace(TbPolicyDescription.Text))
            {
                TbkDescriptionError.Visibility = Visibility.Visible;
            }
        }

        private void ShowInvalidFieldsAlertDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                "Por favor corrija los campos marcados en rojo para guardar los cambios",
                "Campos inválidos",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
        }
        private void ShowSaveChangesConfirmationDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                "¿Está seguro que desea guardar los cambios realizados a la " +
                "política de otorgamiento de crédito?",
                "Confirmación de actualización",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (buttonClicked == MessageBoxResult.Yes)
            {
                editedPolicy.Title = TbPolicyTitle.Text.Trim();
                editedPolicy.Description = TbPolicyDescription.Text.Trim();
                editedPolicy.EffectiveDate = DpkEffectiveDate.SelectedDate.Value;
                if(RbActiveStatus.IsChecked.Value)
                {
                    editedPolicy.IsActive = true;
                } else if(RbInactiveStatus.IsChecked.Value)
                {
                    editedPolicy.IsActive = false;
                }

                UpdateCreditGrantingPolicy();
            }
        }

        private void UpdateCreditGrantingPolicy()
        {
            CreditGrantingPoliciesClient policiesService = new CreditGrantingPoliciesClient();

            try
            {
                bool successfulUpdate = policiesService.UpdateCreditGrantingPolicy(editedPolicy);
                if (successfulUpdate)
                {
                    ShowSuccessfulUpdateInformationDialog();
                }
                else
                {
                    MarkTitleFieldAsDuplicated();
                    ShowDuplicatedTitleAlertDialog();
                }
            }
            catch (FaultException<ServiceFault> fault)
            {
                ShowErrorSavingChangesDialog(fault.Detail.Message);
            }
            catch (EndpointNotFoundException)
            {
                string errorMessage = "Servidor no disponible. Por el momento el servidor no se encuentra " +
                    "disponible, intente más tarde";
                ShowErrorSavingChangesDialog(errorMessage);
            }
            catch (CommunicationException)
            {
                string errorMessage = "Error de conexión. Ocurrió un error de conexión, " +
                    "por favor compruebe su conexión de Internet e intente de nuevo";
                ShowErrorSavingChangesDialog(errorMessage);
            }
        }

        private void ShowErrorSavingChangesDialog(string message)
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                message,
                "No fue posible actualizar la política",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );

            if (buttonClicked == MessageBoxResult.OK)
            {
                RedirectToMainMenu();
            }
        }

        private void RedirectToMainMenu() {
            NavigationService.Navigate(new MainMenuController());
        }

        private void ShowSuccessfulUpdateInformationDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                "La información de la política de otorgamiento de crédito ha sido actualizada correctamente",
                "Actualización exitosa",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            if (buttonClicked == MessageBoxResult.OK)
            {
                RedirectToCreditGrantingPolicyList();
            }
        }

        private void MarkTitleFieldAsDuplicated()
        {
            TbPolicyTitle.Style = (Style)FindResource("TextInputError");

            TbkTitleError.Text = "Título en uso";
            TbkTitleError.Visibility = Visibility.Visible;
        }

        private void ShowDuplicatedTitleAlertDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                "El título que intenta utilizar para esta política ya se encuentra siendo " +
                "utilizado por alguna otra, por favor intente con uno diferente",
                "Título duplicado",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
        }
    }
}
