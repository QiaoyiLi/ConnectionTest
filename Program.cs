using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Npgsql;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            DataSet ds = new DataSet();
            try
            {
                // PostgeSQL-style connection string
                string connstring = String.Format("Server={0};Port={1};" +
                                    "User Id={2};Password={3};Database={4};",
                                    "127.0.0.1", "3306", "brillist_production",
                                    "tk6mW@HaVokVBoY", "brillist_production");
                // Making connection with Npgsql provider
                NpgsqlConnection conn = new NpgsqlConnection(connstring);
                conn.Open();
                // quite complex sql statement
                string sql = "SELECT * FROM \"DevelTestTemp\"";
                // data adapter making request from our connection
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, conn);
                ds.Reset();
                da.Fill(ds);
                foreach (DataTable table in ds.Tables)
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        foreach (DataColumn dc in table.Columns)
                        {
                            Console.Write(dr[dc] + " ");
                        }
                        Console.WriteLine("\n");
                    }
                }
                conn.Close();
            }
            catch (NpgsqlException ee)
            {
                throw ee;
            }
        }


    }
}