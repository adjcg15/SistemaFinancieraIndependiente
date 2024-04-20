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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SFIClient.Views
{
    /// <summary>
    /// Lógica de interacción para ModifyPersonalReferenceView.xaml
    /// </summary>
    public partial class ModifyPersonalReferenceController : Page
    {
        private readonly ClientsServiceClient clientsServiceClient = new ClientsServiceClient();
        private readonly PersonalReference personalReference;
        private readonly string currentIneKey;
        private readonly Client client;
        public ModifyPersonalReferenceController(PersonalReference personalReference, Client client)
        {
            InitializeComponent();
            this.personalReference = personalReference;
            this.client = client;
            currentIneKey = personalReference.IneKey;
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            ShowPersonalReference();
        }

        private void ShowPersonalReference()
        {
            TbName.Text = personalReference.Name;
            TbSurname.Text = personalReference.Surname;
            TbLastName.Text = personalReference.LastName;
            TbPhoneNumber.Text = personalReference.PhoneNumber;
            TbKinship.Text = personalReference.Kinship;
            TbRelationship.Text = personalReference.RelationshipYears;
            TbIneKey.Text = personalReference.IneKey;
            TbStreet.Text = personalReference.Address.Street;
            TbNeighborhood.Text = personalReference.Address.Neighborhod;
            TbInteriorNumber.Text = personalReference.Address.InteriorNumber;
            TbOutdoorNumber.Text = personalReference.Address.OutdoorNumber;
            TbPostCode.Text = personalReference.Address.PostCode;
            TbCity.Text = personalReference.Address.City;
            TbMunicipality.Text = personalReference.Address.Municipality;
            TbState.Text = personalReference.Address.State;
        }

        private void BtnDiscardUpdatePersonalReferenceClick(object sender, RoutedEventArgs e)
        {
            ShowDiscardUpdatePersonalReferenceDialog();
        }

        private void ShowDiscardUpdatePersonalReferenceDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                "¿Está seguro que desea regresar a la ventana previa? Todos los cambios sin guardar se perderán",
                "Regresar a ventana previa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (buttonClicked == MessageBoxResult.Yes)
            {
                RedirectToSearchClientByRFCView();
            }
        }

        private void RedirectToSearchClientByRFCView()
        {
            NavigationService.Navigate(new SearchClientByRFCController());
        }

        private void BtnCancelUpdatePersonalReferenceClick(object sender, RoutedEventArgs e)
        {
            ShowCancelUpdatePersonalReferenceDialog();
        }

        private void ShowCancelUpdatePersonalReferenceDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                "¿Está seguro que desea cancelar la actualización de la referencia personal del cliente? Todos los cambios sin guardar se perderán",
                "Cancelar cambios",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (buttonClicked == MessageBoxResult.Yes)
            {
                RedirectToSearchClientByRFCView();
            }
        }

        private void BtnUpdatePersonalReferenceClick(object sender, RoutedEventArgs e)
        {
            ShowPersonalReferenceUpdateConfirmationDialog();
        }

        private void ShowPersonalReferenceUpdateConfirmationDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                "¿Está seguro que desea guardar los cambios realizados a la referencia personal del cliente " 
                + client.Name + " " + client.Surname + " " + client.LastName,
                "Confirmación de actualización",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (buttonClicked == MessageBoxResult.Yes)
            {
                if (VerifyPersonalReferenceInformation())
                {
                    UpdatePersonalReference();
                }
            }
        }

        private bool VerifyPersonalReferenceInformation()
        {
            return true;
        }

        private bool UpdatePersonalReference()
        {
            bool updatedPersonalReference = false;

            Address newAddress = new Address
            {
                IdAddress = personalReference.Address.IdAddress,
                Street = TbStreet.Text.Trim(),
                Neighborhod = TbNeighborhood.Text.Trim(),
                InteriorNumber = TbInteriorNumber.Text.Trim(),
                OutdoorNumber = TbOutdoorNumber.Text.Trim(),
                PostCode = TbInteriorNumber.Text.Trim(),
                Municipality = TbMunicipality.Text.Trim(),
                City = TbCity.Text.Trim(),
                State = TbStreet.Text.Trim(),
            };

            PersonalReference newPersonalReference = new PersonalReference
            {
                IdPersonalReference = personalReference.IdPersonalReference,
                Name = TbName.Text.Trim(),
                Surname = TbSurname.Text.Trim(),
                LastName = TbLastName.Text.Trim(),
                PhoneNumber = TbPhoneNumber.Text.Trim(),
                Kinship = TbKinship.Text.Trim(),
                RelationshipYears = TbRelationship.Text.Trim(),
                IneKey = TbIneKey.Text.Trim(),
                ClientRfc = client.Rfc,
                Address = newAddress
            };

            try
            {
                updatedPersonalReference = clientsServiceClient.UpdatePersonalReference(newPersonalReference, currentIneKey);
            }
            catch (FaultException<ServiceFault> fe)
            {
                ShowPersonalReferenceUpdateError(fe.Message);
            }
            catch (EndpointNotFoundException)
            {
                string errorMessage = "Por el momento el servidor no se encuentra disponible, intente más tarde";
                ShowPersonalReferenceUpdateError(errorMessage);
                RedirectToSearchClienByRFCView();
            }
            catch (CommunicationException)
            {
                string errorMessage = "Por el momento el servidor no se encuentra disponible, intente más tarde";
                ShowPersonalReferenceUpdateError(errorMessage);
                RedirectToSearchClienByRFCView();
            }

            return updatedPersonalReference;
        }

        private void ShowPersonalReferenceUpdateError(string message)
        {
            MessageBox.Show(
                message,
                "Sistema no disponible",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }

        private void RedirectToSearchClienByRFCView()
        {
            NavigationService.Navigate(new SearchClientByRFCController());
        }

        private void TbSatateTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbMunicipalityTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbCityTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbPostCodeTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbOutdoorNumberTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbInteriorNumberTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbNeighborhoodTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbStreetTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbIneKeyTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbRelationshipTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbKinshipTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbPhoneNumberTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbLastNameTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbSurnameTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbNameTextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
