
using SFIClient.SFIServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.ServiceModel;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using MessageBox = System.Windows.Forms.MessageBox;
using Page = System.Windows.Controls.Page;
using TextBox = System.Windows.Controls.TextBox;

namespace SFIClient.Views
{
    /// <summary>
    /// Lógica de interacción para ClientRegisterView.xaml
    /// </summary>
    public partial class ClientRegisterController : Page
    {
        private readonly ClientsServiceClient ClientsServiceClient = new ClientsServiceClient();
        private bool textFieldsSame = false;
        private string lastSalary = string.Empty;
        public ClientRegisterController()
        {
            InitializeComponent();
            CollapaseTextFields();
            ApplyRestrictionsOnFields();
        }

        private void CollapaseTextFields()
        {
            TbkClientPhoneNumberThird.Visibility = Visibility.Collapsed;
            GrdClientPhoneNumberThird.Visibility = Visibility.Collapsed;
            TbkClientPhoneNumberFourth.Visibility = Visibility.Collapsed;
            GrdClientPhoneNumberFourth.Visibility = Visibility.Collapsed;
            TbkClientEmailSecond.Visibility = Visibility.Collapsed;
            GrdClientEmailSecond.Visibility = Visibility.Collapsed;
            TbkClientEmailThird.Visibility = Visibility.Collapsed;
            GrdClientEmailThird.Visibility = Visibility.Collapsed;
        }

        private void ApplyRestrictionsOnFields()
        {
            RestrictOnlyNumbers(TbClientInteriorNumber);
            RestrictOnlyNumbers(TbClientOutdoorNumber);
            RestrictOnlyNumbers(TbClientPostCode);
            RestrictOnlyNumbers(TbCardNumber);
            RestrictOnlyNumbers(TbWorkCenterPhoneNumber);
            RestrictOnlyNumbers(TbHumanResourcesPhone);
            //RestrictOnlyNumbers(TbSalary);
            RestrictOnlyNumbers(TbEmployeeSeniority);
            RestrictOnlyNumbers(TbWorkCenterInteriorNumber);
            RestrictOnlyNumbers(TbWorkCenterOutdoorNumber);
            RestrictOnlyNumbers(TbWorkCenterPostCode);
            RestrictOnlyNumbers(TbClientPhoneNumberFirst);
            RestrictOnlyNumbers(TbClientPhoneNumberSecond);
            RestrictOnlyNumbers(TbClientPhoneNumberThird);
            RestrictOnlyNumbers(TbClientPhoneNumberFourth);
            RestrictOnlyNumbers(TbReferencePhoneNumberFirst);
            RestrictOnlyNumbers(TbReferenceInteriorNumberFirst);
            RestrictOnlyNumbers(TbReferenceOutdoorNumberFirst);
            RestrictOnlyNumbers(TbReferencePostCodeFirst);
            RestrictOnlyNumbers(TbReferencePhoneNumberSecond);
            RestrictOnlyNumbers(TbReferenceInteriorNumberSecond);
            RestrictOnlyNumbers(TbReferenceOutdoorNumberSecond);
            RestrictOnlyNumbers(TbReferencePostCodeSecond);
            RestrictOnlyLetters(TbClientName);
            RestrictOnlyLetters(TbClientSurname);
            RestrictOnlyLetters(TbClientLastName);
            RestrictOnlyLetters(TbClientName);
            RestrictOnlyLetters(TbClientCity);
            RestrictOnlyLetters(TbClientMunicipality);
            RestrictOnlyLetters(TbClientState);
            RestrictOnlyLetters(TbHolder);
            RestrictOnlyLetters(TbWorkCenterCity);
            RestrictOnlyLetters(TbWorkCenterMunicipality);
            RestrictOnlyLetters(TbWorkCenterState);
            RestrictOnlyLetters(TbReferenceNameFirst);
            RestrictOnlyLetters(TbReferenceSurnameFirst);
            RestrictOnlyLetters(TbReferenceLastNameFirst);
            RestrictOnlyLetters(TbReferenceCityFirst);
            RestrictOnlyLetters(TbReferenceMunicipalityFirst);
            RestrictOnlyLetters(TbReferenceStateFirst);
            RestrictOnlyLetters(TbReferenceNameSecond);
            RestrictOnlyLetters(TbReferenceSurnameSecond);
            RestrictOnlyLetters(TbReferenceLastNameSecond);
            RestrictOnlyLetters(TbReferenceCitySecond);
            RestrictOnlyLetters(TbReferenceMunicipalitySecond);
            RestrictOnlyLetters(TbReferenceStateSecond);
        }

        private void BtnNewPhoneNumberClick(object sender, RoutedEventArgs e)
        {
            if (TbkClientPhoneNumberThird.Visibility == Visibility.Collapsed)
            {
                TbkClientPhoneNumberThird.Visibility = Visibility.Visible;
                GrdClientPhoneNumberThird.Visibility = Visibility.Visible;
                return;
            }
            if (TbkClientPhoneNumberFourth.Visibility == Visibility.Collapsed
                && TbkClientPhoneNumberThird.Visibility == Visibility.Visible)
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
            if (TbkClientEmailThird.Visibility == Visibility.Collapsed
                && TbkClientEmailSecond.Visibility == Visibility.Visible)
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
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");
            Style borderErrorStyle = (Style)this.FindResource("RoundedBorderError");

            bool correctFields = true;

            List<TextBox> textBoxes = new List<TextBox>
            {
                TbClientName,
                //TbClientSurname,
                TbClientLastName,
                TbClientCurp,
                TbClientRfc
            };

            for (int i = 0; i < textBoxes.Count; i++)
            {
                if (textBoxes[i].Text.Trim().Length == 0)
                {
                    textBoxes[i].Style = textInputErrorStyle;
                    correctFields = false;
                }
            }
            if (DpkClientBirthdate.SelectedDate.ToString().Length == 0)
            {
                correctFields = false;
                BdrClientBirthdate.Style = borderErrorStyle;
            }

            return correctFields;
        }

        private bool VerifyClientAddressTextFields()
        {
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");
            bool correctFields = true;

            List<TextBox> textBoxes = new List<TextBox>
            {
                TbClientStreet,
                TbClientNeighborhood,
                TbClientInteriorNumber,
                TbClientOutdoorNumber,
                TbClientPostCode,
                TbClientCity,
                TbClientMunicipality,
                TbClientState
            };

            for (int i = 0; i < textBoxes.Count; i++)
            {
                if (textBoxes[i].Text.Trim().Length == 0)
                {
                    textBoxes[i].Style = textInputErrorStyle;
                    correctFields = false;
                }
            }

            return correctFields;
        }

        private bool VerifyBankAccountTextFields()
        {
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");
            bool correctFields = true;

            List<TextBox> textBoxes = new List<TextBox>
            {
                TbCardNumber,
                TbBank,
                TbHolder
            };

            for (int i = 0; i < textBoxes.Count; i++)
            {
                if (textBoxes[i].Text.Trim().Length == 0)
                {
                    textBoxes[i].Style = textInputErrorStyle;
                    correctFields = false;
                }
            }

            return correctFields;
        }

        private bool VerifyWorkCenterTextFields()
        {
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");
            bool correctFields = true;

            List<TextBox> textBoxes = new List<TextBox>
            {
                TbCompanyName,
                TbWorkCenterPhoneNumber,
                TbEmployeePosition,
                TbSalary,
                TbEmployeeSeniority,
                TbHumanResourcesPhone
            };

            for (int i = 0; i < textBoxes.Count; i++)
            {
                if (textBoxes[i].Text.Trim().Length == 0)
                {
                    textBoxes[i].Style = textInputErrorStyle;
                    correctFields = false;
                }
            }

            return correctFields;
        }

        private bool VerifyWorkCenterAddressTextFields()
        {
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");
            bool correctFields = true;

            List<TextBox> textBoxes = new List<TextBox>
            {
                TbWorkCenterStreet,
                TbWorkCenterNeighborhood,
                TbWorkCenterInteriorNumber,
                TbWorkCenterOutdoorNumber,
                TbWorkCenterPostCode,
                TbWorkCenterCity,
                TbWorkCenterMunicipality,
                TbWorkCenterState
            };

            for (int i = 0; i < textBoxes.Count; i++)
            {
                if (textBoxes[i].Text.Trim().Length == 0)
                {
                    textBoxes[i].Style = textInputErrorStyle;
                    correctFields = false;
                }
            }

            return correctFields;
        }

        private bool VerifyClientContactMethodsTextFields()
        {
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");
            bool correctFields = true;

            List<TextBox> textBoxes = new List<TextBox>
            {
                TbClientPhoneNumberFirst,
                TbClientPhoneNumberSecond,
                TbClientEmailFirst
            };

            for (int i = 0; i < textBoxes.Count; i++)
            {
                if (textBoxes[i].Text.Trim().Length == 0)
                {
                    textBoxes[i].Style = textInputErrorStyle;
                    correctFields = false;
                }
            }

            return correctFields;
        }

        private bool VerifyFirstReferenceInformationTextFields()
        {
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");
            bool correctFields = true;

            List<TextBox> textBoxes = new List<TextBox>
            {
                TbReferenceNameFirst,
                //TbReferenceSurnameFirst,
                TbReferenceLastNameFirst,
                TbReferencePhoneNumberFirst,
                TbReferenceKinshipFirst,
                TbReferenceRelationshipFirst,
                TbReferenceIneKeyFirst
            };

            for (int i = 0; i < textBoxes.Count; i++)
            {
                if (textBoxes[i].Text.Trim().Length == 0)
                {
                    textBoxes[i].Style = textInputErrorStyle;
                    correctFields = false;
                }
            }

            return correctFields;
        }

        private bool VerifyFirstReferenceAddressTextFields()
        {
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");
            bool correctFields = true;

            List<TextBox> textBoxes = new List<TextBox>
            {
                TbReferenceStreetFirst,
                TbReferenceNeighborhoodFirst,
                TbReferenceInteriorNumberFirst,
                TbReferenceOutdoorNumberFirst,
                TbReferencePostCodeFirst,
                TbReferenceCityFirst,
                TbReferenceMunicipalityFirst,
                TbReferenceStateFirst
            };

            for (int i = 0; i < textBoxes.Count; i++)
            {
                if (textBoxes[i].Text.Trim().Length == 0)
                {
                    textBoxes[i].Style = textInputErrorStyle;
                    correctFields = false;
                }
            }

            return correctFields;
        }

        private bool VerifySecondReferenceInformationTextFields()
        {
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");
            bool correctFields = true;

            List<TextBox> textBoxes = new List<TextBox>
            {
                TbReferenceNameSecond,
                //TbReferenceSurnameSecond,
                TbReferenceLastNameSecond,
                TbReferencePhoneNumberSecond,
                TbReferenceKinshipSecond,
                TbReferenceRelationshipSecond,
                TbReferenceIneKeySecond
            };

            for (int i = 0; i < textBoxes.Count; i++)
            {
                if (textBoxes[i].Text.Trim().Length == 0)
                {
                    textBoxes[i].Style = textInputErrorStyle;
                    correctFields = false;
                }
            }

            return correctFields;
        }

        private bool VerifySecondReferenceAddressTextFields()
        {
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");
            bool correctFields = true;

            List<TextBox> textBoxes = new List<TextBox>
            {
                TbReferenceStreetSecond,
                TbReferenceNeighborhoodSecond,
                TbReferenceInteriorNumberSecond,
                TbReferenceOutdoorNumberSecond,
                TbReferencePostCodeSecond,
                TbReferenceCitySecond,
                TbReferenceMunicipalitySecond,
                TbReferenceStateSecond
            };

            for (int i = 0; i < textBoxes.Count; i++)
            {
                if (textBoxes[i].Text.Trim().Length == 0)
                {
                    textBoxes[i].Style = textInputErrorStyle;
                    correctFields = false;
                }
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

        private void BtnGoBackClick(object sender, RoutedEventArgs e)
        {
            ShowCancelUpdateBankAccountConfirmationMessage();
        }

        private void BtnCancelClick(object sender, RoutedEventArgs e)
        {
            ShowCancelUpdateBankAccountConfirmationMessage();
        }

        private void ShowCancelUpdateBankAccountConfirmationMessage()
        {
            DialogResult resultado = MessageBox.Show(
                "¿Deseas cancelar el registro del cliente?",
                "Confirmación de cancelación",
                MessageBoxButtons.OKCancel);

            if (resultado == DialogResult.OK)
            {
                SearchClientByRFCController searchClientByRFCView = new SearchClientByRFCController();
                this.NavigationService.Navigate(searchClientByRFCView);
            }
        }

        private void BtnRegisterClientClick(object sender, RoutedEventArgs e)
        {
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");
            bool registerClient;

            if (VerifyTextFields() && !textFieldsSame)
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
                        SearchClientByRFCController searchClientByRFCView = new SearchClientByRFCController();
                        this.NavigationService.Navigate(searchClientByRFCView);
                    }
                    else
                    {
                        MessageBox.Show("No fue posible registrar al cliente "
                            + TbClientName.Text + " "
                            + TbClientSurname.Text + " "
                            + TbClientLastName.Text + ", alguna información ya se encuentra registrada, " +
                            "rectifique los campos marcados posibles a duplicación de información",
                            "Error de registro");
                        TbClientRfc.Style = textInputErrorStyle;
                        TbClientCurp.Style = textInputErrorStyle;
                        TbCardNumber.Style = textInputErrorStyle;
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
            bool registerClient = false;
            Address addressWorkCenter = new Address
            {
                Street = TbWorkCenterStreet.Text.Trim(),
                City = TbWorkCenterCity.Text.Trim(),
                Neighborhod = TbWorkCenterNeighborhood.Text.Trim(),
                Municipality = TbWorkCenterMunicipality.Text.Trim(),
                InteriorNumber = TbWorkCenterInteriorNumber.Text.Trim(),
                OutdoorNumber = TbWorkCenterOutdoorNumber.Text.Trim(),
                PostCode = TbWorkCenterPostCode.Text.Trim(),
                State = TbWorkCenterState.Text.Trim()
            };
            Address addresspersonalReferenceFirst = new Address
            {
                Street = TbReferenceStreetFirst.Text.Trim(),
                City = TbReferenceCityFirst.Text.Trim(),
                Neighborhod = TbReferenceNeighborhoodFirst.Text.Trim(),
                Municipality = TbReferenceMunicipalityFirst.Text.Trim(),
                InteriorNumber = TbReferenceInteriorNumberFirst.Text.Trim(),
                OutdoorNumber = TbReferenceOutdoorNumberFirst.Text.Trim(),
                PostCode = TbReferencePostCodeFirst.Text.Trim(),
                State = TbReferenceStateFirst.Text.Trim()
            };
            PersonalReference personalReferenceClientFirst = new PersonalReference
            {
                Name = TbReferenceNameFirst.Text.Trim(),
                Surname = TbReferenceSurnameFirst.Text.Trim(),
                LastName = TbReferenceLastNameFirst.Text.Trim(),
                PhoneNumber = TbReferencePhoneNumberFirst.Text.Trim(),
                Kinship = TbReferenceKinshipFirst.Text.Trim(),
                RelationshipYears = TbReferenceRelationshipFirst.Text.Trim(),
                IneKey = TbReferenceIneKeyFirst.Text.Trim(),
                Address = addresspersonalReferenceFirst
            };
            Address addresspersonalReferenceSecond = new Address
            {
                Street = TbReferenceStreetSecond.Text.Trim(),
                City = TbReferenceCitySecond.Text.Trim(),
                Neighborhod = TbReferenceNeighborhoodSecond.Text.Trim(),
                Municipality = TbReferenceMunicipalitySecond.Text.Trim(),
                InteriorNumber = TbReferenceInteriorNumberSecond.Text.Trim(),
                OutdoorNumber = TbReferenceOutdoorNumberSecond.Text.Trim(),
                PostCode = TbReferencePostCodeSecond.Text.Trim(),
                State = TbReferenceStateSecond.Text.Trim()
            };
            PersonalReference personalReferenceClientSecond = new PersonalReference
            {
                Name = TbReferenceNameSecond.Text.Trim(),
                Surname = TbReferenceSurnameSecond.Text.Trim(),
                LastName = TbReferenceLastNameSecond.Text.Trim(),
                PhoneNumber = TbReferencePhoneNumberSecond.Text.Trim(),
                Kinship = TbReferenceKinshipSecond.Text.Trim(),
                RelationshipYears = TbReferenceRelationshipSecond.Text.Trim(),
                IneKey = TbReferenceIneKeySecond.Text.Trim(),
                Address = addresspersonalReferenceSecond
            };
            BankAccount bankAccount = new BankAccount
            {
                CardNumber = TbCardNumber.Text.Trim(),
                Bank = TbBank.Text.Trim(),
                Holder = TbHolder.Text.Trim()
            };

            Address addressClient = new Address
            {
                Street = TbClientStreet.Text.Trim(),
                City = TbClientCity.Text.Trim(),
                Neighborhod = TbClientNeighborhood.Text.Trim(),
                Municipality = TbClientMunicipality.Text.Trim(),
                InteriorNumber = TbClientInteriorNumber.Text.Trim(),
                OutdoorNumber = TbClientOutdoorNumber.Text.Trim(),
                PostCode = TbClientPostCode.Text.Trim(),
                State = TbClientState.Text.Trim()
            };

            WorkCenter workCenter = new WorkCenter
            {
                CompanyName = TbCompanyName.Text.Trim(),
                PhoneNumber = TbWorkCenterPhoneNumber.Text.Trim(),
                EmployeePosition = TbEmployeePosition.Text.Trim(),
                Salary = decimal.Parse(TbSalary.Text.Trim()),
                EmployeeSeniority = TbEmployeeSeniority.Text.Trim(),
                HumanResourcesPhone = TbHumanResourcesPhone.Text.Trim(),
                Address = addressWorkCenter
            };

            List<ContacMethod> contacMethods = new List<ContacMethod>
            {
                new ContacMethod
                {
                    Value = TbClientPhoneNumberFirst.Text.Trim(),
                    MethodType = "PhoneNumber"
                },
                new ContacMethod
                {
                    Value = TbClientPhoneNumberSecond.Text.Trim(),
                    MethodType = "PhoneNumber"
                },
                new ContacMethod
                {
                    Value = TbClientPhoneNumberThird.Text.Trim(),
                    MethodType = "PhoneNumber"
                },
                new ContacMethod
                {
                    Value = TbClientPhoneNumberFourth.Text.Trim(),
                    MethodType = "PhoneNumber"
                },
                new ContacMethod
                {
                    Value = TbClientEmailFirst.Text.Trim(),
                    MethodType = "Email"
                },
                new ContacMethod
                {
                    Value = TbClientEmailSecond.Text.Trim(),
                    MethodType = "Email"
                },
                new ContacMethod
                {
                    Value = TbClientEmailThird.Text.Trim(),
                    MethodType = "Email"
                }
            };

            List<PersonalReference> personalReferences = new List<PersonalReference>
            {
                personalReferenceClientFirst,
                personalReferenceClientSecond
            };

            Client client = new Client
            {
                Rfc = TbClientRfc.Text.Trim(),
                Curp = TbClientCurp.Text.Trim(),
                Birthdate = DpkClientBirthdate.SelectedDate.Value,
                Name = TbClientName.Text.Trim(),
                LastName = TbClientLastName.Text.Trim(),
                Surname = TbClientSurname.Text.Trim(),
                BankAccount = bankAccount,
                Address = addressClient,
                WorkCenter = workCenter,
                ContacMethods = contacMethods.ToArray(),
                PersonalReferences = personalReferences.ToArray()
            };

            try
            {
                registerClient = ClientsServiceClient.RegisterClient(client);

            }
            catch (FaultException<ServiceFault> fe)
            {
                MessageBox.Show(fe.Message, "Error en la base de datos");
            }
            catch (EndpointNotFoundException)
            {
                MessageBox.Show("No fue posible establecer la conexión con el servicio, intente más tarde", "Error en el servicio");
                //TODO Redirect To Main Menu
            }
            catch (CommunicationException)
            {
                MessageBox.Show("No fue posible establecer la conexión con el servicio, intente más tarde", "Error en el servicio");
                //TODO Redirect To Main Menu
            }

            return registerClient;
        }

        private void VerifyPhoneNumbersAreNotSame()
        {
            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");
            textFieldsSame = false;

            TbClientPhoneNumberFirst.Style = textInputStyle;
            TbClientPhoneNumberSecond.Style = textInputStyle;
            TbClientPhoneNumberThird.Style = textInputStyle;
            TbClientPhoneNumberFourth.Style = textInputStyle;
            TbWorkCenterPhoneNumber.Style = textInputStyle;
            TbHumanResourcesPhone.Style = textInputStyle;
            TbReferencePhoneNumberFirst.Style = textInputStyle;
            TbReferencePhoneNumberSecond.Style = textInputStyle;

            List<TextBox> textBoxes = new List<TextBox>
            {
                TbClientPhoneNumberFirst,
                TbClientPhoneNumberSecond,
                TbClientPhoneNumberThird,
                TbClientPhoneNumberFourth,
                TbWorkCenterPhoneNumber,
                TbHumanResourcesPhone,
                TbReferencePhoneNumberFirst,
                TbReferencePhoneNumberSecond
            };

            for (int i = 0; i < textBoxes.Count; i++)
            {
                for (int j = i + 1; j < textBoxes.Count; j++)
                {
                    if ((textBoxes[i].Text.Trim().Length > 0 && textBoxes[j].Text.Trim().Length > 0)
                        && (textBoxes[i].Text.Trim() == textBoxes[j].Text.Trim()))
                    {
                        textBoxes[i].Style = textInputErrorStyle;
                        textBoxes[j].Style = textInputErrorStyle;
                        textFieldsSame = true;
                    }
                }
            }
        }

        private void VerifyEmailAreNotSame()
        {
            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");
            textFieldsSame = false;

            TbClientEmailFirst.Style = textInputStyle;
            TbClientEmailSecond.Style = textInputStyle;
            TbClientEmailThird.Style = textInputStyle;

            List<TextBox> textBoxes = new List<TextBox>
            {
                TbClientEmailFirst,
                TbClientEmailSecond,
                TbClientEmailThird
            };

            for (int i = 0; i < textBoxes.Count; i++)
            {
                for (int j = i + 1; j < textBoxes.Count; j++)
                {
                    if ((textBoxes[i].Text.Trim().Length > 0 && textBoxes[j].Text.Trim().Length > 0)
                        && (textBoxes[i].Text.Trim() == textBoxes[j].Text.Trim()))
                    {
                        textBoxes[i].Style = textInputErrorStyle;
                        textBoxes[j].Style = textInputErrorStyle;
                        textFieldsSame = true;
                    }
                }
            }
        }

        private void VerifyIneKeyAreNotSame()
        {
            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");

            TbReferenceIneKeyFirst.Style = textInputStyle;
            TbReferenceIneKeySecond.Style = textInputStyle;

            if ((TbReferenceIneKeyFirst.Text.Trim().Length > 0 && TbReferenceIneKeySecond.Text.Trim().Length > 0) &&
                (TbReferenceIneKeyFirst.Text.Trim() == TbReferenceIneKeySecond.Text.Trim()))
            {
                TbReferenceIneKeyFirst.Style = textInputErrorStyle;
                TbReferenceIneKeySecond.Style = textInputErrorStyle;
            }
        }

        private void ListenAndVerifyPostCodeields(object sender)
        {
            TextBox tbPostCode = sender as TextBox;

            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");

            string postCode = tbPostCode.Text.Trim();

            if (postCode.Length < 5)
            {
                tbPostCode.Style = textInputErrorStyle;
            }
            else
            {
                tbPostCode.Style = textInputStyle;
            }
        }

        private void ListenAndVerifyCurpAndIneKeyFields(object sender)
        {
            TextBox tbKey = sender as TextBox;

            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");

            string key = tbKey.Text.Trim();

            if (key.Length < 18)
            {
                tbKey.Style = textInputErrorStyle;
            }
            else
            {
                tbKey.Style = textInputStyle;
            }
        }

        private void ListenAndVerifyEmptyTextFields(object sender)
        {
            TextBox tbKey = sender as TextBox;

            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");

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

        private void TbClientNameTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbClientSurnameTextChanged(object sender, TextChangedEventArgs e)
        {
            //ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbClientLastNameTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void DpkSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            Style roundedBorderError = (Style)this.FindResource("RoundedBorderError");
            Style roundedBorder = (Style)this.FindResource("RoundedBorder");

            if (sender is DatePicker datePicker)
            {
                DateTime selectedDate = datePicker.SelectedDate ?? DateTime.MinValue;
                if (selectedDate.ToShortDateString().Length == 0)
                {
                    BdrClientBirthdate.Style = roundedBorderError;
                }
                else
                {
                    BdrClientBirthdate.Style = roundedBorder;
                }
            }
        }

        private void TbClientCurpTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyCurpAndIneKeyFields(sender);
        }

        private void TbClientRfcTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tbClientRfc = sender as TextBox;

            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");

            string clientRfc = tbClientRfc.Text.Trim();

            if (clientRfc.Length < 12)
            {
                tbClientRfc.Style = textInputErrorStyle;
            }
            else
            {
                tbClientRfc.Style = textInputStyle;
            }
        }

        private void TbClientStreetTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbClientNeighborhoodTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbClientInteriorNumberTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbClientOutdoorNumberTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbClientPostCodeTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyPostCodeields(sender);
        }

        private void TbClientCityTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbClientMunicipalityTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbClientSatateTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbCardNumberTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tbCardNumber = sender as TextBox;

            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");

            string cardNumber = tbCardNumber.Text.Trim();

            if (cardNumber.Length < 16)
            {
                tbCardNumber.Style = textInputErrorStyle;
            }
            else
            {
                tbCardNumber.Style = textInputStyle;
            }
        }

        private void TbBankTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbHolderTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbCompanyNameTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbWorkCenterPhoneNumberTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");
            textBox.Style = textInputStyle;
            ListenAndVerifyEmptyTextFields(sender);
            if (textBox.Text.Trim().Length >= 10)
            {
                VerifyPhoneNumbersAreNotSame();
            }
            else
            {
                textBox.Style = textInputErrorStyle;
            }
        }

        private void TbEmployeePositionTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbSalaryTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
            //TODO
            //string newSalary = TbSalary.Text.Trim();
            //bool isWritingNewSalary = newSalary != lastSalary;

            //if (isWritingNewSalary)
            //{
            //    if (newSalary != string.Empty && /*!DataValidator.IsValidMoneyAmmount(newSalary)*/)
            //    {
            //        TbSalary.Text = lastSalary;
            //        TbSalary.CaretIndex = TbSalary.Text.Trim().Length;
            //    }
            //    else
            //    {
            //        lastSalary = newSalary;
            //    }
            //}
        }

        private void TbEmployeeSeniorityTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbHumanResourcesPhoneTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");
            textBox.Style = textInputStyle;
            ListenAndVerifyEmptyTextFields(sender);
            if (textBox.Text.Trim().Length >= 10)
            {
                VerifyPhoneNumbersAreNotSame();
            }
            else
            {
                textBox.Style = textInputErrorStyle;
            }
        }

        private void TbWorkCenterInteriorNumberTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbWorkCenterOutdoorNumberTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbWorkCenterStreetTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbWorkCenterNeighborhoodTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbWorkCenterPostCodeTextChanged(object sender, TextChangedEventArgs e)
        {

            ListenAndVerifyPostCodeields(sender);
        }

        private void TbWorkCenterCityTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbWorkCenterMunicipalityTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbWorkCenterSatateTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbClientPhoneNumberFirstTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");
            textBox.Style = textInputStyle;
            ListenAndVerifyEmptyTextFields(sender);
            if (textBox.Text.Trim().Length == 10)
            {
                VerifyPhoneNumbersAreNotSame();
            }
            else
            {
                textBox.Style = textInputErrorStyle;
            }
        }

        private void TbClientPhoneNumberSecondTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");
            textBox.Style = textInputStyle;
            ListenAndVerifyEmptyTextFields(sender);
            if (textBox.Text.Trim().Length == 10)
            {
                VerifyPhoneNumbersAreNotSame();
            }
            else
            {
                textBox.Style = textInputErrorStyle;
            }
        }

        private void TbClientPhoneNumberThirdTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");
            TbClientPhoneNumberThird.Style = textInputStyle;
            if (TbClientPhoneNumberThird.Text.Trim().Length > 0)
            {
                if (textBox.Text.Trim().Length == 10)
                {
                    VerifyPhoneNumbersAreNotSame();
                }
                else
                {
                    textBox.Style = textInputErrorStyle;
                }
            }
            else
            {
                textBox.Style = textInputStyle;
            }
        }

        private void TbClientPhoneNumberFourthTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");
            TbClientPhoneNumberThird.Style = textInputStyle;
            if (TbClientPhoneNumberThird.Text.Trim().Length > 0)
            {
                if (textBox.Text.Trim().Length == 10)
                {
                    VerifyPhoneNumbersAreNotSame();
                }
                else
                {
                    textBox.Style = textInputErrorStyle;
                }
            }
            else
            {
                textBox.Style = textInputStyle;
            }
        }

        private void TbClientEmailFirstTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");
            textBox.Style = textInputStyle;
            ListenAndVerifyEmptyTextFields(sender);
            if (textBox.Text.Trim().Length > 0)
            {
                if (VerifyEmailFormat(textBox.Text.Trim()))
                {
                    VerifyEmailAreNotSame();
                }
                else
                {
                    textBox.Style = textInputErrorStyle;
                }
            }
        }

        private void TbClientEmailSecondTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");
            textBox.Style = textInputStyle;
            if (TbClientEmailSecond.Text.Trim().Length > 0)
            {
                if (VerifyEmailFormat(textBox.Text.Trim()))
                {
                    VerifyEmailAreNotSame();
                }
                else
                {
                    textBox.Style = textInputErrorStyle;
                }
            }
            else
            {
                textBox.Style = textInputStyle;
            }
        }

        private void TbClientEmailThirdTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");
            textBox.Style = textInputStyle;
            if (TbClientEmailSecond.Text.Trim().Length > 0)
            {
                if (VerifyEmailFormat(textBox.Text.Trim()))
                {
                    VerifyEmailAreNotSame();
                }
                else
                {
                    textBox.Style = textInputErrorStyle;
                }
            }
            else
            {
                textBox.Style = textInputStyle;
            }
        }

        private void TbReferenceNameFirstTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbReferenceSurnameFirstTextChanged(object sender, TextChangedEventArgs e)
        {
            //ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbReferenceLastNameFirstTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbReferencePhoneNumberFirstTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");
            textBox.Style = textInputStyle;
            ListenAndVerifyEmptyTextFields(sender);
            if (textBox.Text.Trim().Length == 10)
            {
                VerifyPhoneNumbersAreNotSame();
            }
            else
            {
                textBox.Style = textInputErrorStyle;
            }
        }

        private void TbReferenceKinshipFirstTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbReferenceRelationshipFirstTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbReferenceIneKeyFirstTextChanged(object sender, TextChangedEventArgs e)
        {
            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");

            if (TbReferenceIneKeyFirst.Text.Trim().Length < 18)
            {
                TbReferenceIneKeyFirst.Style = textInputErrorStyle;
            }
            else
            {
                TbReferenceIneKeyFirst.Style = textInputStyle;
                VerifyIneKeyAreNotSame();
            }
        }

        private void TbReferenceStreetFirstTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbReferenceNeighborhoodFirstTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbReferenceInteriorNumberFirstTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbReferenceOutdoorNumberFirstTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbReferencePostCodeFirstTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyPostCodeields(sender);
        }

        private void TbReferenceCityFirstTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbReferenceMunicipalityFirstTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbReferenceSatateFirstTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbReferenceNameSecondTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbReferenceSurnameSecondTextChanged(object sender, TextChangedEventArgs e)
        {
            //ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbReferenceLastNameSecondTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbReferencePhoneNumberSecondTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");
            textBox.Style = textInputStyle;
            ListenAndVerifyEmptyTextFields(sender);
            if (textBox.Text.Trim().Length == 10)
            {
                VerifyPhoneNumbersAreNotSame();
            }
            else
            {
                textBox.Style = textInputErrorStyle;
            }
        }

        private void TbReferenceKinshipSecondTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbReferenceRelationshipSecondTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbReferenceIneKeySecondTextChanged(object sender, TextChangedEventArgs e)
        {
            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("TextInputError2");

            if (TbReferenceIneKeySecond.Text.Trim().Length < 18)
            {
                TbReferenceIneKeySecond.Style = textInputErrorStyle;
            }
            else
            {
                TbReferenceIneKeySecond.Style = textInputStyle;
                VerifyIneKeyAreNotSame();
            }
        }

        private void TbReferenceStreetSecondTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbReferenceNeighborhoodSecondTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbReferenceInteriorNumberSecondTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbReferenceOutdoorNumberSecondTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbReferencePostCodeSecondTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyPostCodeields(sender);
        }

        private void TbReferenceCitySecondTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbReferenceMunicipalitySecondTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }

        private void TbReferenceSatateSecondTextChanged(object sender, TextChangedEventArgs e)
        {
            ListenAndVerifyEmptyTextFields(sender);
        }
    }
}
