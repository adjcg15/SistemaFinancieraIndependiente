using SFIClient.SFIServices;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MessageBox = System.Windows.MessageBox;

namespace SFIClient.Views
{
    /// <summary>
    /// Lógica de interacción para ModifyCreditConditionApplicableToCreditView.xaml
    /// </summary>
    public partial class ModifyCreditConditionApplicableToCreditController : Page
    {
        private Credit credit;
        public ModifyCreditConditionApplicableToCreditController(Credit credit)
        {
            InitializeComponent();
            this.credit = credit;
            //TODO recuperar salario del cliente, actualmente manda excepcion
            //this.clientSalary = clientSalary;
            ShowCreditAndClientInformation();
        }
        private void ShowCreditAndClientInformation()
        {
            TbkCreditInvoice.Text = credit.Invoice;
            TbkClientName.Inlines.Clear();
            TbkClientName.Inlines.Add(new Run($"{credit.Client.Name} {credit.Client.Surname} {credit.Client.LastName}"));

            BldApprovedAmountCredit.Inlines.Clear();
            BldApprovedAmountCredit.Inlines.Add(new Run($"{credit.AmountApproved} MXN"));

            SpnClientSalaryAmmount.Inlines.Clear();
            //SpnClientSalaryAmmount.Inlines.Add(new Run($"{clientSalary} MXN"));
        }
        private void ShowReturnPreviousPageConfirmationDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                $"¿Está seguro de que desea regresar a la lista de créditos? Todos los cambios sin guardar se perderán",
                "Regresar a ventana previa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );
            if (buttonClicked == MessageBoxResult.Yes)
            {
                RedirectToConsultCreditsList();
            }
        }
        private void ShowDiscardChangesConfirmationDialog()
        {
            MessageBoxResult buttonClicked = MessageBox.Show(
                $"¿Está seguro de que desea cancelar modificacion de la condición de crédito aplicable a crédito? " +
                $"Todos los cambios sin guardar se perderán",
                "Descartar cambios",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );
            if (buttonClicked == MessageBoxResult.Yes)
            {
                RedirectToConsultCreditsList();
            }
        }
        private void RedirectToConsultCreditsList()
        {
            CreditsListController creditList = new CreditsListController();
            this.NavigationService.Navigate(creditList);
            NavigationService.RemoveBackEntry();
        }
        private void BtnSaveChangesClick(object sender, RoutedEventArgs e)
        {

        }
        private void BtnCancelChangesClick(object sender, RoutedEventArgs e)
        {
            ShowDiscardChangesConfirmationDialog();
        }

        private void BtnBackPreviousPageClick(object sender, RoutedEventArgs e)
        {
            ShowReturnPreviousPageConfirmationDialog();
        }
    }
}
