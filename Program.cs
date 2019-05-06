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
        static void insertQuery(NpgsqlConnection conn, string fileString)
        {
            // file height represents which row currently is reading
            int FileHeight = 0;
            var fileReader = File.ReadLines(fileString);
            string TableName = "DevelTestTemp";
            // File Headers are the first row of the target CSV file, it represents the column header values
            string[] fileHeaders = new string[fileReader.First().Length];
            string sqlInsertHeader = "INSERT INTO \""+TableName+"\" (";

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
                        sqlInsertHeader += "\"" + fileHeaders[i] + "\"";
                        if (i != length - 1)
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
                        if (i != length - 1)
                            sqlInsertCommand += "\',\'";
                    }
                    sqlInsertCommand += "\');";
                }
                Console.WriteLine(sqlInsertCommand);

                // Query
                try
                {
                    conn.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand(sqlInsertCommand);
                    cmd.Connection = conn;
                    NpgsqlDataReader dr = cmd.ExecuteReader();
                    conn.Close();
                }
                catch (NpgsqlException ee)
                {
                    conn.Close();
                    Console.WriteLine(ee);
                }
            }
        }

        static void selectQuery(NpgsqlConnection conn)
        {
            // Select Query
            try
            {
                string TableName = "DevelTestTemp";
                string outputStr = "";
                conn.Open();
                string SelectQuery = "SELECT * FROM \""+TableName+"\" limit 10;";
                NpgsqlCommand cmd = new NpgsqlCommand(SelectQuery);
                cmd.Connection = conn;
                NpgsqlDataReader dr = cmd.ExecuteReader();
                // Printing
                Console.WriteLine("\n" + SelectQuery);
                Console.WriteLine(("").PadRight(48, '-'));
                string Headers = ("|id").PadRight(15) + "|" +
                                 ("Name").PadRight(15) + "|" +
                                 ("Rate").PadRight(15) + "|";
                Console.WriteLine(Headers);
                Console.WriteLine(("").PadRight(48, '-'));
                while (dr.Read())
                {
                    outputStr = ("|" + dr["Id"].ToString()).PadRight(15) + "|" +
                                (dr["Name"].ToString()).PadRight(15) + "|" +
                                (dr["Rate"].ToString()).PadRight(15) + "|";
                    Console.WriteLine(outputStr);
                }
                Console.WriteLine(("").PadRight(48, '-'));
                conn.Close();
            }
            catch (NpgsqlException ee)
            {
                Console.WriteLine(ee);
            }
        }

        static void Main(string[] args)
        {
            string fileString = "C:\\Users\\devel\\Downloads\\test_inputs.csv";
            string connstring = String.Format("Server={0};Port={1};" +
                "User Id={2};Password={3};Database={4};",
                "127.0.0.1", "3306", "brillist_production",
                "tk6mW@HaVokVBoY", "brillist_production");
            NpgsqlConnection conn = new NpgsqlConnection(connstring);

            insertQuery(conn, fileString);
            selectQuery(conn);
            return;
        }
    }
}