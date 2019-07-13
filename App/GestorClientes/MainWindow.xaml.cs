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
            if (cbxtipoInsercion.SelectedItem != null)
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
                        cbxDniClienteRepaInsert.ItemsSource = gestion._dao.selectClienteDni();
                        gestion._dao.Desconectar();
                    }
                    //----Fin preparacion de combobox de pestaña añadir en la tabla reapracion---
                }
            }
            else//En caso  de no haber nada selecionado como cuando ocurre al darle al boton de volver, ocultamos todo para cuadno vuelve a la ventana insercion estar como al pricipio
            {
                gridClienteInsert.Visibility = Visibility.Hidden;
                gridServicioInsert.Visibility = Visibility.Hidden;
                gridReparacionInsert.Visibility = Visibility.Hidden;
                cbxDniClienteRepaInsert.ItemsSource = null;
                cbxMatriculaRepaInsert.ItemsSource = null;
                cbxServicioInsert.ItemsSource = null;
            }
        }

        //Dado un Dni de un cliente selecionado, rellena el cbxMatriculaInsert con la coleccion de matriculas disponibles para ese cliente
        //Y  Para preaprar el numReparacion de forma automatica dada una fecha un dniCliente y una matricula en caso de que cambie el DniCliente selecionado
        private void CbxDniClienteInsert_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GestionVM gestion = new GestionVM();
            if (!gestion._dao.EstadoConexion())
            {
                if (cbxDniClienteRepaInsert.SelectedItem != null)
                {
                    gestion._dao.Conectar();
                    cbxMatriculaRepaInsert.ItemsSource = gestion._dao.selectClienteMatricula(cbxDniClienteRepaInsert.SelectedItem.ToString());
                    gestion._dao.Desconectar();
                }
            }

            //Para preaprar el numReparacion de forma automatica dada una fecha un dniCliente y una matricula en caso de que cambie el DniCliente selecionado
            if (dpFecha.SelectedDate != null && cbxDniClienteRepaInsert.SelectedItem != null && cbxMatriculaRepaInsert.SelectedItem != null)
            {
                if (!gestion._dao.EstadoConexion())
                {

                    int numeroDeReparaciones = -1;
                    gestion._dao.Conectar();
                    //Buscar numero de registros de en reparaciones con esa misma fecha dni y matricula de reparaciones
                    numeroDeReparaciones = gestion._dao.selectNumRepara(cbxDniClienteRepaInsert.SelectedItem.ToString(), cbxMatriculaRepaInsert.SelectedItem.ToString(), dpFecha.SelectedDate.ToString());
                    //Si el resultado es 0 o cualquier otro mayor(osea no hay reparaciones en esa fecha para ese cliente y esa matricula o 1 2 etc...) entonces esta sera la numero el resultado +1 p ++ que es lo mismo
                    numeroDeReparaciones++;

                    tbxNumReparacion.Text = numeroDeReparaciones.ToString();//asignamos el numero de reparacion automaticamente en el textbox sin intervencion de usuario
                    gestion._dao.Desconectar();
                }
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
            cbxtipoInsercion.SelectedItem = null;
            lbMensajeInsercion.Content = string.Empty;


        }

        //tabInsertar=añadir
        private void BtnIraTabInsertar_Click(object sender, RoutedEventArgs e)
        {
            cbxtipoInsercion.SelectedItem = null;
            tbAnadir.IsEnabled = true;
            tbListado.IsEnabled = false;
            tbAnadir.Focus();

        }
        #endregion


        #region Modificaciones de registros
        private void CbxDniClienteMod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GestionVM gestion = new GestionVM();
            if (!gestion._dao.EstadoConexion())
            {
                if (cbxDniClienteRepaMod.SelectedItem != null)
                {
                    gestion._dao.Conectar();
                    cbxMatriculaRepaMod.ItemsSource = gestion._dao.selectClienteMatricula(cbxDniClienteRepaMod.SelectedItem.ToString());
                    gestion._dao.Desconectar();
                }
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

                    case "reparacion": //Las reparaciones no se pueden modificar                       
                        MessageBox.Show("Las reparaciones no pueden ser modificas.\nPuedes eliminar la reparación selecionada y posteriormente crear una nueva con los datos que necesites.", "(◑ω◐)¡Ops!.");
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
            lbMensajeMod.Content = string.Empty;//Limpieza de mensajes al darle a volver

        }

        #endregion
        #region Apartado general estetico de columnas de el datagrid
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
                                    //lbmensaje.Content = "Se ha eliminado con existo los registros indicados";
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
                                    //lbmensaje.Content = "Se ha eliminado con existo los registros indicados";
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
                                    Reparacion r = new Reparacion();
                                    r = (Reparacion)dtgDatos.SelectedItems[i];
                                    if (gestion._dao.DeleteReparacion(r.NumReparacion, r.DniCliente, r.MatriCoche, r.Fecha))
                                        regisRepaborradoconExito++;
                                }
                                if (regisRepaborradoconExito == dtgDatos.SelectedItems.Count)
                                {
                                    //lbmensaje.Content = "Se ha eliminado con exito los registros indicados";
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

        //Para preparar el numReparacion de la insercion de reparaciones en caso de que cambie la fecha selecionada
        private void DpFecha_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpFecha.SelectedDate != null && cbxDniClienteRepaInsert.SelectedItem != null && cbxMatriculaRepaInsert.SelectedItem != null)
            {
                GestionVM gestion = new GestionVM();
                if (!gestion._dao.EstadoConexion())
                {
                    int numeroDeReparaciones = -1;
                    gestion._dao.Conectar();
                    //Buscar numero de registros de en reparaciones con esa misma fecha dni y matricula de reparaciones
                    numeroDeReparaciones = gestion._dao.selectNumRepara(cbxDniClienteRepaInsert.SelectedItem.ToString(), cbxMatriculaRepaInsert.SelectedItem.ToString(), dpFecha.SelectedDate.ToString());
                    //Si el resultado es 0 o cualquier otro mayor(osea no hay reparaciones en esa fecha para ese cliente y esa matricula o 1 2 etc...) entonces esta sera la numero el resultado +1 p ++ que es lo mismo
                    numeroDeReparaciones++;

                    tbxNumReparacion.Text = numeroDeReparaciones.ToString();//asignamos el numero de reparacion automaticamente en el textbox sin intervencion de usuario
                    gestion._dao.Desconectar();
                }
            }
        }
        //Para preparar el numReparacion de la insercion de reparaciones en caso de que cambie la matricula selecionada
        private void CbxMatriculaRepaInsert_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpFecha.SelectedDate != null && cbxDniClienteRepaInsert.SelectedItem != null && cbxMatriculaRepaInsert.SelectedItem != null)
            {
                GestionVM gestion = new GestionVM();
                if (!gestion._dao.EstadoConexion())
                {

                    int numeroDeReparaciones = -1;
                    gestion._dao.Conectar();
                    //Buscar numero de registros de en reparaciones con esa misma fecha dni y matricula de reparaciones
                    numeroDeReparaciones = gestion._dao.selectNumRepara(cbxDniClienteRepaInsert.SelectedItem.ToString(), cbxMatriculaRepaInsert.SelectedItem.ToString(), dpFecha.SelectedDate.ToString());
                    //Si el resultado es 0 o cualquier otro mayor(osea no hay reparaciones en esa fecha para ese cliente y esa matricula o 1 2 etc...) entonces esta sera la numero el resultado +1 p ++ que es lo mismo
                    numeroDeReparaciones++;

                    tbxNumReparacion.Text = numeroDeReparaciones.ToString();//asignamos el numero de reparacion automaticamente en el textbox sin intervencion de usuario
                    gestion._dao.Desconectar();
                }
            }
        }


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

        //Al ponerse a0 el componente tbxInserCorrect es por que la insercion fue correcta y cambia el foco a tblistado 
        private void TbxInserCorrect_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.Parse(tbxInserCorrect.Text) == 0)
            {
                tbListado.IsEnabled = true;
                tbListado.Focus();
                tbAnadir.IsEnabled = false;
                
            }
        }

        private void TbxModCorrect_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.Parse(tbxModCorrect.Text) == 0)
            {
                tbListado.IsEnabled = true;
                tbListado.Focus();
                tbModificacion.IsEnabled = false;
            }
        }

        private void StackPanel_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (spFiltros.IsEnabled)
            {
                GestionVM gestion = new GestionVM();
                if (!gestion._dao.EstadoConexion())
                {
                    gestion._dao.Conectar();
                    cbxfiltroDni.ItemsSource = gestion._dao.selectClienteDni();
                    gestion._dao.Desconectar();
                }
            }
        }
    }
}
