using SFIClient.Controlls;
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
    /// Lógica de interacción para SearchClientByRFCView.xaml
    /// </summary>
    public partial class SearchClientByRFCView : Page
    {
        private readonly ClientsServiceClient clientsServiceClient = new ClientsServiceClient();
        private List<Client> clientsList = new List<Client>();
        public SearchClientByRFCView()
        {
            InitializeComponent();
            LoadClients();
        }

        private void LoadClients()
        {
            try
            {
                clientsList = clientsServiceClient.RecoverClients().ToList();
                if (clientsList.Count != 0)
                {
                    AddClientsToClientsList(clientsList);
                }
                else
                {
                    SkpRegisterClient.Children.Clear();
                    SkpRegisterClient.Visibility = Visibility.Collapsed;
                    SkpSearchClient.Visibility = Visibility.Collapsed;
                    SkpRegisterClientNow.Children.Add(BtnRegisterClient);
                }
            }
            catch (FaultException fe)
            {
                MessageBox.Show(fe.Message);
            }
        }

        private void AddClientsToClientsList(List<Client> clientsList)
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

        private void ShowClientWithAllOptions(Client client)
        {
            ClientControll clientControll = new ClientControll();
            clientControll.LblClientRFC.Content = client.Rfc;
            clientControll.LblClientName.Content = client.LastName + " " + client.Surname + " " + client.Name;
            clientControll.LblClientCreditStatus.Content = "Cliente sin crédito y sin solicitud activa";
            ItcClients.Items.Add(clientControll);
        }

        private void ShowClientWithAllOptionsWithoutCreditApplication(Client client)
        {
            ClientControll clientControll = new ClientControll();
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
        }

        private void BtnSearchClient_Click(object sender, RoutedEventArgs e)
        {
            string rfcWanted = TbRFCClient.Text.ToUpper().Trim();

            ItcClients.Items.Clear();
            for (int i = 0; i < clientsList.Count; i++)
            {
                if (clientsList[i].Rfc.Contains(rfcWanted))
                {
                    ClientControll clientControll = new ClientControll();
                    clientControll.LblClientRFC.Content = clientsList[i].Rfc;
                    ItcClients.Items.Add(clientControll);
                }
            }
        }
    }
}
