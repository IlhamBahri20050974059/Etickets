using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Etickets.Controllers
{
    public class ClassProcedure
    {
        private static ConnectionInfo getConnectionInfo()
        {
            ConnectionInfo connectionInfo = new ConnectionInfo();
            var conStr = System.Configuration.ConfigurationManager.ConnectionStrings["eTicketsEntities"].ConnectionString;
            var ecsb = new System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder(conStr);
            var csb = new SqlConnectionStringBuilder(ecsb.ProviderConnectionString);
            connectionInfo.IntegratedSecurity = csb.IntegratedSecurity;
            connectionInfo.ServerName = csb.DataSource;
            connectionInfo.DatabaseName = csb.InitialCatalog;
            connectionInfo.UserID = csb.UserID;
            connectionInfo.Password = csb.Password;
            return connectionInfo;
        }

        public static string SetDBLogonForReport(ReportDocument reportDoc)
        {
            ConnectionInfo connectionInfo = getConnectionInfo();

            Tables tables = reportDoc.Database.Tables;
            foreach (Table table in tables)
            {
                TableLogOnInfo tableLogonInfo = table.LogOnInfo;
                tableLogonInfo.ConnectionInfo = connectionInfo;
                table.ApplyLogOnInfo(tableLogonInfo);
            }

            foreach (ReportDocument subreport in reportDoc.Subreports)
            {
                Tables myTablesSub = subreport.Database.Tables;
                foreach (Table table in myTablesSub)
                {
                    TableLogOnInfo tableLogonInfo = table.LogOnInfo;
                    tableLogonInfo.ConnectionInfo = connectionInfo;
                    table.ApplyLogOnInfo(tableLogonInfo);
                }
            }

            return "OK";
        }


        public static string SetDBLogonForReport(ReportDocument reportDoc, string[] exctablename)
        {
            ConnectionInfo connectionInfo = getConnectionInfo();

            Tables tables = reportDoc.Database.Tables;
            foreach (Table table in tables)
            {
                if (!exctablename.Contains(table.Name))
                {
                    TableLogOnInfo tableLogonInfo = table.LogOnInfo;
                    tableLogonInfo.ConnectionInfo = connectionInfo;
                    table.ApplyLogOnInfo(tableLogonInfo);
                }
            }

            foreach (ReportDocument subreport in reportDoc.Subreports)
            {
                Tables myTablesSub = subreport.Database.Tables;
                foreach (Table table in myTablesSub)
                {
                    if (!exctablename.Contains(table.Name))
                    {
                        TableLogOnInfo tableLogonInfo = table.LogOnInfo;
                        tableLogonInfo.ConnectionInfo = connectionInfo;
                        table.ApplyLogOnInfo(tableLogonInfo);
                    }
                }
            }

            return "OK";
        }

        public static string SetDBLogonForReportTable(Table reportTbl)
        {
            ConnectionInfo connectionInfo = getConnectionInfo();

            TableLogOnInfo tableLogonInfo = reportTbl.LogOnInfo;
            tableLogonInfo.ConnectionInfo = connectionInfo;
            reportTbl.ApplyLogOnInfo(tableLogonInfo);

            return "OK";
        }
    }
}