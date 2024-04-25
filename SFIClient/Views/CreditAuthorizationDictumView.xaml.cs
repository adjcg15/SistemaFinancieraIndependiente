using SFIClient.Controlls;
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
    /// Lógica de interacción para CreditAuthorizationDictumView.xaml
    /// </summary>
    public partial class CreditAuthorizationDictumController : Page
    {
        private readonly CreditGrantingPoliciesClient creditGrantingPoliciesClient = new CreditGrantingPoliciesClient();
        private readonly CreditsServiceClient credititsServiceClient = new CreditsServiceClient();
        private CreditGrantingPolicy[] creditGrantingPolicesList;
        private CreditApplication creditApplication;
        private readonly string creditInvoice;
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
                creditGrantingPolicesList = creditGrantingPoliciesClient.GetAllCreditGrantingPolicies();
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
            MessageBoxResult messageBoxResult = MessageBox.Show(
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
            NavigationService.Navigate(new SearchClientByRFCController());
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

        private void ShowErrorRecoveringCreditApplicationContent(string message)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show(
                "Por el momento no se puede generar ningún dictamen",
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
        }

        private void BtnDownloadINEClick(object sender, RoutedEventArgs e)
        {

        }

        private void BtnDownloadProofOfAddressSClick(object sender, RoutedEventArgs e)
        {

        }

        private void BtnDownloadProofOfIncomeClick(object sender, RoutedEventArgs e)
        {

        }

        private void SaveDocumentToPath()
        {

        }

        private void RbRejectApplicationChecked(object sender, RoutedEventArgs e)
        {
            tbkPolicys.Visibility = Visibility.Collapsed;
            skpCreditGrantingPolices.Visibility = Visibility.Collapsed;
            ShowDictumJustificationField();
            ShowGenerateDictumButton();
        }

        private void RbApproveApplicationChecked(object sender, RoutedEventArgs e)
        {
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
                policyControl.tbkPolicyName.ToolTip = polices.Description;
                skpCreditGrantingPolices.Children.Add(policyControl);
            }
            scvCreditAuthorizationDictum.ScrollToBottom();
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

        private void BtnGenerateDictumClick(object sender, RoutedEventArgs e)
        {

        }

        private void BtnCancelDictumGenerationClick(object sender, RoutedEventArgs e)
        {

        }

        private void BtnDiscardDictumGenerationClick(object sender, RoutedEventArgs e)
        {

        }

        private void ShowGenerateDictumConfirmationDIalog()
        {

        }

        private void ShowCancelDictumGenerationDialog()
        {

        }

        private void ShowDiscardDictumGenerationDialog()
        {

        }

        private void RedirectToCreditApplicationListView()
        {

        }
    }
}
