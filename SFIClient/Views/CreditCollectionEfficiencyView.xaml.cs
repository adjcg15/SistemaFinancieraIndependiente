using LiveCharts.Wpf;
using LiveCharts;
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
    public partial class CreditCollectionEfficiencyController : Page
    {
        private readonly string creditInvoice;
        private Credit credit;

        public CreditCollectionEfficiencyController(string creditInvoice)
        {
            InitializeComponent();

            this.creditInvoice = creditInvoice;
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            RecoverCreditInformation();
        }

        private void RecoverCreditInformation()
        {
            CreditsServiceClient creditsService = new CreditsServiceClient();

            try
            {
                Credit recoveredCredit = creditsService.GetCreditForCollectionEfficiency(creditInvoice);
                credit = recoveredCredit;

                ShowCreditInformation();
                ShowClientInformation();
                ShowEfficiencyInformation();
                ShowEfficiencyOnChart();
            }
            catch (FaultException<ServiceFault> fault)
            {
                ShowErrorRecoveringCreditDialog(fault.Detail.Message);
            }
            catch (EndpointNotFoundException)
            {
                string errorMessage = "Servidor no disponible. No fue posible recuperar la información " +
                    "del cliente, por favor inténtelo más tarde";
                ShowErrorRecoveringCreditDialog(errorMessage);
            }
            catch (CommunicationException)
            {
                string errorMessage = "Error de conexión. Ocurrió un error de conexión, por favor compruebe " +
                    "su conexión de Internet e intente de nuevo";
                ShowErrorRecoveringCreditDialog(errorMessage);
            }
        }

        private void ShowCreditInformation()
        {
            TbkCreditInvoice.Text = credit.Invoice;
            TbkCreditAmountApproved.Text = credit.AmountApproved.ToString("C", new System.Globalization.CultureInfo("es-MX"));
        }

        private void ShowClientInformation()
        {
            Client client = credit.Client;
            TbkClientName.Text = client.Name + " " + client.LastName + " " + client.Surname;
            TbkClientRFC.Text = client.Rfc;
        }

        private void ShowEfficiencyInformation()
        {
            List<Payment> payments = credit.Payments.ToList();
            int totalPayments = payments.Count;

            int totalPaidPayments = payments
                .Where(payment => payment.reconciliation_date.HasValue)
                .ToList()
                .Count;

            List<Payment> paymentsPlannedToDate = payments
                .Where(payment => payment.planned_date.Date.CompareTo(DateTime.Now.Date) <= 0)
                .ToList();
            int totalPaymentPlannedToDate = paymentsPlannedToDate.Count;
            int totalPaidPaymentsToDate = paymentsPlannedToDate
                .Where(payment => payment.reconciliation_date.HasValue)
                .ToList()
                .Count;

            TbkTotalPayments.Text = totalPayments.ToString();
            TbkTotalPaymentPlannedToDate.Text = totalPaymentPlannedToDate.ToString();
            TbkTotalPaidPayments.Text = totalPaidPayments.ToString();

            if (totalPaymentPlannedToDate > 0) {
                double efficiencyPercentage = ((double)totalPaidPaymentsToDate / (double)totalPaymentPlannedToDate) * 100.0;
                TbkEfficiency.Text = efficiencyPercentage.ToString("0.0") + "%";
            } else
            {
                TbkEfficiency.Text = "No aplica";
            }
        }

        private void ShowErrorRecoveringCreditDialog(string message)
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                message,
                "No fue posible recuperar la información del crédito",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );

            if (buttonClicked == MessageBoxResult.OK)
            {
                RediectToLogin();
            }
        }

        private void ShowEfficiencyOnChart()
        {
            List<Payment> payments = credit.Payments.ToList();
            int paidPayments = 0,
                advancedPayments = 0,
                pendingPayments = 0,
                latePayments = 0,
                unpdaidPayments = 0;

            foreach (var payment in payments)
            {
                if(payment.planned_date.Date.CompareTo(DateTime.Now.Date) > 0)
                {
                    if (!payment.reconciliation_date.HasValue)
                    {
                        pendingPayments++;
                    }
                    else
                    {
                        advancedPayments++;
                    }
                } 
                else
                {
                    if(!payment.reconciliation_date.HasValue)
                    {
                        unpdaidPayments++;
                    }
                    else
                    {
                        DateTime reconciliationDate = payment.reconciliation_date.Value;
                        DateTime plannedDate = payment.planned_date;

                        int comparisonResult = reconciliationDate.Date.CompareTo(plannedDate.Date);
                        if (comparisonResult > 0)
                        {
                            latePayments++;
                        }
                        else if (comparisonResult < 0)
                        {
                            advancedPayments++;
                        }
                        else
                        {
                            paidPayments++;
                        }
                    }
                }
            }

            SeriesCollection pieSeries = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Pagos realizados en tiempo",
                    Values = new ChartValues<double> { paidPayments },
                    Fill = (SolidColorBrush)FindResource("PrimaryColor"),
                    DataLabels = true
                },

                new PieSeries
                {
                    Title = "Pagos tardíos",
                    Values = new ChartValues<double> { latePayments },
                    Fill = (SolidColorBrush)FindResource("DarkPrimaryColor"),
                    DataLabels = true
                },

                new PieSeries
                {
                    Title = "Pagos pendientes",
                    Values = new ChartValues<double> { pendingPayments },
                    Fill = (SolidColorBrush)FindResource("Gray"),
                    DataLabels = true
                },

                new PieSeries
                {
                    Title = "Pagos adelantados",
                    Values = new ChartValues<double> { advancedPayments },
                    Fill = (SolidColorBrush)FindResource("LightGreen"),
                    DataLabels = true
                },

                new PieSeries
                {
                    Title = "Pagos no realizados",
                    Values = new ChartValues<double> { unpdaidPayments },
                    Fill = (SolidColorBrush)FindResource("LightRed"),
                    DataLabels = true
                }
            };

            pcCreditEfficiency.Series = pieSeries;
        }

        private void RediectToLogin()
        {
            NavigationService.Navigate(new LoginController());
        }

        private void BtnReturnToPreviousPageClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CreditsListController());
        }
    }
}
