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
    public partial class RegisterCreditGrantingPolicyController : Page
    {
        private readonly CreditGrantingPolicy newPolicy = new CreditGrantingPolicy();

        public RegisterCreditGrantingPolicyController()
        {
            InitializeComponent();
            DpkEffectiveDate.DisplayDateStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        }
        private bool VerifyPolicyInformation()
        {
            bool isValidPolicy = true;

            if (string.IsNullOrEmpty(TbPolicyName.Text))
            {
                HighlightInvalidFields(TbPolicyName);
                isValidPolicy = false;
            }
            else
            {
                ClearFieldHighlight(TbPolicyName);
            }

            if (!RbActivePolicy.IsChecked.HasValue || !RbInactivePolicy.IsChecked.HasValue ||
                (!RbActivePolicy.IsChecked.Value && !RbInactivePolicy.IsChecked.Value))
            {
                HighlightInvalidFields(RbActivePolicy);
                HighlightInvalidFields(RbInactivePolicy);
                isValidPolicy = false;
            }
            else
            {
                ClearFieldHighlight(RbActivePolicy);
                ClearFieldHighlight(RbInactivePolicy);
            }

            if (!DpkEffectiveDate.SelectedDate.HasValue)
            {
                HighlightInvalidFields(DpkEffectiveDate);
                isValidPolicy = false;
            }
            else
            {
                ClearFieldHighlight(DpkEffectiveDate);
            }

            if (string.IsNullOrEmpty(TbPolicyDescription.Text))
            {
                HighlightInvalidFields(TbPolicyDescription);
                isValidPolicy = false;
            }
            else
            {
                ClearFieldHighlight(TbPolicyDescription);
            }

            return isValidPolicy;
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
        private void BtnSavePolicyGrantingCreditClick(object sender, RoutedEventArgs e)
        {
            bool isValidPolicy = VerifyPolicyInformation();
            if (isValidPolicy)
            {
                newPolicy.Title = TbPolicyName.Text.Trim();
                newPolicy.Description = TbPolicyDescription.Text.Trim();
                newPolicy.EffectiveDate = DpkEffectiveDate.SelectedDate ?? DateTime.Now;
                newPolicy.IsActive = RbActivePolicy.IsChecked ?? false;

                ShowCreateNewPolicyConfirmDialog();
            }
            else
            {
                ShowInvalidFieldsDialog();
            }
        }
        private void ShowInvalidFieldsDialog()
        {
            MessageBox.Show("Por favor, verifique que los campos marcados en rojo " +
                "cuenten con una respuesta y que haya seleccionado una fecha de vigencia.",
                                "Campos requeridos", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        private void ShowCreateNewPolicyConfirmDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                $"¿Desea registrar la política de otorgamiento de crédito {newPolicy.Title}?",
                "Confirme el registro de la solicitud",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (buttonClicked == MessageBoxResult.Yes)
            {
                RegisterPolicyGranting();
            }

        }
        private void RegisterPolicyGranting()
        {
            CreditGrantingPoliciesClient policyClient = new CreditGrantingPoliciesClient();
            try
            {
                bool isRegistered = policyClient.RegisterCreditGrantingPolicy(newPolicy);
                if (isRegistered)
                {
                    MessageBox.Show($"La política de otorgamiento de crédito {newPolicy.Title} " +
                        $"se ha registrado correctamente.",
                        "Registro exitoso", MessageBoxButton.OK, MessageBoxImage.Information);
                    RedirectToMainMenu();
                }
                else
                {
                    MessageBox.Show($"Ya existe una política de otorgamiento de crédito con el nombre" +
                        $" {newPolicy.Title}. No se pudo registrar.",
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
        private void ShowErrorRecoveringCreditTypesDialog(string message)
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                message,
                "Error de operación",
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
            NavigationService.Navigate(new CreditGrantingPolicyListController());
        }

        private void BtnCancelSavePolicyGrantingCreditClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CreditGrantingPolicyListController());
        }

        private void BtnGoBackClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CreditGrantingPolicyListController());
        }
    }
}
