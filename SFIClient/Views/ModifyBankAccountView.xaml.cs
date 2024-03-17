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
        ClientsServiceClient clientsServiceClient = new ClientsServiceClient();
        private string carNumber;
        public ModifyBankAccountView(Client client)
        {
            InitializeComponent();
            TbkClientName.Text = client.Name + " " + client.Surname + " " +client.LastName;
            carNumber = client.Card_number;
            LoadBankDetails();
        }

        private void LoadBankDetails()
        {
            try
            {
                BankAccount bankAccount = clientsServiceClient.RecoverBankDetails(carNumber);
                TbCardNumber.Text = bankAccount.Card_number;
                TbHolder.Text = bankAccount.Holder;
                TbBank.Text = bankAccount.Bank;
            }
            catch (FaultException fe)
            {
                MessageBox.Show(fe.Message);
            }
            catch (EndpointNotFoundException)
            {
                MessageBox.Show("No fue posible establecer la conexión con el servicio, intente más tarde");
                //TODO Redirect To Main Menu
            }
            catch (CommunicationException)
            {
                MessageBox.Show("No fue posible establecer la conexión con el servicio, intente más tarde");
                //TODO Redirect To Main Menu
            }
        }

        private void TbCardNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tbCardNumber = sender as TextBox;

            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError");

            string cardNumber = tbCardNumber.Text.Trim();

            if (cardNumber.Length != 16)
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

        }
    }
}
