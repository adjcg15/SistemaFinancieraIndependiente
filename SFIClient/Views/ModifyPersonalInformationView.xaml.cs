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
    public partial class ModifyPersonalInformationController : Page
    {
        private readonly string clientRFC;

        public ModifyPersonalInformationController(string clientRFC)
        {
            InitializeComponent();
            this.clientRFC = clientRFC;
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            LoadClientPersonalInformation();
        }

        private void LoadClientPersonalInformation()
        {
            ClientsServiceClient clientsService = new ClientsServiceClient();

            try
            {
                Client client = clientsService.GetClientPersonalInformation(clientRFC);
                ShowClientPersonalInformation(client);
            }
            catch (FaultException<ServiceFault> fault)
            {
                ShowErrorRecoveringClientPersonalInformationDialog(fault.Detail.Message);
            }
            catch (EndpointNotFoundException)
            {
                string errorMessage = "Servidor no disponible. No fue posible recuperar la información " +
                    "del cliente, por favor inténtelo más tarde";
                ShowErrorRecoveringClientPersonalInformationDialog(errorMessage);
            }
            catch (CommunicationException)
            {
                string errorMessage = "Error de conexión. Ocurrió un error de conexión, por favor compruebe " +
                    "su conexión de Internet e intente de nuevo";
                ShowErrorRecoveringClientPersonalInformationDialog(errorMessage);
            }
        }

        private void ShowErrorRecoveringClientPersonalInformationDialog(string message)
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

        private void RedirectToMainMenu() {
            NavigationService.Navigate(new MainMenuController());
        }

        private void ShowClientPersonalInformation(Client client)
        {
            string fullName = client.Name + " " + client.LastName + " " + client.Surname;

            TbkClientName.Text = fullName;
            TbClientName.Text = client.Name;
            TbClientLastName.Text = client.LastName;
            TbClientSurname.Text = client.Surname;
            DpkClientBirthDate.SelectedDate = client.Birthdate;
            TbClientCURP.Text = client.Curp;
            TbClientRFC.Text = client.Rfc;

            if(client.Address != null)
            {
                TbAddressStreet.Text = client.Address.Street;
                TbAddressNeighborhood.Text = client.Address.Neighborhod;
                TbAddressInteriorNumber.Text = client.Address.InteriorNumber;
                TbAddressOutdoorNumber.Text = client.Address.OutdoorNumber;
                TbAddressPostCode.Text = client.Address.PostCode;
                TbAddressCity.Text = client.Address.City;
                TbAddressMunicipality.Text = client.Address.Municipality;
                TbAddressState.Text = client.Address.State;
            }
        }

        private void BtnReturnToPreviousPageClick(object sender, RoutedEventArgs e)
        {
            ShowReturnToPreviousPageConfirmationDialog();
        }

        private void ShowReturnToPreviousPageConfirmationDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                "¿Está seguro que desea regresar a la ventana previa? Todos los cambios sin guardar se perderán",
                "Regresar a ventana previa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (buttonClicked == MessageBoxResult.Yes)
            {
                RedirectToSearchClientByRFC();
            }
        }

        private void RedirectToSearchClientByRFC()
        {
            NavigationService.Navigate(new SearchClientByRFCController());
        }

        private void BtnCancelClick(object sender, RoutedEventArgs e)
        {
            ShowDiscardChangesConfirmationDialog();
        }

        private void ShowDiscardChangesConfirmationDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                "¿Está seguro que desea cancelar la actualización del cliente? Todos los cambios " +
                "sin guardar se perderán",
                "Descartar cambios",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (buttonClicked == MessageBoxResult.Yes)
            {
                RedirectToSearchClientByRFC();
            }
        }

        private void BtnSaveChangesClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
