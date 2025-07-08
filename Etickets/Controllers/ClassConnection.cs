using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Etickets.Controllers
{
    public class ClassConnection
    {
        private string ConnString;
        private string ConnStringEtickets;

        public ClassConnection()
        {
            ConnString = System.Configuration.ConfigurationManager.AppSettings["ConnString"];
            ConnStringEtickets = System.Configuration.ConfigurationManager.AppSettings["ConnStringEtickets"];
        }

        public DataTable GetDataTable(string SQLQuery, string TableName)
        {
            DataTable objDtTable = null;
            using (var objCon = new SqlConnection(ConnString))
            {
                if (objCon.State == ConnectionState.Closed)
                    objCon.Open();
                var objCmd = new SqlCommand(SQLQuery, objCon);
                objCmd.CommandTimeout = 0;
                var objDtSet = new DataSet();
                var objdtAdapter = new SqlDataAdapter(objCmd);
                objdtAdapter.Fill(objDtSet, TableName);
                objDtTable = objDtSet.Tables[TableName];
            }
            return objDtTable;
        }

        public bool ExecSqlCommand(string SQLQuery, ref string Error)
        {
            var i = 0;
            return ExecSqlCommand(SQLQuery, ref Error, ref i);
        }
        public bool ExecSqlCommand(string SQLQuery, ref string Error, ref int resultCount)
        {
            bool result = false;
            try
            {
                using (var objCon = new SqlConnection(ConnString))
                {
                    if (objCon.State == ConnectionState.Closed)
                        objCon.Open();
                    var objCmd = new SqlCommand(SQLQuery, objCon);
                    objCmd.CommandTimeout = 0;
                    resultCount = objCmd.ExecuteNonQuery();
                    result = resultCount > 0;
                }
            }
            catch (Exception ex)
            {
                Error = ex.ToString();
            }
            return result;
        }

        public DataTable GetDataTableSigmparo(string SQLQuery, string TableName)
        {
            DataTable objDtTable = null;
            using (var objCon = new SqlConnection(ConnStringEtickets))
            {
                if (objCon.State == ConnectionState.Closed)
                    objCon.Open();
                var objCmd = new SqlCommand(SQLQuery, objCon);
                objCmd.CommandTimeout = 0;
                var objDtSet = new DataSet();
                var objdtAdapter = new SqlDataAdapter(objCmd);
                objdtAdapter.Fill(objDtSet, TableName);
                objDtTable = objDtSet.Tables[TableName];
            }
            return objDtTable;
        }
    }
}