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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MessageBox = System.Windows.Forms.MessageBox;
using TextBox = System.Windows.Controls.TextBox;

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
                & VerifyClientContactMethodsTextFields()
                & VerifyFirstReferenceInformationTextFields()
                & VerifyFirstReferenceAddressTextFields()
                & VerifySecondReferenceInformationTextFields()
                & VerifySecondReferenceAddressTextFields();
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
            if (!VerifyNumberFormat(TbCardNumber.Text.Trim()) && TbCardNumber.Text.Trim().Length < 16)
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

            if (!VerifyNumberFormat(TbClientPhoneNumberFirst.Text.Trim()) && TbClientPhoneNumberFirst.Text.Trim().Length < 10)
            {
                correctFields = false;
                TbClientPhoneNumberFirst.Style = textInputErrorStyle;
            }
            if (!VerifyNumberFormat(TbClientPhoneNumberSecond.Text.Trim()) && TbClientPhoneNumberSecond.Text.Trim().Length < 10)
            {
                correctFields = false;
                TbClientPhoneNumberSecond.Style = textInputErrorStyle;
            }
            if (TbClientPhoneNumberThird.Text.Trim().Length > 0 
                && !VerifyNumberFormat(TbClientPhoneNumberThird.Text.Trim()) 
                && TbClientPhoneNumberThird.Text.Trim().Length < 10)
            {
                correctFields = false;
                TbClientPhoneNumberThird.Style = textInputErrorStyle;
            }
            if (TbClientPhoneNumberFourth.Text.Trim().Length > 0 
                && !VerifyNumberFormat(TbClientPhoneNumberFourth.Text.Trim()) 
                && TbClientPhoneNumberFourth.Text.Trim().Length < 10)
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

        private bool VerifyFirstReferenceInformationTextFields()
        {
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError");
            bool correctFields = true;

            if (TbReferenceNameFirst.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbReferenceNameFirst.Style = textInputErrorStyle;
            }
            if (TbReferenceSurnameFirst.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbReferenceSurnameFirst.Style = textInputErrorStyle;
            }
            if (TbReferenceLastNameFirst.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbReferenceLastNameFirst.Style = textInputErrorStyle;
            }
            if (!VerifyNumberFormat(TbReferencePhoneNumberFirst.Text.Trim()) && TbReferencePhoneNumberFirst.Text.Trim().Length < 10)
            {
                correctFields = false;
                TbReferencePhoneNumberFirst.Style = textInputErrorStyle;
            }
            if (TbReferenceKinshipFirst.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbReferenceKinshipFirst.Style = textInputErrorStyle;
            }
            if (TbReferenceRelationshipFirst.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbReferenceRelationshipFirst.Style = textInputErrorStyle;
            }
            if (TbReferenceIneKeyFirst.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbReferenceIneKeyFirst.Style = textInputErrorStyle;
            }

            return correctFields;
        }

        private bool VerifyFirstReferenceAddressTextFields()
        {
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError");
            bool correctFields = true;

            if (TbReferenceStreetFirst.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbReferenceStreetFirst.Style = textInputErrorStyle;
            }
            if (TbReferenceNeighborhoodFirst.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbReferenceNeighborhoodFirst.Style = textInputErrorStyle;
            }
            if (TbReferenceInteriorNumberFirst.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbReferenceInteriorNumberFirst.Style = textInputErrorStyle;
            }
            if (TbReferenceOutdoorNumberFirst.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbReferenceOutdoorNumberFirst.Style = textInputErrorStyle;
            }
            if (TbReferencePostCodeFirst.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbReferencePostCodeFirst.Style = textInputErrorStyle;
            }
            if (TbReferenceCityFirst.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbReferenceCityFirst.Style = textInputErrorStyle;
            }
            if (TbReferenceMunicipalityFirst.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbReferenceMunicipalityFirst.Style = textInputErrorStyle;
            }
            if (TbReferenceSatateFirst.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbReferenceSatateFirst.Style = textInputErrorStyle;
            }

            return correctFields;
        }

        private bool VerifySecondReferenceInformationTextFields()
        {
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError");
            bool correctFields = true;

            if (TbReferenceNameSecond.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbReferenceNameSecond.Style = textInputErrorStyle;
            }
            if (TbReferenceSurnameSecond.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbReferenceSurnameSecond.Style = textInputErrorStyle;
            }
            if (TbReferenceLastNameSecond.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbReferenceLastNameSecond.Style = textInputErrorStyle;
            }
            if (!VerifyNumberFormat(TbReferencePhoneNumberSecond.Text.Trim()) && TbReferencePhoneNumberSecond.Text.Trim().Length < 10)
            {
                correctFields = false;
                TbReferencePhoneNumberSecond.Style = textInputErrorStyle;
            }
            if (TbReferenceKinshipSecond.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbReferenceKinshipSecond.Style = textInputErrorStyle;
            }
            if (TbReferenceRelationshipSecond.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbReferenceRelationshipSecond.Style = textInputErrorStyle;
            }
            if (TbReferenceIneKeySecond.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbReferenceIneKeySecond.Style = textInputErrorStyle;
            }

            return correctFields;
        }

        private bool VerifySecondReferenceAddressTextFields()
        {
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError");
            bool correctFields = true;

            if (TbReferenceStreetSecond.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbReferenceStreetSecond.Style = textInputErrorStyle;
            }
            if (TbReferenceNeighborhoodSecond.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbReferenceNeighborhoodSecond.Style = textInputErrorStyle;
            }
            if (TbReferenceInteriorNumberSecond.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbReferenceInteriorNumberSecond.Style = textInputErrorStyle;
            }
            if (TbReferenceOutdoorNumberSecond.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbReferenceOutdoorNumberSecond.Style = textInputErrorStyle;
            }
            if (TbReferencePostCodeSecond.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbReferencePostCodeSecond.Style = textInputErrorStyle;
            }
            if (TbReferenceCitySecond.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbReferenceCitySecond.Style = textInputErrorStyle;
            }
            if (TbReferenceMunicipalitySecond.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbReferenceMunicipalitySecond.Style = textInputErrorStyle;
            }
            if (TbReferenceSatateSecond.Text.Trim().Length == 0)
            {
                correctFields = false;
                TbReferenceSatateSecond.Style = textInputErrorStyle;
            }

            return correctFields;
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

        private bool VerifyNumberFormat(string number)
        {
            if (long.TryParse(number, out long num))
            {
                return num > 0;
            }
            else
            {
                return false;
            }
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
            DialogResult resultado = MessageBox.Show(
                "¿Deseas cancelar el registro del cliente?", 
                "Confirmación de cancelación", 
                MessageBoxButtons.OKCancel);

            if (resultado == DialogResult.OK)
            {
                SearchClientByRFCView searchClientByRFCView = new SearchClientByRFCView();
                this.NavigationService.Navigate(searchClientByRFCView);
            }
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
                DialogResult resultado = MessageBox.Show("¿Deseas registrar al cliente?", 
                    "Confirmación de registro", 
                    MessageBoxButtons.OKCancel);

                if (resultado == DialogResult.OK)
                {
                    registerClient = RegisterClient();
                    if (registerClient)
                    {
                        MessageBox.Show("Se registró el cliente " 
                            + TbClientName.Text + " " 
                            + TbClientSurname.Text + " " 
                            + TbClientLastName.Text + " correctamente", 
                            "Actualización exitosa");
                        SearchClientByRFCView searchClientByRFCView = new SearchClientByRFCView();
                        this.NavigationService.Navigate(searchClientByRFCView);
                    }
                    else
                    {
                        MessageBox.Show("No fue posible registrar al cliente " 
                            + TbClientName.Text + " " 
                            + TbClientSurname.Text + " " 
                            + TbClientLastName.Text + ", ya se encuentra registrado", 
                            "Error de actualización");
                    }
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

        private void ListenAndValidateNumberFields(object sender)
        {
            TextBox tbNumber = sender as TextBox;

            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError");

            string number = tbNumber.Text.Trim();

            if (!VerifyNumberFormat(number))
            {
                tbNumber.Style = textInputErrorStyle;
            }
            else
            {
                tbNumber.Style = textInputStyle;
            }
        }

        private void TbClientCurpTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tbClientCurp = sender as TextBox;

            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError");

            string clientCurp = tbClientCurp.Text.Trim();

            if (clientCurp.Length > 18)
            {
                tbClientCurp.Style = textInputErrorStyle;
            }
            else
            {
                tbClientCurp.Style = textInputStyle;
            }
        }

        private void TbClientRfcTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tbClientRfc = sender as TextBox;

            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError");

            string clientRfc = tbClientRfc.Text.Trim();

            if (clientRfc.Length > 12)
            {
                tbClientRfc.Style = textInputErrorStyle;
            }
            else
            {
                tbClientRfc.Style = textInputStyle;
            }
        }

        private void TbClientInteriorNumberTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndValidateNumberFields(sender);
        }

        private void TbClientOutdoorNumberTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndValidateNumberFields(sender);
        }

        private void TbClientPostCodeTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbCardNumberTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tbCardNumber = sender as TextBox;

            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError");

            string cardNumber = tbCardNumber.Text.Trim();

            if (!VerifyNumberFormat(cardNumber) || cardNumber.Length < 16)
            {
                tbCardNumber.Style = textInputErrorStyle;
            }
            else
            {
                tbCardNumber.Style = textInputStyle;
            }
        }

        private void TbWorkCenterPhoneNumberTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbSalaryTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndValidateNumberFields(sender);
        }

        private void TbEmployeeSeniorityTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndValidateNumberFields(sender);
        }

        private void TbHumanResourcesPhoneTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbWorkCenterInteriorNumberTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndValidateNumberFields(sender);
        }

        private void TbWorkCenterOutdoorNumberTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndValidateNumberFields(sender);
        }

        private void TbWorkCenterPostCodeTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbClientPhoneNumberFirstTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbClientPhoneNumberSecondTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbClientPhoneNumberThirdTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbClientPhoneNumberFourthTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbClientEmailFirstTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbClientEmailSecondTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbClientEmailThirdTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbReferencePhoneNumberFirstTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbReferenceRelationshipFirstTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndValidateNumberFields(sender);
        }

        private void TbReferenceIneKeyFirstTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbReferenceInteriorNumberFirstTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndValidateNumberFields(sender);
        }

        private void TbReferenceOutdoorNumberFirstTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndValidateNumberFields(sender);
        }

        private void TbReferencePostCodeFirstTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbReferencePhoneNumberSecondTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbReferenceRelationshipSecondTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndValidateNumberFields(sender);
        }

        private void TbReferenceIneKeySecondTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TbReferenceInteriorNumberSecondTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndValidateNumberFields(sender);
        }

        private void TbReferenceOutdoorNumberSecondTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndValidateNumberFields(sender);
        }

        private void TbReferencePostCodeSecondTextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}