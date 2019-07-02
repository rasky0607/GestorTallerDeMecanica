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


namespace GestorClientes
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
           
        }
        #region Añadir registros
        //Mostrar los distintos grid con los distintos campos segun en que tabla se quiere realizar la insercion
        private void CbxtipoInsercion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxtipoInsercion.SelectedItem.ToString() == "cliente"){
                gridClienteInsert.Visibility = Visibility.Visible;
                gridServicioInsert.Visibility = Visibility.Hidden;
                gridCocheInsert.Visibility = Visibility.Hidden;
                gridReparacionInsert.Visibility = Visibility.Hidden;
            }

            if (cbxtipoInsercion.SelectedItem.ToString() == "coche"){
                gridClienteInsert.Visibility = Visibility.Hidden;
                gridServicioInsert.Visibility = Visibility.Hidden;
                gridCocheInsert.Visibility = Visibility.Visible;
                gridReparacionInsert.Visibility = Visibility.Hidden;
            }

            if (cbxtipoInsercion.SelectedItem.ToString() == "servicio") {
                gridClienteInsert.Visibility = Visibility.Hidden;
                gridServicioInsert.Visibility = Visibility.Visible;
                gridCocheInsert.Visibility = Visibility.Hidden;
                gridReparacionInsert.Visibility = Visibility.Hidden;

            }

            if (cbxtipoInsercion.SelectedItem.ToString() == "reparacion") {
                gridClienteInsert.Visibility = Visibility.Hidden;
                gridServicioInsert.Visibility = Visibility.Hidden;
                gridCocheInsert.Visibility = Visibility.Hidden;
                gridReparacionInsert.Visibility = Visibility.Visible;
            }
        }

        private void BtncancelarInsert_Click(object sender, RoutedEventArgs e)
        {
            tbListado.IsEnabled = true;
            tbListado.Focus();
            tbAnadir.IsEnabled = false;
            
        }

        //tabInsertar=añadir
        private void BtnIraTabInsertar_Click(object sender, RoutedEventArgs e)
        {
            tbAnadir.IsEnabled = true;
            tbListado.IsEnabled = false;
            tbAnadir.Focus();

        }
        #endregion
    }
}
