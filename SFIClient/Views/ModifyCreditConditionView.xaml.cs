using SFIClient.SFIServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// <summary>
    /// Lógica de interacción para ModifyCreditConditionView.xaml
    /// </summary>
    public partial class ModifyCreditConditionController : Page
    {
        readonly CreditConditionsServiceClient creditConditionsServiceClient = new CreditConditionsServiceClient();
        CreditCondition newCondition = new CreditCondition();
        public ModifyCreditConditionController()
        {
            InitializeComponent();
        }
        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            LoadCreditTypes();
            ApplyNumericRestrictions();
        }
        private void LoadBankAccount()
        {
            try
            {
                newCondition = creditConditionsServiceClient.Rw
                bankAccount = clientsServiceClient.RecoverBankDetails(cardNumber);
                TbCardNumber.Text = bankAccount.CardNumber.Trim();
                TbHolder.Text = bankAccount.Holder.Trim();
                TbBank.Text = bankAccount.Bank.Trim();
            }
            catch (FaultException<ServiceFault> fe)
            {
                MessageBox.Show(fe.Message, "Error en la base de datos");
            }
            catch (EndpointNotFoundException)
            {
                MessageBox.Show("No fue posible establecer la conexión con el servicio, intente más tarde", "Error en el servicio");
                //TODO Redirect To Main Menu
            }
            catch (CommunicationException)
            {
                MessageBox.Show("No fue posible establecer la conexión con el servicio, intente más tarde", "Error en el servicio");
                //TODO Redirect To Main Menu
            }
        }
        private void CbCreditTypesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbCreditTypes.SelectedIndex != -1)
            {
                CreditType selectedCreditType = (CreditType)CbCreditTypes.SelectedItem;
                newCondition.CreditType = selectedCreditType;
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
        private void BtnGoBackClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ConsultConditionsCreditView());
        }
        private void BtnSaveModificationOfCreditConditionClick(object sender, RoutedEventArgs e)
        {

        }

        private void BtnCancelModificationOfCreditConditionClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
