using Microsoft.Win32;
using SFIClient.Controlls;
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
        private Client requestingClient;
        private string lastRequestedAmount = string.Empty;
        private string lastMinimumAcceptedAmount = string.Empty;
        private readonly CreditApplication newApplication = new CreditApplication();
        private readonly List<DigitalDocument> attachedDocuments = new List<DigitalDocument>();

        public CredditApplicationController()
        {
            InitializeComponent();

            LoadCreditTypes();
            ShowClientInformation();
        }

        public CredditApplicationController(Client requestingClient)
        {
            InitializeComponent();

            this.requestingClient = requestingClient;

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
            //TODO: redireccionar cuando exista el menú principal
            Console.WriteLine("Redireccionando al menú principal...");
        }

        private void ShowClientInformation()
        {
            //TODO: remover asignación del ciente
            requestingClient = new Client();
            requestingClient.Name = "Ángel";
            requestingClient.Surname = "De la cruz";
            requestingClient.LastName = "García";
            requestingClient.Curp = "CUGA010415HVZRRNA2";
            requestingClient.Rfc = "MALJ800515K1A";

            TbkClientName.Text = $"{requestingClient.Name} {requestingClient.Surname} {requestingClient.LastName}";
            SpnClientCurp.Inlines.Add(new Run(requestingClient.Curp));
            //BldClientSalary.Inlines.Add(new Run(requestingClient.WorkCenter.Salary));
            //SpnClientWorkCenterName.Inlines.Add(new Run(requestingClient.WorkCenter.companyName));
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
                if (newAmount != string.Empty && !IsValidMoneyAmount(newAmount))
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
                if (newAmount != string.Empty && !IsValidMoneyAmount(newAmount))
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

        //TODO: ver si se puede crear un helper
        private bool IsValidMoneyAmount(string amount)
        {
            return amount.Length <= 7 
                && double.TryParse(amount, out double result) 
                && result >= 0 
                && result <= 1000000;
        }

        private void BtnGenerateCreditApplicationClick(object sender, RoutedEventArgs e)
        {
            bool isValidApplication = VerifyCreditApplicationInformation();

            Console.WriteLine($"Hay {attachedDocuments.Count} documentos adjuntos");
            if(isValidApplication)
            {
                ShowApplicationGenerationConfirmationDialog();
            }
        }

        private bool VerifyCreditApplicationInformation()
        {
            return true;
        }

        private void ShowApplicationGenerationConfirmationDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                $"“¿Desea generar la solicitud de crédito por cantidad aspiradaMXN, " +
                $"bajo las condiciones de crédito identificador de condición de crédito, " +
                $"para el cliente nombre del cliente?",
                "Confirme el registro de la solicitud",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (buttonClicked == MessageBoxResult.Yes)
            {
                
            }
            else if (buttonClicked == MessageBoxResult.No)
            {

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
                    ineFile.Name = FileToolkit.GenerateDefaultIneName(requestingClient);
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
                digitalDocument => digitalDocument.Name == FileToolkit.GenerateDefaultIneName(requestingClient)
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
                    proofOfAddressFile.Name = FileToolkit.GenerateDefaultAddressDocumentName(requestingClient);
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
                digitalDocument => digitalDocument.Name == FileToolkit.GenerateDefaultAddressDocumentName(requestingClient)
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
                    proofOfIncomeFile.Name = FileToolkit.GenerateDefaultIncomeDocumentName(requestingClient);
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
                digitalDocument => digitalDocument.Name == FileToolkit.GenerateDefaultIncomeDocumentName(requestingClient)
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
    }
}
