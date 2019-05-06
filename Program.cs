// To initialize connection, it will require proxy server running on port 3306
// On development computer, please follow following steps:
//                  1. run cmd
//                  2. >cd Desktop
//                  3. >cloud_sql_proxy.exe -instances=brillist:us-east1:brillist-production=tcp:3306
//                  done, don't close the cmd window, otherwise it will shut down proxy server


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using Npgsql;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            // file height represents which row currently is reading
            int FileHeight = 0;                                                                             
            var fileReader = File.ReadLines("C:\\Users\\devel\\Downloads\\test_inputs.csv");

            // File Headers are the first row of the target CSV file, it represents the column header values
            string[] fileHeaders = new string[fileReader.First().Length];                                   
            string sqlInsertHeader = "INSERT INTO \"DevelTestTemp\" (";
            foreach (string line in fileReader)
            {
                string sqlInsertCommand = "";
                FileHeight++;
                string[] values = line.Split(','); 
                int length = values.Length;
                if (FileHeight == 1)
                {
                    fileHeaders = values;
                    for (int i = 0; i < length; i++)
                    {
                        sqlInsertHeader += "\""+ fileHeaders[i]+"\"";
                        if (i != length-1)
                            sqlInsertHeader += ",";
                    }
                    sqlInsertHeader += ")";
                }
                else
                {
                    sqlInsertCommand += sqlInsertHeader;
                    sqlInsertCommand += "values (\'";
                    for (int i = 0; i < length; i++)
                    {
                        sqlInsertCommand += values[i];
                        if (i != length-1)
                            sqlInsertCommand += "\',\'";
                    }
                    sqlInsertCommand += "\');";
                }
                Console.WriteLine(sqlInsertCommand);
                Console.WriteLine("\n current height:"+FileHeight+"\n");

                //space for adding connection and sql queries
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
                    NpgsqlCommand cmd = new NpgsqlCommand(sqlInsertCommand);
                    cmd.Connection = conn;
                    NpgsqlDataReader dr = cmd.ExecuteReader();
                    Int32 nbCertByExercice = 0;
                    while (dr.Read())
                    {
                        nbCertByExercice = dr.GetInt32(0);
                    }
                    Console.WriteLine("" + nbCertByExercice);
                    conn.Close();
                }
                catch (NpgsqlException ee)
                {
                    throw ee;
                }

            }


            // Delete from "DevelTestTemp" where "Rate"=5;
            //DataSet ds = new DataSet();
            //try
            //{
            //    // PostgeSQL-style connection string
            //    string connstring = String.Format("Server={0};Port={1};" +
            //                        "User Id={2};Password={3};Database={4};",
            //                        "127.0.0.1", "3306", "brillist_production",
            //                        "tk6mW@HaVokVBoY", "brillist_production");
            //    // Making connection with Npgsql provider
            //    NpgsqlConnection conn = new NpgsqlConnection(connstring);
            //    conn.Open();
            //    string sql = "SELECT * FROM \"DevelTestTemp\"";
            //    // data adapter making request from connection
            //    NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, conn);
            //    ds.Reset();
            //    da.Fill(ds);
            //    foreach (DataTable table in ds.Tables)
            //    {
            //        foreach (DataRow dr in table.Rows)
            //        {
            //            foreach (DataColumn dc in table.Columns)
            //            {
            //                Console.Write(dr[dc] + " ");
            //            }
            //            Console.WriteLine("\n");
            //        }
            //    }
            //    conn.Close();
            //}
            //catch (NpgsqlException ee)
            //{
            //    throw ee;
            //}
            return;
        }
    }
}