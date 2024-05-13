using LiveCharts;
using LiveCharts.Definitions.Charts;
using LiveCharts.Wpf;
using SFIClient.Controlls;
using SFIClient.Session;
using SFIClient.SFIServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private List<Credit> creditsWithPaymentsList = new List<Credit>();
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
            FillYearsCombobox(currentYear);
            FillMonthsCombobox(currentMonth);
            RbUnsettledPayments.IsChecked = true;
            LoadCraditsWithPaymentsInTheMonthAndYear(currentMonth, currentYear);
        }

        private void FillYearsCombobox(int currentYear)
        {
            ObservableCollection<int> years = new ObservableCollection<int>
            {
                2024,
                2023,
                2022,
                2021,
                2020,
                2019,
                2018,
                2017,
                2016,
                2015,
                2014
            };

            CbYear.ItemsSource = years;
            CbYear.SelectedIndex = years.IndexOf(currentYear);
        }

        private void FillMonthsCombobox(int currentMonth)
        {
            ObservableCollection<string> months = new ObservableCollection<string>
            {
                "Enero",
                "Febrero",
                "Marzo",
                "Abril",
                "Mayo",
                "Junio",
                "Julio",
                "Agosto",
                "Septiembre",
                "Octubre",
                "Noviembre",
                "Diciembre"
            };

            CbMonth.ItemsSource = months;
            CbMonth.SelectedIndex = currentMonth - 1;
        }

        private void LoadCraditsWithPaymentsInTheMonthAndYear(int currentMonth, int currentYear)
        {

            try
            {
                creditsWithPaymentsList = creditsServiceClient.RecoverCreditsWithPaymentsInTheMonthAndYear(currentMonth, currentYear).ToList();
                if (creditsWithPaymentsList.Count != 0)
                {
                    TbkAmountPaid.Text = "";
                    TbkTotalAmount.Text = "";
                    SkpPaidCredits.Children.Clear();
                    SkpUnpaidCredits.Children.Clear();
                    GrdCollectionEfficiencyInformation.Visibility = Visibility.Visible;
                    SkpCreditsPaymentsTable.Visibility = Visibility.Visible;
                    SkpUnpaidCredits.Visibility = Visibility.Visible;
                    SkpPaidCredits.Visibility = Visibility.Visible;
                    SkpEmptyCollectionEfficiencies.Visibility = Visibility.Collapsed;
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
            SkpEmptyCollectionEfficiencies.Visibility = Visibility.Visible;
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
            SystemSession.Employee = null;
        }

        private void CalculateAndShowTheAmountOfCredits()
        {
            int creditsPaid = 0;
            int creditsDoesNotPaid = 0;
            int allCredits;

            allCredits = creditsWithPaymentsList.Count;

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

            TbkTotalAmount.Inlines.Add(new Run(totalAmount.ToString("C", new System.Globalization.CultureInfo("es-MX"))));
            TbkAmountPaid.Inlines.Add(new Run(amountPaid.ToString("C", new System.Globalization.CultureInfo("es-MX"))));
            TbkCollectionEfficiencyPercentage.Text = collectionEfficiencyPercentage.ToString() + "%";

            ShowAmountPaidOnChart(amountPaid, amountDoesNotPaid);

        }

        private void ShowAmountPaidOnChart(double amountPaid, double amountDoesNotPaid)
        {
            SeriesCollection pieSeries = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Cantidad pagada",
                    Values = new ChartValues<double> { amountPaid },
                    Fill = (SolidColorBrush)FindResource("PrimaryColor"),
                    DataLabels = true
                },

                new PieSeries
                {
                    Title = "Cantidad pendiente",
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
            ScvOverallCollectionEfficiency.ScrollToBottom();
            TbkPaymentDate.Text = "Fecha planeada";
            SkpPaidCredits.Visibility = Visibility.Collapsed;
            SkpUnpaidCredits.Visibility = Visibility.Visible;
        }

        private void BtnGenerateEfficiencyClick(object sender, RoutedEventArgs e)
        {
            int selectedMonth = CbMonth.SelectedIndex + 1;
            int selectedYear = (int)CbYear.SelectedValue;
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
            ScvOverallCollectionEfficiency.ScrollToBottom();
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
