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
    /// Lógica de interacción para ClientInformationView.xaml
    /// </summary>
    public partial class ClientInformationController : Page
    {
        private readonly ClientsServiceClient clientsServiceClient = new ClientsServiceClient();
        private readonly Client client;
        public ClientInformationController(Client client)
        {
            InitializeComponent();
            this.client = client;
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            LoadClient();
        }

        private void LoadClient()
        {
            try
            {
                Client fullClient = clientsServiceClient.RecoverClient(client.Rfc);
                ShowClientInformation(fullClient);
            }
            catch (FaultException<ServiceFault> fe)
            {
                ShowErrorRecoveringClient(fe.Detail.Message);
                RedirectToSearchClientByRFCView();
            }
            catch (EndpointNotFoundException)
            {
                string errorMessage = "No fue posible recuperar la información del cliente, inténtelo de nuevo más tarde";
                ShowErrorRecoveringClient(errorMessage);
                RedirectToSearchClientByRFCView();
            }
            catch (CommunicationException)
            {
                string errorMessage = "No fue posible recuperar la información del cliente, inténtelo de nuevo más tarde";
                ShowErrorRecoveringClient(errorMessage);
                RedirectToSearchClientByRFCView();
            }
        }

        private void ShowClientInformation(Client client)
        {
            List<string> phoneNumbers = new List<string>();
            foreach (var phoneNumber in client.ContacMethods)
            {
                if (phoneNumber.MethodType == "PhoneNumber")
                {
                    phoneNumbers.Add(phoneNumber.Value);
                }
            }
            List<string> emails = new List<string>();
            foreach (var email in client.ContacMethods)
            {
                if (email.MethodType == "Email")
                {
                    emails.Add(email.Value);
                }
            }
            TbkFullName.Text = $"{client.Name} {client.Surname} {client.LastName}";
            TbkBirthDate.Text = Utilities.DateToolkit.FormatAsText(client.Birthdate);
            TbkRFC.Text = client.Rfc;
            TbkCURP.Text = client.Curp;
            TbkAddress.Text = Utilities.AddressToolkit.FormatAsText(client.Address);

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

            TbkCardNumber.Text = 
                $"{client.BankAccount.CardNumber.Substring(0, 4)} {client.BankAccount.CardNumber.Substring(4, 4)} " +
                $"{client.BankAccount.CardNumber.Substring(8, 4)} {client.BankAccount.CardNumber.Substring(12)}";
            TbkBank.Text = client.BankAccount.Bank;
            TbkHolder.Text = client.BankAccount.Holder;

            TbkCompanyName.Text = client.WorkCenter.CompanyName;
            TbkEmployeePosition.Text = client.WorkCenter.EmployeePosition;
            TbkSalary.Inlines.Add(new Run(client.WorkCenter.Salary.ToString("C", new System.Globalization.CultureInfo("es-MX"))));
            TbkEmployeeSeniority.Text = client.WorkCenter.EmployeeSeniority + " años";
            TbkWorkCenterPhoneNumber.Text = Utilities.NumberFormatToolkit.FormatAsTwelveDigits(client.WorkCenter.PhoneNumber);
            TbkHumanResourcesPhoneNumber.Text = Utilities.NumberFormatToolkit.FormatAsTenDigits(client.WorkCenter.HumanResourcesPhone);
            TbkWorkCenterAddress.Text = Utilities.AddressToolkit.FormatAsText(client.WorkCenter.Address);

            TbkFirstPersonalReferenceFullName.Text = 
                $"{client.PersonalReferences[0].Name} {client.PersonalReferences[0].Surname} {client.PersonalReferences[0].LastName}";
            TbkFirstPersonalReferencePhoneNumber.Text = 
                Utilities.NumberFormatToolkit.FormatAsTenDigits(client.PersonalReferences[0].PhoneNumber);
            TbkFirstPersonalReferenceKinship.Text = client.PersonalReferences[0].Kinship;
            TbkFirstPersonalReferenceRelationship.Text = client.PersonalReferences[0].RelationshipYears + " años";
            TbkFirstPersonalReferenceIneKey.Text = client.PersonalReferences[0].IneKey;
            TbkFirstPersonalReferenceAddress.Text = Utilities.AddressToolkit.FormatAsText(client.PersonalReferences[0].Address);

            TbkSecondPersonalReferenceFullName.Text =
                $"{client.PersonalReferences[1].Name} {client.PersonalReferences[1].Surname} {client.PersonalReferences[1].LastName}";
            TbkFirstPersonalReferencePhoneNumber.Text =
                Utilities.NumberFormatToolkit.FormatAsTenDigits(client.PersonalReferences[1].PhoneNumber);
            TbkSecondPersonalReferenceKinship.Text = client.PersonalReferences[1].Kinship;
            TbkSecondPersonalReferenceRelationship.Text = client.PersonalReferences[1].RelationshipYears + " años";
            TbkSecondPersonalReferenceIneKey.Text = client.PersonalReferences[1].IneKey;
            TbkSecondPersonalReferenceAddress.Text = Utilities.AddressToolkit.FormatAsText(client.PersonalReferences[1].Address);
        }

        private void BtnGoToSearchClientByRFCViewClick(object sender, RoutedEventArgs e)
        {
            RedirectToSearchClientByRFCView();
        }

        private void RedirectToSearchClientByRFCView()
        {
            NavigationService.Navigate(new SearchClientByRFCController());
        }

        private void ShowErrorRecoveringClient(string message)
        {
            MessageBox.Show(
                message,
                "Servicio no disponible",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
    }
}