using Microsoft.Win32;
using SFIClient.Controlls;
using SFIClient.SFIServices;
using SFIClient.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Lógica de interacción para CreditAuthorizationDictumView.xaml
    /// </summary>
    public partial class CreditAuthorizationDictumController : Page
    {
        private readonly CreditGrantingPoliciesClient creditGrantingPoliciesClient = new CreditGrantingPoliciesClient();
        private readonly CreditsServiceClient credititsServiceClient = new CreditsServiceClient();
        private CreditGrantingPolicy[] creditGrantingPolicesList;
        private CreditApplication creditApplication;
        private bool dictumIsApproved;
        private readonly List<CreditGrantingPolicy> creditGrantingPolicesListThatApply = new List<CreditGrantingPolicy>();
        private readonly string creditInvoice;
        private string lastAmountApproved = string.Empty;
        public CreditAuthorizationDictumController(string invoice)
        {
            InitializeComponent();
            creditInvoice = invoice;
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            LoadCreditGrantingPolices();
        }

        private void LoadCreditGrantingPolices()
        {
            try
            {
                creditGrantingPolicesList = creditGrantingPoliciesClient.RecoverActivesCreditGrantingPolicies();
                if (creditGrantingPolicesList.Length != 0)
                {
                    LoadCreditApplicationContent();
                }
                else
                {
                    ShowNoPolicesExistsAlertDialog();
                    RedirectToCreditApplicationsListView();
                }
            }
            catch (FaultException<ServiceFault> fe)
            {
                ShowErrorRecoveringCreditGrantingPolices(fe.Message);
            }
            catch (EndpointNotFoundException)
            {
                string errorMessage = "Por el momento el servidor no se encuentra disponible, intente más tarde";
                ShowErrorRecoveringCreditGrantingPolices(errorMessage);
                RedirectToCreditApplicationsListView();
            }
            catch (CommunicationException)
            {
                string errorMessage = "Por el momento el servidor no se encuentra disponible, intente más tarde";
                ShowErrorRecoveringCreditGrantingPolices(errorMessage);
                RedirectToCreditApplicationsListView();
            }
        }

        private void ShowNoPolicesExistsAlertDialog()
        {
            MessageBox.Show(
                "Por el momento no se puede generar ningún dictamen",
                "No existen políticas registradas",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }

        private void ShowErrorRecoveringCreditGrantingPolices(string message)
        {

        }

        private void RedirectToCreditApplicationsListView()
        {
            NavigationService.Navigate(new CreditApplicationsListController());
        }

        private void LoadCreditApplicationContent()
        {
            try
            {
                creditApplication = credititsServiceClient.RecoverCreditApplication(creditInvoice);
                if (creditApplication != null)
                {
                    ShowCreditApplicationContent(creditApplication);
                }
            }
            catch (FaultException<ServiceFault> fe)
            {
                ShowErrorRecoveringCreditApplicationContent(fe.Message);
            }
            catch (EndpointNotFoundException)
            {
                string errorMessage = "Por el momento el servidor no se encuentra disponible, intente más tarde";
                ShowErrorRecoveringCreditApplicationContent(errorMessage);
                RedirectToCreditApplicationsListView();
            }
            catch (CommunicationException)
            {
                string errorMessage = "Por el momento el servidor no se encuentra disponible, intente más tarde";
                ShowErrorRecoveringCreditApplicationContent(errorMessage);
                RedirectToCreditApplicationsListView();
            }
        }

        private void ShowErrorRecoveringCreditApplicationContent(string errorMessage)
        {
            MessageBox.Show(
                errorMessage,
                "Sistema no disponible",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }

        private void ShowCreditApplicationContent(CreditApplication creditApplication)
        {
            List<string> phoneNumbers = new List<string>();
            foreach (var phoneNumber in creditApplication.Client.ContacMethods)
            {
                if (phoneNumber.MethodType == "PhoneNumber")
                {
                    phoneNumbers.Add(phoneNumber.Value);
                }
            }
            List<string> emails = new List<string>();
            foreach (var email in creditApplication.Client.ContacMethods)
            {
                if (email.MethodType == "Email")
                {
                    emails.Add(email.Value);
                }
            }
            TbkFullName.Text = $"{creditApplication.Client.Name} {creditApplication.Client.Surname} {creditApplication.Client.LastName}";
            TbkBirthDate.Text = Utilities.DateToolkit.FormatAsText(creditApplication.Client.Birthdate);
            TbkRFC.Text = creditApplication.Client.Rfc;
            TbkCURP.Text = creditApplication.Client.Curp;
            TbkAddress.Text = Utilities.AddressToolkit.FormatAsText(creditApplication.Client.Address);

            if (phoneNumbers.Count >= 2)
            {
                brdFirstPhoneNumber.Visibility = Visibility.Visible;
                brdSecondPhoneNumber.Visibility = Visibility.Visible;
                tbkFirstPhoneNumber.Text = Utilities.NumberFormatToolkit.FormatAsTenDigits(phoneNumbers[0]);
                tbkSecondPhoneNumber.Text = Utilities.NumberFormatToolkit.FormatAsTenDigits(phoneNumbers[1]);
            }
            else if (phoneNumbers.Count >= 3)
            {
                brdThirdPhoneNumber.Visibility = Visibility.Visible;
                tbkThirdPhoneNumber.Text = Utilities.NumberFormatToolkit.FormatAsTenDigits(phoneNumbers[2]);
            }
            else if (phoneNumbers.Count == 4)
            {
                brdThirdPhoneNumber.Visibility = Visibility.Visible;
                tbkFourthPhoneNumber.Text = Utilities.NumberFormatToolkit.FormatAsTenDigits(phoneNumbers[3]);
            }

            if (emails.Count >= 1)
            {
                brdFirstEmail.Visibility = Visibility.Visible;
                tbkFirstEmail.Text = emails[0];
            }
            else if (phoneNumbers.Count >= 2)
            {
                brdSecondEmail.Visibility = Visibility.Visible;
                tbkSecondEmail.Text = emails[1];
            }
            else if (phoneNumbers.Count == 3)
            {
                brdThirdEmail.Visibility = Visibility.Visible;
                tbkThirdEmail.Text = emails[3];
            }

            TbkCompanyName.Text = creditApplication.Client.WorkCenter.CompanyName;
            TbkEmployeePosition.Text = creditApplication.Client.WorkCenter.EmployeePosition;
            TbkSalary.Inlines.Add(new Run(creditApplication.Client.WorkCenter.Salary.ToString("C", new System.Globalization.CultureInfo("es-MX"))));
            TbkEmployeeSeniority.Text = creditApplication.Client.WorkCenter.EmployeeSeniority + " años";
            TbkWorkCenterPhoneNumber.Text = Utilities.NumberFormatToolkit.FormatAsTwelveDigits(creditApplication.Client.WorkCenter.PhoneNumber);
            TbkHumanResourcesPhoneNumber.Text = Utilities.NumberFormatToolkit.FormatAsTenDigits(creditApplication.Client.WorkCenter.HumanResourcesPhone);

            CreditApplicationCreditConditionControl creditConditionControl = new CreditApplicationCreditConditionControl(creditApplication.CreditCondition);
            skpCreditCondition.Children.Add(creditConditionControl);

            TbkCreditType.Text = creditApplication.CreditType.Name;
            TbkAmountAspirated.Inlines.Add(new Run(creditApplication.RequestedAmount.ToString("C", new System.Globalization.CultureInfo("es-MX"))));
            TbkMinimumAcceptedAmount.Inlines.Add(new Run(creditApplication.MinimumAmountAccepted.ToString("C", new System.Globalization.CultureInfo("es-MX"))));
            TbkPurposeOfCredit.Text = creditApplication.Purpose;

            BtnDownloadINE.Content = creditApplication.DigitalDocuments[0].Name;
            BtnDownloadProofOfAddress.Content = creditApplication.DigitalDocuments[1].Name;
            BtnDownloadProofOfIncome.Content = creditApplication.DigitalDocuments[2].Name;
        }

        private void BtnDownloadINEClick(object sender, RoutedEventArgs e)
        {
            byte[] fileInBytes = creditApplication.DigitalDocuments[0].Content;

            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog
            {
                Filter = "Archivos PDF (*.pdf)|*.pdf",
                Title = "Guardar PDF",
                FileName = creditApplication.DigitalDocuments[0].Name
            };

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SaveDocumentToPath(saveFileDialog.FileName, fileInBytes);
            }
        }

        private void BtnDownloadProofOfAddressClick(object sender, RoutedEventArgs e)
        {
            byte[] fileInBytes = creditApplication.DigitalDocuments[1].Content;

            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog
            {
                Filter = "Archivos PDF (*.pdf)|*.pdf",
                Title = "Guardar PDF",
                FileName = creditApplication.DigitalDocuments[1].Name
            };

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SaveDocumentToPath(saveFileDialog.FileName, fileInBytes);
            }
        }

        private void BtnDownloadProofOfIncomeClick(object sender, RoutedEventArgs e)
        {
            byte[] fileInBytes = creditApplication.DigitalDocuments[2].Content;

            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog
            {
                Filter = "Archivos PDF (*.pdf)|*.pdf",
                Title = "Guardar PDF",
                FileName = creditApplication.DigitalDocuments[2].Name
            };

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SaveDocumentToPath(saveFileDialog.FileName, fileInBytes);
            }
        }

        private void SaveDocumentToPath(string pathFile, byte[] fileInBytes)
        {
            try
            {
                File.WriteAllBytes(pathFile, fileInBytes);
            }
            catch (IOException)
            {
                ShowFileDownloadError();
            }
        }

        private void ShowFileDownloadError()
        {
            MessageBox.Show(
                "No se pudo descargar el archivo, inténtelo de nuevo",
                "Error al descragar el archivo",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }

        private void RbRejectApplicationChecked(object sender, RoutedEventArgs e)
        {
            dictumIsApproved = false;
            TbkAmountApproved.Visibility = Visibility.Collapsed;
            GrdAmountApproved.Visibility = Visibility.Collapsed;
            tbkPolicys.Visibility = Visibility.Collapsed;
            skpCreditGrantingPolices.Visibility = Visibility.Collapsed;
            ShowDictumJustificationField();
            ShowGenerateDictumButton();
        }

        private void RbApproveApplicationChecked(object sender, RoutedEventArgs e)
        {
            dictumIsApproved = true;
            ShowAmountApprovedField();
            ShowCreditGrantingPolices();
            ShowDictumJustificationField();
            ShowGenerateDictumButton();
        }

        private void ShowCreditGrantingPolices()
        {
            skpCreditGrantingPolices.Children.Clear();
            tbkPolicys.Visibility = Visibility.Visible;
            skpCreditGrantingPolices.Visibility = Visibility.Visible;
            foreach (var polices in creditGrantingPolicesList)
            {
                CreditAuthorizationDictumPolicyControl policyControl = new CreditAuthorizationDictumPolicyControl();
                policyControl.tbkPolicyName.Text = polices.Title;
                policyControl.CheckCreditGrantingPolicy += CkbSelectCreditGrantingPolicyChecked;
                policyControl.UncheckCreditGrantingPolicy += CkbSelectCreditGrantingPolicyUnchecked;
                policyControl.tbkPolicyName.ToolTip = polices.Description;
                skpCreditGrantingPolices.Children.Add(policyControl);
            }
            scvCreditAuthorizationDictum.ScrollToBottom();
        }

        private void ShowAmountApprovedField()
        {
            TbkAmountApproved.Visibility = Visibility.Visible;
            GrdAmountApproved.Visibility = Visibility.Visible;
        }

        private void ShowDictumJustificationField()
        {
            tbkJustification.Visibility = Visibility.Visible;
            grdJusitification.Visibility = Visibility.Visible;
        }

        private void ShowGenerateDictumButton()
        {
            btnGenerateDictum.Visibility = Visibility.Visible;
        }

        private void CkbSelectCreditGrantingPolicyChecked(object sender, EventArgs e)
        {
            CreditAuthorizationDictumPolicyControl policyControl = (CreditAuthorizationDictumPolicyControl)sender;
            foreach (var policy in creditGrantingPolicesList)
            {
                if (policy.Title == policyControl.tbkPolicyName.Text)
                {
                    creditGrantingPolicesListThatApply.Add(policy);
                    break;
                }
            }
            tbkPolicys.Foreground = (Brush)this.FindResource("DarkGray");
        }

        private void CkbSelectCreditGrantingPolicyUnchecked(object sender, EventArgs e)
        {
            CreditAuthorizationDictumPolicyControl policyControl = (CreditAuthorizationDictumPolicyControl)sender;
            foreach (var policy in creditGrantingPolicesListThatApply)
            {
                if (policy.Title == policyControl.tbkPolicyName.Text)
                {
                    creditGrantingPolicesListThatApply.Remove(policy);
                    break;
                }
            }
            if (creditGrantingPolicesListThatApply.Count == 0)
            {
                tbkPolicys.Foreground = (Brush)this.FindResource("Red");
            }
        }

        private void BtnGenerateDictumClick(object sender, RoutedEventArgs e)
        {
            bool isValidInformation = VerifyDictumInformation();
            if (isValidInformation)
            {
                ShowGenerateDictumConfirmationDIalog();
            }
            else
            {
                HighLightInvalidFields();
                ShowInvalidFieldsAlertDialog();
            }
        }

        private void BtnCancelDictumGenerationClick(object sender, RoutedEventArgs e)
        {
            ShowCancelDictumGenerationDialog();
        }

        private void BtnDiscardDictumGenerationClick(object sender, RoutedEventArgs e)
        {
            ShowDiscardDictumGenerationDialog();
        }

        private void ShowGenerateDictumConfirmationDIalog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                "¿Está seguro que desea guardar los cambios realizados a la referencia personal del cliente "
                + creditApplication.Client.Name + " " + creditApplication.Client.Surname + " " + creditApplication.Client.LastName,
                "Confirmación de actualización",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (buttonClicked == MessageBoxResult.Yes)
            {
                if (dictumIsApproved)
                {
                    GenerateApprovalDictum();
                }
                else
                {
                    GenerateRejectedDictum();
                }
            }
        }

        private void ShowCancelDictumGenerationDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                "¿Está seguro que desea cancelar la generación del dictamen? Todos los cambios sin guardar se perderán",
                "Cancelar cambios",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (buttonClicked == MessageBoxResult.Yes)
            {
                RedirectToCreditApplicationsListView();
            }
        }

        private void ShowDiscardDictumGenerationDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                "¿Está seguro que desea regresar a la ventana previa? Todos los cambios sin guardar se perderán",
                "Regresar a ventana previa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (buttonClicked == MessageBoxResult.Yes)
            {
                RedirectToCreditApplicationsListView();
            }
        }

        private bool VerifyDictumInformation()
        {
            float amountAspired = (float)creditApplication.RequestedAmount;
            float minimunAmount = (float)creditApplication.MinimumAmountAccepted;
            float amountApproved;
            bool isValidFields = true;

            if (dictumIsApproved)
            {
                if (tbJustification.Text.Trim().Length == 0) isValidFields = false;
                if (creditGrantingPolicesListThatApply.Count == 0) isValidFields = false;
                if (tbAmountApproved.Text.Trim().Length == 0)
                {
                    isValidFields = false;
                }
                else
                {
                    amountApproved = float.Parse(tbAmountApproved.Text.Trim().ToString());
                    if (amountApproved < minimunAmount || amountApproved > amountAspired) isValidFields = false;
                }
            }
            else
            {
                if (tbJustification.Text.Trim().Length == 0) isValidFields = false;
            }

            return isValidFields;
        }
        private void HighLightInvalidFields()
        {
            float amountAspired = (float)creditApplication.RequestedAmount;
            float minimunAmount = (float)creditApplication.MinimumAmountAccepted;
            float amountApproved;
            Style textInputErrorStyle = (Style)this.FindResource("SecondTextInputError");
            if (dictumIsApproved)
            {
                if (tbJustification.Text.Trim().Length == 0) tbJustification.Style = textInputErrorStyle;
                if (creditGrantingPolicesListThatApply.Count == 0) tbkPolicys.Foreground = (Brush)this.FindResource("Red");
                if (tbAmountApproved.Text.Trim().Length == 0)
                {
                    tbAmountApproved.Style = textInputErrorStyle;
                }
                else
                {
                    amountApproved = float.Parse(tbAmountApproved.Text.Trim().ToString());
                    if (amountApproved < minimunAmount || amountApproved > amountAspired) tbAmountApproved.Style = textInputErrorStyle;
                }
            }
            else
            {
                if (tbJustification.Text.Trim().Length == 0) tbJustification.Style = textInputErrorStyle;
            }
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

        private void GenerateApprovalDictum()
        {
            Dictum dictum = new Dictum
            {
                EmployeeNumber = Session.SystemSession.Employee.EmployeeNumber,
                Justification = tbJustification.Text.Trim(),
                GenerationDate = DateTime.Now,
                IsApproved = true
            };

            try
            {
                bool dictumGeneration = credititsServiceClient.GenerateApprovedDictum(creditGrantingPolicesList, creditGrantingPolicesListThatApply.ToArray(), dictum, creditApplication, float.Parse(tbAmountApproved.Text));
                if (dictumGeneration)
                {
                    string message = "Se ha generado el dictamen para el cliente " + creditApplication.Client.Name + " " + creditApplication.Client.Surname + " " +
                        creditApplication.Client.LastName + " así como su credito de manera exitosa";
                    ShowDictumSuccesfullGenerationMessageDialog(message);
                    RedirectToCreditApplicationsListView();
                }
                else
                {
                    ShowExistingDictumMessageDialog();
                    RedirectToCreditApplicationsListView();
                }
            }
            catch (FaultException<ServiceFault> fe)
            {
                ShowErrorDictumGeneration(fe.Message);
            }
            catch (EndpointNotFoundException)
            {
                string errorMessage = "Por el momento el servidor no se encuentra disponible, intente más tarde";
                ShowErrorDictumGeneration(errorMessage);
                RedirectToCreditApplicationsListView();
            }
            catch (CommunicationException)
            {
                string errorMessage = "Por el momento el servidor no se encuentra disponible, intente más tarde";
                ShowErrorDictumGeneration(errorMessage);
                RedirectToCreditApplicationsListView();
            }
        }

        private void ShowDictumSuccesfullGenerationMessageDialog(string message)
        {
            MessageBox.Show(
                message,
                "Generación existosa",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void ShowExistingDictumMessageDialog()
        {
            MessageBox.Show(
                "Lo sentimos, parece ser que ya se generó un dictamen para la solicitud de crédito actual, se " +
                "redirigirá a la lista de solicitudes",
                "Generación de dictamen fallida",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
        }

        private void ShowErrorDictumGeneration(string errorMessage)
        {
            MessageBox.Show(
                errorMessage,
                "Sistema no disponible",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }

        private void GenerateRejectedDictum()
        {
            Dictum dictum = new Dictum
            {
                Justification = tbJustification.Text.Trim(),
                GenerationDate = DateTime.Now,
                IsApproved = false,
                EmployeeNumber = Session.SystemSession.Employee.EmployeeNumber
            };
            try
            {
                bool dictumGeneration = credititsServiceClient.GenerateRejectedDictum(dictum, creditApplication);
                if (dictumGeneration)
                {
                    string message = "Se ha generado el dictamen para el cliente " + creditApplication.Client.Name + " " + creditApplication.Client.Surname + " " +
                        creditApplication.Client.LastName + " de manera exitosa";
                    ShowDictumSuccesfullGenerationMessageDialog(message);
                    RedirectToCreditApplicationsListView();
                }
                else
                {
                    ShowExistingDictumMessageDialog();
                    RedirectToCreditApplicationsListView();
                }
            }
            catch (FaultException<ServiceFault> fe)
            {
                ShowErrorDictumGeneration(fe.Message);
            }
            catch (EndpointNotFoundException)
            {
                string errorMessage = "Por el momento el servidor no se encuentra disponible, intente más tarde";
                ShowErrorDictumGeneration(errorMessage);
                RedirectToCreditApplicationsListView();
            }
            catch (CommunicationException)
            {
                string errorMessage = "Por el momento el servidor no se encuentra disponible, intente más tarde";
                ShowErrorDictumGeneration(errorMessage);
                RedirectToCreditApplicationsListView();
            }
        }

        private void TbJustificationTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tbJusitification = sender as TextBox;

            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("SecondTextInputError");

            string justification = tbJusitification.Text.Trim();

            if (justification.Length == 0)
            {
                tbJusitification.Style = textInputErrorStyle;
            }
            else
            {
                tbJusitification.Style = textInputStyle;
            }
        }

        private void TbAmountApprovedTextChanged(object sender, TextChangedEventArgs e)
        {
            Style textInputStyle = (Style)this.FindResource("TextInput");
            Style textInputErrorStyle = (Style)this.FindResource("SecondTextInputError");

            string newAmount = tbAmountApproved.Text.Trim();
            bool isWritingNewAmmount = newAmount != lastAmountApproved;
            float amountAspired = (float)creditApplication.RequestedAmount;
            float minimunAmount = (float)creditApplication.MinimumAmountAccepted;
            float amountApproved;

            if (isWritingNewAmmount)
            {
                if (newAmount != string.Empty && !DataValidator.IsValidMoneyAmount(newAmount))
                {
                    tbAmountApproved.Text = lastAmountApproved;
                    tbAmountApproved.CaretIndex = tbAmountApproved.Text.Length;
                }
                else
                {
                    lastAmountApproved = newAmount;
                }
            }

            if (tbAmountApproved.Text.Trim().Length == 0)
            {
                tbAmountApproved.Style = textInputErrorStyle;
            }
            else
            {
                amountApproved = float.Parse(tbAmountApproved.Text.Trim().ToString());
                if (amountApproved < minimunAmount || amountApproved > amountAspired) 
                {
                    tbAmountApproved.Style = textInputErrorStyle;
                }
                else
                {
                    tbAmountApproved.Style = textInputStyle;
                }
            }
        }
    }
}
