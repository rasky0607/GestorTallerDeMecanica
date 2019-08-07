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
using System.IO;

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
                        cbxIDClienteRepaInsert.ItemsSource = gestion._dao.selectClienteIdCliente();
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
                cbxIDClienteRepaInsert.ItemsSource = null;
                cbxMatriculaRepaInsert.ItemsSource = null;
                cbxServicioInsert.ItemsSource = null;
            }
        }

        //Dado un idCliente de un cliente selecionado, rellena el cbxMatriculaInsert con la coleccion de matriculas disponibles para ese cliente
        //Y  Para preaprar el numReparacion de forma automatica dada una fecha un idCliente y una matricula en caso de que cambie el idCliente selecionado
        private void CbxIdClienteInsert_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GestionVM gestion = new GestionVM();
            if (!gestion._dao.EstadoConexion())
            {
                if (cbxIDClienteRepaInsert.SelectedItem != null)
                {
                    gestion._dao.Conectar();
                    cbxMatriculaRepaInsert.ItemsSource = gestion._dao.selectClienteMatricula(int.Parse(cbxIDClienteRepaInsert.SelectedItem.ToString()));
                    tbxNombreClienteReparacion.Text = gestion._dao.selectClienteNombre(int.Parse(cbxIDClienteRepaInsert.SelectedItem.ToString()));
                    gestion._dao.Desconectar();
                }
            }

            //Para preaprar el numReparacion de forma automatica dada una fecha un idCliente y una matricula en caso de que cambie el idCliente selecionado
            if (dpFecha.SelectedDate != null && cbxIDClienteRepaInsert.SelectedItem != null && cbxMatriculaRepaInsert.SelectedItem != null)
            {
                if (!gestion._dao.EstadoConexion())
                {

                    int numeroDeReparaciones = -1;
                    gestion._dao.Conectar();
                    //Buscar numero de registros de en reparaciones con esa misma fecha idCliente y matricula de reparaciones
                    numeroDeReparaciones = gestion._dao.selectNumRepara(int.Parse(cbxIDClienteRepaInsert.SelectedItem.ToString()), cbxMatriculaRepaInsert.SelectedItem.ToString(), dpFecha.SelectedDate.ToString());
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
                gestion.Listado = gestion.Conversion(gestion._dao.selectReparacion());
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
            cbxtipoInsercion.ItemsSource = null;
            tbAnadir.IsEnabled = true;
            tbListado.IsEnabled = false;
            tbAnadir.Focus();
            //Vaciar todos los componentes de la pestaña insertar (de todas las tablas)
            LimpiezaDeTextoEnCompoentesDeInsercion();
            List<string> listadoTablasBD = new List<string>();
            listadoTablasBD.Add("cliente");
            listadoTablasBD.Add("servicio");
            listadoTablasBD.Add("reparacion");
            cbxtipoInsercion.ItemsSource = listadoTablasBD;
           
          
        }
        #endregion


        #region Modificaciones de registros
       

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
                        gridServicioMod.Visibility = Visibility.Hidden;
                        gridFacturaMod.Visibility = Visibility.Hidden;
                        break;

                    case "servicio":
                        tbListado.IsEnabled = false;
                        tbModificacion.IsEnabled = true;
                        tbModificacion.Focus();
                        gridClienteMod.Visibility = Visibility.Hidden;
                        gridFacturaMod.Visibility = Visibility.Hidden;
                        gridServicioMod.Visibility = Visibility.Visible;
                        break;

                    case "reparacion": //Las reparaciones no se pueden modificar                       
                        MessageBox.Show("Las reparaciones no pueden ser modificas.\nPuedes eliminar la reparación selecionada y posteriormente crear una nueva con los datos que necesites.", "(◑ω◐)¡Ops!.");
                        break;

                    case "factura":
                        Factura f = new Factura();
                        f = (Factura)dtgDatos.SelectedItem;
                        if (f.EstadoFactura is "ANULADA")
                            System.Windows.MessageBox.Show("Una factura ANULADA no puede volver a anularse.", "(◑ω◐)¡Ops!.");
                        else
                        {
                            tbListado.IsEnabled = false;
                            tbModificacion.IsEnabled = true;
                            tbModificacion.Focus();
                            gridClienteMod.Visibility = Visibility.Hidden;
                            gridServicioMod.Visibility = Visibility.Hidden;
                            gridFacturaMod.Visibility = Visibility.Visible;

                            //Preparacion  añadir de los combobox  una reparacion con los clientes,matriculas de vehiculos y servicios posibles
                            GestionVM gestion = new GestionVM();
                            if (!gestion._dao.EstadoConexion())
                            {
                                gestion._dao.Conectar();
                                cbxServicioFactura.ItemsSource = gestion._dao.selectServicioDescripcion();
                                cbxIDClienteFactura.ItemsSource = gestion._dao.selectClienteIdCliente();
                                gestion._dao.Desconectar();
                                cbxIDClienteFactura.SelectedIndex = -1;
                                cbxMatriculaFactura.SelectedIndex = -1;
                                tbxNombreClienteFactura.Text = string.Empty;

                            }
                            //----Fin preparacion de combobox de pestaña añadir en la tabla reapracion---
                        }
                        break;
                }
            }
            else//Si no ha selecionado un registro
            {
                MessageBox.Show("¡ATENCIÓN!:\nDebe selecionar un registro antes de ir a la ventana de Modificaciones. ", "(◑ω◐)¡Ops!.");
            }
        }

        //Preparacion de Combobox para ala hora de modificar una factura, la cuale s muy similar a la insercion de una reparacion
        private void CbxIDClienteFactura_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GestionVM gestion = new GestionVM();
            if (!gestion._dao.EstadoConexion())
            {
                if (cbxIDClienteFactura.SelectedItem != null)
                {
                    gestion._dao.Conectar();
                    cbxMatriculaFactura.ItemsSource = gestion._dao.selectClienteMatricula(int.Parse(cbxIDClienteFactura.SelectedItem.ToString()));
                    tbxNombreClienteFactura.Text = gestion._dao.selectClienteNombre(int.Parse(cbxIDClienteFactura.SelectedItem.ToString()));
                    gestion._dao.Desconectar();
                }
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
                gestion.Listado = gestion.Conversion(gestion._dao.selectReparacion());
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
                #region Ocultar colulmnas
                if (dtgDatos.Columns.Count >= 4)//Cuando el numero total de columnas del datagris es mayor o igual que 4
                {
                    //En reparacion
                    if (dtgDatos.Columns[5].Header is "CodServicio")//cuando el nombre de la cabecera de esa columna coincide con CodServicio
                    {
                        dtgDatos.Columns[5].Visibility = Visibility.Collapsed;//Plegamos o escondemos la columna CodServicio cuando esta aparece(en este caso solo ocurre cuando listamos la tabla reparacion)
                    }
                    if (dtgDatos.Columns[0].Header is "NumReparacion")//cuando el nombre de la cabecera de esa columna coincide con CodServicio
                    {
                        dtgDatos.Columns[0].Visibility = Visibility.Collapsed;//Plegamos o escondemos la columna CodServicio cuando esta aparece(en este caso solo ocurre cuando listamos la tabla reparacion)
                    }
                    //-----------//
                    #endregion

                    #region Cambiar nombre de columnas
                    //Cambio de nombres esteticos en reparacion
                    if (dtgDatos.Columns[2].Header is "NombreCliRepa")
                    {
                        dtgDatos.Columns[2].Header = "Nombre";
                    }

                    if (dtgDatos.Columns[3].Header is "ApellidosCliRepa")
                    {
                        dtgDatos.Columns[3].Header = "Apellidos";
                    }

                    if (dtgDatos.Columns[4].Header is "MatriCoche")
                    {
                        dtgDatos.Columns[4].Header = "Matricula";
                    }

                    if (dtgDatos.Columns[6].Header is "NombreServicio")
                    {
                        dtgDatos.Columns[6].Header = "Servicio";
                    }
                    if (lbTablaListada.Content.ToString() == "Facturas")
                    {
                        //Tabla Facturas
                        if (dtgDatos.Columns[0].Header is "NumeroFactura")
                        {
                            dtgDatos.Columns[0].Header = "Numero de Factura";
                        }

                        if (dtgDatos.Columns[2].Header is "EstadoFactura")
                        {
                            dtgDatos.Columns[2].Header = "Estado de Factura";
                        }

                        if (dtgDatos.Columns[3].Header is "NumeroFacturaAnulada")
                        {
                            dtgDatos.Columns[3].Header = "Numero Factura Anulada";
                        }

                        if (dtgDatos.Columns[4].Header is "idCliente")
                        {
                            dtgDatos.Columns[4].Header = "ID Cliente";
                        }

                        if (dtgDatos.Columns[5].Header is "NombreCliente")
                        {
                            dtgDatos.Columns[5].Header = "Nombre";
                        }

                        if (dtgDatos.Columns[6].Header is "ApellidosCliente")
                        {
                            dtgDatos.Columns[6].Header = "Apellidos";
                        }

                        if (dtgDatos.Columns[8].Header is "CodServicio")
                        {
                            dtgDatos.Columns[8].Header = "Codigo servicio";
                        }

                        if (dtgDatos.Columns[9].Header is "NombreServicio")
                        {
                            dtgDatos.Columns[9].Header = "Servicio";
                        }
                    }
                    //-----------//
                    #endregion

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
                                    if (gestion._dao.DeleteCliente(c.IdCliente, c.Matricula))
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
                                    if (gestion._dao.DeleteReparacion(r.NumReparacion, r.IdCliente, r.MatriCoche, r.Fecha))
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


        private void BtnAcercaDe_Click(object sender, RoutedEventArgs e)
        {
            AcercaDe vAcercaDe = new AcercaDe();
            vAcercaDe.Show();
        }

        //Para preparar el numReparacion de la insercion de reparaciones en caso de que cambie la fecha selecionada
        private void DpFecha_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpFecha.SelectedDate != null && cbxIDClienteRepaInsert.SelectedItem != null && cbxMatriculaRepaInsert.SelectedItem != null)
            {
                GestionVM gestion = new GestionVM();
                if (!gestion._dao.EstadoConexion())
                {
                    int numeroDeReparaciones = -1;
                    gestion._dao.Conectar();
                    //Buscar numero de registros de en reparaciones con esa misma fecha idCliente y matricula de reparaciones
                    numeroDeReparaciones = gestion._dao.selectNumRepara(int.Parse(cbxIDClienteRepaInsert.SelectedItem.ToString()), cbxMatriculaRepaInsert.SelectedItem.ToString(), dpFecha.SelectedDate.ToString());
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
            if (dpFecha.SelectedDate != null && cbxIDClienteRepaInsert.SelectedItem != null && cbxMatriculaRepaInsert.SelectedItem != null)
            {
                GestionVM gestion = new GestionVM();
                if (!gestion._dao.EstadoConexion())
                {

                    int numeroDeReparaciones = -1;
                    gestion._dao.Conectar();
                    //Buscar numero de registros de en reparaciones con esa misma fecha idCliente y matricula de reparaciones
                    numeroDeReparaciones = gestion._dao.selectNumRepara(int.Parse(cbxIDClienteRepaInsert.SelectedItem.ToString()), cbxMatriculaRepaInsert.SelectedItem.ToString(), dpFecha.SelectedDate.ToString());
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
                //Desconecxion
                GestionVM g = (GestionVM)gdBase.DataContext;
                g._dao.Desconectar();
                #region Copias de seguridad SQLite
                //Copia de Seguridad 1
                /* if (!File.Exists("./CopiasDeSeguridad/copiaBDTaller"))//Si no existe
                     File.Copy("./taller", "./CopiasDeSeguridad/copiaBDTaller");
                 else if (File.Exists("./CopiasDeSeguridad/copiaBDTaller") && !File.Exists("./CopiasDeSeguridad/copiaBDTaller"))//SI existe el original pero no el 1
                     File.Copy("./taller", "./CopiasDeSeguridad/copiaBDTaller1");
                 else if (File.GetCreationTime("./CopiasDeSeguridad/copiaBDTaller1") > File.GetCreationTime("./CopiasDeSeguridad/copiaBDTaller"))
                 {
                     File.Delete("./CopiasDeSeguridad/copiaBDTaller");
                     File.Copy("./taller", "./CopiasDeSeguridad/copiaBDTaller");

                 }
                 else if (File.GetCreationTime("./CopiasDeSeguridad/copiaBDTaller1") < File.GetCreationTime("./CopiasDeSeguridad/copiaBDTaller"))
                 {
                     File.Delete("./CopiasDeSeguridad/copiaBDTaller1");
                     File.Copy("./taller", "./CopiasDeSeguridad/copiaBDTaller1");
                 }*/

                //Copia de seguridad 2(copiando uno a uno los registros y añadiendolos a un fichero .sqlite con las palabras reservadas de el motor para realizar las inserciones
                #endregion
                g._dao.CopiaSeguridad();

                #region Copia de Seguridad SQL
                
                #endregion

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
                    cbxfiltroMatriculaCoche.ItemsSource = gestion._dao.selectMatriculasCocheClientes();
                    gestion._dao.Desconectar();
                }
            }
        }

        private void MiMenu_Git_Pablo_Click_1(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/rasky0607/");
        }


        private void LimpiezaDeTextoEnCompoentesDeInsercion()
        {
            //Cliente
            tbxnombre.Text = string.Empty;
            tbxapellidos.Text = string.Empty;
            tbxtlf.Text = string.Empty;
            tbxMatricula.Text = string.Empty;
            tbxModelo.Text = string.Empty;
            tbxMarca.Text = string.Empty;
            //Servicio
            tbxDescripcion.Text = string.Empty;
            tbxPrecio.Text = string.Empty;
            //Reparacion
            cbxIDClienteRepaInsert.SelectedItem = 0;
            cbxMatriculaRepaInsert.SelectedItem = null;
            cbxServicioInsert.SelectedItem = null;
            dpFecha.SelectedDate = DateTime.Now;
            tbxNumReparacion.Text = "1";
            tbxNombreClienteReparacion.Text = string.Empty;

        }

        //Ocultar o mostrar el mensaje que indica que tabla se esta listando en cada momento en la pestaña o tab listado.
        private void TbListado_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (lbTablaListada != null)
            {
                if (tbListado.IsEnabled == true)
                    lbTablaListada.Visibility = Visibility.Visible;
                else
                    lbTablaListada.Visibility = Visibility.Hidden;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ExtraerFacturasCSV ventanaExtracionCSV = new ExtraerFacturasCSV();
            ventanaExtracionCSV.Show();
        }
    }
}
