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
using LiveCharts;
using LiveCharts.Definitions.Series;
using LiveCharts.Wpf;

namespace SFIClient.Views
{
    /// <summary>
    /// Lógica de interacción para OverallCollectionEfficiency.xaml
    /// </summary>
    public partial class OverallCollectionEfficiencyController : Page
    {
        public OverallCollectionEfficiencyController()
        {
            InitializeComponent();
            ShowChart();
        }

        public void ShowChart()
        {
            SeriesCollection pieSeries = new SeriesCollection();
            // Agregar datos de ejemplo
            pieSeries.Add(new PieSeries
            {
                Title = "Category A",
                Values = new ChartValues<double> { 10 }
            });

            pieSeries.Add(new PieSeries
            {
                Title = "Category B",
                Values = new ChartValues<double> { 20 }
            });

            pieSeries.Add(new PieSeries
            {
                Title = "Category C",
                Values = new ChartValues<double> { 30 }
            });

            // Asignar la colección de series al PieChart
            pcChart.Series = pieSeries;
        }
    }
}
