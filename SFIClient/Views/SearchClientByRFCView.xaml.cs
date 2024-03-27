using SFIClient.Controlls;
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
using System.Windows.Forms;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MessageBox = System.Windows.Forms.MessageBox;
using SFIClient.Session;

namespace SFIClient.Views
{
    /// <summary>
    /// Lógica de interacción para SearchClientByRFCView.xaml
    /// </summary>
    public partial class SearchClientByRFCController : Page
    {
        private readonly ClientsServiceClient clientsServiceClient = new ClientsServiceClient();
        private List<Client> clientsList = new List<Client>();
        public SearchClientByRFCController()
        {
            InitializeComponent();
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            LoadClients();
        }

        private void LoadClients()
        {
            try
            {
                clientsList = clientsServiceClient.RecoverClients().ToList();
                if (clientsList.Count != 0)
                {
                    AddClientsToClientsList();
                }
                else
                {
                    SkpRegisterClient.Children.Clear();
                    SkpRegisterClient.Visibility = Visibility.Collapsed;
                    SkpSearchClient.Visibility = Visibility.Collapsed;
                    SkpRegisterClientNow.Children.Add(BtnRegisterClient);
                }
            }
            catch (FaultException<ServiceFault> fe)
            {
                MessageBox.Show(fe.Detail.Message, "Error en la base de datos");
            }
            catch (EndpointNotFoundException)
            {
                MessageBox.Show("No fue posible establecer la conexión con el servicio, intente más tarde", "Error en el servicio");
                RedirectToLoginView();
            }
            catch (CommunicationException)
            {
                MessageBox.Show("No fue posible establecer la conexión con el servicio, intente más tarde", "Error en el servicio");
                RedirectToLoginView();
            }
        }

        private void RedirectToLoginView()
        {
            NavigationService.Navigate(new LoginController());
            SystemSession.Employee = null;
        }

        private void BtnGoToLoginViewClick(object sender, EventArgs e)
        {
            RedirectToLoginView();
        }

        private void AddClientsToClientsList()
        {
            SkpNoRegisteredClients.Visibility = Visibility.Collapsed;
            SkpRegisterClientNow.Visibility = Visibility.Collapsed;
            for (int i = 0; i < clientsList.Count; i++)
            {
                if (clientsList[i].Has_active_credit || clientsList[i].Has_credit_application)
                {
                    ShowClientWithAllOptionsWithoutCreditApplication(clientsList[i]);
                }
                else
                {
                    ShowClientWithAllOptions(clientsList[i]);
                }
            }
        }

        private void AddSearchedClientToClientList(Client client)
        {
            SkpNoRegisteredClients.Visibility = Visibility.Collapsed;
            SkpRegisterClientNow.Visibility = Visibility.Collapsed;
            if (client.Has_active_credit || client.Has_credit_application)
            {
                ShowClientWithAllOptionsWithoutCreditApplication(client);
            }
            else
            {
                ShowClientWithAllOptions(client);
            }
        }

        private void ShowClientWithAllOptions(Client client)
        {
            ClientControll clientControll = new ClientControll();
            clientControll.ButtonSeePersonalInformation += BtnSeePersonalInformationClick;
            clientControll.ButtonModifyPersonalInformation += BtnModifyPersonalInformationClick;
            clientControll.ButtonModifyBankAccount += BtnModifyBankAccountClick;
            clientControll.ButtonModifyWorkCenter += BtnModifyWorkCenterClick;
            clientControll.ButtonModifyPersonalReferences += BtnModifyPersonalReferencesClick;
            clientControll.ButtonApplyForCredit += BtnApplyForCreditClick;
            clientControll.LblClientRFC.Content = client.Rfc;
            clientControll.LblClientName.Content = client.LastName + " " + client.Surname + " " + client.Name;
            clientControll.LblClientCreditStatus.Content = "Cliente sin crédito y sin solicitud activa";
            ItcClients.Items.Add(clientControll);
        }

        private void ShowClientWithAllOptionsWithoutCreditApplication(Client client)
        {
            ClientControll clientControll = new ClientControll();
            clientControll.ButtonSeePersonalInformation += BtnSeePersonalInformationClick;
            clientControll.ButtonModifyPersonalInformation += BtnModifyPersonalInformationClick;
            clientControll.ButtonModifyBankAccount += BtnModifyBankAccountClick;
            clientControll.ButtonModifyWorkCenter += BtnModifyWorkCenterClick;
            clientControll.ButtonModifyPersonalReferences += BtnModifyPersonalReferencesClick;
            clientControll.LblClientRFC.Content = client.Rfc;
            clientControll.LblClientName.Content = client.LastName + " " + client.Surname + " " + client.Name;
            BitmapImage bitmap = new BitmapImage(new Uri("pack://application:,,,/Assets/ApplyForCreditDisabledIcon.png"));
            Image image = new Image
            {
                Source = bitmap,
                Height = 25,
                Width = 34,
            };
            clientControll.BtnApplyForCredit.Content = image;
            clientControll.BtnApplyForCredit.IsEnabled = false;
            if (client.Has_active_credit)
            {
                clientControll.LblClientCreditStatus.Content = "Cliente con crédito activo";
            }
            else if (client.Has_credit_application)
            {

                clientControll.LblClientCreditStatus.Content = "Cliente con solicitud de crédito activa";
            }
            ItcClients.Items.Add(clientControll);
        }

        private void BtnSeePersonalInformationClick(object sender, EventArgs e)
        {
            ClientControll clientControll = (ClientControll)sender;
        }

        private void BtnModifyBankAccountClick(object sender, EventArgs e)
        {
            ClientControll clientControll = (ClientControll)sender;
            Client client = new Client();
            for (int i = 0; i < clientsList.Count; i++)
            {
                if (clientsList[i].Rfc == clientControll.LblClientRFC.Content.ToString())
                {
                    client = clientsList[i];
                    break;
                }
            }
            if (client != null)
            {
                this.NavigationService.Navigate(new ModifyBankAccountController(client));
            }
        }

        private void BtnModifyPersonalInformationClick(object sender, EventArgs e)
        {
            ClientControll clientControll = (ClientControll)sender;
        }

        private void BtnModifyWorkCenterClick(object sender, EventArgs e)
        {
            ClientControll clientControll = (ClientControll)sender;
        }

        private void BtnModifyPersonalReferencesClick(object sender, EventArgs e)
        {
            ClientControll clientControll = (ClientControll)sender;
        }

        private void BtnApplyForCreditClick(object sender, EventArgs e)
        {
            ClientControll clientControll = (ClientControll)sender;
            Client client = new Client();
            for (int i = 0; i < clientsList.Count; i++)
            {
                if (clientsList[i].Rfc == clientControll.LblClientRFC.Content.ToString())
                {
                    client = clientsList[i];
                    break;
                }
            }
            if (client != null)
            {
                this.NavigationService.Navigate(new CredditApplicationController(client));
            }
        }

        private void BtnSearchClientClick(object sender, RoutedEventArgs e)
        {
            bool clientExists = false;
            string rfcWanted = TbRFCClient.Text.ToUpper().Trim();

            ItcClients.Items.Clear();
            for (int i = 0; i < clientsList.Count; i++)
            {
                if (clientsList[i].Rfc.Contains(rfcWanted))
                {
                    clientExists = true;
                    AddSearchedClientToClientList(clientsList[i]);
                }
                else
                {
                    clientExists = false;
                }
            }

            if (!clientExists)
            {
                ShowGoToRegisterClientConfirmationMessage();
            }
        }

        private void ShowGoToRegisterClientConfirmationMessage()
        {
            DialogResult resultado = MessageBox.Show("¿Deseas registrar al cliente?", "El cliente no existe", MessageBoxButtons.OKCancel);

            if (resultado == DialogResult.OK)
            {
                ClientRegisterController clientRegisterView = new ClientRegisterController();
                this.NavigationService.Navigate(clientRegisterView);
            }
            else if (resultado == DialogResult.Cancel)
            {
                AddClientsToClientsList();
                TbRFCClient.Text = string.Empty;
            }
        }

        private void BtnRegisterClientClick(object sender, RoutedEventArgs e)
        {
            ClientRegisterController clientRegisterView = new ClientRegisterController();
            this.NavigationService.Navigate(clientRegisterView);
        }

        private void BtnGoToLoginViewClick(object sender, RoutedEventArgs e)
        {
            RedirectToLoginView();
        }
    }
}
