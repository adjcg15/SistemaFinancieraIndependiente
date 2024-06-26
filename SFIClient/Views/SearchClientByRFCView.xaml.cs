﻿using SFIClient.Controlls;
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
using SFIClient.Session;
using System.ServiceModel.Channels;
using System.Windows.Markup;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace SFIClient.Views
{
    /// <summary>
    /// Lógica de interacción para SearchClientByRFCView.xaml
    /// </summary>
    public partial class SearchClientByRFCController : Page
    {
        private readonly ClientsServiceClient clientsServiceClient = new ClientsServiceClient();
        private List<Client> clientsList = new List<Client>();
        public SearchClientByRFCController()
        {
            InitializeComponent();
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            LoadClients();
        }

        private void LoadClients()
        {
            try
            {
                clientsList = clientsServiceClient.RecoverClients().ToList();
                if (clientsList.Count != 0)
                {
                    AddClientsToClientsList();
                }
                else
                {
                    ShowUnregisteredClientsMessage();
                }
            }
            catch (FaultException<ServiceFault> fe)
            {
                ShowErrorRecoveringClients(fe.Detail.Message);
                RedirectToLoginView();
            }
            catch (EndpointNotFoundException)
            {
                string message = "No fue posible recuperar los clientes, inténtelo de nuevo más tarde";
                ShowErrorRecoveringClients(message);
                RedirectToLoginView();
            }
            catch (CommunicationException)
            {
                string message = "No fue posible recuperar los clientes, inténtelo de nuevo más tarde";
                ShowErrorRecoveringClients(message); 
                RedirectToLoginView();
            }
        }

        private void ShowUnregisteredClientsMessage()
        {
            SkpRegisterClient.Children.Clear();
            SkpRegisterClient.Visibility = Visibility.Collapsed;
            SkpSearchClient.Visibility = Visibility.Collapsed;
            SkpRegisterClientNow.Children.Add(BtnRegisterClient);
        }

        private void ShowErrorRecoveringClients(string message)
        {
            MessageBox.Show(
                message,
                "Servicio no disponible",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }

        private void RedirectToLoginView()
        {
            NavigationService.Navigate(new LoginController());
            SystemSession.Employee = null;
        }

        private void BtnGoToLoginViewClick(object sender, EventArgs e)
        {
            RedirectToLoginView();
        }

        private void AddClientsToClientsList()
        {
            SkpNoRegisteredClients.Visibility = Visibility.Collapsed;
            SkpRegisterClientNow.Visibility = Visibility.Collapsed;
            for (int i = 0; i < clientsList.Count; i++)
            {
                if (clientsList[i].HasActiveCredit || clientsList[i].HasCreditApplication)
                {
                    ShowClientWithAllOptionsWithoutCreditApplication(clientsList[i]);
                }
                else
                {
                    ShowClientWithAllOptions(clientsList[i]);
                }
            }
        }

        private void AddSearchedClientToClientList(Client client)
        {
            SkpNoRegisteredClients.Visibility = Visibility.Collapsed;
            SkpRegisterClientNow.Visibility = Visibility.Collapsed;
            if (client.HasActiveCredit || client.HasCreditApplication)
            {
                ShowClientWithAllOptionsWithoutCreditApplication(client);
            }
            else
            {
                ShowClientWithAllOptions(client);
            }
        }

        private void ShowClientWithAllOptions(Client client)
        {
            ClientControl clientControl = new ClientControl();
            clientControl.ButtonSeePersonalInformation += BtnSeePersonalInformationClick;
            clientControl.ButtonModifyPersonalInformation += BtnModifyPersonalInformationClick;
            clientControl.ButtonModifyBankAccount += BtnModifyBankAccountClick;
            clientControl.ButtonModifyWorkCenter += BtnModifyWorkCenterClick;
            clientControl.ButtonModifyPersonalReferences += BtnModifyPersonalReferencesClick;
            clientControl.ButtonApplyForCredit += BtnApplyForCreditClick;
            clientControl.LblClientRFC.Content = client.Rfc;
            clientControl.LblClientName.Content = client.LastName + " " + client.Surname + " " + client.Name;
            clientControl.LblClientCreditStatus.Content = "Cliente sin crédito y sin solicitud activa";
            ItcClients.Items.Add(clientControl);
        }

        private void ShowClientWithAllOptionsWithoutCreditApplication(Client client)
        {
            ClientControl clientControl = new ClientControl();
            clientControl.ButtonSeePersonalInformation += BtnSeePersonalInformationClick;
            clientControl.ButtonModifyPersonalInformation += BtnModifyPersonalInformationClick;
            clientControl.ButtonModifyBankAccount += BtnModifyBankAccountClick;
            clientControl.ButtonModifyWorkCenter += BtnModifyWorkCenterClick;
            clientControl.ButtonModifyPersonalReferences += BtnModifyPersonalReferencesClick;
            clientControl.LblClientRFC.Content = client.Rfc;
            clientControl.LblClientName.Content = client.LastName + " " + client.Surname + " " + client.Name;
            BitmapImage bitmap = new BitmapImage(new Uri("pack://application:,,,/Assets/ApplyForCreditDisabledIcon.png"));
            Image image = new Image
            {
                Source = bitmap,
                Height = 25,
                Width = 34,
            };
            clientControl.BtnApplyForCredit.Content = image;
            clientControl.BtnApplyForCredit.IsEnabled = false;
            if (client.HasActiveCredit)
            {
                clientControl.LblClientCreditStatus.Content = "Cliente con crédito activo";
            }
            else if (client.HasCreditApplication)
            {

                clientControl.LblClientCreditStatus.Content = "Cliente con solicitud de crédito activa";
            }
            ItcClients.Items.Add(clientControl);
        }

        private void BtnSeePersonalInformationClick(object sender, EventArgs e)
        {
            ClientControl clientControl = (ClientControl)sender;
            Client client = new Client();
            for (int i = 0; i < clientsList.Count; i++)
            {
                if (clientsList[i].Rfc == clientControl.LblClientRFC.Content.ToString())
                {
                    client = clientsList[i];
                    break;
                }
            }
            if (client != null)
            {
                this.NavigationService.Navigate(new ClientInformationController(client));
            }
        }

        private void BtnModifyBankAccountClick(object sender, EventArgs e)
        {
            ClientControl clientControll = (ClientControl)sender;
            Client client = new Client();
            for (int i = 0; i < clientsList.Count; i++)
            {
                if (clientsList[i].Rfc == clientControll.LblClientRFC.Content.ToString())
                {
                    client = clientsList[i];
                    break;
                }
            }
            if (client != null)
            {
                this.NavigationService.Navigate(new ModifyBankAccountController(client));
            }
        }

        private void BtnModifyPersonalInformationClick(object sender, EventArgs e)
        {
            ClientControl clientCard = (ClientControl)sender;
            string clientRFC = clientCard.LblClientRFC.Content.ToString();

            NavigationService.Navigate(new ModifyPersonalInformationController(clientRFC));
        }

        private void BtnModifyWorkCenterClick(object sender, EventArgs e)
        {
            ClientControl clientControl = (ClientControl)sender;
            string clientRFC = clientControl.LblClientRFC.Content.ToString();

            NavigationService.Navigate(new ModifyWorkCenterController(clientRFC));
        }

        private void BtnModifyPersonalReferencesClick(object sender, EventArgs e)
        {
            ClientControl clientControl = (ClientControl)sender;
            Client client = new Client();
            for (int i = 0; i < clientsList.Count; i++)
            {
                if (clientsList[i].Rfc == clientControl.LblClientRFC.Content.ToString())
                {
                    client = clientsList[i];
                    break;
                }
            }
            if (client != null)
            {
                try
                {
                    PersonalReference[] personalReferencesList = new PersonalReference[2];
                    personalReferencesList = clientsServiceClient.RecoverPersonalReferences(client.Rfc);
                    ShowPersonalReferencesToModify(client, personalReferencesList);
                }
                catch (FaultException<ServiceFault> fe)
                {
                    ShowErrorRecoveringPersonalReferences(fe.Detail.Message);
                    RedirectToLoginView();
                }
                catch (EndpointNotFoundException)
                {
                    string errorMessage = "No fue posible recuperar las referencias personales del cliente, inténtelo de nuevo más tarde";
                    ShowErrorRecoveringPersonalReferences(errorMessage);
                    RedirectToLoginView();
                }
                catch (CommunicationException)
                {
                    string errorMessage = "No fue posible recuperar las referencias personales del cliente, inténtelo de nuevo más tarde";
                    ShowErrorRecoveringPersonalReferences(errorMessage);
                    RedirectToLoginView();
                }
            }
        }

        private void ShowErrorRecoveringPersonalReferences(string message)
        {
            MessageBox.Show(
                message,
                "Sistema no disponible",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }

        private void ShowPersonalReferencesToModify(Client client, PersonalReference[] personalReferences)
        {
            PersonalReferencesOptionsDialog personalReferencesOptionsDialog =
                new PersonalReferencesOptionsDialog("El cliente " 
                + client.Name + " " + client.Surname + " " + client.LastName 
                + " tiene las siguientes referencias.\n¿Cuál deseas modificar?", 
                personalReferences[0], personalReferences[1]);
            DialogResult result = personalReferencesOptionsDialog.ShowDialog();

            if (result == DialogResult.Yes)
            {
                this.NavigationService.Navigate(new ModifyPersonalReferenceController(personalReferences[0], client));
            }
            else if (result == DialogResult.No)
            {
                this.NavigationService.Navigate(new ModifyPersonalReferenceController(personalReferences[1], client));
            }
        }

        private void BtnApplyForCreditClick(object sender, EventArgs e)
        {
            ClientControl clientControl = (ClientControl)sender;
            Client client = new Client();
            for (int i = 0; i < clientsList.Count; i++)
            {
                if (clientsList[i].Rfc == clientControl.LblClientRFC.Content.ToString())
                {
                    client = clientsList[i];
                    break;
                }
            }
            if (client != null)
            {
                this.NavigationService.Navigate(new CredditApplicationController(client));
            }
        }

        private void BtnSearchClientClick(object sender, RoutedEventArgs e)
        {
            bool clientExists = true;
            string rfcWanted = TbRFCClient.Text.ToUpper().Trim();

            ItcClients.Items.Clear();
            for (int i = 0; i < clientsList.Count; i++)
            {
                if (clientsList[i].Rfc.Contains(rfcWanted))
                {
                    AddSearchedClientToClientList(clientsList[i]);
                }
            }

            if (ItcClients.Items.Count == 0)
            {
                clientExists = false;
            }

            if (!clientExists)
            {
                ShowNonExistingClientAndRegisterNewClientMessage();
            }
        }

        private void ShowNonExistingClientAndRegisterNewClientMessage()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                "¿Deseas registrar al cliente?", 
                "El cliente no existe", 
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (buttonClicked == MessageBoxResult.Yes)
            {
                ClientRegisterController clientRegisterView = new ClientRegisterController();
                this.NavigationService.Navigate(clientRegisterView);
            }
            else if (buttonClicked == MessageBoxResult.No)
            {
                AddClientsToClientsList();
                TbRFCClient.Text = string.Empty;
            }
        }

        private void BtnRegisterClientClick(object sender, RoutedEventArgs e)
        {
            ClientRegisterController clientRegisterView = new ClientRegisterController();
            this.NavigationService.Navigate(clientRegisterView);
        }

        private void BtnGoToLoginViewClick(object sender, RoutedEventArgs e)
        {
            RedirectToLoginView();
        }
    }

    class PersonalReferencesOptionsDialog : Form
    {
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Label lblFirstReference;
        private System.Windows.Forms.Label lblSecondReference;
        private System.Windows.Forms.Button btnOption1;
        private System.Windows.Forms.Button btnOption2;

        public PersonalReferencesOptionsDialog(string message, PersonalReference firstReference, PersonalReference secondReference)
        {
            InitializeComponent();
            lblMessage.Text = message;
            lblFirstReference.Text = "(1). " + firstReference.Name + " " + firstReference.Surname + " " + firstReference.LastName;
            lblSecondReference.Text = "(2). " + secondReference.Name + " " + secondReference.Surname + " " + secondReference.LastName;

            btnOption1.Text = "1";
            btnOption2.Text = "2";
        }

        private void InitializeComponent()
        {
            this.lblMessage = new System.Windows.Forms.Label();
            this.lblFirstReference = new System.Windows.Forms.Label();
            this.lblSecondReference = new System.Windows.Forms.Label();
            this.btnOption1 = new System.Windows.Forms.Button();
            this.btnOption2 = new System.Windows.Forms.Button();
            this.SuspendLayout();

            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(9, 9);
            this.lblMessage.Font = new System.Drawing.Font(this.lblMessage.Font.FontFamily, 10f, this.lblMessage.Font.Style);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "label1";

            this.lblFirstReference.AutoSize = true;
            this.lblFirstReference.Location = new System.Drawing.Point(9, 70);
            this.lblFirstReference.Font = new System.Drawing.Font(this.lblFirstReference.Font.FontFamily, 9f, this.lblFirstReference.Font.Style);
            this.lblFirstReference.Name = "lblFirstReference";
            this.lblFirstReference.TabIndex = 0;
            this.lblFirstReference.Text = "label2";

            this.lblSecondReference.AutoSize = true;
            this.lblSecondReference.Location = new System.Drawing.Point(9, 100);
            this.lblSecondReference.Font = new System.Drawing.Font(this.lblSecondReference.Font.FontFamily, 9f, this.lblSecondReference.Font.Style);
            this.lblSecondReference.Name = "lblSecondReference";
            this.lblSecondReference.TabIndex = 0;
            this.lblSecondReference.Text = "label3";

            this.btnOption1.Location = new System.Drawing.Point(9, 161);
            this.btnOption1.Name = "btnOption1";
            this.btnOption1.Size = new System.Drawing.Size(90, 33);
            this.btnOption1.TabIndex = 1;
            this.btnOption1.Text = "button1";
            this.btnOption1.UseVisualStyleBackColor = true;
            this.btnOption1.Click += new EventHandler(this.BtnOption1_Click);
            
            this.btnOption2.Location = new System.Drawing.Point(119, 161);
            this.btnOption2.Name = "btnOption2";
            this.btnOption2.Size = new System.Drawing.Size(90, 33);
            this.btnOption2.TabIndex = 2;
            this.btnOption2.Text = "button2";
            this.btnOption2.UseVisualStyleBackColor = true;
            this.btnOption2.Click += new EventHandler(this.BtnOption2_Click);
            
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.btnOption2);
            this.Controls.Add(this.btnOption1);
            this.Controls.Add(this.lblSecondReference);
            this.Controls.Add(this.lblFirstReference);
            this.Controls.Add(this.lblMessage);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Name = "PersonalReferencesOptionsDialog";
            this.Text = "Seleccione la referencia personal a modificar";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void BtnOption1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            Close();
        }

        private void BtnOption2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }
    }
}
