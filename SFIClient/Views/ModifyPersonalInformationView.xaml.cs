using SFIClient.SFIServices;
using SFIClient.Utilities;
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
        private Client client;

        public ModifyPersonalInformationController(string clientRFC)
        {
            InitializeComponent();
            this.clientRFC = clientRFC;
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            LoadClientPersonalInformation();
            ApplyFieldsRestrictions();
        }

        private void LoadClientPersonalInformation()
        {
            ClientsServiceClient clientsService = new ClientsServiceClient();

            try
            {
                Client client = clientsService.GetClientPersonalInformation(clientRFC);
                this.client = client;
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

        private void ApplyFieldsRestrictions()
        {
            RestrictToPlainText(TbClientName);
            RestrictToPlainText(TbClientLastName);
            RestrictToPlainText(TbClientSurname);
            RestrictToPlainText(TbAddressStreet);
            RestrictToPlainText(TbAddressNeighborhood);
            RestrictToPlainText(TbAddressCity);
            RestrictToPlainText(TbAddressMunicipality);
            RestrictToPlainText(TbAddressState);

            RestrictToNumeric(TbAddressInteriorNumber);
            RestrictToNumeric(TbAddressOutdoorNumber);
            RestrictToNumeric(TbAddressPostCode);
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
            HideFieldsErrors();

            bool isValidClientInformation = ValidateClientPersonalInformation();
            if(isValidClientInformation)
            {
                ShowSaveChangesConfirmationDialog();
            }
            else
            {
                HighlightInvalidFields();
                ShowInvalidFieldsAlertDialog();
            }
        }

        private void HideFieldsErrors()
        {
            TbClientName.Style = (Style)FindResource("TextInput");
            TbClientLastName.Style = (Style)FindResource("TextInput");
            TbClientSurname.Style = (Style)FindResource("TextInput");
            BdrClientBirthDate.BorderBrush = (Brush)FindResource("MediumGray");

            TbAddressStreet.Style = (Style)FindResource("TextInput");
            TbAddressNeighborhood.Style = (Style)FindResource("TextInput");
            TbAddressInteriorNumber.Style = (Style)FindResource("TextInput");
            TbAddressOutdoorNumber.Style = (Style)FindResource("TextInput");
            TbAddressPostCode.Style = (Style)FindResource("TextInput");
            TbAddressCity.Style = (Style)FindResource("TextInput");
            TbAddressMunicipality.Style = (Style)FindResource("TextInput");
            TbAddressState.Style = (Style)FindResource("TextInput");
        }

        private bool ValidateClientPersonalInformation()
        {
            bool isValidInformation =
                !string.IsNullOrWhiteSpace(TbClientName.Text)
                && !string.IsNullOrWhiteSpace(TbClientLastName.Text)
                && !string.IsNullOrWhiteSpace(TbClientSurname.Text)
                && DpkClientBirthDate.SelectedDate.HasValue 
                && DpkClientBirthDate.SelectedDate.Value < DateTime.Now.AddYears(-18)
                && !string.IsNullOrWhiteSpace(TbAddressStreet.Text)
                && !string.IsNullOrWhiteSpace(TbAddressNeighborhood.Text)
                && !string.IsNullOrWhiteSpace(TbAddressInteriorNumber.Text)
                && !string.IsNullOrWhiteSpace(TbAddressOutdoorNumber.Text)
                && !string.IsNullOrWhiteSpace(TbAddressPostCode.Text)
                && !string.IsNullOrWhiteSpace(TbAddressCity.Text)
                && !string.IsNullOrWhiteSpace(TbAddressMunicipality.Text)
                && !string.IsNullOrWhiteSpace(TbAddressState.Text);

            return isValidInformation;
        }

        private void HighlightInvalidFields()
        {
            if (string.IsNullOrWhiteSpace(TbClientName.Text))
            {
                TbClientName.Style = (Style)FindResource("TextInputError");
            }

            if (string.IsNullOrWhiteSpace(TbClientLastName.Text))
            {
                TbClientLastName.Style = (Style)FindResource("TextInputError");
            }

            if (string.IsNullOrWhiteSpace(TbClientSurname.Text))
            {
                TbClientSurname.Style = (Style)FindResource("TextInputError");
            }

            if (!DpkClientBirthDate.SelectedDate.HasValue
                || DpkClientBirthDate.SelectedDate.Value < DateTime.Now.AddYears(-18))
            {
                BdrClientBirthDate.BorderBrush = (Brush)FindResource("Red");
            }

            if (string.IsNullOrWhiteSpace(TbAddressStreet.Text))
            {
                TbAddressStreet.Style = (Style)FindResource("TextInputError");
            }

            if (string.IsNullOrWhiteSpace(TbAddressNeighborhood.Text))
            {
                TbAddressNeighborhood.Style = (Style)FindResource("TextInputError");
            }

            if (string.IsNullOrWhiteSpace(TbAddressInteriorNumber.Text))
            {
                TbAddressInteriorNumber.Style = (Style)FindResource("TextInputError");
            }

            if (string.IsNullOrWhiteSpace(TbAddressOutdoorNumber.Text))
            {
                TbAddressOutdoorNumber.Style = (Style)FindResource("TextInputError");
            }

            if (string.IsNullOrWhiteSpace(TbAddressPostCode.Text))
            {
                TbAddressPostCode.Style = (Style)FindResource("TextInputError");
            }

            if (string.IsNullOrWhiteSpace(TbAddressCity.Text))
            {
                TbAddressCity.Style = (Style)FindResource("TextInputError");
            }

            if (string.IsNullOrWhiteSpace(TbAddressMunicipality.Text))
            {
                TbAddressMunicipality.Style = (Style)FindResource("TextInputError");
            }

            if (string.IsNullOrWhiteSpace(TbAddressState.Text))
            {
                TbAddressState.Style = (Style)FindResource("TextInputError");
            }
        }
        
        private void ShowInvalidFieldsAlertDialog()
        {
            MessageBox.Show(
                "Verifique que la información ingresada en los campos marcados " +
                "en rojo sea correcta. Recuerde que el cliente debe ser mayor de edad",
                "Campos inválidos",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
        }

        private void ShowSaveChangesConfirmationDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                "¿Está seguro que desea guardar los cambios realizados al cliente" 
                + client.Name + " " + client.LastName,
                "Confirmación de actualización",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );


            if ( buttonClicked == MessageBoxResult.Yes )
            {
                UpdateClientPersonalInformation();
            }
        }

        private void UpdateClientPersonalInformation()
        {

        }

        private void RestrictToPlainText(TextBox textBox)
        {
            textBox.PreviewTextInput += (sender, e) =>
            {
                if (!DataValidator.IsPlainText(new string(e.Text[e.Text.Length - 1], 1)))
                {
                    e.Handled = true;
                }
            };
        }

        private void RestrictToNumeric(TextBox textBox)
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
    }
}
