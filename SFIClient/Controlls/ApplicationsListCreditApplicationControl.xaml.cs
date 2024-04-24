using SFIClient.SFIServices;
using SFIClient.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace SFIClient.Controlls
{
    public partial class ApplicationsListCreditApplicationControl : UserControl
    {
        public CreditApplication BindedCreditApplication { get; }
        public event EventHandler<CreditApplication> WriteDictumClick;

        public ApplicationsListCreditApplicationControl(CreditApplication creditApplication)
        {
            InitializeComponent();
            BindedCreditApplication = creditApplication;

            ShowCreditApplicationInformation();
            if(creditApplication.Dictum != null)
            {
                ShowCreditApplicationDictumInformation();
            }
        }

        private void ShowCreditApplicationInformation()
        {
            SpnExpeditionDate.Inlines.Clear();
            SpnExpeditionDate.Inlines.Add(
                new Run(DateToolkit.FormatAsDateWithHour(BindedCreditApplication.ExpeditionDate))
            );

            SpnRequestedAmount.Inlines.Clear();
            SpnExpeditionDate.Inlines.Add(
                new Run(BindedCreditApplication.RequestedAmount.ToString("C", new System.Globalization.CultureInfo("es-MX")))
            );

            SpnCreditType.Inlines.Clear();
            if(BindedCreditApplication.CreditType != null)
            {
                SpnCreditType.Inlines.Add(
                    new Run(BindedCreditApplication.CreditType.Name)
                );
            }
        }

        private void ShowCreditApplicationDictumInformation()
        {
            BtnGenerateDictum.Visibility = Visibility.Visible;

            Dictum dictum = BindedCreditApplication.Dictum;
            SolidColorBrush primaryColor = (SolidColorBrush)Application.Current.Resources["PrimaryColor"];
            SolidColorBrush gray = (SolidColorBrush)Application.Current.Resources["Gray"];
            if (dictum.IsApproved)
            {
                TbkApprovalDate.Text = "Solicitud aprobada el " 
                    + DateToolkit.FormatAsDateWithHour(dictum.GenerationDate);
                BdrDecorator.BorderBrush = primaryColor;
                BdrDecorator.Background = primaryColor;
            } else
            {
                TbkApprovalDate.Text = "Rechazada el "
                    + DateToolkit.FormatAsDateWithHour(dictum.GenerationDate);
                BdrDecorator.BorderBrush = gray;
                BdrDecorator.Background = gray;
            }
        }

        private void BtnGenerateDictumClick(object sender, RoutedEventArgs e)
        {
            if (BindedCreditApplication.Dictum == null)
            {
                WriteDictumClick?.Invoke(this, BindedCreditApplication);
            }
        }
    }
}
