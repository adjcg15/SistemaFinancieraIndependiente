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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private void TbCardNumberTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tbCardNumber = sender as TextBox;

            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError");

            string cardNumber = tbCardNumber.Text.Trim();

            if (!VerifyCarNumberFormat(cardNumber))
            {
                tbCardNumber.Style = textInputErrorStyle;
            }
            else
            {
                tbCardNumber.Style = textInputStyle;
            }
        }

        private void BtnGoBackClick(object sender, RoutedEventArgs e)
        {
            SearchClientByRFCView searchClientByRFCView = new SearchClientByRFCView();
            this.NavigationService.Navigate(searchClientByRFCView);
        }

        private void BtnCancelClick(object sender, RoutedEventArgs e)
        {
            SearchClientByRFCView searchClientByRFCView = new SearchClientByRFCView();
            this.NavigationService.Navigate(searchClientByRFCView);
        }

        private void BtnUpdateDataClick(object sender, RoutedEventArgs e)
        {
            bool updateData;
            
            ReloadStylesToTextInputs();

            if (VerifyTextFields())
            {

                BankAccount newBankAccount = new BankAccount();
                newBankAccount.Card_number = TbCardNumber.Text.Trim();
                newBankAccount.Bank = TbBank.Text.Trim();
                newBankAccount.Holder = TbHolder.Text.Trim();
                try
                {
                    updateData = UpdateBankAccount(newBankAccount);
                    if (updateData)
                    {
                        MessageBox.Show("Se actualizaron los datos bancarios de " + TbkClientName.Text + " correctamente", "Actualización exitosa");
                        SearchClientByRFCView searchClientByRFCView = new SearchClientByRFCView();
                        this.NavigationService.Navigate(searchClientByRFCView);
                    }
                    else
                    {
                        MessageBox.Show("No fue posible actualizar los datos bancarios de " + TbkClientName.Text + ", ya se encuentra registrada esa información", "Error de actualización");
                    }
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
            else
            {
                MessageBox.Show("Verifique que la información ingresada no esté vacía", "Campos inválidos");
            }
        }

        private bool VerifyTextFields()
        {
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError");
            bool correctFields = true;
            if (!VerifyCarNumberFormat(TbCardNumber.Text.Trim()))
            {
                correctFields = false;
                TbCardNumber.Style = textInputErrorStyle;
            }
            if (TbBank.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbBank.Style = textInputErrorStyle;
            }
            if (TbHolder.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbHolder.Style = textInputErrorStyle;
            }

            return correctFields;
        }

        private void ReloadStylesToTextInputs()
        {
            Style textInputStyle = (Style)this.FindResource("TextInput");

            TbCardNumber.Style = textInputStyle;
            TbHolder.Style = textInputStyle;
            TbBank.Style = textInputStyle;
        }

        private bool VerifyCarNumberFormat(string cardNumber)
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
    }
}
