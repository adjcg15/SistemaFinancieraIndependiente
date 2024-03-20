using SFIClient.SFIService;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MessageBox = System.Windows.Forms.MessageBox;
using TextBox = System.Windows.Controls.TextBox;

namespace SFIClient.Views
{
    /// <summary>
    /// Lógica de interacción para ModifyBankAccountView.xaml
    /// </summary>
    public partial class ModifyBankAccountView : Page
    {
        readonly ClientsServiceClient clientsServiceClient = new ClientsServiceClient();
        BankAccount bankAccount = new BankAccount();
        private readonly string cardNumber;
        public ModifyBankAccountView(Client client)
        {
            InitializeComponent();
            TbkClientName.Text = client.Name + " " + client.Surname + " " +client.LastName;
            cardNumber = client.Card_number;
            LoadBankDetails();
        }

        private void LoadBankDetails()
        {
            try
            {
                bankAccount = clientsServiceClient.RecoverBankDetails(cardNumber);
                TbCardNumber.Text = bankAccount.Card_number.Trim();
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

        private void BtnGoBackClick(object sender, RoutedEventArgs e)
        {
            ShowCancelUpdateBankAccountConfirmationMessage();
        }

        private void BtnCancelClick(object sender, RoutedEventArgs e)
        {
            ShowCancelUpdateBankAccountConfirmationMessage();
        }
        private void ShowCancelUpdateBankAccountConfirmationMessage()
        {
            DialogResult resultado = MessageBox.Show(
                "¿Deseas cancelar la actualización de la cuenta bancaria del cliente?",
                "Confirmación de cancelación",
                MessageBoxButtons.OKCancel);

            if (resultado == DialogResult.OK)
            {
                SearchClientByRFCView searchClientByRFCView = new SearchClientByRFCView();
                this.NavigationService.Navigate(searchClientByRFCView);
            }
        }

        private void BtnUpdateBankAccountClick(object sender, RoutedEventArgs e)
        {
            bool updateData;

            if (VerifyTextFields())
            {
                DialogResult resultado = MessageBox.Show(
                    "¿Deseas actualizar la cuenta bancaria del cliente?", 
                    "Confirmación de actualización", 
                    MessageBoxButtons.OKCancel);

                if (resultado == DialogResult.OK)
                {
                    BankAccount newBankAccount = new BankAccount();
                    newBankAccount.Card_number = TbCardNumber.Text.Trim();
                    newBankAccount.Bank = TbBank.Text.Trim();
                    newBankAccount.Holder = TbHolder.Text.Trim();

                    updateData = UpdateBankAccount(newBankAccount);
                    if (updateData)
                    {
                        MessageBox.Show(
                            "Se actualizaron los datos bancarios de " + TbkClientName.Text + " correctamente", 
                            "Actualización exitosa");
                        SearchClientByRFCView searchClientByRFCView = new SearchClientByRFCView();
                        this.NavigationService.Navigate(searchClientByRFCView);
                    }
                    else
                    {
                        MessageBox.Show(
                            "No fue posible actualizar los datos bancarios de " + TbkClientName.Text + ", ya se encuentra registrada esa información", 
                            "Error de actualización");
                    }
                }
            }
            else
            {
                MessageBox.Show("Verifique que la información ingresada sea correcta", "Campos inválidos");
            }
        }

        private bool VerifyTextFields()
        {
            bool correctFields = true;

            if (!VerifyCardNumberFormat(TbCardNumber.Text.Trim())) correctFields = false;
            if (TbBank.Text.Trim().Length == 0) correctFields = false;
            if (TbHolder.Text.Trim().Length == 0) correctFields = false;

            return correctFields;
        }

        private bool VerifyCardNumberFormat(string cardNumber)
        {
            bool cardNumberIsCorrect;
            long number;
            if (long.TryParse(cardNumber, out number) && cardNumber.Length == 16)
            {
                cardNumberIsCorrect = true;
            }
            else
            {
                cardNumberIsCorrect = false;
            }

            return cardNumberIsCorrect;
        }

        private bool UpdateBankAccount(BankAccount bankAccount)
        {
            bool updateBankAccount = false;
            try
            {
                updateBankAccount = clientsServiceClient.UpdateBankAccount(bankAccount, cardNumber);
                
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

            return updateBankAccount;
        }
        private void ListenAndVerifyEmptyTextFields(object sender)
        {
            TextBox tbKey = sender as TextBox;

            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError");

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
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError");

            string cardNumber = tbCardNumber.Text.Trim();

            if (!VerifyCardNumberFormat(cardNumber))
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