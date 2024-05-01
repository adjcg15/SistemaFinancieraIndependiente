using Microsoft.Win32;
using SFIClient.Controlls;
using SFIClient.Session;
using SFIClient.SFIServices;
using SFIClient.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SFIClient.Views
{
    public partial class CredditApplicationController : Page
    {
        private string lastRequestedAmount = string.Empty;
        private string lastMinimumAcceptedAmount = string.Empty;
        private readonly CreditApplication newApplication = new CreditApplication();
        private readonly List<DigitalDocument> attachedDocuments = new List<DigitalDocument>();

        public CredditApplicationController(Client requestingClient)
        {
            InitializeComponent();

            newApplication.Client = requestingClient;
            newApplication.EmployeeNumber = SystemSession.Employee.EmployeeNumber;
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            LoadCreditTypes();
            ShowClientInformation();
        }

        private void LoadCreditTypes()
        {
            CreditsServiceClient creditsService = new CreditsServiceClient();

            try
            {
                List<CreditType> creditTypesRecovered = creditsService.GetAllCreditTypes().ToList();
                FillCreditTypesCombobox(creditTypesRecovered);
            }
            catch (FaultException<ServiceFault> fault)
            {
                ShowErrorRecoveringCreditTypesDialog(fault.Detail.Message);
            }
            catch (EndpointNotFoundException)
            {
                string errorMessage = "El servidor no se encuentra disponible, intente más tarde";
                ShowErrorRecoveringCreditTypesDialog(errorMessage);
            }
            catch (CommunicationException)
            {
                string errorMessage = "No fue posible acceder a la información debido a un error de conexión";
                ShowErrorRecoveringCreditTypesDialog(errorMessage);
            }
        }

        private void FillCreditTypesCombobox(List<CreditType> creditTypes)
        {
            ObservableCollection<CreditType> creditTypesPresented = new ObservableCollection<CreditType>();
            creditTypes.ForEach(creditType => creditTypesPresented.Add(creditType));

            CbCreditTypes.ItemsSource = creditTypesPresented;
        }

        private void ShowErrorRecoveringCreditTypesDialog(string message)
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                message,
                "Tipos de créditos no disponibles",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );

            if(buttonClicked == MessageBoxResult.OK)
            {
                RedirectToMainMenu();
            }
        }

        private void RedirectToMainMenu()
        {
            NavigationService.Navigate(new SearchClientByRFCController());
        }

        private void ShowClientInformation()
        {
            TbkClientName.Text = $"{newApplication.Client.Name} {newApplication.Client.Surname} {newApplication.Client.LastName}";

            SpnClientCurp.Inlines.Clear();
            SpnClientCurp.Inlines.Add(new Run(newApplication.Client.Curp));

            BldClientSalary.Inlines.Clear();
            SpnClientWorkCenterName.Inlines.Clear();
            if(newApplication.Client.WorkCenter != null)
            {
                BldClientSalary.Inlines.Add(new Run(newApplication.Client.WorkCenter.Salary.ToString("C", new System.Globalization.CultureInfo("es-MX"))));
                SpnClientWorkCenterName.Inlines.Add(new Run(newApplication.Client.WorkCenter.CompanyName));
            }
            else
            {
                TbkWorkCenterInformation.Visibility = Visibility.Collapsed;
            }
        }

        private void CbCreditTypesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(CbCreditTypes.SelectedIndex != -1)
            {
                newApplication.CreditCondition = null;

                CreditType selectedCreditType = (CreditType)CbCreditTypes.SelectedItem;
                newApplication.CreditType = selectedCreditType;
                LoadCreditConditionsByCreditType(selectedCreditType.Identifier);
            }
        }

        private void LoadCreditConditionsByCreditType(int creditConditionId)
        {
            CreditConditionsServiceClient creditConditionsService = new CreditConditionsServiceClient();

            try
            {
                List<CreditCondition> creditConditionsRecovered = 
                    creditConditionsService.RecoverCreditConditionsByCreditType(creditConditionId).ToList();
                ShowApplicableCreditConditions(creditConditionsRecovered);
            }
            catch (FaultException<ServiceFault> fault)
            {
                ShowErrorRecoveringCreditConditionsDialog(fault.Detail.Message);
            }
            catch (EndpointNotFoundException)
            {
                string errorMessage = "El servidor no se encuentra disponible, intente más tarde";
                ShowErrorRecoveringCreditConditionsDialog(errorMessage);
            }
            catch (CommunicationException)
            {
                string errorMessage = "No fue posible acceder a la información debido a un error de conexión";
                ShowErrorRecoveringCreditConditionsDialog(errorMessage);
            }
        }

        private void ShowApplicableCreditConditions(List<CreditCondition> applicableCreditConditions)
        {
            GrdEmptyConditionsMessage.Visibility = Visibility.Collapsed;
            SkpApplicableCreditConditions.Visibility = Visibility.Visible;
            SkpApplicableCreditConditions.Children.Clear();

            applicableCreditConditions.ForEach(conditon =>
            {
                var creditConditionCard = new CreditApplicationCreditConditionControl(conditon);
                creditConditionCard.CardClick += HighlightCreditConditionCard;

                SkpApplicableCreditConditions.Children.Add(creditConditionCard);
            });
        }

        private void HighlightCreditConditionCard(object sender, CreditCondition selectedCondition)
        {
            foreach (var child in SkpApplicableCreditConditions.Children)
            {
                if (child is CreditApplicationCreditConditionControl creditConditionCard)
                {

                    if(creditConditionCard.BindedCondition.Identifier == selectedCondition.Identifier)
                    {
                        SolidColorBrush primaryColor = (SolidColorBrush)Application.Current.Resources["PrimaryColor"];
                        SolidColorBrush lightGray = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ECECEC"));

                        creditConditionCard.BdrCreditConditionCard.BorderBrush = primaryColor;
                        creditConditionCard.BdrCreditConditionCard.Background = lightGray;

                        newApplication.CreditCondition = selectedCondition;
                    }
                    else
                    {
                        SolidColorBrush defaultBorderColor = (SolidColorBrush)Application.Current.Resources["LightGray"];
                        SolidColorBrush defaultBgColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FAFAFA"));

                        creditConditionCard.BdrCreditConditionCard.BorderBrush = defaultBorderColor;
                        creditConditionCard.BdrCreditConditionCard.Background = defaultBgColor;
                    }
                }
            }
        }

        private void ShowErrorRecoveringCreditConditionsDialog(string message)
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                message,
                "Condiciones de crédito no disponibles",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );

            if (buttonClicked == MessageBoxResult.OK)
            {
                RedirectToMainMenu();
            }
        }

        private void TbRequestedAmountTextChanged(object sender, TextChangedEventArgs e)
        {
            string newAmount = TbRequestedAmount.Text;
            bool isWritingNewAmmount = newAmount != lastRequestedAmount;

            if (isWritingNewAmmount)
            {
                if (newAmount != string.Empty && !DataValidator.IsValidMoneyAmount(newAmount))
                {
                    TbRequestedAmount.Text = lastRequestedAmount;
                    TbRequestedAmount.CaretIndex = TbRequestedAmount.Text.Length;
                }
                else
                {
                    lastRequestedAmount = newAmount;
                }
            }
        }

        private void TbMinimumAcceptedAmountTextChanged(object sender, TextChangedEventArgs e)
        {
            string newAmount = TbMinimumAcceptedAmount.Text;
            bool isWritingNewAmmount = newAmount != lastMinimumAcceptedAmount;

            if (isWritingNewAmmount)
            {
                if (newAmount != string.Empty && !DataValidator.IsValidMoneyAmount(newAmount))
                {
                    TbMinimumAcceptedAmount.Text = lastMinimumAcceptedAmount;
                    TbMinimumAcceptedAmount.CaretIndex = TbMinimumAcceptedAmount.Text.Length;
                }
                else
                {
                    lastMinimumAcceptedAmount = newAmount;
                }
            }
        }

        private void BtnGenerateCreditApplicationClick(object sender, RoutedEventArgs e)
        {
            HideFieldsErrors();

            bool isValidApplication = VerifyCreditApplicationInformation();
            if(isValidApplication)
            {
                bool areValidRequestedAmounts = VerifyRequestedAmounts();

                if(areValidRequestedAmounts)
                {
                    newApplication.DigitalDocuments = attachedDocuments.ToArray();
                    newApplication.RequestedAmount = decimal.Parse(TbRequestedAmount.Text);
                    newApplication.MinimumAmountAccepted = decimal.Parse(TbMinimumAcceptedAmount.Text);
                    newApplication.Purpose = TbCreditPurpose.Text.Trim();

                    ShowApplicationGenerationConfirmationDialog();
                }
                else
                {
                    HighlightAmountFields();
                    ShowInvalidAmountsAlertDialog();
                }
            }
            else
            {
                HighlightInvalidFields();
                ShowInvalidFieldsAlertDialog();
            }
        }

        private void HideFieldsErrors()
        {
            Style comboBoxDefaultStyle = (Style)FindResource("ComboBox");
            Style textInputDefaultStyle = (Style)FindResource("TextInput");
            Style fileUploaderDefaultStyle = (Style)FindResource("FileUploader");

            CbCreditTypes.Style = comboBoxDefaultStyle;
            TbRequestedAmount.Style = textInputDefaultStyle;
            TbMinimumAcceptedAmount.Style = textInputDefaultStyle;
            TbCreditPurpose.Style = textInputDefaultStyle;
            BtnAttachIne.Style = fileUploaderDefaultStyle;
            BtnAttachProofOfAddress.Style = fileUploaderDefaultStyle;
            BtnAttachProofOfIncome.Style = fileUploaderDefaultStyle;
            SpnCreditConditionLabel.Foreground = Brushes.Black;
        }

        private bool VerifyCreditApplicationInformation()
        {
            bool isValidApplication = true;

            if(isValidApplication)
            {
                isValidApplication = newApplication.CreditType != null;
            }

            if (isValidApplication)
            {
                isValidApplication = TbRequestedAmount.Text != "";
            }

            if (isValidApplication)
            {
                isValidApplication = TbMinimumAcceptedAmount.Text != "";
            }

            if (isValidApplication)
            {
                isValidApplication = TbCreditPurpose.Text.Trim() != "";
            }

            if (isValidApplication)
            {
                isValidApplication = attachedDocuments.Count == 3;
            }

            if (isValidApplication)
            {
                isValidApplication = newApplication.CreditCondition != null;
            }

            return isValidApplication;
        }

        private bool VerifyRequestedAmounts()
        {
            return decimal.TryParse(TbRequestedAmount.Text, out decimal requestedAmount)
                && decimal.TryParse(TbMinimumAcceptedAmount.Text, out decimal minimumAcceptedAmount)
                && minimumAcceptedAmount <= requestedAmount;
        }

        private void HighlightAmountFields()
        {
            Style textInputErrorStyle = (Style)FindResource("TextInputError");

            TbRequestedAmount.Style = textInputErrorStyle;
            TbMinimumAcceptedAmount.Style = textInputErrorStyle;
        }

        private void ShowInvalidAmountsAlertDialog()
        {
            MessageBox.Show(
                "Por favor verifique que el monto mínimo aceptado sea menor al monto solicitado",
                "Montos inválidos",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
        }

        private void HighlightInvalidFields()
        {
            Style comboBoxErrorStyle = (Style)FindResource("ComboBoxError");
            Style textInputErrorStyle = (Style)FindResource("TextInputError");
            Style fileUploadertErrorStyle = (Style)FindResource("FileUploaderError");

            if (newApplication.CreditType == null)
            {
                CbCreditTypes.Style = comboBoxErrorStyle;
            }

            if (TbRequestedAmount.Text == "")
            {
                TbRequestedAmount.Style = textInputErrorStyle;
            }

            if (TbMinimumAcceptedAmount.Text == "")
            {
                TbMinimumAcceptedAmount.Style = textInputErrorStyle;
            }

            if (TbCreditPurpose.Text.Trim() == "")
            {
                TbCreditPurpose.Style = textInputErrorStyle;
            }

            if (!attachedDocuments.Any(document => document.Name == FileToolkit.GenerateDefaultIneName(newApplication.Client)))
            {
                BtnAttachIne.Style = fileUploadertErrorStyle;
            }

            if (!attachedDocuments.Any(document => document.Name == FileToolkit.GenerateDefaultAddressDocumentName(newApplication.Client)))
            {
                BtnAttachProofOfAddress.Style = fileUploadertErrorStyle;
            }

            if (!attachedDocuments.Any(document => document.Name == FileToolkit.GenerateDefaultIncomeDocumentName(newApplication.Client)))
            {
                BtnAttachProofOfIncome.Style = fileUploadertErrorStyle;
            }

            if (newApplication.CreditCondition == null)
            {
                SpnCreditConditionLabel.Foreground = (SolidColorBrush)FindResource("Red");
            }
        }

        private void ShowInvalidFieldsAlertDialog()
        {
            MessageBox.Show(
                "Por favor ingrese información en los campos marcados con rojo",
                "Campos inválidos",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
        }

        private void ShowApplicationGenerationConfirmationDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                $"¿Desea generar la solicitud de crédito por {newApplication.RequestedAmount}MXN, " +
                $"bajo las condiciones de crédito {newApplication.CreditCondition.Identifier}, " +
                $"para el cliente {newApplication.Client.Name} {newApplication.Client.Surname} {newApplication.Client.LastName}?",
                "Confirme el registro de la solicitud",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (buttonClicked == MessageBoxResult.Yes)
            {
                RegisterCreditApplication();
            }
        }

        private void RegisterCreditApplication()
        {
            CreditsServiceClient creditsService = new CreditsServiceClient();

            try
            {
                creditsService.RegisterCreditApplication(newApplication);
                ShowSuccessApplicationRegistrationDialog();
            }
            catch (FaultException<ServiceFault> fault)
            {
                ShowErrorRegisteringCreditApplicationDialog(fault.Detail.Message);
            }
            catch (EndpointNotFoundException)
            {
                string errorMessage = "El servidor no se encuentra disponible, intente más tarde";
                ShowErrorRegisteringCreditApplicationDialog(errorMessage);
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex.Message);
                string errorMessage = "No fue posible guardar la información debido a un error de conexión";
                ShowErrorRegisteringCreditApplicationDialog(errorMessage);
            }
        }

        private void ShowErrorRegisteringCreditApplicationDialog(string message)
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                message,
                "Error al registrar la solicitud",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );

            if (buttonClicked == MessageBoxResult.OK)
            {
                RedirectToMainMenu();
            }
        }

        private void ShowSuccessApplicationRegistrationDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                "La información de la solicitud de crédito ha sido guardada correctamente",
                "Solicitud de crédito registrada",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            if (buttonClicked == MessageBoxResult.OK)
            {
                RedirectToMainMenu();
            }
        }

        private void BtnAttachIneClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Filter = "Documentos pdf|*.pdf;"
            };

            if (fileDialog.ShowDialog() == true)
            {
                string inePath = fileDialog.FileName;
                byte[] ineContent = FileToolkit.GetFileContent(inePath);

                if(ineContent == null)
                {
                    ShowFileOutsizedWarningDialog();
                }
                else
                {
                    DigitalDocument ineFile = new DigitalDocument();
                    ineFile.Content = ineContent;
                    ineFile.Name = FileToolkit.GenerateDefaultIneName(newApplication.Client);
                    ineFile.Format = FileToolkit.INE;

                    BtnAttachIne.Visibility = Visibility.Hidden;
                    BtnDetachIne.Content = ineFile.Name;
                    BtnDetachIne.Visibility = Visibility.Visible;
                    attachedDocuments.Add(ineFile);
                }
            }
        }

        private void BtnDetachIneClick(object sender, RoutedEventArgs e)
        {
            attachedDocuments.RemoveAll(
                digitalDocument => digitalDocument.Name == FileToolkit.GenerateDefaultIneName(newApplication.Client)
            );

            BtnAttachIne.Visibility = Visibility.Visible;
            BtnDetachIne.Visibility = Visibility.Hidden;
        }

        private void BtnAttachProofOfAddressClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Filter = "Documentos pdf|*.pdf;"
            };

            if (fileDialog.ShowDialog() == true)
            {
                string proofOfAddressPath = fileDialog.FileName;
                byte[] proofOfAddressContent = FileToolkit.GetFileContent(proofOfAddressPath);

                if (proofOfAddressContent == null)
                {
                    ShowFileOutsizedWarningDialog();
                }
                else
                {
                    DigitalDocument proofOfAddressFile = new DigitalDocument();
                    proofOfAddressFile.Content = proofOfAddressContent;
                    proofOfAddressFile.Name = FileToolkit.GenerateDefaultAddressDocumentName(newApplication.Client);
                    proofOfAddressFile.Format = FileToolkit.PROOF_ADDRESS;

                    BtnAttachProofOfAddress.Visibility = Visibility.Hidden;
                    BtnDetachProofOfAddress.Content = proofOfAddressFile.Name;
                    BtnDetachProofOfAddress.Visibility = Visibility.Visible;
                    attachedDocuments.Add(proofOfAddressFile);
                }
            }
        }

        private void BtnDetachProofOfAddressClick(object sender, RoutedEventArgs e)
        {
            attachedDocuments.RemoveAll(
                digitalDocument => digitalDocument.Name == FileToolkit.GenerateDefaultAddressDocumentName(newApplication.Client)
            );

            BtnAttachProofOfAddress.Visibility = Visibility.Visible;
            BtnDetachProofOfAddress.Visibility = Visibility.Hidden;
        }

        private void BtnAttachProofOfIncomeClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Filter = "Documentos pdf|*.pdf;"
            };

            if (fileDialog.ShowDialog() == true)
            {
                string proofOfIncomePath = fileDialog.FileName;
                byte[] proofOfIncomeContent = FileToolkit.GetFileContent(proofOfIncomePath);

                if (proofOfIncomeContent == null)
                {
                    ShowFileOutsizedWarningDialog();
                }
                else
                {
                    DigitalDocument proofOfIncomeFile = new DigitalDocument();
                    proofOfIncomeFile.Content = proofOfIncomeContent;
                    proofOfIncomeFile.Name = FileToolkit.GenerateDefaultIncomeDocumentName(newApplication.Client);
                    proofOfIncomeFile.Format = FileToolkit.PROOF_INCOME;

                    BtnAttachProofOfIncome.Visibility = Visibility.Hidden;
                    BtnDetachProofOfIncome.Content = proofOfIncomeFile.Name;
                    BtnDetachProofOfIncome.Visibility = Visibility.Visible;
                    attachedDocuments.Add(proofOfIncomeFile);
                }
            }
        }

        private void BtnDetachProofOfIncomeClick(object sender, RoutedEventArgs e)
        {
            attachedDocuments.RemoveAll(
                digitalDocument => digitalDocument.Name == FileToolkit.GenerateDefaultIncomeDocumentName(newApplication.Client)
            );

            BtnAttachProofOfIncome.Visibility = Visibility.Visible;
            BtnDetachProofOfIncome.Visibility = Visibility.Hidden;
        }

        private void ShowFileOutsizedWarningDialog()
        {
            MessageBox.Show(
                "El archivo que intenta adjuntar es demasiado pesado, por favor " +
                "opte por una versión más ligera (150kb o menos).",
                "Archivo muy grande",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
        }

        private void BtnDiscardApplicationClick(object sender, RoutedEventArgs e)
        {
            ShowDiscardApplicationQuestionDialog();
        }

        private void ShowDiscardApplicationQuestionDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                "Se perderá toda la información ingresada, ¿esta seguro que desea descartar la solicitud de crédito?",
                "Confirmar descartado de solicitid",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (buttonClicked == MessageBoxResult.Yes)
            {
                RedirectToMainMenu();
            }
        }
    }
}
