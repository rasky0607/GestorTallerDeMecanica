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
            string sql = "select * from cliente;";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Cliente miCliente = new Cliente();
                    miCliente.Id = int.Parse(lector["id"].ToString());
                    miCliente.Dni = lector["dni"].ToString();
                    miCliente.Nombre = lector["nombre"].ToString();
                    miCliente.Apellidos = lector["apellidos"].ToString();
                    miCliente.Tlf = int.Parse(lector["tlf"].ToString());
                    lClientes.Add(miCliente);
                }
                lector.Close();
            }
            catch(Exception e) {
                throw new Exception(e.Message);
            }
            return lClientes;

        }


        public List<Coche> selectCoche()
        {
            List<Coche> lCoche = new List<Coche>();
            string sql = "select * from coche;";
            SQLiteCommand sqlYconec = new SQLiteCommand(sql, conexion);

            SQLiteDataReader lector = null;

            try
            {
                lector = sqlYconec.ExecuteReader();
                while (lector.Read())
                {
                    Coche miCoche = new Coche();
                    miCoche.Matricula = lector["matricula"].ToString();
                    miCoche.Marca = lector["marca"].ToString();
                    miCoche.Modelo = lector["modelo"].ToString();
                    lCoche.Add(miCoche);
                }
                lector.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lCoche;

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
                    miReparacion.Id = int.Parse(lector["id"].ToString());
                    miReparacion.IdCliente = int.Parse(lector["idCliente"].ToString());
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

        public bool InsertCliente(string dni,string nombre,string apellidos, int tlf)
        {
            try
            {
                string sql;           
                sql = "INSERT INTO cliente (dni,nombre,apellidos,tlf) values ('"+dni+"','"+nombre+"','"+apellidos+"',"+tlf+")";
                SQLiteCommand cmd = new SQLiteCommand(sql, conexion);
                cmd.ExecuteNonQuery();
            }
            catch
            {
                throw;
            }        
            return true;

        }

        public bool InsertCoche(string matricula, string marca, string modelo)
        {
            try
            {
                if (matricula is null)
                    throw new Exception();
                string sql;
                sql = "INSERT INTO coche (matricula,marca,modelo) values ('" + matricula + "','" + marca + "','" + modelo +"')";
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
        public bool InsertReparacion(int idReparacion, int idCliente,string matriculaCoche, int codServicio,DateTime fecha)
        {
            try
            {
                if (idReparacion <= 0 || idCliente < 0 || matriculaCoche is null || codServicio < 0 || fecha == null)
                    throw new Exception();
                string sql;
                sql = "INSERT INTO reparacion (id,idCliente,matriculaCoche,codServicio,fecha) values ('" + idReparacion + "'," + idCliente + "'"+matriculaCoche+ ""+codServicio+ ""+fecha+")";
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
