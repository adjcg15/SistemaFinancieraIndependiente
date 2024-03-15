﻿using SFIClient.Controlls;
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
    /// Lógica de interacción para SearchClientByRFCView.xaml
    /// </summary>
    public partial class SearchClientByRFCView : Page
    {
        public SearchClientByRFCView()
        {
            InitializeComponent();
            for (int i = 0; i < 15; i++)
            {
                ClientControll clienteControl = new ClientControll();
                icClients.Items.Add(clienteControl);
            }
        }
    }
}
