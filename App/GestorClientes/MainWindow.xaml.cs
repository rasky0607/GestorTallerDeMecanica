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
using System.Threading;
using System.Diagnostics;

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
            if (cbxtipoInsercion.SelectedItem.ToString() == "cliente")
            {
                gridClienteInsert.Visibility = Visibility.Visible;
                gridServicioInsert.Visibility = Visibility.Hidden;
                gridReparacionInsert.Visibility = Visibility.Hidden;
            }

            if (cbxtipoInsercion.SelectedItem.ToString() == "coche")
            {
                gridClienteInsert.Visibility = Visibility.Hidden;
                gridServicioInsert.Visibility = Visibility.Hidden;
                gridReparacionInsert.Visibility = Visibility.Hidden;
            }

            if (cbxtipoInsercion.SelectedItem.ToString() == "servicio")
            {
                gridClienteInsert.Visibility = Visibility.Hidden;
                gridServicioInsert.Visibility = Visibility.Visible;
                gridReparacionInsert.Visibility = Visibility.Hidden;

            }

            if (cbxtipoInsercion.SelectedItem.ToString() == "reparacion")
            {
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

        //Dado un Dni de un cliente selecionado, rellena el cbxMatriculaInsert con la coleccion de matriculas disponibles para ese cliente
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
        private void btnVolverInsert_Click(object sender, RoutedEventArgs e)
        {
            tbListado.IsEnabled = true;
            tbListado.Focus();
            GestionVM gestion = new GestionVM();
            if (!gestion._dao.EstadoConexion())
            {
                gestion._dao.Conectar();
                gestion.Listado = gestion.conversion(gestion._dao.selectReparacion());
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
            catch
            {
                throw new Exception("Fallo al cerrar la aplicacion.");
            }
        }

        //POR AQUI!!
        #region Modificaciones de registros
        private void CbxDniClienteMod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GestionVM gestion = new GestionVM();
            if (!gestion._dao.EstadoConexion())
            {
                gestion._dao.Conectar();
                cbxMatriculaRepaMod.ItemsSource = gestion._dao.selectClienteMatricula(cbxDniClienteRepaMod.SelectedItem.ToString());
                gestion._dao.Desconectar();
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dtgDatos.SelectedItem != null)
            {

                switch (tbxtipoTablaMod.Text)
                {
                    case "cliente":
                        tbListado.IsEnabled = false;
                        tbModificacion.IsEnabled = true;
                        tbModificacion.Focus();
                        gridClienteMod.Visibility = Visibility.Visible;
                        gridReparacionMod.Visibility = Visibility.Hidden;
                        gridServicioMod.Visibility = Visibility.Hidden;
                        break;

                    case "servicio":
                        tbListado.IsEnabled = false;
                        tbModificacion.IsEnabled = true;
                        tbModificacion.Focus();
                        gridClienteMod.Visibility = Visibility.Hidden;
                        gridReparacionMod.Visibility = Visibility.Hidden;
                        gridServicioMod.Visibility = Visibility.Visible;
                        break;

                    case "reparacion":
                        /* gridClienteMod.Visibility = Visibility.Hidden;
                         gridReparacionMod.Visibility = Visibility.Visible;
                         gridServicioMod.Visibility = Visibility.Hidden;
                         //Preparacion  Modificar de los combobox  una reparacion con los clientes,matriculas de vehiculos y servicios posibles y preparacion de el resto de campos de esta tabla
                         if (dtgDatos.SelectedItem != null)
                         {
                             GestionVM gestion = new GestionVM();
                             if (!gestion._dao.EstadoConexion())
                             {
                                 gestion._dao.Conectar();
                                 cbxServicioRepaMod.ItemsSource = gestion._dao.selectServicioDescripcion();
                                 cbxDniClienteRepaMod.ItemsSource = gestion._dao.selectClienteDni();
                                 cbxServicioRepaMod.ItemsSource = gestion._dao.selectServicioDescripcion();

                                 Reparacion r = new Reparacion();
                                /* r = (Reparacion)dtgDatos.SelectedItem;
                                 cbxDniClienteRepaMod.SelectedItem = r.DniCliente;
                                 cbxMatriculaRepaMod.SelectedItem = r.MatriCoche;
                                 cbxServicioRepaMod.SelectedItem = r.NombreServicio;
                                 dpFechaRepaMod.SelectedDate = DateTime.Parse(r.Fecha);
                                 tbxNumReparacionRepaMod.Text = r.NumReparacion.ToString();
                                 gestion._dao.Desconectar();
                             }
                         }*/
                        MessageBox.Show("Las reparaciones no pueden ser modificas.\nPuedes eliminar la reparación selecionada y posteriormente crear una nueva con los datos que necesites.", "(◑ω◐)¡Ops!.");
                        //----Fin preparacion de combobox de pestaña Modificar en la tabla reapracion y preparacion de el resto de campos de esta tabla---
                        break;
                }
            }
            else//Si no ha selecionado un registro
            {
                MessageBox.Show("¡ATENCIÓN!:\nDebe selecionar un registro antes de ir a la ventana de Modificaciones. ", "(◑ω◐)¡Ops!.");
            }
        }

        private void btnVolverMod_Click(object sender, RoutedEventArgs e)
        {
            //limpiamos la propiedad selecteitem, por si vuelve a pulsar editar sin selecionar nada al volver, no le lleva al apestaña editar por el anterior selecteItem selecionado
            dtgDatos.SelectedItem = null;

            tbListado.IsEnabled = true;
            tbListado.Focus();
            GestionVM gestion = new GestionVM();
            if (!gestion._dao.EstadoConexion())
            {
                gestion._dao.Conectar();
                gestion.Listado = gestion.conversion(gestion._dao.selectReparacion());
                gestion._dao.Desconectar();
            }
            tbModificacion.IsEnabled = false;
        }

        #endregion
        #region Apartado general estetico
        private void DtgDatos_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (this.dtgDatos.Columns != null)
            {
                if (dtgDatos.Columns.Count >= 4)//Cuando el numero total de columnas del datagris es mayor o igual que 4
                {
                    if (dtgDatos.Columns[3].Header is "CodServicio")//cuando el nombre de la cabecera de esa columna coincide con CodServicio
                    {
                        dtgDatos.Columns[3].Visibility = Visibility.Collapsed;//Plegamos o escondemos la columna CodServicio cuando esta aparece(en este caso solo ocurre cuando listamos la tabla reparacion)
                    }
                    if (dtgDatos.Columns[0].Header is "IdCliente")
                    {
                        dtgDatos.Columns[0].Visibility = Visibility.Collapsed;//Plegamos o escondemos la columna IdCliente cuando esta aparece(en este caso solo ocurre cuando listamos la tabla clientes)                                 
                    }
                }
            }
        }




        #endregion

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dtgDatos.SelectedItems != null)
            {
                GestionVM gestion = new GestionVM();
                if (!gestion._dao.EstadoConexion())
                {

                    gestion._dao.Conectar();
                    switch (tbxtipoTablaMod.Text)
                    {
                        case "cliente":
                            int regisCliborradoconExito = 0;
                            try
                            {
                                for (int i = 0; i < dtgDatos.SelectedItems.Count; i++)
                                {
                                    Cliente c = new Cliente();
                                    c = (Cliente)dtgDatos.SelectedItems[i];
                                    if (gestion._dao.DeleteCliente(c.Dni, c.Matricula))
                                        regisCliborradoconExito++;
                                }
                                if (regisCliborradoconExito == dtgDatos.SelectedItems.Count)
                                {
                                    lbmensaje.Content = "Se ha eliminado con existo los registros indicados";
                                    //dtgDatos.ItemsSource = gestion.conversion(gestion._dao.selectCliente());
                                }                               
                            }
                            catch
                            {
                                MessageBox.Show("Uno o alguno de los cliente selecionados no pueden ser eliminados,posiblemente tengan algun registro\nde reparaciones con el que esten relacionados.\nAntes de eliminarlo\\s debes borrar las reparacion\\es con las que esten relacionado\\s", "(◑ω◐)¡Ops!.");
                            }
                            break;

                        case "servicio":
                            int regisSerborradoconExito = 0;
                            try
                            {
                                for (int i = 0; i < dtgDatos.SelectedItems.Count; i++)
                                {
                                    Servicio s = new Servicio();
                                    s = (Servicio)dtgDatos.SelectedItems[i];
                                    if (gestion._dao.DeleteServicio(s.Codigo))
                                        regisSerborradoconExito++;
                                }
                                if (regisSerborradoconExito == dtgDatos.SelectedItems.Count)
                                {
                                    lbmensaje.Content = "Se ha eliminado con existo los registros indicados";
                                    //dtgDatos.ItemsSource = gestion.conversion(gestion._dao.selectServicio());
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Uno o alguno de los servicios seleccionados no pueden ser eliminados,posiblemente tengan algun registro\nde reparaciones con el que esten relacionados.\nAntes de eliminarlo\\s debes borrar las reparacion\\es con las que esten relacionado\\s", "(◑ω◐)¡Ops!.");
                            }

                            break;
                            //REVISAR
                        case "reparacion":
                            int regisRepaborradoconExito = 0;
                            try
                            {
                                for (int i = 0; i < dtgDatos.SelectedItems.Count; i++)
                                {
                                    Servicio s = new Servicio();
                                    s = (Servicio)dtgDatos.SelectedItems[i];
                                    if (gestion._dao.DeleteServicio(s.Codigo))
                                        regisRepaborradoconExito++;
                                }
                                if (regisRepaborradoconExito == dtgDatos.SelectedItems.Count)
                                {
                                    lbmensaje.Content = "Se ha eliminado con exito los registros indicados";
                                   // dtgDatos.ItemsSource = gestion.conversion(gestion._dao.selectReparacion());
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Uno o alguno de los servicios selecionados no pueden ser eliminados.\nPosiblemente tengan algun registro\nde reparaciones con el que esten relacionados.\nAntes de eliminarlo\\s debes borrar las reparacion\\es con las que esten relacionado\\s", "(◑ω◐)¡Ops!.");
                            }

                            break;
                    }
                    gestion._dao.Desconectar();


                }
            }
            else//Si no ha selecionado un registro
            {
                MessageBox.Show("¡ATENCIÓN!:\nDebe selecionar algun/os registro/os antes de poder eliminar uno o varios registros. ", "(◑ω◐)¡Ops!.");
            }
        }

        private void MiMenu_Git_Pablo_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/rasky0607/");
        }

        private void BtnAcercaDe_Click(object sender, RoutedEventArgs e)
        {
            AcercaDe vAcercaDe = new AcercaDe();
            vAcercaDe.Show();
        }

       
    }
}
