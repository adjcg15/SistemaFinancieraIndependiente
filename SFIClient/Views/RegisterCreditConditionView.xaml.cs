using SFIClient.SFIServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
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
    /// <summary>
    /// Lógica de interacción para RegisterCreditConditionView.xaml
    /// </summary>
    public partial class RegisterCreditConditionView : Page
    {
        private bool isLostFocusHandled = false;
        private readonly CreditCondition newCondition = new CreditCondition();
        public RegisterCreditConditionView()
        {
            InitializeComponent();
            InitializeEventHandlers();
            LoadCreditTypes();
            ApplyNumericRestrictions();
        }
        private void CbCreditTypesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbCreditTypes.SelectedIndex != -1)
            {
                CreditType selectedCreditType = (CreditType)CbCreditTypes.SelectedItem;
                newCondition.CreditType = selectedCreditType;
            }
        }
        private void InitializeEventHandlers()
        {
            TbCreditConditionName.LostFocus += TbCreditConditionNameLostFocus;
        }
        private bool VerifyCreditConditionInformationFields()
        {
            bool isValidCreditCondition = true;

            if (string.IsNullOrEmpty(TbCreditConditionName.Text) || !IsValidCreditConditionNameFormat(TbCreditConditionName.Text))
            {
                HighlightInvalidFields(TbCreditConditionName);
                isValidCreditCondition = false;
            }
            else
            {
                ClearFieldHighlight(TbCreditConditionName);
            }
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
        private void BtnSaveCreditConditionClick(object sender, RoutedEventArgs e)
        {
            bool isValidCreditCondition = VerifyCreditConditionInformationFields();
            if (isValidCreditCondition)
            {
                newCondition.Identifier = TbCreditConditionName.Text.Trim();
                newCondition.IsActive = RbActivePolicy.IsChecked ?? false;
                newCondition.IsIvaApplied = RbApplyIVa.IsChecked ?? false;
                newCondition.CreditType = (CreditType)CbCreditTypes.SelectedItem;
                newCondition.Identifier = TbCreditConditionName.Text.Trim();
                newCondition.PaymentMonths = Convert.ToInt32(TbPaymentMonths.Text.Trim());
                newCondition.InterestRate = Convert.ToDouble(TbInterestRate.Text.Trim());
                newCondition.InterestOnArrears = Convert.ToDouble(TbInterestOnArrears.Text.Trim());
                newCondition.AdvancePaymentReduction = Convert.ToDouble(TbAdvancePaymentReduction.Text.Trim());

                ShowCreateCreditConditionConfirmDialog();
            }
            else
            {
                ShowInvalidFieldsDialog();
            }
        }
        private void ShowCreateCreditConditionConfirmDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                $"¿Desea registrar la condición de crédito {newCondition.Identifier}?",
                "Confirme el registro de la solicitud",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (buttonClicked == MessageBoxResult.Yes)
            {
                RegisterCreditCondition();
            }

        }
        private void RegisterCreditCondition()
        {
            CreditConditionsServiceClient creditConditiionClient = new CreditConditionsServiceClient();
            try
            {
                bool isRegistered = creditConditiionClient.RegisterCreditCondition(newCondition);
                if (isRegistered)
                {
                    MessageBox.Show($"La política de otorgamiento de crédito " +
                        $"{newCondition.Identifier} se ha registrado correctamente.",
                                    "Registro exitoso", MessageBoxButton.OK, MessageBoxImage.Information);
                    RedirectToMainMenu();
                }
                else
                {
                    MessageBox.Show($"Ya existe una política de otorgamiento de crédito con el nombre " +
                        $"{newCondition.Identifier}. No se pudo registrar.",
                                    "Error de registro", MessageBoxButton.OK, MessageBoxImage.Error);
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
        private void ClearCreditConditionFields()
        {
            TbCreditConditionName.Text = string.Empty;
            ClearFieldHighlight(TbCreditConditionName);
            RbActivePolicy.IsChecked = false;
            RbInactivePolicy.IsChecked = false;
            CbCreditTypes.SelectedIndex = -1;
            TbPaymentMonths.Text = string.Empty;
            RbApplyIVa.IsChecked = false;
            RbDontApplyIVA.IsChecked = false;
            TbInterestRate.Text = string.Empty;
            TbInterestOnArrears.Text = string.Empty;
            TbAdvancePaymentReduction.Text = string.Empty;
        }
        private void ShowInvalidFieldsDialog()
        {
            MessageBox.Show("Por favor, verifique que los campos marcados en rojo " +
                "cuenten con una respuesta o que esté dada en un formato válido.",
                                "Campos requeridos", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        private void BtnCancelRegisterOfCreditConditionClick(object sender, RoutedEventArgs e)
        {
            RedirectToMainMenu();
        }
        private void BtnGoBackClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainMenuController());
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
                "No fue posible recuperar los tipos de crédito existentes, por favor intenta más tarde",
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
            ConsultConditionsCreditView consultCreditConditions = new ConsultConditionsCreditView();
            this.NavigationService.Navigate(consultCreditConditions);
            NavigationService.RemoveBackEntry();
        }
        private void TbCreditConditionNameLostFocus(object sender, RoutedEventArgs e)
        {
            if (isLostFocusHandled)
            {
                return;
            }
            isLostFocusHandled = true;
            TextBox textBox = sender as TextBox;
            string input = textBox.Text;
            Regex regex = new Regex(@"^CCP\d{3}$");
            if (!regex.IsMatch(input))
            {
                MessageBox.Show("El nombre debe tener el formato CCP000 donde '000' " +
                    "puede ser cualquier número entre 0 y 9.",
                    "Formato inválido", MessageBoxButton.OK, MessageBoxImage.Warning);

                textBox.Style = (Style)FindResource("TextInputError");
                return;
            }
            textBox.Style = (Style)FindResource("TextInput");
        }
        private void TbCreditConditionNameTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string input = textBox.Text;

            Regex regex = new Regex(@"^CCP\d{3}$");

            if (!regex.IsMatch(input))
            {
                textBox.Style = (Style)FindResource("TextInputError");
                isLostFocusHandled = false;
            }
            else
            {
                textBox.Style = (Style)FindResource("TextInput");
            }
        }
        private bool IsValidCreditConditionNameFormat(string name)
        {
            Regex regex = new Regex(@"^CCP\d{3}$");
            return regex.IsMatch(name);
        }
    }
}
