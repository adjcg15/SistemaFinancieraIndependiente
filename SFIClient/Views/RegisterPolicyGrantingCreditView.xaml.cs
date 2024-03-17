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

namespace SFIClient.Views
{
    /// <summary>
    /// Lógica de interacción para RegisterPolicyGrantingCreditView.xaml
    /// </summary>
    public partial class RegisterPolicyGrantingCreditView : Page
    {
        public RegisterPolicyGrantingCreditView()
        {
            InitializeComponent();
        }

        private bool ValidateFields()
        {
            List<string> emptyFields = new List<string>();

            if (string.IsNullOrEmpty(TxtCreditType.Text))
            {
                emptyFields.Add("Nombre");
            }

            if (!RbActivePolicy.IsChecked.HasValue || !RbInactivePolicy.IsChecked.HasValue ||
                (!RbActivePolicy.IsChecked.Value && !RbInactivePolicy.IsChecked.Value))
            {
                emptyFields.Add("Estado");
            }

            if (!DpkEffectiveDate.SelectedDate.HasValue)
            {
                emptyFields.Add("Fecha de vigencia");
            }

            if (string.IsNullOrEmpty(TbPolicyDescription.Text))
            {
                emptyFields.Add("Descripción");
            }

            if (emptyFields.Any())
            {
                string emptyFieldsMessage = "Por favor, verifique que los siguientes campos requeridos cuenten con una respuesta:\n";
                emptyFieldsMessage += string.Join("\n", emptyFields);
                MessageBox.Show(emptyFieldsMessage, "Campos requeridos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }


        private void ClickBtnAcceptRegister(object sender, RoutedEventArgs e)
        {
            if (ValidateFields())
            {
                MessageBox.Show("Guardado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
