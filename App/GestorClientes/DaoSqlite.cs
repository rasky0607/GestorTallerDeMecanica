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
            string cadenaConecxion = string.Format("Data Source=" + db + ";Version=3;FailIfMissing=true;");
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
            string sql = "select dni,nombre,apellidos,tlf,matricula,marca,modelo from cliente;";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Cliente miCliente = new Cliente();
                    miCliente.Dni = lector["dni"].ToString();
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

        public List<string> selectClienteDni()
        {
            List<string> lClientes = new List<string>();
            string sql = "select dni from cliente;";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {                               
                    lClientes.Add(lector["dni"].ToString());              
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lClientes;

        }

        public List<string> selectClienteMatricula(string dni)
        {
            List<string> listMatricula = new List<string>();
            string sql = "select matricula from cliente where dni='"+dni+"';";
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

        public List<Reparacion> selectReparacion()
        {
            List<Reparacion> lReparacion = new List<Reparacion>();
            string sql = "select * from reparacion;";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Reparacion miReparacion = new Reparacion();
                    miReparacion.NumReparacion = int.Parse(lector["numReparacion"].ToString());
                    miReparacion.DniCliente = lector["dniCliente"].ToString();
                    miReparacion.MatriCoche = lector["matriCoche"].ToString();
                    miReparacion.CodServicio = int.Parse(lector["codServicio"].ToString());
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

        public bool InsertCliente(string dni,string nombre,string apellidos, int tlf,string matricula, string marca, string modelo)
        {
            try
            {
                string sql;           
                sql = "INSERT INTO cliente (dni,nombre,apellidos,tlf,matricula,marca,modelo) values ('"+dni+"','"+nombre+"','"+apellidos+"',"+tlf+",'"+matricula+ "','"+marca+ "','"+modelo+"');";
                SQLiteCommand cmd = new SQLiteCommand(sql, conexion);
                cmd.ExecuteNonQuery();
            }
            catch
            {
                throw;
            }        
            return true;

        }
   
        public bool InsertServicio(string descripcion, double precio)
        {
            try
            {
                if (descripcion is null)
                    throw new Exception();
                string sql;
                sql = "INSERT INTO servicio (descripcion,precio) values ('" + descripcion + "'," + precio + ")";
                SQLiteCommand cmd = new SQLiteCommand(sql, conexion);
                cmd.ExecuteNonQuery();
            }
            catch
            {
                throw;
            }
            return true;

        }

        //REVISAR PROFUNDAMENTE(Sin probar y con cosas aun por picar) 
        public bool InsertReparacion(int idReparacion, string dniCliente,string matriculaCoche, int codServicio,string fecha)
        {
            try
            {
                if (idReparacion <= 0 || dniCliente  is null || matriculaCoche is null || codServicio < 0 || fecha == null)
                    throw new Exception();
                string sql;
                sql = "INSERT INTO reparacion (numReparacion,dniCliente,matriCoche,codServicio,fecha) values (" + idReparacion + ",'" + dniCliente + "','"+matriculaCoche+ "',"+codServicio+ ",'"+DateTime.Parse(fecha)+"')";
                SQLiteCommand cmd = new SQLiteCommand(sql, conexion);
                cmd.ExecuteNonQuery();
            }
            catch
            {
                throw;
            }
            return true;

        }






    }
}
