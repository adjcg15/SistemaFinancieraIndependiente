using SFIClient.Controlls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SFIClient.Views
{
    public partial class CreditGrantingPolicyListController : Page
    {
        public CreditGrantingPolicyListController()
        {
            InitializeComponent();

            SizeChanged += CreditGrantingPolicyListViewSizeChanged;
        }

        private void CreditGrantingPolicyListViewSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Page page = (Page)sender;
            double pageWidth = page.ActualWidth;

            ItemsPanelTemplate grantingPoliciesPanelTemplate = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(UniformGrid)));
            int totalPanelColumns = 1;
            if(pageWidth > 800)
            {
                totalPanelColumns = 2;
            }
            
            if(pageWidth > 1200)
            {
                totalPanelColumns = 3;
            }

            if (pageWidth > 1700)
            {
                totalPanelColumns = 4;
            }

            grantingPoliciesPanelTemplate.VisualTree.SetValue(UniformGrid.ColumnsProperty, totalPanelColumns);
            ItcGrantingPolicies.ItemsPanel = grantingPoliciesPanelTemplate;
        }
    }
}
