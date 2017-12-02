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
        static  string connUrl = @"Provider=microsoft.ace.oledb.12.0;Data Source=Database.accdb;Persist Security Info=False";//Microsoft.Jet.OLEDB.4.0
        static  void Main()
        {


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
        public int Update(string sql) {

            using (OleDbConnection conn = new OleDbConnection(connUrl))
            {
                using (OleDbCommand comm = new OleDbCommand(sql, conn))
                {
                    conn.Open();
                    
                    return comm.ExecuteNonQuery();
                }
            }

        }


    }
}
