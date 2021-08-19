using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Serverapplication
{
    class DAO
    {
        public const string cs = @"Data Source =(local), 1433; Initial Catalog = EdTrab; User ID =sa; Password =adorolilika1;";
        static string infoSeparador = ".";

        public static SqlConnection conectar()
        {
            try
            {
                SqlConnection conn = new SqlConnection(cs);
                conn.Open();
                return conn;


            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public static Networkstatus inserir(string login,string password)
        {
            String sql = "INSERT INTO PLAYERS (login, password,pontos)"//botei pontos
                + "VALUES (@login, @password, 0)"; // botei 0

            

            SqlCommand command = new SqlCommand(sql, conectar());

            SqlParameter par = new SqlParameter("@login", login);
            par.SqlDbType = System.Data.SqlDbType.VarChar;
            command.Parameters.Add(par);

         

            par = new SqlParameter("@password", password);
            par.SqlDbType = System.Data.SqlDbType.VarChar;
            command.Parameters.Add(par);

            


            try
            {
                SqlDataReader reader = command.ExecuteReader();
                
                if (reader.Read())
                {
                    String pk = reader.GetValue(0).ToString(); 
                }
                return Networkstatus.RIGHT;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Networkstatus.ERROR;
            }
        }

        public static Networkstatus login(string login, string password)
        {
            String sql = "SELECT PASSWORD FROM PLAYERS WHERE LOGIN = @login";
            



            SqlCommand command = new SqlCommand(sql, conectar());

            SqlParameter par = new SqlParameter("@login", login);
            par.SqlDbType = System.Data.SqlDbType.VarChar;
            command.Parameters.Add(par);


            try
            {
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    String pk = reader.GetValue(0).ToString();
                    if (pk.Equals(password))
                    {
                        return Networkstatus.RIGHT; 
                    }
                }
                return Networkstatus.ERROR;
                
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return Networkstatus.ERROR;
                
            }
        }

        public static string  pontos()//ler pontos e nome
        {
            String sql = "SELECT LOGIN,PONTOS FROM PLAYERS";
            SqlCommand command = new SqlCommand(sql, conectar());



            try
            {
                SqlDataReader reader = command.ExecuteReader();

                string retorno = string.Empty;
                
                while (reader.Read())
                {
                    retorno += $"{reader.GetValue(0)}{infoSeparador}{reader.GetValue(1)};";
                    
                }
                Console.WriteLine(retorno);
                return retorno;
            }
            
            catch(Exception e)
            {
                Console.WriteLine(e);
                return Networkstatus.ERROR.ToString();
            }
        }

        public static Networkstatus AlterPontos(string login, float addpontos) // não implementado
        {
            String sql = $"UPDATE PLAYERS SET PONTOS = @pontos  WHERE LOGIN = @login";
            SqlCommand command = new SqlCommand(sql, conectar());

            SqlParameter par = new SqlParameter("@login", login);
            par.SqlDbType = System.Data.SqlDbType.VarChar;
            command.Parameters.Add(par);

            par = new SqlParameter("@pontos", addpontos);
            par.SqlDbType = System.Data.SqlDbType.Float;
            command.Parameters.Add(par);

            try
            {
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    String pk = reader.GetValue(0).ToString();
                }
                return Networkstatus.RIGHT;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Networkstatus.ERROR;
            }

        }


    }
}
