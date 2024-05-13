
using SFIClient.Session;
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
using TextBox = System.Windows.Controls.TextBox;

namespace SFIClient.Views
{
    /// <summary>
    /// Lógica de interacción para ModifyBankAccountView.xaml
    /// </summary>
    public partial class ModifyBankAccountController : Page
    {
        readonly ClientsServiceClient clientsServiceClient = new ClientsServiceClient();
        BankAccount bankAccount = new BankAccount();
        private readonly Client client;
        public ModifyBankAccountController(Client client)
        {
            InitializeComponent();
            TbkClientName.Text = client.Name + " " + client.Surname + " " + client.LastName;
            this.client = client;
            ApplyRestApplyRestrictionsOnFields();
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            LoadBankAccount();
        }

        private void ApplyRestApplyRestrictionsOnFields()
        {
            RestrictOnlyNumbers(TbCardNumber);
            RestrictOnlyLetters(TbHolder);
        }

        private void LoadBankAccount()
        {
            try
            {
                bankAccount = clientsServiceClient.RecoverBankDetails(client.Card_number);
                TbCardNumber.Text = bankAccount.CardNumber.Trim();
                TbHolder.Text = bankAccount.Holder.Trim();
                TbBank.Text = bankAccount.Bank.Trim();
            }
            catch (FaultException<ServiceFault> fe)
            {
                ShowErrorRecoveringBankAccount(fe.Detail.Message);
                RedirectToLoginView();
            }
            catch (EndpointNotFoundException)
            {
                string message = "No fue posible recuperar la cuenta bancaria del cliente, inténtelo de nuevo más tarde";
                ShowErrorRecoveringBankAccount(message);
                RedirectToLoginView();
            }
            catch (CommunicationException)
            {
                string message = "No fue posible recuperar la cuenta bancaria del cliente, inténtelo de nuevo más tarde";
                ShowErrorRecoveringBankAccount(message);
                RedirectToLoginView();
            }
        }

        private void RestrictOnlyLetters(TextBox textBox)
        {
            textBox.PreviewTextInput += (sender, e) =>
            {
                if (char.IsDigit(e.Text, e.Text.Length - 1))
                {
                    e.Handled = true;
                }
            };
        }

        private void RestrictOnlyNumbers(TextBox textBox)
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

        private void ShowErrorRecoveringBankAccount(string message)
        {
            MessageBox.Show(
                message,
                "Servicio no disponible",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }

        private void BtnDiscardUpdateBankAccountClick(object sender, RoutedEventArgs e)
        {
            ShowDiscardUpdateBankAccountConfirmationMessage();
        }

        private void BtnCancelUpdateBankAccountClick(object sender, RoutedEventArgs e)
        {
            ShowCancelUpdateBankAccountConfirmationMessage();
        }

        private void ShowDiscardUpdateBankAccountConfirmationMessage()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                "¿Está seguro que desea regresar a la ventana previa? Todos los cambios sin guardar se perderán",
                "Regresar a ventana previa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (buttonClicked == MessageBoxResult.Yes)
            {
                RedirectToSearchClientByRfcView();
            }
        }

        private void ShowCancelUpdateBankAccountConfirmationMessage()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                "¿Deseas cancelar la actualización de la cuenta bancaria del cliente?",
                "Confirmación de cancelación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (buttonClicked == MessageBoxResult.Yes)
            {
                RedirectToSearchClientByRfcView();
            }
        }

        private void BtnUpdateBankAccountClick(object sender, RoutedEventArgs e)
        {
            bool isValidInformation = VerifyTextFields();

            if (isValidInformation)
            {
                ShowClientUpdateConfirmationDialog();
            }
            else
            {
                HighLightInvalidFields();
                ShowInvalidFieldsAlertDialog();
            }
        }

        private void HighLightInvalidFields()
        {
            Style textInputErrorStyle = (Style)this.FindResource("SecondTextInputError");
            if (TbCardNumber.Text.Trim().Length < 16) TbCardNumber.Style = textInputErrorStyle;
            if (TbBank.Text.Trim().Length == 0) TbCardNumber.Style = textInputErrorStyle;
            if (TbHolder.Text.Trim().Length == 0) TbCardNumber.Style = textInputErrorStyle;
        }

        private void ShowInvalidFieldsAlertDialog()
        {
            MessageBox.Show(
                "Verifique que la información ingresada sea correcta y no existan campos vacíos",
                "Campos inválidos",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
        }

        private void ShowClientUpdateConfirmationDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                    "¿Deseas actualizar la cuenta bancaria del cliente" + 
                    $"{client.Name} {client.Surname} {client.LastName}" + " ?",
                    "Confirmación de actualización",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

            if (buttonClicked == MessageBoxResult.Yes)
            {
                BankAccount newBankAccount = new BankAccount();
                newBankAccount.CardNumber = TbCardNumber.Text.Trim();
                newBankAccount.Bank = TbBank.Text.Trim();
                newBankAccount.Holder = TbHolder.Text.Trim();

                UpdateBankAccount(newBankAccount);
            }
        }

        private void RedirectToSearchClientByRfcView()
        {
            NavigationService.Navigate(new SearchClientByRFCController());
        }

        private void RedirectToLoginView()
        {
            NavigationService.Navigate(new LoginController());
            SystemSession.Employee = null;
        }

        private bool VerifyTextFields()
        {
            bool correctFields = true;

            if (TbCardNumber.Text.Trim().Length < 16) correctFields = false;
            if (TbBank.Text.Trim().Length == 0) correctFields = false;
            if (TbHolder.Text.Trim().Length == 0) correctFields = false;

            return correctFields;
        }

        private void UpdateBankAccount(BankAccount bankAccount)
        {
            try
            {
                bool statusUpdate = clientsServiceClient.UpdateBankAccount(bankAccount, client.Card_number);
                if (statusUpdate)
                {
                    ShowBankAccountSuccessfulUpdateMessageDialog();
                    RedirectToSearchClientByRfcView();
                }
                else
                {
                    ShowExistingBankAccountMessageDialog();
                }

            }
            catch (FaultException<ServiceFault> fe)
            {
                ShowErrorUpdateBankAccount(fe.Detail.Message);
                RedirectToLoginView();
            }
            catch (EndpointNotFoundException)
            {
                string message = "No fue posible actualizar la cuenta bancaria del cliente, inténtelo de nuevo más tarde";
                ShowErrorUpdateBankAccount(message);
                RedirectToLoginView();
            }
            catch (CommunicationException)
            {
                string message = "No fue posible actualizar la cuenta bancaria del cliente, inténtelo de nuevo más tarde";
                ShowErrorUpdateBankAccount(message);
                RedirectToLoginView();
            }
        }

        private void ShowBankAccountSuccessfulUpdateMessageDialog()
        {
            MessageBox.Show(
                "La cuenta bancaria del cliente " + $"{client.Name} {client.Surname} {client.LastName}" + 
                " ha sido actualizada correctamente",
                "Actualización existosa",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void ShowExistingBankAccountMessageDialog()
        {
            Style textInputErrorStyle = (Style)this.FindResource("SecondTextInputError");
            MessageBox.Show(
                "Verifique que la información ingresada para el número de tarjeta sea la correcta",
                "Cuenta bancaria ya registrada en el sistema",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
            TbCardNumber.Style = textInputErrorStyle;
        }

        private void ShowErrorUpdateBankAccount(string message)
        {
            MessageBox.Show(
                message,
                "Sistema no disponible",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }

        private void ListenAndVerifyEmptyTextFields(object sender)
        {
            TextBox tbKey = sender as TextBox;

            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("SecondTextInputError");

            string key = tbKey.Text.Trim();

            if (key.Length == 0)
            {
                tbKey.Style = textInputErrorStyle;
            }
            else
            {
                tbKey.Style = textInputStyle;
            }
        }

        private void TbCardNumberTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tbCardNumber = sender as TextBox;

            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("SecondTextInputError");

            string cardNumber = tbCardNumber.Text.Trim();

            if (cardNumber.Length < 16)
            {
                tbCardNumber.Style = textInputErrorStyle;
            }
            else
            {
                tbCardNumber.Style = textInputStyle;
            }
        }

        private void TbBankTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbHolderTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }
    }
}
