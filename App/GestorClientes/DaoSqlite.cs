using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Añadido
using System.Data;
using System.Data.SQLite;
using System.Collections;


namespace GestorClientes
{
    class DaoSqlite
    {
        public SQLiteConnection conexion;

        public bool Conectar()
        {
            string db = @".\taller";
            string cadenaConecxion = string.Format("Data Source=" + db + ";Version=3;FailIfMissing=true;foreign keys=true;");//PRobando foreign keys=true;
            try
            {
                conexion = new SQLiteConnection(cadenaConecxion);
                conexion.Open();
                return true;
            }
            catch
            {
                throw;
            }
        }

        public bool Desconectar()
        {
            try
            {
                conexion.Close();
                conexion = null;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool EstadoConexion()
        {
            if (conexion != null && conexion.State == ConnectionState.Open)
            {
                return true;
            }
            else
                return false;
        }
   
        public List<Cliente> selectCliente()
        {
            List<Cliente> lClientes = new List<Cliente>();
            string sql = "select idCliente,nombre,apellidos,tlf,matricula,marca,modelo from cliente;";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Cliente miCliente = new Cliente();
                    miCliente.IdCliente = int.Parse(lector["idCliente"].ToString());
                    miCliente.Nombre = lector["nombre"].ToString();
                    miCliente.Apellidos = lector["apellidos"].ToString();
                    miCliente.Tlf = int.Parse(lector["tlf"].ToString());
                    miCliente.Matricula = lector["matricula"].ToString();
                    miCliente.Marca = lector["marca"].ToString();
                    miCliente.Modelo = lector["modelo"].ToString();
                    lClientes.Add(miCliente);
                }
                lector.Close();
            }
            catch(Exception e) {
                throw new Exception(e.Message);
            }
            return lClientes;

        }

        public List<string> selectClienteIdCliente()
        {
            List<string> lClientes = new List<string>();
            string sql = "select idCliente from cliente;";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {                               
                    lClientes.Add(lector["idCliente"].ToString());              
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lClientes;

        }
        
        public List<string> selectClienteMatricula(int idCliente)
        {
            List<string> listMatricula = new List<string>();
            string sql = "select matricula from cliente where idCliente='" + idCliente+"';";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Cliente cli = new Cliente();
                    cli.Matricula=lector["matricula"].ToString();
                    listMatricula.Add(cli.Matricula);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listMatricula;

        }

        //Dado un idCliente devuelvo su nombre
        public string selectClienteNombre(int idCliente)
        {
            string nombre = string.Empty;
            string sql = "select nombre from cliente where idCliente='" + idCliente + "';";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {                 
                    nombre = lector["nombre"].ToString();                                     
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return nombre;

        }
     
        public List<Reparacion> selectReparacion()
        {
            //select numReparacion,idCliente,matriCoche,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r order by fecha and order by idCliente;
            //select numReparacion,idCliente,matriCoche,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r order by fecha,idCliente;
            List<Reparacion> lReparacion = new List<Reparacion>();
            //string sql = "select * from reparacion;";
            string sql = "select numReparacion,idCliente,(select nombre from cliente where idCliente=r.idCliente)as nombre,matriCoche,codServicio,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r order by idCliente,fecha desc";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Reparacion miReparacion = new Reparacion();
                    miReparacion.NumReparacion = int.Parse(lector["numReparacion"].ToString());
                    miReparacion.IdCliente = int.Parse(lector["idCliente"].ToString());
                    miReparacion.NombreCliRepa = lector["nombre"].ToString();
                    miReparacion.MatriCoche = lector["matriCoche"].ToString();
                    miReparacion.CodServicio = int.Parse(lector["codServicio"].ToString());
                    miReparacion.NombreServicio = lector["servicio"].ToString();
                    miReparacion.Fecha = DateTime.Parse(lector["fecha"].ToString()).ToShortDateString();
            
                    lReparacion.Add(miReparacion);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lReparacion;

        }

        public List<Servicio> selectServicio()
        {
            List<Servicio> lServicio = new List<Servicio>();
            string sql = "select * from servicio;";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Servicio miServicio = new Servicio();
                    miServicio.Codigo = int.Parse(lector["codigo"].ToString());
                    miServicio.Descripcion = lector["descripcion"].ToString();
                    miServicio.Precio = double.Parse(lector["precio"].ToString());


                    lServicio.Add(miServicio);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lServicio;

        }

        public List<string> selectServicioDescripcion()
        {
            List<string> lServicio = new List<string>();
            string sql = "select descripcion from servicio;";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    lServicio.Add(lector["descripcion"].ToString());             
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lServicio;

        }

        public int selectServicioCodigo(string descripcion)
        {
            Servicio miservicio = new Servicio();
            string sql = "select codigo from servicio where descripcion='"+descripcion+"';";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    miservicio.Codigo = int.Parse(lector["codigo"].ToString());  
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return miservicio.Codigo;

        }

        public int selectNumRepara(int idCliRepara,string matriculaRepa, string fecha)
        {
            int nreparaciones = -1;
            //select count(*)as cantidad from reparacion where idCliente=1 and matriCoche='2019OPL' and fecha='2019-07-11';
            string sql = "select count(*)as cantidad from reparacion where idCliente='" + idCliRepara + "'and matriCoche='"+matriculaRepa+ "'and fecha='" + (DateTime.Parse(fecha)).ToString("yyyy-MM-dd") + "'";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                   nreparaciones = int.Parse(lector["cantidad"].ToString());
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return nreparaciones;

        }

        public List<string> VerTablas()
        {
            List<string> lTablas = new List<string>();
            string sql = "SELECT name FROM sqlite_master WHERE type =\"table\" and name !=\"sqlite_sequence\"";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);
            //SELECT name FROM sqlite_master WHERE type = "table" and name !='sqlite_sequence'; <== Esto equivale desde .NET a ==>  .tables
            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    string m = lector["name"].ToString();

                    lTablas.Add(m);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lTablas;

        }

        public bool InsertCliente(string nombre,string apellidos, int tlf,string matricula, string marca, string modelo)
        {
            try
            {
                if (matricula is null || matricula == " ")
                    throw new Exception("El campo matricula no pueden estar vacios!");
                else
                {
                    //Obtencion  del IdCliente autoincremental buscando el max(idcleinte) o el id mas alto  y sumandole 1 para el siguiente cliente
                    #region obtencion
                    string sql1;
                    sql1 = "select max(idCliente)as maximoid from cliente";
                    SQLiteCommand cmd1 = new SQLiteCommand(sql1, conexion);
                    int idCliente=0;
                    SQLiteDataReader lector = null;

                    #region Comprobacion de que hay al menos algun id dado de alta
                    //Comprobacion de si hay algun cliente para que la funcion max no devuelva un resultado vacio
                    int numeroClientes = -1;
                   string sqlcount = "select count(*)as numeroClientes from cliente";
                    SQLiteCommand cmdcount = new SQLiteCommand(sqlcount, conexion);
                    try
                    {
                        lector = cmdcount.ExecuteReader();
                        while (lector.Read())
                        {

                            numeroClientes = int.Parse(lector["numeroClientes"].ToString());

                        }
                        lector.Close();                       
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                    #endregion

                    //Si es  count o numeroClientes es  mayor que 0, hace esto si no no
                    if (numeroClientes > 0)
                    {
                        try
                        {
                            lector = cmd1.ExecuteReader();
                            while (lector.Read())
                            {

                                idCliente = int.Parse(lector["maximoid"].ToString());

                            }
                            lector.Close();
                            idCliente++;
                        }
                        catch (Exception e)
                        {
                            throw new Exception(e.Message);
                        }
                    }
                    else
                        idCliente = 1;

                    #endregion
                    //Insercion
                    string sql;
                    sql = "INSERT INTO cliente (idCliente,nombre,apellidos,tlf,matricula,marca,modelo) values ("+idCliente+",'" + nombre.ToUpper() + "','" + apellidos.ToUpper() + "'," + tlf + ",'" + matricula.ToUpper() + "','" + marca.ToUpper() + "','" + modelo.ToUpper() + "');";
                    SQLiteCommand cmd = new SQLiteCommand(sql, conexion);
                    cmd.ExecuteNonQuery();
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }        
            return true;

        }
   
        public bool InsertServicio(string descripcion, double precio)
        {
            try
            {
                //Estas lineas estan para modificar la culturalizacion de forma que indicamos el separador de numperos decimales es el . y no la, para que ala hora de la insercion no de fallos al insertar numbreservicio, 20,05 si no que inserte 20.05
                System.Globalization.CultureInfo c = new System.Globalization.CultureInfo("es-ES");
                c.NumberFormat.NumberDecimalSeparator = ".";
                c.NumberFormat.CurrencyDecimalSeparator = ".";
                c.NumberFormat.PercentDecimalSeparator = ".";
                c.NumberFormat.CurrencyDecimalSeparator = ".";
                System.Threading.Thread.CurrentThread.CurrentCulture = c;
                //----------//
              
                if (descripcion is null || descripcion== " ")
                    throw new Exception();
                string sql;
                sql = "INSERT INTO servicio (descripcion,precio) values ('" + descripcion.ToUpper() + "'," +precio + ")";
                SQLiteCommand cmd = new SQLiteCommand(sql, conexion);
                cmd.ExecuteNonQuery();
            }
            catch
            {
                throw;
            }
            return true;

        }

        public bool InsertReparacion(int idReparacion, int idCliente,string matriculaCoche, int codServicio,string fecha)
        {
            try
            {
                if (idReparacion <= 0 || idCliente  <=0 || matriculaCoche is null || codServicio < 0 || fecha == null)
                    throw new Exception();
                string sql;            
                sql = "INSERT INTO reparacion (numReparacion,idCliente,matriCoche,codServicio,fecha) values (" + idReparacion + ",'" + idCliente + "','"+matriculaCoche+ "',"+codServicio+ ",'"+fecha+"')";
               
                SQLiteCommand cmd = new SQLiteCommand(sql, conexion);
                cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
            return true;

        }

        public bool UpdateCliente(int idCliente, string nombre, string apellidos, int tlf, string matricula, string marca, string modelo)
        {
            try
            {
                if ( matricula is null || matricula == " ")
                    throw new Exception("El campo matricula no pueden estar vacios!");
                else
                {
                    //update cliente set nombre='Fran',apellidos='Lanzat' where idCliente=1;
                    //Actualizacion
                    string sql;
                    sql = "update cliente set nombre='"+nombre.ToUpper() + "',apellidos='"+apellidos.ToUpper() + "',tlf="+tlf+",matricula='"+matricula.ToUpper() + "',marca='"+marca.ToUpper() + "',modelo='"+modelo.ToUpper() + "' where idCliente="+idCliente+"";
                    SQLiteCommand cmd = new SQLiteCommand(sql, conexion);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return true;

        }
        //cambiado
        public bool UpdateServicio(int codiServicio,string descripcion, double precio)
        {
            try
            {
                //Estas lineas estan para modificar la culturalizacion de forma que indicamos el separador de numperos decimales es el . y no la, para que ala hora de la insercion no de fallos al insertar numbreservicio, 20,05 si no que inserte 20.05
                System.Globalization.CultureInfo c = new System.Globalization.CultureInfo("es-ES");
                c.NumberFormat.NumberDecimalSeparator = ".";
                c.NumberFormat.CurrencyDecimalSeparator = ".";
                c.NumberFormat.PercentDecimalSeparator = ".";
                c.NumberFormat.CurrencyDecimalSeparator = ".";
                System.Threading.Thread.CurrentThread.CurrentCulture = c;
                //----------//
                if (descripcion == " " || descripcion is null)
                    throw new Exception("El campo descripcion no puede estar vacio!");
                else
                {
                  
                    //Actualizacion
                    string sql;
                   sql = "update servicio set descripcion='"+descripcion.ToUpper() + "',precio="+precio+" where codigo="+codiServicio+"";
                    SQLiteCommand cmd = new SQLiteCommand(sql, conexion);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return true;

        }


        /*No se podra Actualizacion reparacion por consenso ya que es como una factura y necesita de los 4 
         * campos para  modificarla,es mejor crear y eliminarla y crear una nueva reparacion*/

        //Eliminar
        //--------------//
        public bool DeleteCliente(int idCliente,string matricula)
        {
            //delete from cliente where idCliente=1 and matricula='2218CL';
            try
            {                              
                    string sql;
                    sql = "delete from cliente where idCliente='" + idCliente+"' and matricula='"+matricula+"'";
                    SQLiteCommand cmd = new SQLiteCommand(sql, conexion);
                    cmd.ExecuteNonQuery();
                
            }
            catch
            {
                throw;
            }
            return true;
        }

        public bool DeleteServicio(int codigo)
        {
            //delete from cliente where idCliente=1 and matricula='2218CL';
            try
            {
                string sql;
                sql = "delete from servicio where codigo=" +codigo + "";
                SQLiteCommand cmd = new SQLiteCommand(sql, conexion);
                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return true;
        }

        public bool DeleteReparacion(int numReparacion, int idCliente, string matriculaCoche, string fecha)
        {
            //delete from cliente where idCliente=1 and matricula='2218CL';
            //// "INSERT INTO reparacion (numReparacion,idCliente,matriCoche,codServicio,fecha) values (" + idReparacion + ",'" + idCliente + "','"+matriculaCoche+ "',"+codServicio+ ",'"+DateTime.Parse(fecha).ToShortDateString()+"')"
            try
            {
                string sql;
                sql = "delete from reparacion where numReparacion=" + numReparacion + " and idCliente='" + idCliente + "'" + " and matriCoche='" + matriculaCoche+ "' and fecha='" + Convert.ToDateTime(DateTime.Parse(fecha)).ToString("yyyy-MM-dd")+"'";
                SQLiteCommand cmd = new SQLiteCommand(sql, conexion);
                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return true;
        }

        //Preparacion Filtros
        //------------------------//

        public List<string> selectMatriculasCocheClientes()
        {
            List<string> listMatricula = new List<string>();
            string sql = "select distinct(matricula) from cliente";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Cliente cli = new Cliente();
                    cli.Matricula = lector["matricula"].ToString();
                    listMatricula.Add(cli.Matricula);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listMatricula;

        }

        //Dada una matricula, IdCliente de clientes relacionada con ella
        public List<int> selectIdClienteClientes(string matricula)
        {
            List<int> listIdCliente = new List<int>();
            string sql = "select idCliente from cliente where matricula='"+matricula+"'";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Cliente cli = new Cliente();
                    cli.IdCliente = int.Parse(lector["idCliente"].ToString());
                    listIdCliente.Add(cli.IdCliente);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listIdCliente;

        }

        //----------//

        //Consultas de filtro:
        //------------------------//

        //Con un idCliente selecionado 
        public List<Reparacion> selectReparacionFiltroFecha(string matriculaCoche, string fecha)
        {           
            List<Reparacion> lReparacion = new List<Reparacion>();            
            string sql = "select numReparacion,idCliente,(select nombre from cliente where idCliente=r.idCliente)as nombre,matriCoche,codServicio,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r where matriCoche='" + matriculaCoche + "' and fecha='"+fecha+ "' order by idCliente,fecha desc";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Reparacion miReparacion = new Reparacion();
                    miReparacion.NumReparacion = int.Parse(lector["numReparacion"].ToString());
                    miReparacion.IdCliente = int.Parse(lector["idCliente"].ToString());
                    miReparacion.NombreCliRepa = lector["nombre"].ToString();
                    miReparacion.MatriCoche = lector["matriCoche"].ToString();
                    miReparacion.CodServicio = int.Parse(lector["codServicio"].ToString());
                    miReparacion.NombreServicio = lector["servicio"].ToString();
                    miReparacion.Fecha = DateTime.Parse(lector["fecha"].ToString()).ToShortDateString();

                    lReparacion.Add(miReparacion);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lReparacion;

        }

        //Sin idCliente selecionado ni matricula
        public List<Reparacion> selectReparacionFiltroFecha(string fecha)
        {
            List<Reparacion> lReparacion = new List<Reparacion>();
            string sql = "select numReparacion,idCliente,(select nombre from cliente where idCliente=r.idCliente)as nombre,matriCoche,codServicio,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r where  fecha='" + fecha + "' order by idCliente,fecha desc";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Reparacion miReparacion = new Reparacion();
                    miReparacion.NumReparacion = int.Parse(lector["numReparacion"].ToString());
                    miReparacion.IdCliente = int.Parse(lector["idCliente"].ToString());
                    miReparacion.NombreCliRepa = lector["nombre"].ToString();
                    miReparacion.MatriCoche = lector["matriCoche"].ToString();
                    miReparacion.CodServicio = int.Parse(lector["codServicio"].ToString());
                    miReparacion.NombreServicio = lector["servicio"].ToString();
                    miReparacion.Fecha = DateTime.Parse(lector["fecha"].ToString()).ToShortDateString();

                    lReparacion.Add(miReparacion);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lReparacion;

        }

        //sin idCliente selecioando pero si con matricula para un mes 
        public List<Reparacion> selectReparacionFiltroFechaMes(string matriculaCoche, string fecha)
        {
            //select strftime('%m','2019-07-10'); Extraemos el mes concreto
            //select * from reparacion where idCliente=1 and strftime('%m','2019-07-10')= strftime('%m',fecha);
            //select numReparacion,(select nombre from cliente where idCliente=r.idCliente)as nombre,matriCoche,codServicio,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r where matriCoche='2218CL' and strftime('%m',fecha)=strftime('%m','2019-06-01');
            List<Reparacion> lReparacion = new List<Reparacion>();
            //string sql = "select numReparacion,idCliente,matriCoche,codServicio,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r where matriCoche='" + matriculaCoche + "' and strftime('%m',fecha)=strftime('%m','" + fecha + "')";
            string sql = "select numReparacion,idCliente,(select nombre from cliente where idCliente=r.idCliente)as nombre,matriCoche,codServicio,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r where matriCoche='"+matriculaCoche+ "' and strftime('%m',fecha)=strftime('%m','" + fecha + "') and  strftime('%Y',fecha)=strftime('%Y','" + fecha + "') order by idCliente,fecha desc";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Reparacion miReparacion = new Reparacion();
                    miReparacion.NumReparacion = int.Parse(lector["numReparacion"].ToString());
                    miReparacion.IdCliente = int.Parse(lector["idCliente"].ToString());
                    miReparacion.NombreCliRepa = lector["nombre"].ToString();
                    miReparacion.MatriCoche = lector["matriCoche"].ToString();
                    miReparacion.CodServicio = int.Parse(lector["codServicio"].ToString());
                    miReparacion.NombreServicio = lector["servicio"].ToString();
                    miReparacion.Fecha = DateTime.Parse(lector["fecha"].ToString()).ToShortDateString();

                    lReparacion.Add(miReparacion);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lReparacion;

        }

        //Sin idCliente selecionado ni matricula para un mes
        public List<Reparacion> selectReparacionFiltroFechaMes(string fecha)
        {
            //select strftime('%m','2019-07-10'); Extraemos el mes concreto
            //select * from reparacion where idCliente=1 and strftime('%m','2019-07-10')= strftime('%m',fecha);
            List<Reparacion> lReparacion = new List<Reparacion>();
            string sql = "select numReparacion,idCliente,(select nombre from cliente where idCliente=r.idCliente)as nombre,matriCoche,codServicio,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r where strftime('%m',fecha)=strftime('%m','" + fecha + "') and  strftime('%Y',fecha)=strftime('%Y','" + fecha + "') order by idCliente,fecha desc";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Reparacion miReparacion = new Reparacion();
                    miReparacion.NumReparacion = int.Parse(lector["numReparacion"].ToString());
                    miReparacion.IdCliente = int.Parse(lector["idCliente"].ToString());
                    miReparacion.NombreCliRepa = lector["nombre"].ToString();
                    miReparacion.MatriCoche = lector["matriCoche"].ToString();
                    miReparacion.CodServicio = int.Parse(lector["codServicio"].ToString());
                    miReparacion.NombreServicio = lector["servicio"].ToString();
                    miReparacion.Fecha = DateTime.Parse(lector["fecha"].ToString()).ToShortDateString();

                    lReparacion.Add(miReparacion);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lReparacion;

        }

        //Selecionando solo matricula y sin fecha marcada //POR AQUI
        public List<Reparacion> selectReparacion(string matricula)
        {
            //select numReparacion,idCliente,matriCoche,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r;
            List<Reparacion> lReparacion = new List<Reparacion>();
            //string sql = "select * from reparacion;";
            string sql = "select numReparacion,idCliente,(select nombre from cliente where idCliente=r.idCliente)as nombre,matriCoche,codServicio,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r where matriCoche='"+matricula+ "' order by idCliente,fecha desc";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Reparacion miReparacion = new Reparacion();
                    miReparacion.NumReparacion = int.Parse(lector["numReparacion"].ToString());
                    miReparacion.IdCliente = int.Parse(lector["idCliente"].ToString());
                    miReparacion.NombreCliRepa = lector["nombre"].ToString();
                    miReparacion.MatriCoche = lector["matriCoche"].ToString();
                    miReparacion.CodServicio = int.Parse(lector["codServicio"].ToString());
                    miReparacion.NombreServicio = lector["servicio"].ToString();
                    miReparacion.Fecha = DateTime.Parse(lector["fecha"].ToString()).ToShortDateString();

                    lReparacion.Add(miReparacion);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lReparacion;

        }

        public List<Reparacion> selectReparacionUnIdCliUnaMatricula(string matricula, int idCliente)
        {
            //select numReparacion,idCliente,matriCoche,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r;
            List<Reparacion> lReparacion = new List<Reparacion>();
            //string sql = "select * from reparacion;";
            string sql = "select numReparacion,idCliente,(select nombre from cliente where idCliente=r.idCliente)as nombre,matriCoche,codServicio,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r where idCliente="+idCliente+" and matriCoche='"+matricula+ "' order by idCliente,fecha desc";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Reparacion miReparacion = new Reparacion();
                    miReparacion.NumReparacion = int.Parse(lector["numReparacion"].ToString());
                    miReparacion.IdCliente = int.Parse(lector["idCliente"].ToString());
                    miReparacion.NombreCliRepa = lector["nombre"].ToString();
                    miReparacion.MatriCoche = lector["matriCoche"].ToString();
                    miReparacion.CodServicio = int.Parse(lector["codServicio"].ToString());
                    miReparacion.NombreServicio = lector["servicio"].ToString();
                    miReparacion.Fecha = DateTime.Parse(lector["fecha"].ToString()).ToShortDateString();

                    lReparacion.Add(miReparacion);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lReparacion;

        }

        public List<Reparacion> selectReparacionUnIdCliUnaMatriculaEnMes(string matricula, int idCliente,string fecha)
        {
            //select numReparacion,idCliente,matriCoche,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r;
            List<Reparacion> lReparacion = new List<Reparacion>();
            //string sql = "select * from reparacion;";  strftime('%m',fecha)=strftime('%m','" + fecha + "')"
            string sql = "select numReparacion,idCliente,(select nombre from cliente where idCliente=r.idCliente)as nombre,matriCoche,codServicio,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r where idCliente=" + idCliente + " and matriCoche='" + matricula + "' and  strftime('%m',fecha)=strftime('%m','" + fecha + "') and  strftime('%Y',fecha)=strftime('%Y','" + fecha + "') order by idCliente,fecha desc";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Reparacion miReparacion = new Reparacion();
                    miReparacion.NumReparacion = int.Parse(lector["numReparacion"].ToString());
                    miReparacion.IdCliente = int.Parse(lector["idCliente"].ToString());
                    miReparacion.NombreCliRepa = lector["nombre"].ToString();
                    miReparacion.MatriCoche = lector["matriCoche"].ToString();
                    miReparacion.CodServicio = int.Parse(lector["codServicio"].ToString());
                    miReparacion.NombreServicio = lector["servicio"].ToString();
                    miReparacion.Fecha = DateTime.Parse(lector["fecha"].ToString()).ToShortDateString();

                    lReparacion.Add(miReparacion);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lReparacion;

        }


        public List<Reparacion> selectReparacionUnIdCliUnaMatriculaEnFecha(string matricula, int idCliente, string fecha)
        {
            //select numReparacion,idCliente,matriCoche,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r;
            List<Reparacion> lReparacion = new List<Reparacion>();
            //string sql = "select * from reparacion;";  strftime('%m',fecha)=strftime('%m','" + fecha + "')"
            string sql = "select numReparacion,idCliente,(select nombre from cliente where idCliente=r.idCliente)as nombre,matriCoche,codServicio,(select descripcion from servicio where codigo=r.codServicio)as servicio,fecha from reparacion r where idCliente=" + idCliente + " and matriCoche='" + matricula + "' and fecha='" + fecha + "' order by idCliente,fecha desc";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Reparacion miReparacion = new Reparacion();
                    miReparacion.NumReparacion = int.Parse(lector["numReparacion"].ToString());
                    miReparacion.IdCliente = int.Parse(lector["idCliente"].ToString());
                    miReparacion.NombreCliRepa = lector["nombre"].ToString();
                    miReparacion.MatriCoche = lector["matriCoche"].ToString();
                    miReparacion.CodServicio = int.Parse(lector["codServicio"].ToString());
                    miReparacion.NombreServicio = lector["servicio"].ToString();
                    miReparacion.Fecha = DateTime.Parse(lector["fecha"].ToString()).ToShortDateString();

                    lReparacion.Add(miReparacion);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lReparacion;

        }    

        public double selectReparacionFiltroCalculoMes(string fecha)
        {
            double total = 0;
            //select round(sum(precio),2)as total from servicio where codigo in(select codServicio from reparacion where strftime('%m','2019-07-10')= strftime('%m',fecha));
            List<Reparacion> lReparacion = new List<Reparacion>();
            //string sql = "select * from reparacion;";
            string sql = "select sum((select precio from servicio where codigo=r.codServicio))as total from reparacion r where strftime('%m',fecha)=strftime('%m','" + fecha + "') and  strftime('%Y',fecha)=strftime('%Y','" + fecha + "')";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {                   
                     double.TryParse(lector["total"].ToString(),out total);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return total;

        }

        //------------------------//

        //Facturacion
        //-----------------//
        public double selectServicioPrecio(string descripcion)
        {
            Servicio miservicio = new Servicio();
            string sql = "select precio from servicio where descripcion='" + descripcion + "';";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    miservicio.Precio = double.Parse(lector["precio"].ToString());
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return miservicio.Precio;

        }

        public string selectClienteApellidos(int idCliente)
        {
            string apellidos = string.Empty;
            string sql = "select apellidos from cliente where idCliente='" + idCliente + "';";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    apellidos = lector["apellidos"].ToString();
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return apellidos;

        }

        //-------------------//


        //Copia de seguridad .output backupTaller.sqlite
        //POR AQUI ->EN PROCESO COPIA DE SEGURIDAD
        public bool CopiaSeguridad()
        {
            try
            {
                //Estas lineas estan para modificar la culturalizacion de forma que indicamos el separador de numperos decimales es el . y no la, para que ala hora de la insercion no de fallos al insertar numbreservicio, 20,05 si no que inserte 20.05
                System.Globalization.CultureInfo c = new System.Globalization.CultureInfo("es-ES");
                c.NumberFormat.NumberDecimalSeparator = ".";
                c.NumberFormat.CurrencyDecimalSeparator = ".";
                c.NumberFormat.PercentDecimalSeparator = ".";
                c.NumberFormat.CurrencyDecimalSeparator = ".";
                
                System.Threading.Thread.CurrentThread.CurrentCulture = c;
                //----------//

                string sql;
                sql = ".output prueba2.sqlite";
                SQLiteCommand cmd = new SQLiteCommand(sql, conexion);
                cmd.ExecuteReader();
  

                string sql2;
                sql2 = "dump";
                SQLiteCommand cmd2 = new SQLiteCommand(sql2, conexion);
                cmd2.ExecuteReader();

                string sql3;
                sql3 = "output";
                SQLiteCommand cmd3 = new SQLiteCommand(sql3, conexion);
                cmd3.ExecuteReader();
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
            return true;

        }

    }

}
