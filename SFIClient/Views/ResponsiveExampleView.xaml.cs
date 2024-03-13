using SFIClient.SFIServices;
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
    public partial class ResponsiveExampleController : Page
    {
        public ResponsiveExampleController()
        {
            InitializeComponent();

            ServiceExampleClient exampleClient = new ServiceExampleClient();
            DataTypeExample example = exampleClient.GetDataUsingDataContract();
            Console.WriteLine(example.Name + (example.IsExample ? "" : " NO") + " es un ejemplo");
        }
    }
}
