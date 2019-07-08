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
                gridReparacionInsert.Visibility = Visibility.Hidden;
            }

            if (cbxtipoInsercion.SelectedItem.ToString() == "coche"){
                gridClienteInsert.Visibility = Visibility.Hidden;
                gridServicioInsert.Visibility = Visibility.Hidden;
                gridReparacionInsert.Visibility = Visibility.Hidden;
            }

            if (cbxtipoInsercion.SelectedItem.ToString() == "servicio") {
                gridClienteInsert.Visibility = Visibility.Hidden;
                gridServicioInsert.Visibility = Visibility.Visible;
                gridReparacionInsert.Visibility = Visibility.Hidden;

            }

            if (cbxtipoInsercion.SelectedItem.ToString() == "reparacion") {
                gridClienteInsert.Visibility = Visibility.Hidden;
                gridServicioInsert.Visibility = Visibility.Hidden;
                gridReparacionInsert.Visibility = Visibility.Visible;
                //Preparacion  añadir de los combobox  una reparacion con los clientes,matriculas de vehiculos y servicios posibles
                GestionVM gestion = new GestionVM();
                if (!gestion._dao.EstadoConexion())
                {
                    gestion._dao.Conectar();
                    cbxServicioInsert.ItemsSource = gestion._dao.selectServicioDescripcion();
                    cbxDniClienteInsert.ItemsSource = gestion._dao.selectClienteDni();                  
                    gestion._dao.Desconectar();
                }
                //----Fin preparacion de combobox de pestaña añadir en la tabla reapracion---
            }
        }

        private void btnVolverInsert_Click(object sender, RoutedEventArgs e)
        {
            tbListado.IsEnabled = true;
            tbListado.Focus();
            GestionVM gestion = new GestionVM();
            if (!gestion._dao.EstadoConexion())
            {
                gestion._dao.Conectar();
                gestion.Listado =gestion.conversion(gestion._dao.selectReparacion());
                gestion._dao.Desconectar();
            }
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

        //Cuando se cierra la ventana, desconectamos obtenido el objeto del contexto de datos de el grid base de la ventana
        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                GestionVM g = (GestionVM)gdBase.DataContext;
                g._dao.Desconectar();
            }
            catch {
                throw new Exception("Fallo al cerrar la aplicacion.");
            }
        }

        private void CbxDniClienteInsert_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GestionVM gestion = new GestionVM();
            if (!gestion._dao.EstadoConexion())
            {
                gestion._dao.Conectar();
                cbxMatriculaInsert.ItemsSource = gestion._dao.selectClienteMatricula(cbxDniClienteInsert.SelectedItem.ToString());                  
                gestion._dao.Desconectar();
            }
        }

        private void DtgDatos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Reparacion r = new Reparacion();
              r = (Reparacion) dtgDatos.SelectedItem;
        }

        private void CbxtipoTablaMod_TextChanged(object sender, TextChangedEventArgs e)
        {
            switch (tbxtipoTablaMod.Text)
            {
                case "cliente":
                    gridClienteMod.Visibility = Visibility.Visible;
                    gridReparacionMod.Visibility = Visibility.Hidden;
                    gridServicioMod.Visibility = Visibility.Hidden;
                    break;

                case "servicio":
                    gridClienteMod.Visibility = Visibility.Hidden;
                    gridReparacionMod.Visibility = Visibility.Hidden;
                    gridServicioMod.Visibility = Visibility.Visible;
                    break;

                case "reparacion":
                    gridClienteMod.Visibility = Visibility.Hidden;
                    gridReparacionMod.Visibility = Visibility.Visible;
                    gridServicioMod.Visibility = Visibility.Hidden;
                    break;
            }
        }
    }
}
