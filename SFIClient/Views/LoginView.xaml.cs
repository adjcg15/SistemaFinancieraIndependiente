using SFIClient.Session;
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
    public partial class LoginController : Page
    {
        public LoginController()
        {
            InitializeComponent();
        }

        private void BtnLoginClick(object sender, RoutedEventArgs e)
        {
            string employeeNumber = TbEmployeeNumber.Text.Trim();
            string password = PbPassword.Password.Trim();

            Style textFieldDefaultStyle = (Style)FindResource("TextInput");
            Style passwordFieldDefaultStyle = (Style)FindResource("PasswordInput");
            TbEmployeeNumber.Style = textFieldDefaultStyle;
            PbPassword.Style = passwordFieldDefaultStyle;

            if (employeeNumber == "" || password == "")
            {
                ShowEmptyFieldsWarningDialog();
                HighlightInvalidFields();
            } 
            else
            {
                Login();
            }
        }

        private void ShowEmptyFieldsWarningDialog()
        {
            MessageBox.Show(
                "Por favor ingrese información en los campos marcados. La contraseña y el número " +
                "de empleado son obligatorios.",
                "Campos obligatorios",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
        }

        private void HighlightInvalidFields()
        {
            string employeeNumber = TbEmployeeNumber.Text.Trim();
            string password = PbPassword.Password.Trim();

            Style textFieldErrorStyle = (Style)FindResource("TextInputError");
            Style passwordFieldErrorStyle = (Style)FindResource("PasswordInputError");

            if (employeeNumber == "")
            {
                TbEmployeeNumber.Style = textFieldErrorStyle;
            }

            if(password == "")
            {
                PbPassword.Style = passwordFieldErrorStyle;
            }
        }

        private void Login()
        {
            string employeeNumber = TbEmployeeNumber.Text.Trim();
            string passwordHash = Security.HashPasswordWithSHA256(PbPassword.Password.Trim());

            AuthenticationServiceClient authenticationService = new AuthenticationServiceClient();
            try
            {
                Employee employeeAccount = authenticationService.Login(employeeNumber, passwordHash);
                
                if(employeeAccount == null)
                {
                    ShowInvalidCredentialsWarningDialog();
                }
                else
                {
                    StartEmployeeSession(employeeAccount);
                    RedirectToMainPage();
                }
            }
            catch (FaultException<ServiceFault> fault)
            {
                ShowErrorLoggingInDialog(fault.Detail.Message);
            }
            catch (EndpointNotFoundException)
            {
                string errorMessage = "El servidor no se encuentra disponible, intente más tarde";
                ShowErrorLoggingInDialog(errorMessage);
            }
            catch (CommunicationException)
            {
                string errorMessage = "No fue posible acceder a la información debido a un error de conexión";
                ShowErrorLoggingInDialog(errorMessage);
            }
        }

        private void ShowErrorLoggingInDialog(string message)
        {
            MessageBox.Show(
                message,
                "Sistema no disponible",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }

        private void ShowInvalidCredentialsWarningDialog()
        {
            MessageBox.Show(
                "Verifique su número de empleado y la contraseña ingresados",
                "Credenciales incorrectas",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
        }

        private void StartEmployeeSession(Employee employee)
        {
            SystemSession.Employee = employee;
        }

        private void RedirectToMainPage()
        {
            switch (SystemSession.Employee.EmployeeRole.Name)
            {
                case Security.Roles.MANAGER:
                case Security.Roles.DEBT_COLLECTOR:
                    NavigationService.Navigate(new MainMenuController());
                    break;
                case Security.Roles.CREDIT_ANALYST:
                    NavigationService.Navigate(new CreditApplicationsListController());
                    break;
                case Security.Roles.CREDIT_ADVISOR:
                    NavigationService.Navigate(new SearchClientByRFCController());
                    break;
                
            }
        }
    }
}
