using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
    /// Lógica de interacción para ClientRegisterView.xaml
    /// </summary>
    public partial class ClientRegisterView : Page
    {
        public ClientRegisterView()
        {
            InitializeComponent();
            TbkClientPhoneNumberThird.Visibility = Visibility.Collapsed;
            GrdClientPhoneNumberThird.Visibility = Visibility.Collapsed;
            TbkClientPhoneNumberFourth.Visibility = Visibility.Collapsed;
            GrdClientPhoneNumberFourth.Visibility = Visibility.Collapsed;
            TbkClientEmailSecond.Visibility = Visibility.Collapsed;
            GrdClientEmailSecond.Visibility = Visibility.Collapsed;
            TbkClientEmailThird.Visibility = Visibility.Collapsed;
            GrdClientEmailThird.Visibility = Visibility.Collapsed;
        }

        private void BtnGoBackClick(object sender, RoutedEventArgs e)
        {
            SearchClientByRFCView searchClientByRFCView = new SearchClientByRFCView();
            this.NavigationService.Navigate(searchClientByRFCView);
        }

        private void BtnNewPhoneNumberClick(object sender, RoutedEventArgs e)
        {
            if (TbkClientPhoneNumberThird.Visibility == Visibility.Collapsed)
            {
                TbkClientPhoneNumberThird.Visibility = Visibility.Visible;
                GrdClientPhoneNumberThird.Visibility = Visibility.Visible;
                return;
            }
            if (TbkClientPhoneNumberFourth.Visibility == Visibility.Collapsed && TbkClientPhoneNumberThird.Visibility == Visibility.Visible)
            {
                TbkClientPhoneNumberFourth.Visibility = Visibility.Visible;
                GrdClientPhoneNumberFourth.Visibility = Visibility.Visible;
            }
            if (TbkClientPhoneNumberFourth.Visibility == Visibility.Visible)
            {
                SkpNewPhoneNumber.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnNewEmailClick(object sender, RoutedEventArgs e)
        {
            if (TbkClientEmailSecond.Visibility == Visibility.Collapsed)
            {
                TbkClientEmailSecond.Visibility = Visibility.Visible;
                GrdClientEmailSecond.Visibility = Visibility.Visible;
                return;
            }
            if (TbkClientEmailThird.Visibility == Visibility.Collapsed && TbkClientEmailSecond.Visibility == Visibility.Visible)
            {
                TbkClientEmailThird.Visibility = Visibility.Visible;
                GrdClientEmailThird.Visibility = Visibility.Visible;
            }
            if (TbkClientEmailThird.Visibility == Visibility.Visible)
            {
                SkpNewEmail.Visibility = Visibility.Collapsed;
            }
        }

        private bool VerifyTextFields()
        {
            return 
                VerifyPersonalInformationTextFields() 
                & VerifyClientAddressTextFields() 
                & VerifyBankAccountTextFields() 
                & VerifyWorkCenterTextFields()
                & VerifyWorkCenterAddressTextFields()
                & VerifyClientContactMethodsTextFields();
        }

        private bool VerifyPersonalInformationTextFields()
        {
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError");
            Style borderErrorStyle = (Style)this.FindResource("RoundedBorderError");

            bool correctFields = true;
            if (TbClientName.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbClientName.Style = textInputErrorStyle;
            }
            if (TbClientSurname.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbClientSurname.Style = textInputErrorStyle;
            }
            if (TbClientLastName.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbClientLastName.Style = textInputErrorStyle;
            }
            if (DpkClientBirthdate.SelectedDate.ToString().Length == 0)
            {
                correctFields = false;
                BdrClientBirthdate.Style = borderErrorStyle;
            }
            if (TbClientCurp.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbClientCurp.Style = textInputErrorStyle;
            }
            if (TbClientRfc.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbClientRfc.Style = textInputErrorStyle;
            }

            return correctFields;
        }

        private bool VerifyClientAddressTextFields()
        {
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError");
            bool correctFields = true;

            if (TbClientStreet.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbClientStreet.Style = textInputErrorStyle;
            }
            if (TbClientNeighborhood.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbClientNeighborhood.Style = textInputErrorStyle;
            }
            if (TbClientInteriorNumber.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbClientInteriorNumber.Style = textInputErrorStyle;
            }
            if (TbClientOutdoorNumber.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbClientOutdoorNumber.Style = textInputErrorStyle;
            }
            if (TbClientPostCode.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbClientPostCode.Style = textInputErrorStyle;
            }
            if (TbClientCity.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbClientCity.Style = textInputErrorStyle;
            }
            if (TbClientMunicipality.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbClientMunicipality.Style = textInputErrorStyle;
            }
            if (TbClientSatate.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbClientSatate.Style = textInputErrorStyle;
            }

            return correctFields;
        }

        private bool VerifyBankAccountTextFields()
        {
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError");
            bool correctFields = true;
            if (!VerifyCarNumberFormat(TbCardNumber.Text.Trim()))
            {
                correctFields = false;
                TbCardNumber.Style = textInputErrorStyle;
            }
            if (TbBank.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbBank.Style = textInputErrorStyle;
            }
            if (TbHolder.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbHolder.Style = textInputErrorStyle;
            }

            return correctFields;
        }

        private bool VerifyWorkCenterTextFields()
        {
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError");
            bool correctFields = true;

            if (TbCompanyName.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbCompanyName.Style = textInputErrorStyle;
            }
            if (TbWorkCenterPhoneNumber.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbWorkCenterPhoneNumber.Style = textInputErrorStyle;
            }
            if (TbEmployeePosition.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbEmployeePosition.Style = textInputErrorStyle;
            }
            if (TbSalary.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbSalary.Style = textInputErrorStyle;
            }
            if (TbEmployeeSeniority.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbEmployeeSeniority.Style = textInputErrorStyle;
            }
            if (TbHumanResourcesPhone.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbHumanResourcesPhone.Style = textInputErrorStyle;
            }

            return correctFields;
        }

        private bool VerifyWorkCenterAddressTextFields()
        {
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError");
            bool correctFields = true;

            if (TbWorkCenterStreet.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbWorkCenterStreet.Style = textInputErrorStyle;
            }
            if (TbWorkCenterNeighborhood.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbWorkCenterNeighborhood.Style = textInputErrorStyle;
            }
            if (TbWorkCenterInteriorNumber.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbWorkCenterInteriorNumber.Style = textInputErrorStyle;
            }
            if (TbWorkCenterOutdoorNumber.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbWorkCenterOutdoorNumber.Style = textInputErrorStyle;
            }
            if (TbWorkCenterPostCode.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbWorkCenterPostCode.Style = textInputErrorStyle;
            }
            if (TbWorkCenterCity.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbWorkCenterCity.Style = textInputErrorStyle;
            }
            if (TbWorkCenterMunicipality.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbWorkCenterMunicipality.Style = textInputErrorStyle;
            }
            if (TbWorkCenterSatate.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbWorkCenterSatate.Style = textInputErrorStyle;
            }

            return correctFields;
        }

        private bool VerifyClientContactMethodsTextFields()
        {
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError");
            bool correctFields = true;

            if (!VerifyPhoneNumberFormat(TbClientPhoneNumberFirst.Text.Trim()))
            {
                correctFields = false;
                TbClientPhoneNumberFirst.Style = textInputErrorStyle;
            }
            if (!VerifyPhoneNumberFormat(TbClientPhoneNumberSecond.Text.Trim()))
            {
                correctFields = false;
                TbClientPhoneNumberSecond.Style = textInputErrorStyle;
            }
            if (TbClientPhoneNumberThird.Text.Trim().Length > 0 && !VerifyPhoneNumberFormat(TbClientPhoneNumberThird.Text.Trim()))
            {
                correctFields = false;
                TbClientPhoneNumberThird.Style = textInputErrorStyle;
            }
            if (TbClientPhoneNumberFourth.Text.Trim().Length > 0 && !VerifyPhoneNumberFormat(TbClientPhoneNumberFourth.Text.Trim()))
            {
                correctFields = false;
                TbClientPhoneNumberFourth.Style = textInputErrorStyle;
            }
            if (TbClientEmailFirst.Text.Trim().Length == 0 || !VerifyEmailFormat(TbClientEmailFirst.Text.Trim()))
            {
                correctFields = false;
                TbClientEmailFirst.Style = textInputErrorStyle;
            }
            if (TbClientEmailSecond.Text.Trim().Length > 0 && !VerifyEmailFormat(TbClientEmailSecond.Text.Trim()))
            {
                correctFields = false;
                TbClientEmailSecond.Style = textInputErrorStyle;
            }
            if (TbClientEmailThird.Text.Trim().Length > 0 && !VerifyEmailFormat(TbClientEmailThird.Text.Trim()))
            {
                correctFields = false;
                TbClientEmailThird.Style = textInputErrorStyle;
            }


            return correctFields;
        }

        private bool VerifyPhoneNumberFormat(string phoneNumber)
        {
            bool phoneNumberFormat;
            long number;

            if (long.TryParse(phoneNumber, out number) && phoneNumber.Length == 10)
            {
                phoneNumberFormat = true;
            }
            else
            {
                phoneNumberFormat = false;
            }

            return phoneNumberFormat;
        }

        private bool VerifyEmailFormat(string email)
        {
            bool emailFormat;
            try
            {
                MailAddress mailAddress = new MailAddress(email);
                emailFormat = true;
            }
            catch (FormatException)
            {
                emailFormat = false;
            }

            return emailFormat;
        }

        private bool VerifyCarNumberFormat(string cardNumber)
        {
            bool cardNumberIsCorrect;
            long number;
            if (long.TryParse(cardNumber, out number) && cardNumber.Length == 16)
            {
                cardNumberIsCorrect = true;
            }
            else
            {
                cardNumberIsCorrect = false;
            }

            return cardNumberIsCorrect;
        }

        private void ReloadStylesForPersonalInformation()
        {
            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style borderStyle = (Style)this.FindResource("RoundedBorder");

            TbClientName.Style = textInputStyle;
            TbClientSurname.Style = textInputStyle;
            TbClientLastName.Style = textInputStyle;
            BdrClientBirthdate.Style = borderStyle;
            TbClientCurp.Style = textInputStyle;
            TbClientRfc.Style = textInputStyle;
        }

        private void ReloadStylesForClientAddress()
        {
            Style textInputStyle = (Style)this.FindResource("TextInput");

            TbClientStreet.Style = textInputStyle;
            TbClientNeighborhood.Style = textInputStyle;
            TbClientInteriorNumber.Style = textInputStyle;
            TbClientOutdoorNumber.Style = textInputStyle;
            TbClientPostCode.Style = textInputStyle;
            TbClientCity.Style = textInputStyle;
            TbClientMunicipality.Style = textInputStyle;
            TbClientSatate.Style = textInputStyle;
        }

        private void ReloadStylesForBankAccount()
        {
            Style textInputStyle = (Style)this.FindResource("TextInput");

            TbCardNumber.Style = textInputStyle;
            TbHolder.Style = textInputStyle;
            TbBank.Style = textInputStyle;
        }

        private void ReloadStylesForWorkCenter()
        {
            Style textInputStyle = (Style)this.FindResource("TextInput");

            TbCompanyName.Style = textInputStyle;
            TbWorkCenterPhoneNumber.Style = textInputStyle;
            TbEmployeePosition.Style = textInputStyle;
            TbSalary.Style = textInputStyle;
            TbEmployeeSeniority.Style = textInputStyle;
            TbHumanResourcesPhone.Style = textInputStyle;
            TbWorkCenterStreet.Style = textInputStyle;
            TbWorkCenterNeighborhood.Style = textInputStyle;
            TbWorkCenterInteriorNumber.Style = textInputStyle;
            TbWorkCenterOutdoorNumber.Style = textInputStyle;
            TbWorkCenterPostCode.Style = textInputStyle;
            TbWorkCenterCity.Style = textInputStyle;
            TbWorkCenterMunicipality.Style = textInputStyle;
            TbWorkCenterSatate.Style = textInputStyle;
        }

        private void ReloadStylesForClientsContactMethods()
        {
            Style textInputStyle = (Style)this.FindResource("TextInput");

            TbClientPhoneNumberFirst.Style = textInputStyle;
            TbClientPhoneNumberSecond.Style = textInputStyle;
            TbClientPhoneNumberThird.Style = textInputStyle;
            TbClientPhoneNumberFourth.Style = textInputStyle;
            TbClientEmailFirst.Style = textInputStyle;
            TbClientEmailSecond.Style = textInputStyle;
            TbClientEmailThird.Style = textInputStyle;
        }

        private void ReloadStylesForPersonalReferenceFirst()
        {
            Style textInputStyle = (Style)this.FindResource("TextInput");

            TbReferenceNameFirst.Style = textInputStyle;
            TbReferenceSurnameFirst.Style = textInputStyle;
            TbReferenceLastNameFirst.Style = textInputStyle;
            TbReferencePhoneNumberFirst.Style = textInputStyle;
            TbReferenceKinshipFirst.Style = textInputStyle;
            TbReferenceRelationshipFirst.Style = textInputStyle;
            TbReferenceIneKeyFirst.Style = textInputStyle;
            TbReferenceStreetFirst.Style = textInputStyle;
            TbReferenceNeighborhoodFirst.Style = textInputStyle;
            TbReferenceInteriorNumberFirst.Style = textInputStyle;
            TbReferenceOutdoorNumberFirst.Style = textInputStyle;
            TbReferencePostCodeFirst.Style = textInputStyle;
            TbReferenceCityFirst.Style = textInputStyle;
            TbReferenceMunicipalityFirst.Style = textInputStyle;
            TbReferenceSatateFirst.Style = textInputStyle;
        }

        private void ReloadStylesForPersonalReferenceSecond()
        {
            Style textInputStyle = (Style)this.FindResource("TextInput");

            TbReferenceNameSecond.Style = textInputStyle;
            TbReferenceSurnameSecond.Style = textInputStyle;
            TbReferenceLastNameSecond.Style = textInputStyle;
            TbReferencePhoneNumberSecond.Style = textInputStyle;
            TbReferenceKinshipSecond.Style = textInputStyle;
            TbReferenceRelationshipSecond.Style = textInputStyle;
            TbReferenceIneKeySecond.Style = textInputStyle;
            TbReferenceStreetSecond.Style = textInputStyle;
            TbReferenceNeighborhoodSecond.Style = textInputStyle;
            TbReferenceInteriorNumberSecond.Style = textInputStyle;
            TbReferenceOutdoorNumberSecond.Style = textInputStyle;
            TbReferencePostCodeSecond.Style = textInputStyle;
            TbReferenceCitySecond.Style = textInputStyle;
            TbReferenceMunicipalitySecond.Style = textInputStyle;
            TbReferenceSatateSecond.Style = textInputStyle;
        }
        private void BtnCancelClick(object sender, RoutedEventArgs e)
        {
            SearchClientByRFCView searchClientByRFCView = new SearchClientByRFCView();
            this.NavigationService.Navigate(searchClientByRFCView);
        }

        private void BtnRegisterClientClick(object sender, RoutedEventArgs e)
        {
            bool registerClient;

            ReloadStylesForPersonalInformation();
            ReloadStylesForClientAddress();
            ReloadStylesForBankAccount();
            ReloadStylesForWorkCenter();
            ReloadStylesForClientsContactMethods();
            ReloadStylesForPersonalReferenceFirst();
            ReloadStylesForPersonalReferenceSecond();

            if (VerifyTextFields())
            {
                registerClient = RegisterClient();
                if (registerClient)
                {
                    MessageBox.Show("Se registró el cliente " + TbClientName.Text + " " + TbClientSurname.Text + " " + TbClientLastName.Text 
                        + " correctamente", "Actualización exitosa");
                    SearchClientByRFCView searchClientByRFCView = new SearchClientByRFCView();
                    this.NavigationService.Navigate(searchClientByRFCView);
                }
                else
                {
                    MessageBox.Show("No fue posible registrar al cliente " + TbClientName.Text + " " + TbClientSurname.Text + " " + TbClientLastName.Text + ", ya se encuentra registrado", "Error de actualización");
                }
            }
            else
            {
                MessageBox.Show("Verifique que la información ingresada sea correcta", "Campos inválidos");
            }
        }

        private bool RegisterClient()
        {
            bool registerClient = true;
            //TODO
            return registerClient;
        }
    }
}