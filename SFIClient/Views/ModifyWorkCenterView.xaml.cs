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
    /// <summary>
    /// Lógica de interacción para ModifyWorkCenterView.xaml
    /// </summary>
    public partial class ModifyWorkCenterController : Page
    {
        private readonly string clientRFC;
        private Client client;
        public ModifyWorkCenterController(string clientRFC)
        {
            InitializeComponent();
            this.clientRFC = clientRFC;
            LoadClientWorkCenterInformation();
        }
        private void LoadClientWorkCenterInformation()
        {
            ClientsServiceClient clientsService = new ClientsServiceClient();

            try
            {
                Client client = clientsService.GetWorkCenterInformation(clientRFC);
                this.client = client;
                ShowWorkCenterInformation(client);
            }
            catch (FaultException<ServiceFault> fault)
            {
                ShowErrorRecoveringWorkCenterInformation(fault.Detail.Message);
            }
            catch (EndpointNotFoundException)
            {
                string errorMessage = "Servidor no disponible. No fue posible recuperar la información " +
                    "del cliente, por favor inténtelo más tarde";
                ShowErrorRecoveringWorkCenterInformation(errorMessage);
            }
            catch (CommunicationException)
            {
                string errorMessage = "Error de conexión. Ocurrió un error de conexión, por favor compruebe " +
                    "su conexión de Internet e intente de nuevo";
                ShowErrorRecoveringWorkCenterInformation(errorMessage);
            }
        }
        private void ShowWorkCenterInformation(Client client)
        {
            string fullName = client.Name + " " + client.LastName + " " + client.Surname;
            TbkClientName.Text = fullName;
            TbCompanyName.Text = client.WorkCenter.CompanyName;
            TbWorkCenterPhoneNumber.Text = client.WorkCenter.PhoneNumber;
            TbEmployeePosition.Text = client.WorkCenter.EmployeePosition;
            TbSalary.Text = client.WorkCenter.Salary.ToString();
            TbEmployeeSeniority.Text = client.WorkCenter.EmployeeSeniority;
            TbHumanResourcesPhone.Text = client.WorkCenter.HumanResourcesPhone;

            if (client.WorkCenter!= null)
            {
                TbAddressStreet.Text = client.WorkCenter.Address.Street;
                TbAddressNeighborhood.Text = client.WorkCenter.Address.Neighborhod;
                TbAddressInteriorNumber.Text = client.WorkCenter.Address.InteriorNumber.Trim();
                TbAddressOutdoorNumber.Text = client.WorkCenter.Address.OutdoorNumber.Trim();
                TbAddressPostCode.Text = client.WorkCenter.Address.PostCode;
                TbAddressCity.Text = client.WorkCenter.Address.City;
                TbAddressMunicipality.Text = client.WorkCenter.Address.Municipality;
                TbAddressState.Text = client.WorkCenter.Address.State;
            }
        }
        private void ShowErrorRecoveringWorkCenterInformation(string message)
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                message,
                "No fue posible recuperar la información del cliente",
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
            NavigationService.Navigate(new MainMenuController());
        }

        private void TbWorkCenterPhoneNumberTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbSalaryTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbHumanResourcesPhoneTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbEmployeeSeniorityTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbEmployeePositionTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbCompanyNameTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void BtnSaveChangesClick(object sender, RoutedEventArgs e)
        {

        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {

        }

        private void BtnReturnToPreviousPageClick(object sender, RoutedEventArgs e)
        {

        }

        private void BtnCancelClick(object sender, RoutedEventArgs e)
        {

        }
    }
}