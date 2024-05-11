using LiveCharts;
using LiveCharts.Definitions.Charts;
using LiveCharts.Wpf;
using SFIClient.Controlls;
using SFIClient.SFIServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
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
    /// Lógica de interacción para OverallCollectionEfficiency.xaml
    /// </summary>
    public partial class OverallCollectionEfficiencyController : Page
    {
        private readonly Credit[] creditsWithPaymentsList;
        private readonly CreditsServiceClient creditsServiceClient = new CreditsServiceClient();
        public OverallCollectionEfficiencyController()
        {
            InitializeComponent();
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            DateTime dateTimeNow = DateTime.Now;
            int currentMonth = dateTimeNow.Month;
            int currentYear = dateTimeNow.Year;
            LoadCraditsWithPaymentsInTheMonthAndYear(currentMonth, currentYear);
        }

        private void LoadCraditsWithPaymentsInTheMonthAndYear(int currentMonth, int currentYear)
        {

            try
            {
                //creditsWithPaymentsList = creditsServiceClient.RecoverCreditsWithPaymentsInTheMonthAndYear(currentMonth, currentYear);
                if (creditsWithPaymentsList.Length != 0)
                {
                    CalculateAndShowTheAmountOfCredits();
                    CalculateAndShowTheAmountPaid();
                    AddInformationToTheCreditTableWithPyments();
                    ShowCreditTableWithUnsettledPayments();
                }
                else
                {
                    ShowNoCreditsWithPaymentsExistsMessage();
                }
            }
            catch (FaultException<ServiceFault> fe)
            {
                ShowErrorRecoveringCreditsWithPayments(fe.Detail.Message);
                RediredtToLoginView();
            }
            catch (EndpointNotFoundException)
            {
                string errorMessage = "No fue posible generar la eficiencia de cobro para el mes solicitado, por favor inténtelo más tarde";
                ShowErrorRecoveringCreditsWithPayments(errorMessage);
                RediredtToLoginView();
            }
            catch (CommunicationException)
            {
                string errorMessage = "No fue posible generar la eficiencia de cobro para el mes solicitado, por favor inténtelo más tarde";
                ShowErrorRecoveringCreditsWithPayments(errorMessage);
                RediredtToLoginView();
            }
        }

        private void ShowNoCreditsWithPaymentsExistsMessage()
        {
            GrdCollectionEfficiencyInformation.Visibility = Visibility.Collapsed;
            SkpCreditsPaymentsTable.Visibility = Visibility.Collapsed;
            SkpUnpaidCredits.Visibility = Visibility.Collapsed;
            SkpPaidCredits.Visibility = Visibility.Collapsed;
        }

        private void ShowErrorRecoveringCreditsWithPayments(string message)
        {
            MessageBox.Show(
                message,
                "Sistema no disponible",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }

        private void RediredtToLoginView()
        {
            NavigationService.Navigate(new LoginController());
        }

        private void CalculateAndShowTheAmountOfCredits()
        {
            int creditsPaid = 0;
            int creditsDoesNotPaid = 0;
            int allCredits;

            allCredits = creditsWithPaymentsList.Length;

            foreach (var credit in creditsWithPaymentsList)
            {
                if (credit.Payments[0].reconciliation_date != null)
                {
                    creditsPaid += 1;
                }
                else
                {
                    creditsDoesNotPaid += 1;
                }
            }

            TbkAllCredits.Text = allCredits.ToString();
            TbkCreditsPaid.Text = creditsPaid.ToString();

            ShowCreditsOnChart(creditsPaid, creditsDoesNotPaid);
        }

        private void ShowCreditsOnChart(int creditsPaid, int creditsDoesNotPaid)
        {
            SeriesCollection pieSeries = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Créditos que realizaron pago",
                    Values = new ChartValues<double> { creditsPaid },
                    Fill = (SolidColorBrush)FindResource("PrimaryColor"),
                    DataLabels = true
                },

                new PieSeries
                {
                    Title = "Créditos que no realizaron pago",
                    Values = new ChartValues<double> { creditsDoesNotPaid },
                    Fill = (SolidColorBrush)FindResource("DarkPrimaryColor"),
                    DataLabels = true
                }
            };

            pcCredits.Series = pieSeries;
        }

        private void CalculateAndShowTheAmountPaid()
        {
            double totalAmount = 0;
            double amountPaid = 0;
            double amountDoesNotPaid = 0;
            double collectionEfficiencyPercentage;

            foreach (var credit in creditsWithPaymentsList)
            {
                totalAmount += credit.Payments[0].amount;
                if (credit.Payments[0].reconciliation_date != null)
                {
                    amountPaid += credit.Payments[0].amount;
                }
                else
                {
                    amountDoesNotPaid += credit.Payments[0].amount;
                }
            }
            collectionEfficiencyPercentage = (amountPaid / totalAmount) * 100;

            TbkTotalAmount.Text = totalAmount.ToString();
            TbkAmountPaid.Text = amountPaid.ToString();
            TbkCollectionEfficiencyPercentage.Text = collectionEfficiencyPercentage.ToString();

            ShowAmountPaidOnChart(amountPaid, amountDoesNotPaid);

        }

        private void ShowAmountPaidOnChart(double amountPaid, double amountDoesNotPaid)
        {
            SeriesCollection pieSeries = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Créditos que realizaron pago",
                    Values = new ChartValues<double> { amountPaid },
                    Fill = (SolidColorBrush)FindResource("PrimaryColor"),
                    DataLabels = true
                },

                new PieSeries
                {
                    Title = "Créditos que no realizaron pago",
                    Values = new ChartValues<double> { amountDoesNotPaid },
                    Fill = (SolidColorBrush)FindResource("DarkPrimaryColor"),
                    DataLabels = true
                }
            };

            pcAmounts.Series = pieSeries;
        }

        private void AddInformationToTheCreditTableWithPyments()
        {
            foreach (var credit in creditsWithPaymentsList)
            {
                CreditPaymentControl creditPaymentControl = new CreditPaymentControl();
                creditPaymentControl.TbkCreditInvoice.Text = credit.Invoice;
                creditPaymentControl.TbkCreditGranted.Text = credit.AmountApproved.ToString();
                creditPaymentControl.TbkPaidAmount.Text = credit.Payments[0].amount.ToString();
                if (credit.Payments[0].reconciliation_date != null)
                {
                    creditPaymentControl.TbkDate.Text = Utilities.DateToolkit.FormatAsDDMMYYY(credit.Payments[0].reconciliation_date.Value);
                    SkpPaidCredits.Children.Add(creditPaymentControl);
                }
                else
                {
                    creditPaymentControl.TbkDate.Text = Utilities.DateToolkit.FormatAsDDMMYYY(credit.Payments[0].planned_date);
                    SkpUnpaidCredits.Children.Add(creditPaymentControl);
                }
            }
        }

        private void ShowCreditTableWithUnsettledPayments()
        {
            TbkPaymentDate.Text = "Fecha planeada";
            SkpPaidCredits.Visibility = Visibility.Collapsed;
            SkpUnpaidCredits.Visibility = Visibility.Visible;
        }

        private void BtnGenerateEfficiencyClick(object sender, RoutedEventArgs e)
        {
            int selectedMonth = CbMonth.SelectedIndex;
            int selectedYear = CbYear.SelectedIndex;
            LoadCraditsWithPaymentsInTheMonthAndYear(selectedMonth, selectedYear);
        }

        private void BtnRedirectToMainMenuClick(object sender, RoutedEventArgs e)
        {
            RedirectToMainMenu();
        }

        private void RedirectToMainMenu()
        {
            NavigationService.Navigate(new MainMenuController());
        }

        private void ShowCreditTableWithSettledPayments()
        {
            TbkPaymentDate.Text = "Fecha de pago";
            SkpUnpaidCredits.Visibility = Visibility.Collapsed;
            SkpPaidCredits.Visibility = Visibility.Visible;
        }

        private void RbUnsettledPaymentsChecked(object sender, RoutedEventArgs e)
        {
            ShowCreditTableWithUnsettledPayments();
        }

        private void RbSettledPaymentsChecked(object sender, RoutedEventArgs e)
        {
            ShowCreditTableWithSettledPayments();
        }
    }
}
