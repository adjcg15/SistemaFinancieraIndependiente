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
            ApplyRestApplyRestrictionsOnFields();
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            ShowPersonalReference();
        }

        private void ApplyRestApplyRestrictionsOnFields()
        {
            RestrictOnlyLetters(TbName);
            RestrictOnlyLetters(TbSurname);
            RestrictOnlyLetters(TbLastName);
            RestrictOnlyLetters(TbKinship);
            RestrictOnlyLetters(TbCity);
            RestrictOnlyLetters(TbMunicipality);
            RestrictOnlyLetters(TbState);
            RestrictOnlyNumbers(TbRelationship);
            RestrictOnlyNumbers(TbPostCode);
            RestrictOnlyNumbers(TbPhoneNumber);
            RestrictOnlyNumbers(TbInteriorNumber);
            RestrictOnlyNumbers(TbOutdoorNumber);
        }

        private void RestrictOnlyLetters(TextBox textBox)
        {
            textBox.PreviewTextInput += (sender, e) =>
            {
                if (char.IsDigit(e.Text, e.Text.Length - 1))
                {
                    e.Handled = true;
                }
            };
        }

        private void RestrictOnlyNumbers(TextBox textBox)
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

        private void BtnDiscardPersonalReferenceUpdateClick(object sender, RoutedEventArgs e)
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

        private void BtnCancelPersonalReferenceUpdateClick(object sender, RoutedEventArgs e)
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
            bool isValidInformation = VerifyPersonalReferenceInformation();
            if (isValidInformation)
            {
                ShowPersonalReferenceUpdateConfirmationDialog();
            }
            else
            {
                HighLightInvalidFields();
                ShowInvalidFieldsAlertDialog();
            }
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
                UpdatePersonalReference();
            }
        }

        private bool VerifyPersonalReferenceInformation()
        {
            bool validFields = true;
            if (TbName.Text.Trim().Length == 0) validFields = false;
            if (TbLastName.Text.Trim().Length == 0) validFields = false;
            if (TbPhoneNumber.Text.Trim().Length < 10) validFields = false;
            if (TbKinship.Text.Trim().Length == 0) validFields = false;
            if (TbRelationship.Text.Trim().Length == 0) validFields = false;
            if (TbIneKey.Text.Trim().Length < 18) validFields = false;
            if (TbStreet.Text.Trim().Length == 0) validFields = false;
            if (TbNeighborhood.Text.Trim().Length == 0) validFields = false;
            if (TbInteriorNumber.Text.Trim().Length == 0) validFields = false;
            if (TbOutdoorNumber.Text.Trim().Length == 0) validFields = false;
            if (TbPostCode.Text.Trim().Length < 5) validFields = false;
            if (TbCity.Text.Trim().Length == 0) validFields = false;
            if (TbMunicipality.Text.Trim().Length == 0) validFields = false;
            if (TbState.Text.Trim().Length == 0) validFields = false;

            return validFields;
        }

        private void HighLightInvalidFields()
        {
            Style textInputErrorStyle = (Style)this.FindResource("SecondTextInputError");
            if (TbName.Text.Trim().Length == 0) TbName.Style = textInputErrorStyle;
            if (TbLastName.Text.Trim().Length == 0) TbLastName.Style = textInputErrorStyle;
            if (TbPhoneNumber.Text.Trim().Length < 10) TbPhoneNumber.Style = textInputErrorStyle;
            if (TbKinship.Text.Trim().Length == 0) TbKinship.Style = textInputErrorStyle;
            if (TbRelationship.Text.Trim().Length == 0) TbRelationship.Style = textInputErrorStyle;
            if (TbIneKey.Text.Trim().Length < 18) TbIneKey.Style = textInputErrorStyle;
            if (TbStreet.Text.Trim().Length == 0) TbIneKey.Style = textInputErrorStyle;
            if (TbNeighborhood.Text.Trim().Length == 0) TbNeighborhood.Style = textInputErrorStyle;
            if (TbInteriorNumber.Text.Trim().Length == 0) TbInteriorNumber.Style = textInputErrorStyle;
            if (TbOutdoorNumber.Text.Trim().Length == 0) TbOutdoorNumber.Style = textInputErrorStyle;
            if (TbPostCode.Text.Trim().Length < 5) TbPostCode.Style = textInputErrorStyle;
            if (TbCity.Text.Trim().Length == 0) TbCity.Style = textInputErrorStyle;
            if (TbMunicipality.Text.Trim().Length == 0) TbMunicipality.Style = textInputErrorStyle;
            if (TbState.Text.Trim().Length == 0) TbState.Style = textInputErrorStyle;

        }

        private void ShowInvalidFieldsAlertDialog()
        {
            MessageBox.Show(
                "Verifique que la información ingresada sea correcta y no existan campos vacíos",
                "Campos inválidos",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
        }

        private void UpdatePersonalReference()
        {
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
                IneKey = TbIneKey.Text.Trim().ToUpper(),
                ClientRfc = client.Rfc,
                Address = newAddress
            };

            try
            {
                bool updatedPersonalReference = clientsServiceClient.UpdatePersonalReference(newPersonalReference, currentIneKey);
                if (updatedPersonalReference)
                {
                    ShowPersonalReferenceSuccessfulUpdateMessageDialog();
                    RedirectToSearchClientByRFCView();
                }
                else
                {
                    ShowExistingPersonalReferenceMessageDialog();
                }
            }
            catch (FaultException<ServiceFault> fe)
            {
                ShowPersonalReferenceUpdateError(fe.Detail.Message);
                RedirectToSearchClientByRFCView();
            }
            catch (EndpointNotFoundException)
            {
                string errorMessage = "No fue posible actualizar la referencia personal del cliente, inténtelo de nuevo más tarde";
                ShowPersonalReferenceUpdateError(errorMessage);
                RedirectToSearchClientByRFCView();
            }
            catch (CommunicationException)
            {
                string errorMessage = "No fue posible actualizar la referencia personal del cliente, inténtelo de nuevo más tarde";
                ShowPersonalReferenceUpdateError(errorMessage);
                RedirectToSearchClientByRFCView();
            }
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

        private void ShowPersonalReferenceSuccessfulUpdateMessageDialog()
        {
            MessageBox.Show(
                "La información de la referencia personal del cliente " + client.Name + " " + client.Surname + " " + 
                client.LastName + " ha sido actualizada correctamente",
                "Actualización existosa",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void ShowExistingPersonalReferenceMessageDialog()
        {

            Style textInputErrorStyle = (Style)this.FindResource("SecondTextInputError");
            MessageBox.Show(
                "Verifique que la información ingresada para la clave del elector sea la correcta",
                "Referencia personal ya registrada para el cliente",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
            TbIneKey.Style = textInputErrorStyle;
        }

        private void ListenAndVerifyEmptyTextFields(object sender)
        {
            TextBox tbKey = sender as TextBox;

            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("SecondTextInputError");

            string key = tbKey.Text.Trim();

            if (key.Length == 0)
            {
                tbKey.Style = textInputErrorStyle;
            }
            else
            {
                tbKey.Style = textInputStyle;
            }
        }

        private void TbNameTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbSatateTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbMunicipalityTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbCityTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbPostCodeTextChanged(object sender, TextChangedEventArgs e)
        {
            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("SecondTextInputError");

            if (TbPostCode.Text.Trim().Length < 5)
            {
                TbPostCode.Style = textInputErrorStyle;
            }
            else
            {
                TbPostCode.Style = textInputStyle;
            }
        }

        private void TbOutdoorNumberTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbInteriorNumberTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbNeighborhoodTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbStreetTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbIneKeyTextChanged(object sender, TextChangedEventArgs e)
        {
            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("SecondTextInputError");

            if (TbIneKey.Text.Trim().Length < 18)
            {
                TbIneKey.Style = textInputErrorStyle;
            }
            else
            {
                TbIneKey.Style = textInputStyle;
            }
        }

        private void TbRelationshipTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbKinshipTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbPhoneNumberTextChanged(object sender, TextChangedEventArgs e)
        {
            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("SecondTextInputError");

            if (TbPhoneNumber.Text.Trim().Length < 10)
            {
                TbPhoneNumber.Style = textInputErrorStyle;
            }
            else
            {
                TbPhoneNumber.Style = textInputStyle;
            }
        }

        private void TbLastNameTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }
    }
}