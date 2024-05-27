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
        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            LoadClientWorkCenterInformation();
            ApplyFieldsRestrictions();
        }

        private void BtnReturnToPreviousPageClick(object sender, RoutedEventArgs e)
        {
            ShowReturnToPreviousPageConfirmationDialog();

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
        private void BtnSaveChangesClick(object sender, RoutedEventArgs e)
        {
            HideFieldsErrors();

            bool isValidClientWorkCenterInformation = ValidateClientWorkCenterInformation();
            if (isValidClientWorkCenterInformation)
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
            TbCompanyName.Style = (Style)FindResource("TextInput");
            TbWorkCenterPhoneNumber.Style = (Style)FindResource("TextInput");
            TbEmployeePosition.Style = (Style)FindResource("TextInput");
            TbSalary.Style = (Style)FindResource("TextInput");
            TbHumanResourcesPhone.Style = (Style)FindResource("TextInput");
            TbEmployeeSeniority.Style = (Style)FindResource("TextInput");

            TbAddressStreet.Style = (Style)FindResource("TextInput");
            TbAddressNeighborhood.Style = (Style)FindResource("TextInput");
            TbAddressInteriorNumber.Style = (Style)FindResource("TextInput");
            TbAddressOutdoorNumber.Style = (Style)FindResource("TextInput");
            TbAddressPostCode.Style = (Style)FindResource("TextInput");
            TbAddressCity.Style = (Style)FindResource("TextInput");
            TbAddressMunicipality.Style = (Style)FindResource("TextInput");
            TbAddressState.Style = (Style)FindResource("TextInput");
        }
        private bool ValidateClientWorkCenterInformation()
        {
            bool isValidInformation =
                !string.IsNullOrWhiteSpace(TbCompanyName.Text)
                && !string.IsNullOrWhiteSpace(TbWorkCenterPhoneNumber.Text)
                && !string.IsNullOrWhiteSpace(TbEmployeePosition.Text)
                && !string.IsNullOrWhiteSpace(TbSalary.Text)
                && !string.IsNullOrWhiteSpace(TbEmployeeSeniority.Text)
                && !string.IsNullOrWhiteSpace(TbHumanResourcesPhone.Text)
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
            if (string.IsNullOrWhiteSpace(TbCompanyName.Text))
            {
                TbCompanyName.Style = (Style)FindResource("TextInputError");
            }

            if (string.IsNullOrWhiteSpace(TbWorkCenterPhoneNumber.Text))
            {
                TbWorkCenterPhoneNumber.Style = (Style)FindResource("TextInputError");
            }
            if(string.IsNullOrWhiteSpace(TbEmployeePosition.Text))
            {
                TbEmployeePosition.Style = (Style)FindResource("TextInputError");
            }
            if (string.IsNullOrWhiteSpace(TbSalary.Text))
            {
                TbSalary.Style = (Style)FindResource("TextInputError");
            }
            if (string.IsNullOrWhiteSpace(TbEmployeeSeniority.Text))
            {
                TbEmployeeSeniority.Style = (Style)FindResource("TextInputError");
            }
            if (string.IsNullOrWhiteSpace(TbHumanResourcesPhone.Text))
            {
                TbHumanResourcesPhone.Style = (Style)FindResource("TextInputError");
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
                "en rojo sea correcta",
                "Campos inválidos",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
        }

        private void ShowSaveChangesConfirmationDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                "¿Está seguro que desea guardar los cambios realizados al centro de trabajo"
                + client.Name + " " + client.LastName,
                "Confirmación de actualización",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );
            if (buttonClicked == MessageBoxResult.Yes)
            {
                UpdateWorkCenterInformation();
            }
        }

        private void UpdateWorkCenterInformation()
        {
            ClientsServiceClient clientsService = new ClientsServiceClient();

            try
            {
                client.WorkCenter.CompanyName = TbCompanyName.Text.Trim();
                client.WorkCenter.PhoneNumber = TbWorkCenterPhoneNumber.Text.Trim();
                client.WorkCenter.EmployeePosition = TbEmployeePosition.Text.Trim();
                client.WorkCenter.Salary = decimal.Parse(TbSalary.Text.Trim());
                client.WorkCenter.EmployeeSeniority = TbEmployeeSeniority.Text.Trim();
                client.WorkCenter.HumanResourcesPhone = TbHumanResourcesPhone.Text.Trim();

                if (client.WorkCenter.Address == null)
                {
                    client.WorkCenter.Address = new Address();
                }
                client.WorkCenter.Address.Street = TbAddressStreet.Text.Trim();
                client.WorkCenter.Address.Neighborhod = TbAddressNeighborhood.Text.Trim();
                client.WorkCenter.Address.InteriorNumber = TbAddressInteriorNumber.Text.Trim();
                client.WorkCenter.Address.OutdoorNumber = TbAddressOutdoorNumber.Text.Trim();
                client.WorkCenter.Address.PostCode = TbAddressPostCode.Text.Trim();
                client.WorkCenter.Address.City = TbAddressCity.Text.Trim();
                client.WorkCenter.Address.Municipality = TbAddressMunicipality.Text.Trim();
                client.WorkCenter.Address.State = TbAddressState.Text.Trim();

                bool updated = clientsService.UpdateClientWorkCenterlInformation(client);
                if (updated)
                {
                    ShowSuccessfulUpdateInformationDialog();
                }
            }
            catch (FaultException<ServiceFault> fault)
            {
                ShowErrorSavingChangesDialog(fault.Detail.Message);
            }
            catch (EndpointNotFoundException)
            {
                string errorMessage = "Servidor no disponible. No fue posible actualizar la información " +
                    "del centro de trabajo del cliente, por favor inténtelo más tarde";
                ShowErrorSavingChangesDialog(errorMessage);
            }
            catch (CommunicationException)
            {
                string errorMessage = "Error de conexión. Ocurrió un error de conexión, por favor compruebe " +
                    "su conexión de Internet e intente de nuevo";
                ShowErrorSavingChangesDialog(errorMessage);
            }
        }

        private void ShowErrorSavingChangesDialog(string message)
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                message,
                "No fue posible actualizar la información del centro de trabajo del cliente",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );

            if (buttonClicked == MessageBoxResult.OK)
            {
                RedirectToMainMenu();
            }
        }

        private void ShowSuccessfulUpdateInformationDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                "La información del centro de trabajo del cliente " + client.Name + " "
                + client.LastName + " ha sido actualizada correctamente",
                "Actualización exitosa",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            if (buttonClicked == MessageBoxResult.OK)
            {
                RedirectToSearchClientByRFC();
            }
        }
        private void ApplyFieldsRestrictions()
        {
            RestrictToPlainText(TbCompanyName);
            RestrictToPlainText(TbEmployeePosition);
            RestrictToPlainText(TbAddressStreet);
            RestrictToPlainText(TbAddressNeighborhood);
            RestrictToPlainText(TbAddressCity);
            RestrictToPlainText(TbAddressMunicipality);
            RestrictToPlainText(TbAddressState);

            RestrictToNumeric(TbAddressInteriorNumber);
            RestrictToNumeric(TbAddressOutdoorNumber);
            RestrictToNumeric(TbAddressPostCode);
            RestrictToNumeric(TbSalary);
            RestrictToNumeric(TbEmployeeSeniority);
            RestrictToNumeric(TbHumanResourcesPhone);
            RestrictToNumeric(TbWorkCenterPhoneNumber);
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