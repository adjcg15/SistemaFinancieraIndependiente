using SFIClient.Controlls;
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

namespace SFIClient.Views
{
    /// <summary>
    /// Lógica de interacción para ConsultConditionsCreditView.xaml
    /// </summary>
    public partial class ConsultConditionsCreditView : Page
    {
        private readonly CreditConditionsServiceClient creditServiceClient = new CreditConditionsServiceClient();
        private List<CreditCondition> creditConditions = new List<CreditCondition>();

        public ConsultConditionsCreditView()
        {
            InitializeComponent();
            LoadCreditConditions();
        }

        private void LoadCreditConditions()
        {
            try
            {
                creditConditions = creditServiceClient.RecoverAllCreditConditions().ToList();
                if (creditConditions.Count != 0)
                {
                    AddCreditConditionsToUI(creditConditions);
                }
                else
                {
                    SkpRegisterCreditCondition.Visibility = Visibility.Collapsed;
                    SkpNoRegisteredCreditConditions.Visibility = Visibility.Visible;
                }
            }
            catch (FaultException fe)
            {
                MessageBox.Show(fe.Message);
            }
            catch (EndpointNotFoundException)
            {
                MessageBox.Show("No fue posible establecer la conexión con el servicio, intente más tarde");
                // TODO: Redirigir al menú principal
            }
            catch (CommunicationException)
            {
                MessageBox.Show("No fue posible establecer la conexión con el servicio, intente más tarde");
                // TODO: Redirigir al menú principal
            }
        }
        private void ShowClientWithAllOptions(Client client)
        {
            ClientControll clientControll = new ClientControll();
            clientControll.LblClientRFC.Content = client.Rfc;
            clientControll.LblClientName.Content = client.LastName + " " + client.Surname + " " + client.Name;
            clientControll.LblClientCreditStatus.Content = "Cliente sin crédito y sin solicitud activa";
            ItcClients.Items.Add(clientControll);
        }
        private void ShowCreditCondition(CreditCondition creditCondition)
        {
            CreditConditionControl creditConditionControl = new CreditConditionControl();
            creditConditionControl.LblApplyIVA
        }

        private void AddCreditConditionsToUI(List<CreditCondition> creditConditions)
        {
            ItcCreditCondition.Items.Clear();
            foreach (var condition in creditConditions)
            {
                YourNamespace.CreditConditionCard conditionCard = new YourNamespace.CreditConditionCard();
                // Asignar los datos de la condición de crédito a los elementos de la tarjeta
                conditionCard.TbkIdentifier.Text = condition.Identifier;
                conditionCard.TbkIva.Text = condition.IsIvaApplied ? "Aplica IVA" : "No aplica IVA";
                conditionCard.TbkIsActive.Text = condition.IsActive ? "Activa" : "Inactiva";
                conditionCard.SpnPaymentMonths.Text = condition.PaymentMonths.ToString();
                conditionCard.SpnInterestRate.Text = condition.InterestRate.ToString();
                conditionCard.BldInterestOnArrears.Text = condition.InterestOnArrears.ToString();
                conditionCard.BldAdvancePaymentReduction.Text = condition.AdvancePaymentReduction.ToString();
                // Agregar la tarjeta al contenedor de condiciones de crédito
                ItcCreditCondition.Items.Add(conditionCard);
            }
        }
    }
}
