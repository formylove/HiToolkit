using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using System.Data.SqlClient;

namespace robotX
{
    class DBAcesser
    {
        static  string connUrl = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Database.accdb;Persist Security Info=False";
        static string connectionString = @"Data Source=.\SQLEXPRESS;
                                AttachDbFilename=""Database.accdb"";
                                Integrated Security=True;
                                Connect Timeout=30;User Instance=True";
        static  void Main()
        {
        string query = "insert into IPGroups(ip,name) values('11.11.2.3','圣安')";

        using (OleDbConnection conn = new OleDbConnection(connUrl)){
                using (OleDbCommand comm = new OleDbCommand(query, conn)) { 
                        ;
                        conn.Open();
                        comm.ExecuteNonQuery();
                    }
        }


        }
       public  DataTable FetchDataSet (string query)
        {
            using (OleDbConnection conn = new OleDbConnection(connUrl))
            {
                using (OleDbCommand comm = new OleDbCommand(query, conn))
                {
                    conn.Open();
                    OleDbDataAdapter adp = new OleDbDataAdapter();
                    adp.SelectCommand = comm;
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    return dt;
                }
            }


        }
        public int Update(string tName, string condition) {


            return 0;

        }


    }
}
