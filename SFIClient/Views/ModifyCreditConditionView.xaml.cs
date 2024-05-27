using SFIClient.SFIServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace SFIClient.Views
{
    /// <summary>
    /// Lógica de interacción para ModifyCreditConditionView.xaml
    /// </summary>
    public partial class ModifyCreditConditionController : Page
    {
        readonly CreditConditionsServiceClient creditConditionsServiceClient = new CreditConditionsServiceClient();
        CreditCondition newCondition = new CreditCondition();
        private readonly string identifier;
        public ModifyCreditConditionController(CreditCondition creditCondition)
        {
            InitializeComponent();
            identifier = creditCondition.Identifier;
        }
        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            ShowCreditConditionInformation();
            LoadCreditTypes();
            ApplyNumericRestrictions();
        }
        private void ShowCreditConditionInformation()
        {
            try
            {
                newCondition = creditConditionsServiceClient.RecoverCreditConditionDetails(identifier);
                TbkCreditCondition.Text = newCondition.Identifier;
                RbActivePolicy.IsChecked = newCondition.IsActive;
                RbInactivePolicy.IsChecked = !newCondition.IsActive;
                RbApplyIVa.IsChecked = newCondition.IsIvaApplied;
                RbDontApplyIVA.IsChecked = !newCondition.IsIvaApplied;
                CbCreditTypes.SelectedItem = newCondition.CreditType;
                TbPaymentMonths.Text = newCondition.PaymentMonths.ToString();
                TbInterestRate.Text = newCondition.InterestRate.ToString();
                TbInterestOnArrears.Text = newCondition.InterestOnArrears.ToString();
                TbAdvancePaymentReduction.Text = newCondition.AdvancePaymentReduction.ToString();
            }
            catch (FaultException<ServiceFault> fe)
            {
                MessageBox.Show(fe.Message, "Error en la base de datos");
            }
            catch (EndpointNotFoundException)
            {
                MessageBox.Show("No fue posible establecer la conexión con el servicio, intente más tarde", "Error en el servicio");
                RedirectToConsultConditionsCredit();
            }
            catch (CommunicationException)
            {
                MessageBox.Show("No fue posible establecer la conexión con el servicio, intente más tarde", "Error en el servicio");
                RedirectToConsultConditionsCredit();
            }
        }

        private void CbCreditTypesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbCreditTypes.SelectedItem == null)
            {
                TextBlockPlaceholder.Visibility = Visibility.Visible;
                if (CbCreditTypes.SelectedIndex != -1)
                {
                    CreditType selectedCreditType = (CreditType)CbCreditTypes.SelectedItem;
                    newCondition.CreditType = selectedCreditType;
                }
            }
            else
            {
                TextBlockPlaceholder.Visibility = Visibility.Collapsed;
            }
            
        }
        private void LoadCreditTypes()
        {
            CreditsServiceClient creditsService = new CreditsServiceClient();

            try
            {
                List<CreditType> creditTypesRecovered = creditsService.GetAllCreditTypes().ToList();
                FillCreditTypesCombobox(creditTypesRecovered);
            }
            catch (System.ServiceModel.FaultException<ServiceFault> fault)
            {
                ShowErrorRecoveringCreditTypesDialog(fault.Detail.Message);
            }
            catch (System.ServiceModel.EndpointNotFoundException)
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
                "Error en operación",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
        private void ApplyNumericRestrictions()
        {
            var textBoxes = new[]
            {
            TbPaymentMonths,
            TbInterestRate,
            TbInterestOnArrears,
            TbAdvancePaymentReduction
        };
            foreach (var textBox in textBoxes)
            {
                RestrictToNumericInput(textBox);
            }
        }
        private void RestrictToNumericInput(TextBox textBox)
        {
            textBox.PreviewTextInput += (sender, e) =>
            {
                if (!char.IsDigit(e.Text, e.Text.Length - 1))
                {
                    e.Handled = true;
                }
            };
            textBox.PreviewKeyDown += (sender, e) =>
            {
                if (e.Key == Key.Space)
                {
                    e.Handled = true;
                }
            };
        }
        private bool VerifyCreditConditionInformationFields()
        {
            bool isValidCreditCondition = true;
            if (!RbActivePolicy.IsChecked.HasValue || !RbInactivePolicy.IsChecked.HasValue ||
                (!RbActivePolicy.IsChecked.Value && !RbInactivePolicy.IsChecked.Value))
            {
                HighlightInvalidFields(RbActivePolicy);
                HighlightInvalidFields(RbInactivePolicy);
                isValidCreditCondition = false;
            }
            else
            {
                ClearFieldHighlight(RbActivePolicy);
                ClearFieldHighlight(RbInactivePolicy);
            }
            if (!RbApplyIVa.IsChecked.HasValue || !RbDontApplyIVA.IsChecked.HasValue ||
                (!RbApplyIVa.IsChecked.Value && !RbDontApplyIVA.IsChecked.Value))
            {
                HighlightInvalidFields(RbApplyIVa);
                HighlightInvalidFields(RbDontApplyIVA);
                isValidCreditCondition = false;
            }
            else
            {
                ClearFieldHighlight(RbApplyIVa);
                ClearFieldHighlight(RbDontApplyIVA);
            }
            if (CbCreditTypes.SelectedItem == null)
            {
                HighlightInvalidFields(CbCreditTypes);
                isValidCreditCondition = false;
            }
            else
            {
                ClearFieldHighlight(CbCreditTypes);
            }
            if (string.IsNullOrEmpty(TbPaymentMonths.Text))
            {
                HighlightInvalidFields(TbPaymentMonths);
                isValidCreditCondition = false;
            }
            else
            {
                ClearFieldHighlight(TbPaymentMonths);
            }
            if (string.IsNullOrEmpty(TbInterestRate.Text))
            {
                HighlightInvalidFields(TbInterestRate);
                isValidCreditCondition = false;
            }
            else
            {
                ClearFieldHighlight(TbInterestRate);
            }
            if (string.IsNullOrEmpty(TbInterestOnArrears.Text))
            {
                HighlightInvalidFields(TbInterestOnArrears);
                isValidCreditCondition = false;
            }
            else
            {
                ClearFieldHighlight(TbInterestOnArrears);
            }
            if (string.IsNullOrEmpty(TbAdvancePaymentReduction.Text))
            {
                HighlightInvalidFields(TbAdvancePaymentReduction);
                isValidCreditCondition = false;
            }
            else
            {
                ClearFieldHighlight(TbAdvancePaymentReduction);
            }
            return isValidCreditCondition;
        }
        private void HighlightInvalidFields(Control control)
        {
            Style errorStyle = null;
            if (control is TextBox)
            {
                errorStyle = (Style)FindResource("TextInputError");
            }
            else if (control is ComboBox)
            {
                errorStyle = (Style)FindResource("ComboBoxError");
            }
            else if (control is RadioButton)
            {
                errorStyle = (Style)FindResource("RadioButtonError");
                control.Style = errorStyle;
            }
            if (errorStyle != null)
            {
                control.Style = errorStyle;
            }
        }
        private void ClearFieldHighlight(Control control)
        {
            Style defaultStyle = null;
            if (control is TextBox)
            {
                defaultStyle = (Style)FindResource("TextInput");
            }
            else if (control is ComboBox)
            {
                defaultStyle = (Style)FindResource("ComboBox");
            }
            else if (control is RadioButton)
            {
                defaultStyle = (Style)FindResource("RadioButtonStyle");
                control.Style = defaultStyle;
            }
            if (defaultStyle != null)
            {
                control.Style = defaultStyle;
            }
        }
        private void BtnReturnToPreviousPageClick(object sender, RoutedEventArgs e)
        {
            ShowReturnPreviousPageConfirmationDialog();
        }
        private void BtnSaveModificationOfCreditConditionClick(object sender, RoutedEventArgs e)
        {
            bool isValidCreditCondition = VerifyCreditConditionInformationFields();
            if (isValidCreditCondition)
            {
                newCondition.IsActive = RbActivePolicy.IsChecked ?? false;
                newCondition.IsIvaApplied = RbApplyIVa.IsChecked ?? false;
                newCondition.CreditType = (CreditType)CbCreditTypes.SelectedItem;
                newCondition.PaymentMonths = Convert.ToInt32(TbPaymentMonths.Text.Trim());
                newCondition.InterestRate = Convert.ToDouble(TbInterestRate.Text.Trim());
                newCondition.InterestOnArrears = Convert.ToDouble(TbInterestOnArrears.Text.Trim());
                newCondition.AdvancePaymentReduction = Convert.ToDouble(TbAdvancePaymentReduction.Text.Trim());

                ShowSaveChangesConfirmationDialog();
            }
        }
        private void ShowSaveChangesConfirmationDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                $"¿Desea actualizar la información de la condición de crédito {newCondition.Identifier}?",
                "Confirme actualización",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );
            if (buttonClicked == MessageBoxResult.Yes)
            {
                UpdateCreditConditionInformation();
            }
        }
        private void ShowReturnPreviousPageConfirmationDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                $"¿Está seguro de que desea regresar a la ventana previa? Todos los cambios sin guardar se perderán",
                "Regresar a ventana previa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );
            if (buttonClicked == MessageBoxResult.Yes)
            {
                RedirectToConsultConditionsCredit();
            }
        }
        private void ShowDiscardChangesConfirmationDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                $"¿Está seguro de que desea cancelar la actualización de la condición de crédito ? Todos los cambios sin guardar se perderán",
                "Descartar cambios",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );
            if (buttonClicked == MessageBoxResult.Yes)
            {
                RedirectToConsultConditionsCredit();
            }
        }
        private void UpdateCreditConditionInformation()
        {
            CreditConditionsServiceClient creditConditionClient = new CreditConditionsServiceClient();
            try
            {
                if (creditConditionClient.VerifyUsageInCreditApplications(newCondition.Identifier))
                {
                    MessageBox.Show($"La condición de crédito {newCondition.Identifier} " +
                        $"está siendo utilizada en al menos una solicitud de crédito en estado 'CREADA' o 'APROBADA', " +
                        $"por lo que no puede ser modificada, intente nuevamente más tarde.",
                                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (creditConditionClient.VerifyUsageInRegimen(newCondition.Identifier))
                {
                    MessageBox.Show($"La condición de crédito {newCondition.Identifier} " +
                        $"está siendo utilizada en al menos un régimen por lo que no puede ser modificada, " +
                        $"intente nuevamente más tarde.",
                                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                bool isRegistered = creditConditionClient.UpdateCreditCondition(newCondition);
                if (isRegistered)
                {
                    MessageBox.Show($"La condición de crédito {newCondition.Identifier} se ha actualizado correctamente.",
                                    "Actualización exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                    RedirectToConsultConditionsCredit();
                }
            }
            catch (System.ServiceModel.FaultException<ServiceFault> fault)
            {
                ShowErrorRecoveringCreditTypesDialog(fault.Detail.Message);
            }
            catch (System.ServiceModel.EndpointNotFoundException)
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

        private void RedirectToConsultConditionsCredit()
        {
            ConsultConditionsCreditView consultCreditConditions = new ConsultConditionsCreditView();
            this.NavigationService.Navigate(consultCreditConditions);
            NavigationService.RemoveBackEntry();
        }
        private void BtnCancelClick(object sender, RoutedEventArgs e)
        {
            ShowDiscardChangesConfirmationDialog();
        }
    }
}
