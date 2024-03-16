using SFIClient.Controlls;
using System;
using System.Collections.Generic;
using System.Linq;
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

        int clientes = 5;
        bool credit;

        string[] rfcs = { "RFC1", "RFC2", "RFC3", "RFC4", "RFC5" };
        public SearchClientByRFCView()
        {
            InitializeComponent();
            AddClientsToClientsList();
        }

        private void AddClientsToClientsList()
        {

            if (clientes > 0)
            {
                credit = false;
                SkpNoRegisteredClients.Visibility = Visibility.Collapsed;
                SkpRegisterClientNow.Visibility = Visibility.Collapsed;
                for (int i = 0; i < clientes; i++)
                {
                    ClientControll clienteControl = new ClientControll();
                    clienteControl.LblClientRFC.Content = rfcs[i];
                    ItcClients.Items.Add(clienteControl);
                    if (i == 3)
                    {
                        credit = true;
                    }
                    if (credit)
                    {
                        BitmapImage bitmap = new BitmapImage(new Uri("pack://application:,,,/Assets/ApplyForCreditDisabledIcon.png"));
                        Image image = new Image
                        {
                            Source = bitmap,
                            Height = 25,
                            Width = 34,
                        };
                        clienteControl.BtnApplyForCredit.Content = image;
                    }
                }
            }
            else
            {
                SkpRegisterClient.Children.Clear();
                SkpRegisterClient.Visibility = Visibility.Collapsed;
                SkpSearchClient.Visibility = Visibility.Collapsed;
                SkpRegisterClientNow.Children.Add(BtnRegisterClient);
            }
        }

        private void BtnSearchClient_Click(object sender, RoutedEventArgs e)
        {

            string rfcWanted = TbRFCClient.Text.ToUpper().Trim();

            ItcClients.Items.Clear();
            for (int i = 0; i < 5; i++)
            {
                if (rfcs[i].Contains(rfcWanted))
                {
                    ClientControll clientControll = new ClientControll();
                    clientControll.LblClientRFC.Content = rfcs[i];
                    ItcClients.Items.Add(clientControll);
                }
            }
        }
    }
}
